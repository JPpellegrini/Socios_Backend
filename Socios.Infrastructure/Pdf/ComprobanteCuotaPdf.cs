using System.Globalization;
using Microsoft.Extensions.Options;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using Socios.Application.DTOs;
using Socios.Application.Interfaces;

namespace Socios.Infrastructure.Pdf
{
    /// <summary>
    /// Genera el PDF de comprobantes de cuotas adeudadas con QuestPDF.
    /// Diseño: hojas A4 con 6 comprobantes (2 columnas × 3 filas, de 10,5 × 9,9 cm cada uno)
    /// separados por líneas de corte, siguiendo el formato del comprobante del sistema viejo.
    /// </summary>
    public class ComprobanteCuotaPdf : IComprobanteCuotaPdf
    {
        private const int Columnas = 2;
        private const int Filas = 3;
        private const int PorHoja = Columnas * Filas;

        // Lato viene incluida en QuestPDF: el PDF sale igual en Windows y en Linux sin depender
        // de las fuentes instaladas en el servidor (QuestPDF no usa las del sistema por defecto).
        private const string Fuente = "Lato";

        // Líneas de corte punteadas de la hoja (SVG en mm sobre una A4 de 210 × 297).
        private static readonly string LineasDeCorte = ArmarLineasDeCorte();

        private static readonly CultureInfo CulturaAr = new("es-AR");
        private static readonly string[] Meses =
            { "ENE", "FEB", "MAR", "ABR", "MAY", "JUN", "JUL", "AGO", "SEP", "OCT", "NOV", "DIC" };

        private readonly ComprobanteOpciones _opciones;

        static ComprobanteCuotaPdf()
        {
            // QuestPDF exige declarar la licencia antes de generar el primer documento.
            // LICENCIA COMMUNITY: es gratuita SOLO si la organización dueña del software factura
            // menos de USD 1.000.000 al año (ver https://www.questpdf.com/license/). Esta línea es
            // esa declaración: si deja de cumplirse, hay que comprar una licencia paga y cambiarla.
            QuestPDF.Settings.License = LicenseType.Community;
        }

        public ComprobanteCuotaPdf(IOptions<ComprobanteOpciones> opciones)
        {
            // Lo que no venga en el appsettings toma los datos del Centro.
            _opciones = opciones.Value.ConValoresPorDefecto();
        }

        public byte[] Generar(IReadOnlyList<CuotaComprobanteDto> comprobantes)
        {
            var hojas = comprobantes.Chunk(PorHoja).ToList();

            return Document.Create(documento =>
                {
                    foreach (var hoja in hojas)
                        documento.Page(pagina => DibujarHoja(pagina, hoja));
                })
                .WithMetadata(new DocumentMetadata { Title = "Comprobantes de cuotas adeudadas" })
                .GeneratePdf();
        }

        private void DibujarHoja(PageDescriptor pagina, CuotaComprobanteDto[] hoja)
        {
            pagina.Size(PageSizes.A4);
            pagina.Margin(0);
            pagina.DefaultTextStyle(estilo => estilo.FontFamily(Fuente).FontSize(9));
            pagina.Background().Svg(LineasDeCorte).FitArea();

            // Alto de cada fila: un tercio de la hoja (con un margen mínimo por redondeo).
            var altoFila = PageSizes.A4.Height / Filas - 0.01f;

            pagina.Content().Column(columna =>
            {
                for (var f = 0; f < Filas; f++)
                {
                    var fila = f;
                    columna.Item().Height(altoFila).Row(celdas =>
                    {
                        for (var c = 0; c < Columnas; c++)
                        {
                            var indice = fila * Columnas + c;
                            var celda = celdas.RelativeItem();
                            if (indice < hoja.Length)
                                DibujarComprobante(celda, hoja[indice]);
                        }
                    });
                }
            });
        }

        private void DibujarComprobante(IContainer celda, CuotaComprobanteDto c)
        {
            var nicho = c.Nicho is null ? "NO NICHO" : $"NICHO: {c.Nicho}";
            var plan = $"Plan: {(string.IsNullOrWhiteSpace(c.Plan) ? "-" : c.Plan)}";
            var vence = $"Vence: {c.FechaVencimiento.ToString("dd/MM/yyyy", CulturaAr)}";
            var datosInstitucion = $"{_opciones.Domicilio}    Pers. Jurídica {_opciones.PersoneriaJuridica}    Tel.: {_opciones.Telefono}";

            celda
                .Padding(5, Unit.Millimetre)
                .Border(0.8f).BorderColor(Colors.Black).CornerRadius(4, Unit.Millimetre)
                .Padding(4, Unit.Millimetre)
                .Column(col =>
                {
                    // Encabezado de la institución. Los textos que pueden ser largos van en una
                    // altura fija con ScaleToFit: si no entran en una línea, se achican.
                    col.Item().Height(5, Unit.Millimetre).ScaleToFit()
                        .Text(_opciones.Institucion).Bold().FontSize(10.5f);
                    col.Item().Height(4, Unit.Millimetre).ScaleToFit()
                        .Text(datosInstitucion).FontSize(8);

                    // Datos de la cuota: columna izquierda (socio, período, concepto) y derecha.
                    col.Item().PaddingTop(2.5f, Unit.Millimetre).Element(x => FilaDatos(x, "Socio :", c.IdSocio.ToString("D5"), nicho));
                    col.Item().PaddingTop(1, Unit.Millimetre).Element(x => FilaDatos(x, "Cuota :", $"{Meses[c.Mes - 1]} {c.Anio}", plan));
                    col.Item().PaddingTop(1, Unit.Millimetre).Element(x => FilaDatos(x, "Concepto :", c.Concepto, vence));

                    // Socio: "APELLIDO, Nombre" y domicilio.
                    col.Item().PaddingTop(3, Unit.Millimetre).Height(6, Unit.Millimetre).ScaleToFit()
                        .Text(NombreCompleto(c)).Bold().FontSize(12);
                    col.Item().Height(4.5f, Unit.Millimetre).ScaleToFit()
                        .Text(Domicilio(c)).FontSize(8.5f);

                    // Importe adeudado, en su recuadro.
                    col.Item().PaddingTop(2, Unit.Millimetre).Height(13, Unit.Millimetre)
                        .Border(0.8f).BorderColor(Colors.Black).CornerRadius(3, Unit.Millimetre)
                        .PaddingHorizontal(3, Unit.Millimetre).AlignMiddle().AlignRight()
                        .Text($"$ {c.Monto.ToString("N2", CulturaAr)}").Bold().FontSize(18);

                    // Pie: ocupa el espacio que queda y se alinea abajo, así la leyenda y el número
                    // de comprobante quedan contra el borde inferior DENTRO del mismo comprobante.
                    // (Un elemento vacío que se estire ANTES del pie lo empujaría a la hoja siguiente.)
                    col.Item().ExtendVertical().AlignBottom().Column(pie =>
                    {
                        foreach (var linea in _opciones.Leyenda)
                            pie.Item().Text(linea).FontSize(7).ClampLines(1);

                        pie.Item().PaddingTop(1, Unit.Millimetre).Row(fila =>
                        {
                            fila.RelativeItem().Text($"Comprobante N° {c.IdCuota}").FontSize(7);
                            fila.RelativeItem().AlignRight()
                                .Text((c.Apellido ?? string.Empty).ToUpper(CulturaAr)).Bold().FontSize(8).ClampLines(1);
                        });
                    });
                });
        }

        /// <summary>
        /// Una línea de datos: a la izquierda la etiqueta (normal) y el valor (negrita), a la
        /// derecha un dato complementario. Si algo no entra, se recorta con "…".
        /// </summary>
        private static void FilaDatos(IContainer contenedor, string etiqueta, string valor, string derecha)
        {
            contenedor.Row(fila =>
            {
                fila.RelativeItem(58).Row(izquierda =>
                {
                    izquierda.ConstantItem(17, Unit.Millimetre).Text(etiqueta);
                    izquierda.RelativeItem().Text(valor).Bold().FontSize(9.5f).ClampLines(1);
                });
                fila.RelativeItem(42).Text(derecha).ClampLines(1);
            });
        }

        /// <summary>
        /// Arma el SVG de las líneas de corte entre comprobantes. Cada línea se dibuja como una
        /// serie de trazos cortos (en vez de usar stroke-dasharray, que el motor de QuestPDF no
        /// aplica a todas las líneas), así el punteado se ve siempre igual.
        /// </summary>
        private static string ArmarLineasDeCorte()
        {
            const float ancho = 210, alto = 297, trazo = 2, espacio = 1.5f;
            var svg = new System.Text.StringBuilder();
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

        private static string NombreCompleto(CuotaComprobanteDto c)
        {
            var apellido = (c.Apellido ?? string.Empty).ToUpper(CulturaAr).Trim();
            var nombre = (c.Nombre ?? string.Empty).Trim();
            return apellido.Length > 0 && nombre.Length > 0 ? $"{apellido}, {nombre}" : apellido + nombre;
        }

        private static string Domicilio(CuotaComprobanteDto c)
        {
            var calle = $"{c.Calle} {c.Altura}".Trim();
            return string.IsNullOrWhiteSpace(c.Ciudad) ? calle : $"{calle} ({c.Ciudad})".Trim();
        }
    }
}
