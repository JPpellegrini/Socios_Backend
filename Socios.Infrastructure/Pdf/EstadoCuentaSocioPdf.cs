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
    /// Genera el PDF del estado de cuenta individual del socio con QuestPDF.
    /// Diseño: A4 apaisada; encabezado con la institución, el socio y el período; tabla con una
    /// cuota por fila (el encabezado se repite en cada hoja) y los datos del pago si está paga;
    /// fila final con el total del monto y, debajo, lo pagado y lo adeudado del período y la
    /// deuda total del socio a la fecha. Si el período no tiene cuotas, se muestra un aviso.
    /// </summary>
    public class EstadoCuentaSocioPdf : IEstadoCuentaSocioPdf
    {
        private static readonly string[] Meses =
        {
            "ENERO", "FEBRERO", "MARZO", "ABRIL", "MAYO", "JUNIO",
            "JULIO", "AGOSTO", "SEPTIEMBRE", "OCTUBRE", "NOVIEMBRE", "DICIEMBRE"
        };

        private readonly ComprobanteOpciones _opciones;

        static EstadoCuentaSocioPdf() => DeclararLicencia();

        public EstadoCuentaSocioPdf(IOptions<ComprobanteOpciones> opciones)
        {
            // Se reutilizan los datos de la institución que ya se imprimen en los comprobantes.
            _opciones = opciones.Value.ConValoresPorDefecto();
        }

        public byte[] Generar(SocioEstadoCuentaDto estadoCuenta)
        {
            var emision = DateTime.Now;

            return Document.Create(documento =>
                {
                    documento.Page(pagina =>
                    {
                        ConfigurarHoja(pagina, emision);
                        pagina.Header().Element(x => Encabezado(x, estadoCuenta));
                        pagina.Content().PaddingTop(4, Unit.Millimetre).Column(col =>
                        {
                            if (estadoCuenta.Cantidad == 0)
                                col.Item().PaddingVertical(8, Unit.Millimetre).AlignCenter()
                                    .Text("Sin cuotas con vencimiento en el período seleccionado.")
                                    .FontSize(11).FontColor(Colors.Grey.Darken2);
                            else
                                col.Item().Element(x => Tabla(x, estadoCuenta));

                            col.Item().PaddingTop(3, Unit.Millimetre).Element(x => Resumen(x, estadoCuenta));
                        });
                    });
                })
                .WithMetadata(new DocumentMetadata { Title = "Estado de cuenta del socio" })
                .GeneratePdf();
        }

        private void Encabezado(IContainer contenedor, SocioEstadoCuentaDto estadoCuenta)
        {
            var s = estadoCuenta.Socio;
            var cantidad = estadoCuenta.Cantidad;

            contenedor.Column(col =>
            {
                Institucion(col, _opciones);

                col.Item().PaddingTop(3, Unit.Millimetre).Row(fila =>
                {
                    fila.RelativeItem().Text("Estado de cuenta del socio").Bold().FontSize(15);
                    fila.AutoItem().AlignBottom()
                        .Text($"{cantidad} cuota{(cantidad == 1 ? "" : "s")}").Bold().FontSize(10);
                });

                col.Item().PaddingTop(1, Unit.Millimetre).Text(texto =>
                {
                    texto.Span("Socio: ").FontColor(Colors.Grey.Darken2);
                    texto.Span(NombreCompleto(s.Apellido, s.Nombre)).Bold();
                    texto.Span("    N° de socio: ").FontColor(Colors.Grey.Darken2);
                    texto.Span(s.IdSocio.ToString()).Bold();
                    texto.Span("    DNI: ").FontColor(Colors.Grey.Darken2);
                    texto.Span(Valor(s.Dni)).Bold();
                    texto.Span("    Estado: ").FontColor(Colors.Grey.Darken2);
                    texto.Span(Valor(s.Estado)).Bold();
                });

                col.Item().PaddingTop(1, Unit.Millimetre).Text(texto =>
                {
                    texto.Span("Vencimientos del ").FontColor(Colors.Grey.Darken2);
                    texto.Span($"{Fecha(estadoCuenta.FechaDesde)} al {Fecha(estadoCuenta.FechaHasta)}").Bold();
                });

                col.Item().PaddingTop(2, Unit.Millimetre).LineHorizontal(0.8f).LineColor(Colors.Black);
            });
        }

        private static void Tabla(IContainer contenedor, SocioEstadoCuentaDto estadoCuenta)
        {
            var cuotas = estadoCuenta.Cuotas;

            contenedor.Table(tabla =>
            {
                // Anchos relativos pensados para A4 apaisada (277 mm útiles).
                tabla.ColumnsDefinition(columnas =>
                {
                    columnas.RelativeColumn(9);  // Mes
                    columnas.RelativeColumn(5);  // Año
                    columnas.RelativeColumn(8);  // Tipo de cuota
                    columnas.RelativeColumn(10); // Monto
                    columnas.RelativeColumn(9);  // Vencimiento
                    columnas.RelativeColumn(8);  // Estado
                    columnas.RelativeColumn(11); // Fecha y hora de pago
                    columnas.RelativeColumn(11); // Método de pago
                    columnas.RelativeColumn(13); // Cuenta
                    columnas.RelativeColumn(16); // Observación
                });

                // El encabezado de la tabla se repite automáticamente en cada hoja.
                tabla.Header(encabezado =>
                {
                    CeldaTitulo(encabezado.Cell()).Text("Mes");
                    CeldaTitulo(encabezado.Cell()).AlignCenter().Text("Año");
                    CeldaTitulo(encabezado.Cell()).Text("Tipo cuota");
                    CeldaTitulo(encabezado.Cell()).AlignRight().Text("Monto");
                    CeldaTitulo(encabezado.Cell()).AlignCenter().Text("Vencimiento");
                    CeldaTitulo(encabezado.Cell()).Text("Estado");
                    CeldaTitulo(encabezado.Cell()).Text("Fecha y hora pago");
                    CeldaTitulo(encabezado.Cell()).Text("Método de pago");
                    CeldaTitulo(encabezado.Cell()).Text("Cuenta");
                    CeldaTitulo(encabezado.Cell()).Text("Observación");
                });

                for (var i = 0; i < cuotas.Count; i++)
                {
                    var c = cuotas[i];

                    Celda(tabla.Cell(), i).Text(NombreMes(c.Mes));
                    Celda(tabla.Cell(), i).AlignCenter().Text(c.Anio.ToString());
                    Celda(tabla.Cell(), i).Text(c.TipoCuota);
                    Celda(tabla.Cell(), i).AlignRight().Text(PdfComun.Importe(c.Monto) + (c.MontoActualizado ? " *" : ""));
                    Celda(tabla.Cell(), i).AlignCenter().Text(Fecha(c.FechaVencimiento));

                    // Las cuotas adeudadas se destacan en negrita.
                    var estado = Celda(tabla.Cell(), i).Text(c.Estado);
                    if (!c.EstaPaga) estado.Bold();

                    Celda(tabla.Cell(), i).Text(c.FechaHoraPago?.ToString("dd/MM/yyyy HH:mm", CulturaAr) ?? SinDato);
                    Celda(tabla.Cell(), i).Text(Valor(c.MetodoPago));
                    Celda(tabla.Cell(), i).Text(Cuenta(c.Cuenta, c.NroCuenta));
                    Celda(tabla.Cell(), i).Text(Valor(c.Observacion));
                }

                // Totalizador de la columna monto.
                tabla.Cell().ColumnSpan(3).Element(CeldaTotal).AlignRight().Text("TOTAL");
                tabla.Cell().Element(CeldaTotal).AlignRight().Text(PdfComun.Importe(estadoCuenta.Total));
                tabla.Cell().ColumnSpan(6).Element(CeldaTotal);
            });
        }

        /// <summary>Pagado y adeudado del período, deuda total a la fecha y aclaraciones.</summary>
        private static void Resumen(IContainer contenedor, SocioEstadoCuentaDto estadoCuenta)
        {
            contenedor.Column(col =>
            {
                if (estadoCuenta.Cantidad > 0)
                    col.Item().AlignRight().Text(texto =>
                    {
                        texto.DefaultTextStyle(e => e.FontSize(9));
                        texto.Span("Pagado en el período: ").FontColor(Colors.Grey.Darken2);
                        texto.Span(PdfComun.Importe(estadoCuenta.TotalPagado)).Bold();
                        texto.Span("    Adeudado en el período: ").FontColor(Colors.Grey.Darken2);
                        texto.Span(PdfComun.Importe(estadoCuenta.TotalAdeudado)).Bold();
                    });

                // La deuda total ignora el filtro de fechas: una deuda vieja no queda oculta.
                var n = estadoCuenta.CuotasAdeudadasTotal;
                col.Item().PaddingTop(1.5f, Unit.Millimetre).AlignRight().Text(texto =>
                {
                    texto.DefaultTextStyle(e => e.FontSize(10));
                    texto.Span("Deuda total del socio a la fecha: ").Bold();
                    texto.Span(PdfComun.Importe(estadoCuenta.DeudaTotal)).Bold();
                    texto.Span($"  ({n} cuota{(n == 1 ? "" : "s")} adeudada{(n == 1 ? "" : "s")}, sin importar el período)")
                        .FontSize(8).FontColor(Colors.Grey.Darken2);
                });

                if (estadoCuenta.TieneMontosActualizados)
                    col.Item().PaddingTop(2, Unit.Millimetre)
                        .Text("* Cuota vencida: se informa al importe vigente, que es el que se cobra hoy.")
                        .FontSize(7.5f).Italic().FontColor(Colors.Grey.Darken2);
            });
        }

        private static IContainer CeldaTotal(IContainer celda) =>
            celda.Background(Colors.Grey.Lighten2)
                .BorderTop(0.8f).BorderColor(Colors.Black)
                .PaddingVertical(1.5f, Unit.Millimetre).PaddingHorizontal(1.2f, Unit.Millimetre)
                .DefaultTextStyle(estilo => estilo.Bold().FontSize(9));

        private static string NombreMes(int mes) => mes is >= 1 and <= 12 ? Meses[mes - 1] : mes.ToString();

        /// <summary>"BANCO MACRO - 789012"; sin número si la cuenta no lo tiene cargado.</summary>
        private static string Cuenta(string? nombre, int? nroCuenta) =>
            nroCuenta > 0 ? $"{Valor(nombre)} - {nroCuenta}" : Valor(nombre);
    }
}
