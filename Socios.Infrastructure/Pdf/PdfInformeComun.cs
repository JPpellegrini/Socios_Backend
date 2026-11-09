using System.Globalization;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace Socios.Infrastructure.Pdf
{
    /// <summary>
    /// Piezas comunes a los informes en PDF (A4 apaisada): configuración de la hoja,
    /// encabezado de la institución, pie con fecha y número de página y estilos de tabla.
    /// Así todos los informes se ven iguales.
    /// </summary>
    internal static class PdfInformeComun
    {
        // Misma fuente que los comprobantes: viene incluida en QuestPDF (ver ComprobanteCuotaPdf).
        public const string Fuente = "Lato";
        public const string SinDato = "-";

        public static readonly CultureInfo CulturaAr = new("es-AR");

        /// <summary>
        /// Misma declaración de licencia que en ComprobanteCuotaPdf (ver las condiciones allí).
        /// Cada informe la llama desde su constructor estático, porque puede ser el primer
        /// documento que se genere desde que arrancó la API.
        /// </summary>
        public static void DeclararLicencia() => QuestPDF.Settings.License = LicenseType.Community;

        /// <summary>
        /// Hoja A4 (apaisada por defecto) con márgenes, fuente y pie estándar de los informes.
        /// Los informes angostos (por ejemplo, un resumen de totales) pueden usar la hoja vertical.
        /// </summary>
        public static void ConfigurarHoja(PageDescriptor pagina, DateTime emision, bool apaisada = true)
        {
            pagina.Size(apaisada ? PageSizes.A4.Landscape() : PageSizes.A4);
            pagina.Margin(10, Unit.Millimetre);
            pagina.DefaultTextStyle(estilo => estilo.FontFamily(Fuente).FontSize(8.5f));
            pagina.Footer().Element(x => Pie(x, emision));
        }

        /// <summary>Nombre y datos de la institución (los mismos de los comprobantes).</summary>
        public static void Institucion(ColumnDescriptor col, ComprobanteOpciones opciones)
        {
            col.Item().Text(opciones.Institucion).Bold().FontSize(12);
            col.Item().Text($"{opciones.Domicilio}    Pers. Jurídica {opciones.PersoneriaJuridica}    Tel.: {opciones.Telefono}")
                .FontSize(8).FontColor(Colors.Grey.Darken2);
        }

        private static void Pie(IContainer contenedor, DateTime emision)
        {
            contenedor.PaddingTop(2, Unit.Millimetre).Row(fila =>
            {
                fila.RelativeItem().Text($"Emitido el {emision.ToString("dd/MM/yyyy HH:mm", CulturaAr)}")
                    .FontSize(7.5f).FontColor(Colors.Grey.Darken1);
                fila.RelativeItem().AlignRight().Text(texto =>
                {
                    texto.DefaultTextStyle(estilo => estilo.FontSize(7.5f).FontColor(Colors.Grey.Darken1));
                    texto.Span("Página ");
                    texto.CurrentPageNumber();
                    texto.Span(" de ");
                    texto.TotalPages();
                });
            });
        }

        public static IContainer CeldaTitulo(IContainer celda) =>
            celda.Background(Colors.Grey.Lighten2)
                .BorderBottom(0.8f).BorderColor(Colors.Black)
                .PaddingVertical(1.5f, Unit.Millimetre).PaddingHorizontal(1.2f, Unit.Millimetre)
                .DefaultTextStyle(estilo => estilo.Bold().FontSize(8));

        /// <summary>
        /// Celda de datos. Las filas alternan fondo blanco y gris claro. ShowEntire evita que una
        /// celda de dos renglones (ej. "TRANSFERENCIA BANCARIA") quede partida entre dos hojas:
        /// si no entra completa, la fila pasa entera a la hoja siguiente.
        /// </summary>
        public static IContainer Celda(IContainer celda, int fila) =>
            celda.ShowEntire()
                .Background(fila % 2 == 0 ? Colors.White : Colors.Grey.Lighten4)
                .BorderBottom(0.3f).BorderColor(Colors.Grey.Lighten1)
                .PaddingVertical(1.2f, Unit.Millimetre).PaddingHorizontal(1.2f, Unit.Millimetre);

        /// <summary>"APELLIDO, Nombre", o lo que haya si falta alguno de los dos.</summary>
        public static string NombreCompleto(string? apellido, string? nombre)
        {
            var a = (apellido ?? string.Empty).ToUpper(CulturaAr).Trim();
            var n = (nombre ?? string.Empty).Trim();
            return a.Length > 0 && n.Length > 0 ? $"{a}, {n}" : Valor(a + n);
        }

        public static string Domicilio(string? calle, string? altura) => Valor($"{calle} {altura}".Trim());

        public static string Fecha(DateTime? fecha) =>
            fecha?.ToString("dd/MM/yyyy", CulturaAr) ?? SinDato;

        public static string Valor(string? texto) =>
            string.IsNullOrWhiteSpace(texto) ? SinDato : texto.Trim();
    }
}
