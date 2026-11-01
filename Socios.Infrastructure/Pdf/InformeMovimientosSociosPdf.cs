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
    /// Genera el PDF del informe de altas y bajas de socios con QuestPDF.
    /// Diseño: A4 apaisada. Arriba, el resumen del período (totales y un gráfico de barras
    /// horizontales con las altas y las bajas por motivo); debajo, el detalle de movimientos.
    ///
    /// Se usan barras y no una torta: las altas y las bajas no son partes de un mismo total,
    /// y las barras permiten comparar cantidades a simple vista (cada una lleva su número).
    /// </summary>
    public class InformeMovimientosSociosPdf : IInformeMovimientosSociosPdf
    {
        // Azul para altas y naranja para bajas (se evita verde/rojo para no sugerir "bien/mal").
        // El par se distingue también con daltonismo; igualmente cada barra lleva su etiqueta.
        private const string ColorAltas = "#2a78d6";
        private const string ColorBajas = "#eb6834";

        private readonly ComprobanteOpciones _opciones;

        static InformeMovimientosSociosPdf() => DeclararLicencia();

        public InformeMovimientosSociosPdf(IOptions<ComprobanteOpciones> opciones)
        {
            // Se reutilizan los datos de la institución que ya se imprimen en los comprobantes.
            _opciones = opciones.Value.ConValoresPorDefecto();
        }

        public byte[] Generar(SocioMovimientosInformeDto informe)
        {
            var emision = DateTime.Now;

            return Document.Create(documento =>
                {
                    documento.Page(pagina =>
                    {
                        ConfigurarHoja(pagina, emision);
                        pagina.Header().Element(x => Encabezado(x, informe));
                        pagina.Content().PaddingTop(4, Unit.Millimetre).Column(col =>
                        {
                            col.Item().Element(x => Resumen(x, informe));
                            col.Item().PaddingTop(6, Unit.Millimetre)
                                .Text($"Detalle de movimientos ({informe.Movimientos.Count})").Bold().FontSize(11);
                            col.Item().PaddingTop(2, Unit.Millimetre).Element(x => Tabla(x, informe.Movimientos));
                        });
                    });
                })
                .WithMetadata(new DocumentMetadata { Title = "Altas y bajas de socios" })
                .GeneratePdf();
        }

        private void Encabezado(IContainer contenedor, SocioMovimientosInformeDto informe)
        {
            contenedor.Column(col =>
            {
                Institucion(col, _opciones);

                col.Item().PaddingTop(3, Unit.Millimetre).Text("Altas y bajas de socios").Bold().FontSize(15);

                col.Item().PaddingTop(1, Unit.Millimetre).Text(texto =>
                {
                    texto.Span("Período: ").FontColor(Colors.Grey.Darken2);
                    texto.Span($"{Fecha(informe.FechaDesde)} al {Fecha(informe.FechaHasta)}").Bold();
                    texto.Span("    Movimientos: ").FontColor(Colors.Grey.Darken2);
                    texto.Span(DescripcionTipo(informe.Tipo)).Bold();
                });

                col.Item().PaddingTop(2, Unit.Millimetre).LineHorizontal(0.8f).LineColor(Colors.Black);
            });
        }

        // ---------------------------------------------------------------- Resumen y gráfico

        private static void Resumen(IContainer contenedor, SocioMovimientosInformeDto informe)
        {
            var conAltas = informe.Tipo != "BAJAS";
            var conBajas = informe.Tipo != "ALTAS";

            contenedor.Row(fila =>
            {
                // Totales del período, uno debajo del otro.
                fila.ConstantItem(62, Unit.Millimetre).Column(totales =>
                {
                    totales.Spacing(2, Unit.Millimetre);
                    if (conAltas)
                        totales.Item().Element(x => Indicador(x, "Altas", informe.Altas.ToString(), ColorAltas, null));
                    if (conBajas)
                        totales.Item().Element(x => Indicador(x, "Bajas", informe.Bajas.ToString(), ColorBajas, null));
                    if (conAltas && conBajas)
                        totales.Item().Element(x => Indicador(x, "Variación neta",
                            informe.VariacionNeta > 0 ? $"+{informe.VariacionNeta}" : informe.VariacionNeta.ToString(),
                            null, "altas menos bajas"));
                });

                // Con solo altas hay un único número: el gráfico no agrega nada.
                if (conBajas)
                    fila.RelativeItem().PaddingLeft(10, Unit.Millimetre).Element(x => Grafico(x, informe, conAltas));
            });
        }

        /// <summary>Recuadro con un total: etiqueta (con su color, si es una serie del gráfico) y número grande.</summary>
        private static void Indicador(IContainer contenedor, string etiqueta, string valor, string? color, string? aclaracion)
        {
            contenedor
                .Border(0.5f).BorderColor(Colors.Grey.Lighten1).CornerRadius(2, Unit.Millimetre)
                .PaddingVertical(2, Unit.Millimetre).PaddingHorizontal(3, Unit.Millimetre)
                .Row(fila =>
                {
                    fila.RelativeItem().AlignMiddle().Row(titulo =>
                    {
                        if (color is not null)
                            titulo.ConstantItem(3, Unit.Millimetre).AlignMiddle()
                                .Height(3, Unit.Millimetre).Background(color).CornerRadius(0.6f, Unit.Millimetre);
                        titulo.RelativeItem().PaddingLeft(color is null ? 0 : 2, Unit.Millimetre).Column(textos =>
                        {
                            textos.Item().Text(etiqueta).FontSize(9.5f).FontColor(Colors.Grey.Darken3);
                            if (aclaracion is not null)
                                textos.Item().Text(aclaracion).FontSize(7).FontColor(Colors.Grey.Darken1);
                        });
                    });
                    fila.AutoItem().AlignMiddle().Text(valor).Bold().FontSize(18);
                });
        }

        /// <summary>
        /// Barras horizontales sobre una misma escala: una de altas (si corresponde) y una por
        /// cada motivo de baja. Cada barra lleva su cantidad; las de bajas, además, su porcentaje
        /// sobre el total de bajas.
        /// </summary>
        private static void Grafico(IContainer contenedor, SocioMovimientosInformeDto informe, bool conAltas)
        {
            var barras = new List<(string Etiqueta, int Cantidad, string Valor, string Color)>();
            if (conAltas)
                barras.Add(("Altas", informe.Altas, informe.Altas.ToString(), ColorAltas));

            foreach (var motivo in informe.BajasPorMotivo)
            {
                var porcentaje = informe.Bajas == 0 ? 0 : (double)motivo.Cantidad / informe.Bajas;
                barras.Add(($"Baja · {NombreMotivo(motivo.Motivo)}", motivo.Cantidad,
                    $"{motivo.Cantidad}  ({porcentaje.ToString("P0", CulturaAr)})", ColorBajas));
            }

            var maximo = barras.Max(b => b.Cantidad);

            contenedor.Column(col =>
            {
                col.Item().Text(conAltas ? "Altas y bajas por motivo" : "Bajas por motivo").Bold().FontSize(10);
                col.Item().PaddingBottom(2, Unit.Millimetre)
                    .Text("Porcentajes de bajas calculados sobre el total de bajas del período.")
                    .FontSize(7).FontColor(Colors.Grey.Darken1);

                col.Spacing(1.5f, Unit.Millimetre);
                foreach (var barra in barras)
                {
                    col.Item().Row(fila =>
                    {
                        fila.ConstantItem(38, Unit.Millimetre).AlignMiddle().Text(barra.Etiqueta).FontColor(Colors.Grey.Darken3);

                        // Pista de la barra: la parte coloreada es proporcional a la cantidad
                        // respecto de la barra más larga. Una línea de base marca el cero.
                        fila.RelativeItem().AlignMiddle().BorderLeft(0.6f).BorderColor(Colors.Grey.Darken1)
                            .Height(5, Unit.Millimetre).Row(pista =>
                            {
                                if (barra.Cantidad > 0)
                                    pista.RelativeItem(barra.Cantidad).Background(barra.Color).CornerRadius(1, Unit.Millimetre);
                                if (barra.Cantidad < maximo)
                                    pista.RelativeItem(maximo - barra.Cantidad);
                                if (maximo == 0)
                                    pista.RelativeItem();
                            });

                        fila.ConstantItem(26, Unit.Millimetre).PaddingLeft(2, Unit.Millimetre).AlignMiddle()
                            .Text(barra.Valor).Bold();
                    });
                }
            });
        }

        // ---------------------------------------------------------------- Detalle

        private static void Tabla(IContainer contenedor, IReadOnlyList<SocioMovimientoDto> movimientos)
        {
            contenedor.Table(tabla =>
            {
                // Anchos relativos pensados para A4 apaisada (277 mm útiles).
                tabla.ColumnsDefinition(columnas =>
                {
                    columnas.RelativeColumn(16);  // Apellido y nombre
                    columnas.RelativeColumn(12);  // Domicilio
                    columnas.RelativeColumn(8);   // Ciudad
                    columnas.RelativeColumn(8);   // Teléfono
                    columnas.RelativeColumn(9);   // Obra social
                    columnas.RelativeColumn(4.5f); // Edad
                    columnas.RelativeColumn(5.5f); // Sepelio
                    columnas.RelativeColumn(5);   // Nicho
                    columnas.RelativeColumn(7.5f); // Fecha alta
                    columnas.RelativeColumn(7.5f); // Fecha baja
                    columnas.RelativeColumn(14);  // Movimiento
                });

                // El encabezado de la tabla se repite automáticamente en cada hoja.
                tabla.Header(encabezado =>
                {
                    foreach (var titulo in new[] { "Apellido y nombre", "Domicilio", "Ciudad", "Teléfono", "Obra social" })
                        CeldaTitulo(encabezado.Cell()).Text(titulo);

                    foreach (var titulo in new[] { "Edad", "Sepelio", "Nicho", "Fecha alta", "Fecha baja" })
                        CeldaTitulo(encabezado.Cell()).AlignCenter().Text(titulo);

                    CeldaTitulo(encabezado.Cell()).Text("Movimiento");
                });

                for (var i = 0; i < movimientos.Count; i++)
                {
                    var m = movimientos[i];
                    var esAlta = m.Movimiento == "ALTA";

                    Celda(tabla.Cell(), i).Text(NombreCompleto(m.Apellido, m.Nombre)).Bold();
                    Celda(tabla.Cell(), i).Text(Domicilio(m.Calle, m.Altura));
                    Celda(tabla.Cell(), i).Text(Valor(m.Ciudad));
                    Celda(tabla.Cell(), i).Text(Valor(m.Telefono));
                    Celda(tabla.Cell(), i).Text(Valor(m.ObraSocial));
                    Celda(tabla.Cell(), i).AlignCenter().Text(m.Edad?.ToString() ?? SinDato);
                    Celda(tabla.Cell(), i).AlignCenter().Text(m.Sepelio ? "SI" : "NO");
                    Celda(tabla.Cell(), i).AlignCenter().Text(m.Nicho ? "SI" : "NO");
                    Celda(tabla.Cell(), i).AlignCenter().Text(Fecha(m.FechaAlta));
                    Celda(tabla.Cell(), i).AlignCenter().Text(Fecha(m.FechaBaja));

                    // Mismo color que en el gráfico, junto al texto (el texto no depende del color).
                    Celda(tabla.Cell(), i).Row(fila =>
                    {
                        fila.ConstantItem(2.5f, Unit.Millimetre).AlignMiddle().Height(2.5f, Unit.Millimetre)
                            .Background(esAlta ? ColorAltas : ColorBajas).CornerRadius(0.5f, Unit.Millimetre);
                        fila.RelativeItem().PaddingLeft(1.5f, Unit.Millimetre)
                            .Text(esAlta ? "ALTA" : $"BAJA · {Valor(m.MotivoBaja)}").Bold();
                    });
                }
            });
        }

        private static string DescripcionTipo(string tipo) => tipo switch
        {
            "ALTAS" => "Solo altas",
            "BAJAS" => "Solo bajas",
            _ => "Altas y bajas"
        };

        /// <summary>"FALLECIMIENTO" → "Fallecimiento" (más legible en las etiquetas del gráfico).</summary>
        private static string NombreMotivo(string motivo) =>
            motivo.Length == 0 ? SinDato : motivo[0] + motivo[1..].ToLower(CulturaAr);
    }
}
