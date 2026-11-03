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
        private const int PorHoja = PdfComun.Columnas * PdfComun.Filas;

        private readonly ComprobanteOpciones _opciones;

        static ComprobanteCuotaPdf()
        {
            // La licencia de QuestPDF se declara en un solo lugar (ver PdfComun).
            PdfComun.ConfigurarLicencia();
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
            pagina.DefaultTextStyle(estilo => estilo.FontFamily(PdfComun.Fuente).FontSize(9));
            pagina.Background().Svg(PdfComun.LineasDeCorte).FitArea();

            // Alto de cada fila: un tercio de la hoja (con un margen mínimo por redondeo).
            var altoFila = PageSizes.A4.Height / PdfComun.Filas - 0.01f;

            pagina.Content().Column(columna =>
            {
                for (var f = 0; f < PdfComun.Filas; f++)
                {
                    var fila = f;
                    columna.Item().Height(altoFila).Row(celdas =>
                    {
                        for (var c = 0; c < PdfComun.Columnas; c++)
                        {
                            var indice = fila * PdfComun.Columnas + c;
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
            var vence = $"Vence: {c.FechaVencimiento.ToString("dd/MM/yyyy", PdfComun.CulturaAr)}";
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
                    col.Item().PaddingTop(1, Unit.Millimetre).Element(x => FilaDatos(x, "Cuota :", $"{PdfComun.Meses[c.Mes - 1]} {c.Anio}", plan));
                    col.Item().PaddingTop(1, Unit.Millimetre).Element(x => FilaDatos(x, "Concepto :", c.Concepto, vence));

                    // Socio: "APELLIDO, Nombre" y domicilio.
                    col.Item().PaddingTop(3, Unit.Millimetre).Height(6, Unit.Millimetre).ScaleToFit()
                        .Text(PdfComun.NombreCompleto(c.Apellido, c.Nombre)).Bold().FontSize(12);
                    col.Item().Height(4.5f, Unit.Millimetre).ScaleToFit()
                        .Text(PdfComun.Domicilio(c.Calle, c.Altura, c.Ciudad)).FontSize(8.5f);

                    // Importe adeudado, en su recuadro.
                    col.Item().PaddingTop(2, Unit.Millimetre).Height(13, Unit.Millimetre)
                        .Border(0.8f).BorderColor(Colors.Black).CornerRadius(3, Unit.Millimetre)
                        .PaddingHorizontal(3, Unit.Millimetre).AlignMiddle().AlignRight()
                        .Text(PdfComun.Importe(c.Monto)).Bold().FontSize(18);

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
                                .Text((c.Apellido ?? string.Empty).ToUpper(PdfComun.CulturaAr)).Bold().FontSize(8).ClampLines(1);
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
    }
}
