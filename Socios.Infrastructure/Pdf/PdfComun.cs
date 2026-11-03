using System.Globalization;
using System.Text;
using QuestPDF.Infrastructure;

namespace Socios.Infrastructure.Pdf
{
    /// <summary>
    /// Elementos compartidos por los PDF de comprobantes (deuda y pago): licencia de QuestPDF,
    /// fuente, formato argentino, grilla de la hoja y líneas de corte. Así el diseño y la
    /// declaración de licencia viven en un solo lugar.
    /// </summary>
    internal static class PdfComun
    {
        // Lato viene incluida en QuestPDF: el PDF sale igual en Windows y en Linux sin depender
        // de las fuentes instaladas en el servidor (QuestPDF no usa las del sistema por defecto).
        public const string Fuente = "Lato";

        // Hoja A4 dividida en 2 columnas × 3 filas: comprobantes de 10,5 × 9,9 cm.
        public const int Columnas = 2;
        public const int Filas = 3;

        public static readonly CultureInfo CulturaAr = new("es-AR");

        public static readonly string[] Meses =
            { "ENE", "FEB", "MAR", "ABR", "MAY", "JUN", "JUL", "AGO", "SEP", "OCT", "NOV", "DIC" };

        // Líneas de corte punteadas de la hoja (SVG en mm sobre una A4 de 210 × 297).
        public static readonly string LineasDeCorte = ArmarLineasDeCorte();

        /// <summary>
        /// Declara la licencia de QuestPDF; hay que hacerlo antes de generar el primer documento.
        /// LICENCIA COMMUNITY: es gratuita SOLO si la organización dueña del software factura
        /// menos de USD 1.000.000 al año (ver https://www.questpdf.com/license/). Esta línea es
        /// esa declaración: si deja de cumplirse, hay que comprar una licencia paga y cambiarla.
        /// </summary>
        public static void ConfigurarLicencia()
        {
            QuestPDF.Settings.License = LicenseType.Community;
        }

        /// <summary>"APELLIDO, Nombre" (o solo el que haya).</summary>
        public static string NombreCompleto(string? apellido, string? nombre)
        {
            var ape = (apellido ?? string.Empty).ToUpper(CulturaAr).Trim();
            var nom = (nombre ?? string.Empty).Trim();
            return ape.Length > 0 && nom.Length > 0 ? $"{ape}, {nom}" : ape + nom;
        }

        /// <summary>"Calle Altura (Ciudad)", omitiendo lo que falte.</summary>
        public static string Domicilio(string? calle, int? altura, string? ciudad)
        {
            var direccion = $"{calle} {altura}".Trim();
            return string.IsNullOrWhiteSpace(ciudad) ? direccion : $"{direccion} ({ciudad})".Trim();
        }

        /// <summary>Importe con formato argentino: "$ 1.234,56".</summary>
        public static string Importe(decimal monto) => $"$ {monto.ToString("N2", CulturaAr)}";

        /// <summary>
        /// Arma el SVG de las líneas de corte entre comprobantes. Cada línea se dibuja como una
        /// serie de trazos cortos (en vez de usar stroke-dasharray, que el motor de QuestPDF no
        /// aplica a todas las líneas), así el punteado se ve siempre igual.
        /// </summary>
        private static string ArmarLineasDeCorte()
        {
            const float ancho = 210, alto = 297, trazo = 2, espacio = 1.5f;
            var svg = new StringBuilder();
            svg.Append("<svg xmlns=\"http://www.w3.org/2000/svg\" viewBox=\"0 0 210 297\">");
            svg.Append("<g stroke=\"#888888\" stroke-width=\"0.2\">");

            string N(float valor) => valor.ToString("0.##", CultureInfo.InvariantCulture);

            // Verticales (entre columnas).
            for (var c = 1; c < Columnas; c++)
            {
                var x = ancho / Columnas * c;
                for (float y = 0; y < alto; y += trazo + espacio)
                    svg.Append($"<line x1=\"{N(x)}\" y1=\"{N(y)}\" x2=\"{N(x)}\" y2=\"{N(Math.Min(y + trazo, alto))}\"/>");
            }

            // Horizontales (entre filas).
            for (var f = 1; f < Filas; f++)
            {
                var y = alto / Filas * f;
                for (float x = 0; x < ancho; x += trazo + espacio)
                    svg.Append($"<line x1=\"{N(x)}\" y1=\"{N(y)}\" x2=\"{N(Math.Min(x + trazo, ancho))}\" y2=\"{N(y)}\"/>");
            }

            svg.Append("</g></svg>");
            return svg.ToString();
        }
    }
}
