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
    /// Genera el PDF del reporte de ingresos y egresos con QuestPDF.
    /// Diseño: A4 apaisada; encabezado con la institución, el período y los filtros; recuadros con
    /// total de ingresos, de egresos y saldo; luego cada análisis como gráfico + cuadro con los
    /// importes exactos, lado a lado: por actividad (siempre) y por mes (solo si el período abarca
    /// más de un mes); al final el detalle con un movimiento por fila (el encabezado se repite en
    /// cada hoja). Si no hay movimientos, se muestra un aviso.
    /// </summary>
    public class InformeMovimientosCajaPdf : IInformeMovimientosCajaPdf
    {
        // Azul / naranja: se distinguen también con daltonismo y no sugieren "bien / mal"
        // como verde / rojo (un egreso no es algo malo en sí).
        private const string ColorIngresos = "#2a78d6";
        private const string ColorEgresos = "#eb6834";
        private const string ColorGuia = "#e4e7eb";

        private const float AltoColumnas = 42;  // mm del área de columnas del gráfico mensual
        private const int DivisionesEscala = 4; // líneas de guía del gráfico mensual
        private const float AltoBarra = 3.4f;   // mm de cada barra del gráfico por actividad

        private readonly ComprobanteOpciones _opciones;

        static InformeMovimientosCajaPdf() => DeclararLicencia();

        public InformeMovimientosCajaPdf(IOptions<ComprobanteOpciones> opciones)
        {
            // Se reutilizan los datos de la institución que ya se imprimen en los comprobantes.
            _opciones = opciones.Value.ConValoresPorDefecto();
        }

        public byte[] Generar(MovimientosCajaInformeDto informe)
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
                            if (informe.Cantidad == 0)
                            {
                                col.Item().PaddingTop(10, Unit.Millimetre).AlignCenter()
                                    .Text("Sin movimientos con los criterios seleccionados.")
                                    .FontSize(11).FontColor(Colors.Grey.Darken2);
                                return;
                            }

                            col.Item().Element(x => Totales(x, informe));

                            if (informe.MovimientosSinTipoValido > 0)
                                col.Item().PaddingTop(2, Unit.Millimetre)
                                    .Text($"Atención: {informe.MovimientosSinTipoValido} movimiento(s) con un tipo distinto de " +
                                          "INGRESO o EGRESO no suman en los totales.")
                                    .FontSize(8).Italic().FontColor(Colors.Red.Darken2);

                            // Por actividad: gráfico + cuadro. ShowEntire evita partirlo entre hojas.
                            var actividades = informe.PorActividad;
                            col.Item().PaddingTop(6, Unit.Millimetre).ShowEntire().Column(seccion =>
                            {
                                seccion.Item().Element(x => TituloSeccion(x, "Importes por actividad", informe));
                                seccion.Item().PaddingTop(2.5f, Unit.Millimetre).Row(fila =>
                                {
                                    fila.RelativeItem(11).Element(x => GraficoPorActividad(x, actividades));
                                    fila.ConstantItem(8, Unit.Millimetre);
                                    fila.RelativeItem(10).Element(x => TablaSubtotales(x, "Actividad", actividades));
                                });
                            });

                            // Por mes: solo si el período abarca más de un mes.
                            var meses = informe.PorMes;
                            if (meses.Count > 1)
                                col.Item().PaddingTop(7, Unit.Millimetre).ShowEntire().Column(seccion =>
                                {
                                    seccion.Item().Element(x => TituloSeccion(x, "Evolución mensual", informe));
                                    seccion.Item().PaddingTop(2.5f, Unit.Millimetre).Row(fila =>
                                    {
                                        fila.RelativeItem(11).Element(x => GraficoPorMes(x, informe, meses));
                                        fila.ConstantItem(8, Unit.Millimetre);
                                        fila.RelativeItem(10).Element(x => TablaSubtotales(x, "Mes", meses));
                                    });
                                });

                            col.Item().PageBreak();
                            col.Item().Text("Detalle de movimientos").Bold().FontSize(11);
                            col.Item().PaddingTop(1.5f, Unit.Millimetre).Element(x => TablaDetalle(x, informe.Movimientos));
                        });
                    });
                })
                .WithMetadata(new DocumentMetadata { Title = "Ingresos y egresos" })
                .GeneratePdf();
        }

        private void Encabezado(IContainer contenedor, MovimientosCajaInformeDto informe)
        {
            var cantidad = informe.Cantidad;

            contenedor.Column(col =>
            {
                Institucion(col, _opciones);

                col.Item().PaddingTop(3, Unit.Millimetre).Row(fila =>
                {
                    fila.RelativeItem().Text("Ingresos y egresos").Bold().FontSize(15);
                    fila.AutoItem().AlignBottom()
                        .Text($"{cantidad} movimiento{(cantidad == 1 ? "" : "s")}").Bold().FontSize(10);
                });

                col.Item().PaddingTop(1, Unit.Millimetre).Text(texto =>
                {
                    texto.Span("Período: ").FontColor(Colors.Grey.Darken2);
                    texto.Span($"{Fecha(informe.FechaDesde)} al {Fecha(informe.FechaHasta)}").Bold();
                    texto.Span("    Tipo de movimiento: ").FontColor(Colors.Grey.Darken2);
                    texto.Span(DescripcionTipo(informe.TipoMovimiento)).Bold();
                    texto.Span("    Actividad: ").FontColor(Colors.Grey.Darken2);
                    texto.Span(informe.Actividad ?? "Todas").Bold();
                });

                col.Item().PaddingTop(2, Unit.Millimetre).LineHorizontal(0.8f).LineColor(Colors.Black);
            });
        }

        // ---------------------------------------------------------------- Totales

        /// <summary>Recuadros con total de ingresos, de egresos y saldo del período.</summary>
        private static void Totales(IContainer contenedor, MovimientosCajaInformeDto informe)
        {
            contenedor.Row(fila =>
            {
                Recuadro(fila.RelativeItem(), "Total ingresos", informe.TotalIngresos, ColorIngresos);
                fila.ConstantItem(4, Unit.Millimetre);
                Recuadro(fila.RelativeItem(), "Total egresos", informe.TotalEgresos, ColorEgresos);
                fila.ConstantItem(4, Unit.Millimetre);
                Recuadro(fila.RelativeItem(), "Saldo (ingresos − egresos)", informe.Saldo, null);
            });
        }

        /// <summary>Recuadro con un total. Si es una serie de los gráficos, lleva una franja de su color.</summary>
        private static void Recuadro(IContainer contenedor, string titulo, decimal monto, string? color)
        {
            contenedor.Border(0.6f).BorderColor(Colors.Grey.Lighten2).CornerRadius(2, Unit.Millimetre)
                .Row(fila =>
                {
                    // Franja de color a la izquierda: vincula el total con su serie en los gráficos.
                    fila.ConstantItem(1.6f, Unit.Millimetre).Background(color ?? Colors.Grey.Lighten1);
                    fila.RelativeItem().PaddingVertical(2.5f, Unit.Millimetre).PaddingHorizontal(3, Unit.Millimetre)
                        .Column(col =>
                        {
                            col.Item().Text(titulo).FontSize(8).FontColor(Colors.Grey.Darken2);
                            col.Item().Text(Importe(monto)).Bold().FontSize(14);
                        });
                });
        }

        // ---------------------------------------------------------------- Gráficos

        /// <summary>Título de una sección de análisis con la leyenda de las series que tienen datos.</summary>
        private static void TituloSeccion(IContainer contenedor, string titulo, MovimientosCajaInformeDto informe)
        {
            contenedor.Row(fila =>
            {
                fila.RelativeItem().Text(titulo).Bold().FontSize(11);
                fila.AutoItem().AlignBottom().Row(leyenda =>
                {
                    leyenda.Spacing(4, Unit.Millimetre);
                    if (informe.TotalIngresos > 0) ItemLeyenda(leyenda.AutoItem(), "Ingresos", ColorIngresos);
                    if (informe.TotalEgresos > 0) ItemLeyenda(leyenda.AutoItem(), "Egresos", ColorEgresos);
                });
            });
        }

        private static void ItemLeyenda(IContainer contenedor, string texto, string color)
        {
            contenedor.Row(r =>
            {
                r.ConstantItem(2.6f, Unit.Millimetre).AlignMiddle().Height(2.6f, Unit.Millimetre)
                    .Background(color).CornerRadius(1.3f, Unit.Millimetre);
                r.AutoItem().PaddingLeft(1.2f, Unit.Millimetre).Text(texto).FontSize(7.5f).FontColor(Colors.Grey.Darken2);
            });
        }

        /// <summary>
        /// Barras horizontales por actividad (ordenadas de mayor a menor importe), todas sobre la
        /// misma escala. Cada actividad muestra una barra por cada serie con importe, con su valor al lado.
        /// </summary>
        private static void GraficoPorActividad(IContainer contenedor, List<SubtotalCajaDto> actividades)
        {
            var maximo = actividades.Select(a => Math.Max(a.Ingresos, a.Egresos)).DefaultIfEmpty(0).Max();
            if (maximo <= 0)
                return;

            contenedor.Column(barras =>
            {
                barras.Spacing(2.2f, Unit.Millimetre);
                foreach (var a in actividades)
                {
                    barras.Item().Row(fila =>
                    {
                        fila.ConstantItem(34, Unit.Millimetre).AlignMiddle().PaddingRight(2, Unit.Millimetre)
                            .AlignRight().Text(a.Descripcion).FontSize(7.5f).FontColor(Colors.Grey.Darken3);

                        fila.RelativeItem().BorderLeft(0.6f).BorderColor(Colors.Grey.Medium).Column(series =>
                        {
                            series.Spacing(0.7f, Unit.Millimetre);
                            if (a.Ingresos > 0) Barra(series.Item(), a.Ingresos, maximo, ColorIngresos);
                            if (a.Egresos > 0) Barra(series.Item(), a.Egresos, maximo, ColorEgresos);
                        });
                    });
                }
            });
        }

        /// <summary>Barra horizontal proporcional a 'valor' respecto de 'maximo', con su importe al lado.</summary>
        private static void Barra(IContainer contenedor, decimal valor, decimal maximo, string color)
        {
            // La barra más larga ocupa el 72 % de la pista; el resto queda para el importe.
            var largo = Math.Max((float)(valor / maximo) * 72f, 0.5f);
            contenedor.Height(AltoBarra, Unit.Millimetre).Row(pista =>
            {
                pista.RelativeItem(largo).Background(color).CornerRadius(0.8f, Unit.Millimetre);
                pista.RelativeItem(100f - largo).PaddingLeft(1.5f, Unit.Millimetre).AlignMiddle()
                    .Text(Compacto(valor)).FontSize(7).SemiBold().FontColor(Colors.Grey.Darken3);
            });
        }

        /// <summary>
        /// Columnas por mes (ingresos y egresos lado a lado) sobre una escala de valores redondos con
        /// líneas de guía. Debajo de cada mes se indica su saldo.
        /// </summary>
        private static void GraficoPorMes(IContainer contenedor, MovimientosCajaInformeDto informe, List<SubtotalCajaDto> meses)
        {
            var maximo = meses.Select(m => Math.Max(m.Ingresos, m.Egresos)).DefaultIfEmpty(0).Max();
            if (maximo <= 0)
                return;

            var paso = PasoEscala(maximo);
            var tope = paso * DivisionesEscala;
            var altoDivision = AltoColumnas / DivisionesEscala;
            var variosAnios = informe.FechaDesde.Year != informe.FechaHasta.Year;
            const float anchoEje = 15;

            contenedor.Column(col =>
            {
                col.Item().PaddingTop(2, Unit.Millimetre).Row(fila =>
                {
                    // Eje con los valores de las líneas de guía (de arriba hacia abajo).
                    fila.ConstantItem(anchoEje, Unit.Millimetre).Height(AltoColumnas, Unit.Millimetre).Column(eje =>
                    {
                        for (var i = DivisionesEscala; i >= 1; i--)
                            eje.Item().Height(altoDivision, Unit.Millimetre).TranslateY(-1.3f, Unit.Millimetre)
                                .AlignRight().PaddingRight(1.5f, Unit.Millimetre)
                                .Text(Compacto(paso * i)).FontSize(6.5f).FontColor(Colors.Grey.Darken1);
                    });

                    fila.RelativeItem().Height(AltoColumnas, Unit.Millimetre).Layers(capas =>
                    {
                        // Fondo: líneas de guía y línea de base.
                        capas.Layer().Column(guias =>
                        {
                            for (var i = 0; i < DivisionesEscala; i++)
                                guias.Item().Height(altoDivision, Unit.Millimetre).BorderTop(0.4f).BorderColor(ColorGuia);
                        });
                        capas.Layer().AlignBottom().LineHorizontal(0.7f).LineColor(Colors.Grey.Medium);

                        // Columnas.
                        capas.PrimaryLayer().Row(columnas =>
                        {
                            foreach (var m in meses)
                            {
                                columnas.RelativeItem().PaddingHorizontal(1.6f, Unit.Millimetre).Row(par =>
                                {
                                    Columna(par.RelativeItem(), m.Ingresos, tope, ColorIngresos);
                                    par.ConstantItem(0.5f, Unit.Millimetre);
                                    Columna(par.RelativeItem(), m.Egresos, tope, ColorEgresos);
                                });
                            }
                        });
                    });
                });

                // Etiquetas de los meses y saldo de cada uno, alineados con las columnas.
                col.Item().PaddingTop(1.2f, Unit.Millimetre).Row(fila =>
                {
                    fila.ConstantItem(anchoEje, Unit.Millimetre).AlignRight().PaddingRight(1.5f, Unit.Millimetre)
                        .Text("$ 0").FontSize(6.5f).FontColor(Colors.Grey.Darken1);
                    foreach (var m in meses)
                        fila.RelativeItem().AlignCenter().Text(EtiquetaMes(m.Descripcion, variosAnios))
                            .FontSize(7).SemiBold().FontColor(Colors.Grey.Darken3);
                });
                col.Item().PaddingTop(0.6f, Unit.Millimetre).Row(fila =>
                {
                    fila.ConstantItem(anchoEje, Unit.Millimetre).AlignRight().PaddingRight(1.5f, Unit.Millimetre)
                        .Text("Saldo").FontSize(6.5f).FontColor(Colors.Grey.Darken1);
                    foreach (var m in meses)
                        fila.RelativeItem().AlignCenter().Text(Compacto(m.Saldo, conSigno: true))
                            .FontSize(6.5f).FontColor(Colors.Grey.Darken2);
                });
            });
        }

        private static void Columna(IContainer contenedor, decimal valor, decimal tope, string color)
        {
            if (valor <= 0)
                return;

            // Un mínimo visible para importes muy chicos frente al tope de la escala.
            var alto = Math.Max((float)(valor / tope) * AltoColumnas, 0.6f);
            contenedor.AlignBottom().Height(alto, Unit.Millimetre).Background(color).CornerRadius(0.6f, Unit.Millimetre);
        }

        /// <summary>
        /// Paso "redondo" de la escala (1; 1,5; 2; 2,5; 3; 4; 5; 6; 7,5 u 8 por una potencia de 10)
        /// tal que DivisionesEscala pasos cubran el máximo con poco espacio de sobra.
        /// Ej.: máximo 462.499 → paso 150.000 (tope 600.000).
        /// </summary>
        private static decimal PasoEscala(decimal maximo)
        {
            var bruto = (double)maximo / DivisionesEscala;
            var potencia = Math.Pow(10, Math.Floor(Math.Log10(bruto)));
            foreach (var factor in new[] { 1, 1.5, 2, 2.5, 3, 4, 5, 6, 7.5, 8, 10 })
                if (factor * potencia >= bruto)
                    return (decimal)(factor * potencia);
            return (decimal)(10 * potencia);
        }

        /// <summary>
        /// Importe abreviado para gráficos: "$ 850", "$ 12,5 mil", "$ 167 mil", "$ 1,2 M"
        /// (opcionalmente con signo). Desde $ 100 mil se redondea a miles enteros para que entre en
        /// un renglón; los importes exactos están en los cuadros.
        /// </summary>
        private static string Compacto(decimal valor, bool conSigno = false)
        {
            var signo = valor < 0 ? "−" : conSigno && valor > 0 ? "+" : "";
            var abs = Math.Abs(valor);
            string cuerpo = abs switch
            {
                >= 1_000_000 => $"{(abs / 1_000_000).ToString("0.#", CulturaAr)} M",
                >= 100_000 => $"{(abs / 1_000).ToString("0", CulturaAr)} mil",
                >= 1_000 => $"{(abs / 1_000).ToString("0.#", CulturaAr)} mil",
                _ => abs.ToString("0", CulturaAr)
            };
            return $"{signo}$ {cuerpo}";
        }

        /// <summary>"OCTUBRE 2026" → "OCT" (o "OCT 26" si el período abarca varios años).</summary>
        private static string EtiquetaMes(string descripcion, bool conAnio)
        {
            var partes = descripcion.Split(' ');
            var mes = partes[0].Length > 3 ? partes[0][..3] : partes[0];
            return conAnio && partes.Length > 1 ? $"{mes} {partes[1][^2..]}" : mes;
        }

        // ---------------------------------------------------------------- Tablas

        /// <summary>Cuadro con los importes exactos de un análisis (por actividad o por mes).</summary>
        private static void TablaSubtotales(IContainer contenedor, string primeraColumna, List<SubtotalCajaDto> subtotales)
        {
            contenedor.Table(tabla =>
            {
                tabla.ColumnsDefinition(c =>
                {
                    c.RelativeColumn(9);
                    c.RelativeColumn(3);
                    c.RelativeColumn(6);
                    c.RelativeColumn(6);
                    c.RelativeColumn(6);
                });

                tabla.Header(h =>
                {
                    CeldaTitulo(h.Cell()).Text(primeraColumna);
                    CeldaTitulo(h.Cell()).AlignRight().Text("Cant.");
                    CeldaTitulo(h.Cell()).AlignRight().Text("Ingresos");
                    CeldaTitulo(h.Cell()).AlignRight().Text("Egresos");
                    CeldaTitulo(h.Cell()).AlignRight().Text("Saldo");
                });

                for (var i = 0; i < subtotales.Count; i++)
                {
                    var s = subtotales[i];
                    Celda(tabla.Cell(), i).Text(s.Descripcion).FontSize(7.5f);
                    Celda(tabla.Cell(), i).AlignRight().Text(s.Cantidad.ToString()).FontSize(7.5f);
                    Celda(tabla.Cell(), i).AlignRight().Text(Importe(s.Ingresos)).FontSize(7.5f);
                    Celda(tabla.Cell(), i).AlignRight().Text(Importe(s.Egresos)).FontSize(7.5f);
                    Celda(tabla.Cell(), i).AlignRight().Text(Importe(s.Saldo)).FontSize(7.5f).SemiBold();
                }
            });
        }

        private static void TablaDetalle(IContainer contenedor, List<MovimientoCajaDto> movimientos)
        {
            contenedor.Table(tabla =>
            {
                // Anchos relativos pensados para A4 apaisada (277 mm útiles).
                tabla.ColumnsDefinition(columnas =>
                {
                    columnas.RelativeColumn(11); // Fecha y hora
                    columnas.RelativeColumn(7);  // Tipo
                    columnas.RelativeColumn(15); // Apellido y nombre
                    columnas.RelativeColumn(14); // Razón social
                    columnas.RelativeColumn(10); // Monto
                    columnas.RelativeColumn(11); // Método de pago
                    columnas.RelativeColumn(13); // Actividad
                    columnas.RelativeColumn(19); // Observación
                });

                // El encabezado de la tabla se repite automáticamente en cada hoja.
                tabla.Header(encabezado =>
                {
                    CeldaTitulo(encabezado.Cell()).Text("Fecha y hora");
                    CeldaTitulo(encabezado.Cell()).Text("Tipo");
                    CeldaTitulo(encabezado.Cell()).Text("Apellido y nombre");
                    CeldaTitulo(encabezado.Cell()).Text("Razón social");
                    CeldaTitulo(encabezado.Cell()).AlignRight().Text("Monto");
                    CeldaTitulo(encabezado.Cell()).Text("Método de pago");
                    CeldaTitulo(encabezado.Cell()).Text("Actividad");
                    CeldaTitulo(encabezado.Cell()).Text("Observación");
                });

                for (var i = 0; i < movimientos.Count; i++)
                {
                    var m = movimientos[i];

                    Celda(tabla.Cell(), i).Text(m.FechaHoraMovimiento.ToString("dd/MM/yyyy HH:mm", CulturaAr));

                    // Tipo con un punto del color de su serie (el texto no depende del color).
                    Celda(tabla.Cell(), i).Row(r =>
                    {
                        var color = m.EsIngreso ? ColorIngresos : m.EsEgreso ? ColorEgresos : Colors.Grey.Medium;
                        // Alineado con el primer renglón (la fila puede tener dos renglones).
                        r.ConstantItem(2, Unit.Millimetre).PaddingTop(0.9f, Unit.Millimetre).AlignTop()
                            .Height(2, Unit.Millimetre).Background(color).CornerRadius(1, Unit.Millimetre);
                        r.RelativeItem().PaddingLeft(1.2f, Unit.Millimetre).Text(m.TipoMovimiento);
                    });

                    Celda(tabla.Cell(), i).Text(NombreCompleto(m.Apellido, m.Nombre));
                    Celda(tabla.Cell(), i).Text(Valor(m.RazonSocial));
                    Celda(tabla.Cell(), i).AlignRight().Text(PdfComun.Importe(m.Monto));
                    Celda(tabla.Cell(), i).Text(Valor(m.MetodoPago));
                    Celda(tabla.Cell(), i).Text(Valor(m.Actividad));
                    Celda(tabla.Cell(), i).Text(Valor(m.Observacion));
                }
            });
        }

        /// <summary>"$ 1.234,56" o "-$ 1.234,56" (para un saldo negativo).</summary>
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
