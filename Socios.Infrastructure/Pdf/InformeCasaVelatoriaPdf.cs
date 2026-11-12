using Microsoft.Extensions.Options;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using Socios.Application.DTOs;
using Socios.Application.Interfaces;
using static Socios.Infrastructure.Pdf.PdfInformeComun;

namespace Socios.Infrastructure.Pdf
{
    /// <summary>
    /// Genera el PDF del listado para la casa velatoria con QuestPDF.
    /// Diseño: A4 vertical; encabezado con la institución y el título; franja destacada con la
    /// fecha de emisión y la cantidad de socios; el criterio aplicado; la tabla de socios (el
    /// encabezado se repite en cada hoja) y una aclaración de que el listado vale a la fecha de
    /// emisión.
    /// </summary>
    public class InformeCasaVelatoriaPdf : IInformeCasaVelatoriaPdf
    {
        private readonly ComprobanteOpciones _opciones;

        static InformeCasaVelatoriaPdf() => DeclararLicencia();

        public InformeCasaVelatoriaPdf(IOptions<ComprobanteOpciones> opciones)
        {
            // Se reutilizan los datos de la institución que ya se imprimen en los comprobantes.
            _opciones = opciones.Value.ConValoresPorDefecto();
        }

        public byte[] Generar(CasaVelatoriaInformeDto informe)
        {
            var emision = DateTime.Now;

            return Document.Create(documento =>
                {
                    documento.Page(pagina =>
                    {
                        ConfigurarHoja(pagina, emision, apaisada: false);
                        pagina.Header().Element(x => Encabezado(x, informe));
                        pagina.Content().PaddingTop(4, Unit.Millimetre).Column(col =>
                        {
                            if (informe.Cantidad == 0)
                            {
                                col.Item().PaddingTop(10, Unit.Millimetre).AlignCenter()
                                    .Text("No hay socios que cumplan las condiciones a la fecha de emisión.")
                                    .FontSize(11).FontColor(Colors.Grey.Darken2);
                                return;
                            }

                            col.Item().Element(x => Tabla(x, informe));

                            col.Item().PaddingTop(4, Unit.Millimetre).Text(
                                    $"Listado válido a la fecha de emisión ({Fecha(informe.FechaEmision)}). La situación de cada " +
                                    "socio puede cambiar después: un socio que hoy figura puede superar el límite de meses " +
                                    "adeudados más adelante y dejar de estar habilitado.")
                                .FontSize(8).Italic().FontColor(Colors.Grey.Darken2);
                        });
                    });
                })
                .WithMetadata(new DocumentMetadata { Title = "Socios habilitados para sepelio" })
                .GeneratePdf();
        }

        private void Encabezado(IContainer contenedor, CasaVelatoriaInformeDto informe)
        {
            contenedor.Column(col =>
            {
                Institucion(col, _opciones);
                col.Item().PaddingTop(3, Unit.Millimetre).Text("Socios habilitados para el servicio de sepelio").Bold().FontSize(15);
                col.Item().Text("Listado para la casa velatoria").FontSize(9).FontColor(Colors.Grey.Darken2);

                // Franja destacada con la fecha de emisión (dato clave para la casa velatoria).
                col.Item().PaddingTop(3, Unit.Millimetre).Background(Colors.Grey.Lighten4).CornerRadius(2, Unit.Millimetre)
                    .PaddingVertical(2.5f, Unit.Millimetre).PaddingHorizontal(4, Unit.Millimetre).Row(fila =>
                    {
                        fila.RelativeItem().Column(c =>
                        {
                            c.Item().Text("FECHA DE EMISIÓN DEL REPORTE").FontSize(7).FontColor(Colors.Grey.Darken1).LetterSpacing(0.05f);
                            c.Item().Text(Fecha(informe.FechaEmision)).Bold().FontSize(14);
                        });
                        fila.RelativeItem().AlignRight().Column(c =>
                        {
                            c.Item().AlignRight().Text("SOCIOS HABILITADOS").FontSize(7).FontColor(Colors.Grey.Darken1).LetterSpacing(0.05f);
                            c.Item().AlignRight().Text(informe.Cantidad.ToString()).Bold().FontSize(14);
                        });
                    });

                col.Item().PaddingTop(2, Unit.Millimetre).Text(
                        $"Socios activos con sepelio (el nicho solo no da acceso al servicio), con al menos " +
                        $"{informe.MinimoCuotasSepelio} cuotas de sepelio pagadas y que, a la fecha de emisión, no " +
                        $"adeudan más de {informe.MaximoMeses} meses de cuotas vencidas.")
                    .FontSize(8.5f).FontColor(Colors.Grey.Darken2);

                col.Item().PaddingTop(2, Unit.Millimetre).LineHorizontal(0.8f).LineColor(Colors.Black);
            });
        }

        private static void Tabla(IContainer contenedor, CasaVelatoriaInformeDto informe)
        {
            var socios = informe.Socios;

            contenedor.Table(tabla =>
            {
                tabla.ColumnsDefinition(c =>
                {
                    c.RelativeColumn(28); // Apellido y nombre
                    c.RelativeColumn(11); // DNI
                    c.RelativeColumn(6);  // Edad
                    c.RelativeColumn(11); // Cuotas sepelio pagadas
                    c.RelativeColumn(11); // Meses adeudados
                    c.RelativeColumn(6);  // Plan
                    c.RelativeColumn(7);  // Nicho
                });

                // El encabezado de la tabla se repite automáticamente en cada hoja.
                tabla.Header(h =>
                {
                    CeldaTitulo(h.Cell()).Text("Apellido y nombre");
                    CeldaTitulo(h.Cell()).Text("DNI");
                    CeldaTitulo(h.Cell()).AlignCenter().Text("Edad");
                    CeldaTitulo(h.Cell()).AlignCenter().Text("Cuotas sepelio pagadas");
                    CeldaTitulo(h.Cell()).AlignCenter().Text("Meses adeudados");
                    CeldaTitulo(h.Cell()).AlignCenter().Text("Plan");
                    CeldaTitulo(h.Cell()).AlignCenter().Text("Nicho");
                });

                for (var i = 0; i < socios.Count; i++)
                {
                    var s = socios[i];
                    Celda(tabla.Cell(), i).Text(NombreCompleto(s.Apellido, s.Nombre)).Bold();
                    Celda(tabla.Cell(), i).Text(Valor(s.Dni));
                    Celda(tabla.Cell(), i).AlignCenter().Text(s.Edad?.ToString() ?? SinDato);
                    Celda(tabla.Cell(), i).AlignCenter().Text(s.CuotasSepelioPagadas.ToString());
                    Celda(tabla.Cell(), i).AlignCenter().Text(s.MesesAdeudados.ToString());
                    Celda(tabla.Cell(), i).AlignCenter().Text(Valor(s.Plan));
                    Celda(tabla.Cell(), i).AlignCenter().Text(s.Nicho ? "SI" : "NO");
                }
            });
        }
    }
}
