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
    /// Genera el PDF del informe de socios con QuestPDF.
    /// Diseño: A4 apaisada, encabezado con la institución y los filtros aplicados, una tabla
    /// con una fila por socio (el encabezado de la tabla se repite en cada hoja) y pie con
    /// la fecha de emisión y el número de página.
    /// </summary>
    public class InformeSociosPdf : IInformeSociosPdf
    {
        private readonly ComprobanteOpciones _opciones;

        static InformeSociosPdf() => DeclararLicencia();

        public InformeSociosPdf(IOptions<ComprobanteOpciones> opciones)
        {
            // Se reutilizan los datos de la institución que ya se imprimen en los comprobantes.
            _opciones = opciones.Value.ConValoresPorDefecto();
        }

        public byte[] Generar(IReadOnlyList<SocioInformeDto> socios, SocioInformeFiltroDto filtro)
        {
            var emision = DateTime.Now;

            return Document.Create(documento =>
                {
                    documento.Page(pagina =>
                    {
                        ConfigurarHoja(pagina, emision);
                        pagina.Header().Element(x => Encabezado(x, filtro, socios.Count));
                        pagina.Content().PaddingTop(4, Unit.Millimetre).Element(x => Tabla(x, socios));
                    });
                })
                .WithMetadata(new DocumentMetadata { Title = "Informe de socios" })
                .GeneratePdf();
        }

        private void Encabezado(IContainer contenedor, SocioInformeFiltroDto filtro, int cantidad)
        {
            contenedor.Column(col =>
            {
                Institucion(col, _opciones);

                col.Item().PaddingTop(3, Unit.Millimetre).Row(fila =>
                {
                    fila.RelativeItem().Text("Informe de socios").Bold().FontSize(15);
                    fila.AutoItem().AlignBottom().Text($"Total: {cantidad} socio{(cantidad == 1 ? "" : "s")}").Bold().FontSize(10);
                });

                col.Item().PaddingTop(1, Unit.Millimetre).Text(texto =>
                {
                    texto.Span("Estado: ").FontColor(Colors.Grey.Darken2);
                    texto.Span(DescripcionEstado(filtro.Estado)).Bold();
                    texto.Span("    Categorías: ").FontColor(Colors.Grey.Darken2);
                    texto.Span(DescripcionCategorias(filtro)).Bold();
                });

                col.Item().PaddingTop(2, Unit.Millimetre).LineHorizontal(0.8f).LineColor(Colors.Black);
            });
        }

        private static void Tabla(IContainer contenedor, IReadOnlyList<SocioInformeDto> socios)
        {
            contenedor.Table(tabla =>
            {
                // Anchos relativos pensados para A4 apaisada (277 mm útiles).
                tabla.ColumnsDefinition(columnas =>
                {
                    columnas.RelativeColumn(17); // Nombre y apellido
                    columnas.RelativeColumn(13); // Domicilio
                    columnas.RelativeColumn(8);  // Ciudad
                    columnas.RelativeColumn(8);  // Teléfono
                    columnas.RelativeColumn(21); // Mail
                    columnas.RelativeColumn(10); // Obra social
                    columnas.RelativeColumn(5);  // Edad
                    columnas.RelativeColumn(6);  // Sepelio
                    columnas.RelativeColumn(5);  // Nicho
                    columnas.RelativeColumn(7);  // Estado
                });

                // El encabezado de la tabla se repite automáticamente en cada hoja.
                tabla.Header(encabezado =>
                {
                    foreach (var titulo in new[] { "Apellido y nombre", "Domicilio", "Ciudad", "Teléfono", "Mail", "Obra social" })
                        CeldaTitulo(encabezado.Cell()).Text(titulo);

                    foreach (var titulo in new[] { "Edad", "Sepelio", "Nicho", "Estado" })
                        CeldaTitulo(encabezado.Cell()).AlignCenter().Text(titulo);
                });

                for (var i = 0; i < socios.Count; i++)
                {
                    var s = socios[i];

                    Celda(tabla.Cell(), i).Text(NombreCompleto(s.Apellido, s.Nombre)).Bold();
                    Celda(tabla.Cell(), i).Text(Domicilio(s.Calle, s.Altura));
                    Celda(tabla.Cell(), i).Text(Valor(s.Ciudad));
                    Celda(tabla.Cell(), i).Text(Valor(s.Telefono));
                    Celda(tabla.Cell(), i).Text(Valor(s.Mail));
                    Celda(tabla.Cell(), i).Text(Valor(s.ObraSocial));
                    Celda(tabla.Cell(), i).AlignCenter().Text(s.Edad?.ToString() ?? SinDato);
                    Celda(tabla.Cell(), i).AlignCenter().Text(s.Sepelio ? "SI" : "NO");
                    Celda(tabla.Cell(), i).AlignCenter().Text(s.Nicho ? "SI" : "NO");
                    Celda(tabla.Cell(), i).AlignCenter().Text(Valor(s.Estado));
                }
            });
        }

        private static string DescripcionEstado(string estado) => estado switch
        {
            "ACTIVO" => "Activos",
            "INACTIVO" => "Inactivos",
            _ => "Todos"
        };

        private static string DescripcionCategorias(SocioInformeFiltroDto filtro)
        {
            var categorias = new List<string>();
            if (filtro.IncluirAdherentes) categorias.Add("Adherente");
            if (filtro.IncluirSepelio) categorias.Add("Sepelio");
            if (filtro.IncluirNicho) categorias.Add("Nicho");
            return categorias.Count == 3 ? "Todas" : string.Join(", ", categorias);
        }
    }
}
