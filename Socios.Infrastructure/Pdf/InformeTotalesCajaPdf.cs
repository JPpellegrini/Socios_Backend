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
    /// Genera el PDF del informe totalizado de ingresos y egresos con QuestPDF.
    /// Diseño: A4 vertical (es un resumen angosto); encabezado con la institución; franja con el
    /// período; con el filtro "Todos", tres recuadros (ingresos, egresos, resultado); luego una
    /// tarjeta de INGRESOS y otra de EGRESOS con el total por actividad, su participación en la
    /// sección (barra + porcentaje) y el total de la sección. Los egresos se muestran en negativo.
    /// </summary>
    public class InformeTotalesCajaPdf : IInformeTotalesCajaPdf
    {
        // Mismos colores que el informe detallado de ingresos y egresos.
        private const string ColorIngresos = "#2a78d6";
        private const string ColorEgresos = "#eb6834";
        private const string ColorPista = "#eef1f4";

        private static readonly string[] Meses =
        {
            "ENERO", "FEBRERO", "MARZO", "ABRIL", "MAYO", "JUNIO",
            "JULIO", "AGOSTO", "SEPTIEMBRE", "OCTUBRE", "NOVIEMBRE", "DICIEMBRE"
        };

        private readonly ComprobanteOpciones _opciones;

        static InformeTotalesCajaPdf() => DeclararLicencia();

        public InformeTotalesCajaPdf(IOptions<ComprobanteOpciones> opciones)
        {
            // Se reutilizan los datos de la institución que ya se imprimen en los comprobantes.
            _opciones = opciones.Value.ConValoresPorDefecto();
        }

        public byte[] Generar(TotalesCajaInformeDto informe)
        {
            var emision = DateTime.Now;

            return Document.Create(documento =>
                {
                    documento.Page(pagina =>
                    {
                        ConfigurarHoja(pagina, emision, apaisada: false);
                        pagina.Header().Element(x => Encabezado(x, informe));
                        pagina.Content().PaddingTop(5, Unit.Millimetre).Column(col =>
                        {
                            col.Spacing(5, Unit.Millimetre);

                            col.Item().Element(x => FranjaPeriodo(x, informe));

                            if (informe.SinMovimientos)
                            {
                                col.Item().PaddingTop(8, Unit.Millimetre).AlignCenter()
                                    .Text("Sin movimientos con los criterios seleccionados.")
                                    .FontSize(11).FontColor(Colors.Grey.Darken2);
                                return;
                            }

                            if (informe.Resultado.HasValue)
                                col.Item().Element(x => Indicadores(x, informe));

                            if (informe.IncluyeIngresos)
                                col.Item().ShowEntire().Element(x => Seccion(x, "INGRESOS", informe.Ingresos,
                                    informe.TotalIngresos, informe.CantidadIngresos, ColorIngresos, negativo: false));

                            if (informe.IncluyeEgresos)
                                col.Item().ShowEntire().Element(x => Seccion(x, "EGRESOS", informe.Egresos,
                                    informe.TotalEgresos, informe.CantidadEgresos, ColorEgresos, negativo: true));

                            if (informe.Resultado.HasValue)
                                col.Item().ShowEntire().Element(x => Resultado(x, informe.Resultado.Value));

                            if (informe.MovimientosSinTipoValido > 0)
                                col.Item().Text($"Atención: {informe.MovimientosSinTipoValido} movimiento(s) del período con un " +
                                                "tipo distinto de INGRESO o EGRESO no se incluyen en los totales.")
                                    .FontSize(8).Italic().FontColor(Colors.Red.Darken2);
                        });
                    });
                })
                .WithMetadata(new DocumentMetadata { Title = "Ingresos y egresos totalizados" })
                .GeneratePdf();
        }

        private void Encabezado(IContainer contenedor, TotalesCajaInformeDto informe)
        {
            contenedor.Column(col =>
            {
                Institucion(col, _opciones);
                col.Item().PaddingTop(3, Unit.Millimetre).Text("Ingresos y egresos totalizados").Bold().FontSize(15);
                col.Item().PaddingTop(2, Unit.Millimetre).LineHorizontal(0.8f).LineColor(Colors.Black);
            });
        }

        /// <summary>Franja con el período (en meses si abarca meses completos) y el tipo de movimiento.</summary>
        private static void FranjaPeriodo(IContainer contenedor, TotalesCajaInformeDto informe)
        {
            var (desde, hasta) = TextoPeriodo(informe.FechaDesde, informe.FechaHasta);

            contenedor.Background(Colors.Grey.Lighten4).CornerRadius(2, Unit.Millimetre)
                .PaddingVertical(3, Unit.Millimetre).PaddingHorizontal(5, Unit.Millimetre)
                .Row(fila =>
                {
                    Dato(fila.RelativeItem(), "DESDE", desde);
                    Dato(fila.RelativeItem(), "HASTA", hasta);
                    Dato(fila.RelativeItem(), "MOVIMIENTOS", DescripcionTipo(informe.TipoMovimiento));
                });

            static void Dato(IContainer c, string etiqueta, string valor) => c.Column(d =>
            {
                d.Item().Text(etiqueta).FontSize(7).FontColor(Colors.Grey.Darken1).LetterSpacing(0.05f);
                d.Item().Text(valor).Bold().FontSize(11);
            });
        }

        /// <summary>Tres recuadros: total de ingresos, total de egresos y resultado.</summary>
        private static void Indicadores(IContainer contenedor, TotalesCajaInformeDto informe)
        {
            contenedor.Row(fila =>
            {
                Indicador(fila.RelativeItem(), "Ingresos", Importe(informe.TotalIngresos), ColorIngresos);
                fila.ConstantItem(3, Unit.Millimetre);
                Indicador(fila.RelativeItem(), "Egresos", Importe(-informe.TotalEgresos), ColorEgresos);
                fila.ConstantItem(3, Unit.Millimetre);
                Indicador(fila.RelativeItem(), "Resultado", Importe(informe.Resultado!.Value), Colors.Grey.Darken2);
            });

            static void Indicador(IContainer c, string titulo, string valor, string color) =>
                c.Border(0.6f).BorderColor(Colors.Grey.Lighten2).CornerRadius(2, Unit.Millimetre).Row(r =>
                {
                    r.ConstantItem(1.6f, Unit.Millimetre).Background(color);
                    r.RelativeItem().PaddingVertical(2.5f, Unit.Millimetre).PaddingHorizontal(3, Unit.Millimetre).Column(t =>
                    {
                        t.Item().Text(titulo).FontSize(8).FontColor(Colors.Grey.Darken2);
                        t.Item().Text(valor).Bold().FontSize(12.5f);
                    });
                });
        }

        /// <summary>
        /// Tarjeta de una sección (INGRESOS o EGRESOS): una fila por actividad con su cantidad de
        /// movimientos, una barra con su participación en la sección y el importe; al pie, el total.
        /// </summary>
        private static void Seccion(IContainer contenedor, string titulo, List<TotalActividadDto> filas,
            decimal total, int cantidad, string color, bool negativo)
        {
            contenedor.Border(0.6f).BorderColor(Colors.Grey.Lighten2).CornerRadius(2.5f, Unit.Millimetre)
                .Column(tarjeta =>
                {
                    // Título de la tarjeta con una franja del color de la serie.
                    tarjeta.Item().Background(Colors.Grey.Lighten4).PaddingVertical(2.5f, Unit.Millimetre)
                        .PaddingHorizontal(5, Unit.Millimetre).Row(t =>
                        {
                            t.ConstantItem(3, Unit.Millimetre).AlignMiddle().Height(3, Unit.Millimetre)
                                .Background(color).CornerRadius(1.5f, Unit.Millimetre);
                            t.RelativeItem().PaddingLeft(2, Unit.Millimetre).Text(titulo).Bold().FontSize(11).LetterSpacing(0.05f);
                            t.AutoItem().AlignMiddle().Text($"{cantidad} movimiento{(cantidad == 1 ? "" : "s")}")
                                .FontSize(8).FontColor(Colors.Grey.Darken1);
                        });

                    tarjeta.Item().PaddingHorizontal(5, Unit.Millimetre).PaddingVertical(2, Unit.Millimetre).Column(lista =>
                    {
                        if (filas.Count == 0)
                        {
                            lista.Item().PaddingVertical(2, Unit.Millimetre)
                                .Text($"Sin {titulo.ToLower(CulturaAr)} en el período.").FontColor(Colors.Grey.Darken1);
                            return;
                        }

                        foreach (var f in filas)
                        {
                            lista.Item().BorderBottom(0.3f).BorderColor(Colors.Grey.Lighten2)
                                .PaddingVertical(2, Unit.Millimetre).Row(fila =>
                                {
                                    fila.RelativeItem(10).AlignMiddle().Column(a =>
                                    {
                                        a.Item().Text(f.Actividad).FontSize(9.5f).SemiBold();
                                        a.Item().Text($"{f.Cantidad} mov.").FontSize(7).FontColor(Colors.Grey.Darken1);
                                    });

                                    // Participación en la sección: barra sobre una pista gris + porcentaje.
                                    fila.RelativeItem(7).AlignMiddle().Row(p =>
                                    {
                                        var porcentaje = (float)f.Porcentaje;
                                        p.RelativeItem().AlignMiddle().Height(2.4f, Unit.Millimetre).Background(ColorPista)
                                            .CornerRadius(1.2f, Unit.Millimetre).Row(pista =>
                                            {
                                                if (porcentaje > 0)
                                                    pista.RelativeItem(Math.Max(porcentaje, 0.01f)).Background(color)
                                                        .CornerRadius(1.2f, Unit.Millimetre);
                                                if (porcentaje < 1)
                                                    pista.RelativeItem(1 - Math.Min(Math.Max(porcentaje, 0.01f), 1));
                                            });
                                        p.ConstantItem(13, Unit.Millimetre).AlignMiddle().AlignRight()
                                            .Text(Porcentaje(f.Porcentaje)).FontSize(8).FontColor(Colors.Grey.Darken2);
                                    });

                                    fila.RelativeItem(7).AlignMiddle().AlignRight()
                                        .Text(Importe(negativo ? -f.Total : f.Total)).FontSize(10);
                                });
                        }

                        // Total de la sección.
                        lista.Item().PaddingTop(2.5f, Unit.Millimetre).Row(fila =>
                        {
                            fila.RelativeItem().Text($"TOTAL DE {titulo}").Bold().FontSize(10);
                            fila.AutoItem().Text(Importe(negativo ? -total : total)).Bold().FontSize(12);
                        });
                    });
                });
        }

        /// <summary>Resultado del período: ingresos menos egresos.</summary>
        private static void Resultado(IContainer contenedor, decimal resultado)
        {
            contenedor.Border(1).BorderColor(Colors.Grey.Darken2).CornerRadius(2.5f, Unit.Millimetre)
                .PaddingVertical(3.5f, Unit.Millimetre).PaddingHorizontal(5, Unit.Millimetre).Row(fila =>
                {
                    fila.RelativeItem().Column(t =>
                    {
                        t.Item().Text("RESULTADO DEL PERÍODO").Bold().FontSize(11).LetterSpacing(0.05f);
                        t.Item().Text("Total de ingresos menos total de egresos").FontSize(7.5f).FontColor(Colors.Grey.Darken1);
                    });
                    fila.AutoItem().AlignMiddle().Text(Importe(resultado)).Bold().FontSize(15);
                });
        }

        /// <summary>
        /// Si el período abarca meses completos, se muestra en meses ("ENERO 2025" a "DICIEMBRE 2025");
        /// si no, con las fechas exactas.
        /// </summary>
        private static (string Desde, string Hasta) TextoPeriodo(DateTime desde, DateTime hasta)
        {
            var mesesCompletos = desde.Day == 1 && hasta.Day == DateTime.DaysInMonth(hasta.Year, hasta.Month);
            return mesesCompletos
                ? ($"{Meses[desde.Month - 1]} {desde.Year}", $"{Meses[hasta.Month - 1]} {hasta.Year}")
                : (Fecha(desde), Fecha(hasta));
        }

        /// <summary>
        /// Participación legible: sin decimales entre 10 % y 99 %; con un decimal fuera de ese rango
        /// para que una participación chica no figure como "0 %" ni un 99,8 % como "100 %".
        /// </summary>
        private static string Porcentaje(decimal proporcion)
        {
            if (proporcion <= 0) return "0 %";
            if (proporcion < 0.001m) return "< 0,1 %";
            return proporcion is >= 0.1m and < 0.99m
                ? proporcion.ToString("P0", CulturaAr)
                : proporcion.ToString("P1", CulturaAr);
        }

        /// <summary>"$ 1.234,56" o "-$ 1.234,56".</summary>
        private static string Importe(decimal monto) =>
            monto < 0 ? $"-{PdfComun.Importe(-monto)}" : PdfComun.Importe(monto);

        private static string DescripcionTipo(string tipo) => tipo switch
        {
            MovimientosCajaFiltroDto.Ingresos => "Ingresos",
            MovimientosCajaFiltroDto.Egresos => "Egresos",
            _ => "Todos"
        };
    }
}
