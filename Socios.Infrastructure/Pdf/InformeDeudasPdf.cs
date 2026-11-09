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
    /// Genera el PDF del informe de deudas con QuestPDF.
    /// Diseño: A4 apaisada; encabezado con la institución y los filtros; recuadros con el total
    /// adeudado, las cuotas (y cuántas ya vencieron) y los socios con deuda; la deuda por tipo de
    /// cuota con su participación; luego el detalle agrupado por socio (encabezado del socio,
    /// sus cuotas y su subtotal). El encabezado de la tabla se repite en cada hoja.
    /// </summary>
    public class InformeDeudasPdf : IInformeDeudasPdf
    {
        private const string ColorDeuda = "#eb6834";     // naranja: misma paleta que los informes de caja
        private const string ColorVencido = "#b4451f";   // naranja oscuro para el atraso
        private const string ColorPista = "#eef1f4";

        private static readonly string[] Meses =
        {
            "ENERO", "FEBRERO", "MARZO", "ABRIL", "MAYO", "JUNIO",
            "JULIO", "AGOSTO", "SEPTIEMBRE", "OCTUBRE", "NOVIEMBRE", "DICIEMBRE"
        };

        private readonly ComprobanteOpciones _opciones;

        static InformeDeudasPdf() => DeclararLicencia();

        public InformeDeudasPdf(IOptions<ComprobanteOpciones> opciones)
        {
            // Se reutilizan los datos de la institución que ya se imprimen en los comprobantes.
            _opciones = opciones.Value.ConValoresPorDefecto();
        }

        public byte[] Generar(DeudasInformeDto informe)
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
                                    .Text("No hay deudas con los criterios seleccionados.")
                                    .FontSize(11).FontColor(Colors.Grey.Darken2);
                                return;
                            }

                            col.Item().Row(fila =>
                            {
                                fila.RelativeItem(10).Element(x => Indicadores(x, informe));
                                fila.ConstantItem(6, Unit.Millimetre);
                                fila.RelativeItem(11).Element(x => PorTipo(x, informe));
                            });

                            col.Item().PaddingTop(6, Unit.Millimetre).Text("Detalle de cuotas adeudadas").Bold().FontSize(11);
                            col.Item().PaddingTop(1.5f, Unit.Millimetre).Element(x => TablaDetalle(x, informe));

                            col.Item().PaddingTop(2, Unit.Millimetre).Column(notas =>
                            {
                                // Aclara las cuotas que ya vencieron pero siguen marcadas PENDIENTE (solo si las hay).
                                var pendientesConAtraso = informe.Cuotas.Count(c => c.Estado == "PENDIENTE" && c.DiasAtraso > 0);
                                notas.Item().Text($"Atraso: días desde el vencimiento al {Fecha(informe.FechaCorte)}." +
                                                  (pendientesConAtraso > 0
                                                      ? $" {pendientesConAtraso} cuota{(pendientesConAtraso == 1 ? "" : "s")} ya vencida{(pendientesConAtraso == 1 ? "" : "s")} todavía figura{(pendientesConAtraso == 1 ? "" : "n")} como PENDIENTE."
                                                      : ""))
                                    .FontSize(7.5f).Italic().FontColor(Colors.Grey.Darken2);
                                if (informe.TieneMontosActualizados)
                                    notas.Item().Text("* Cuota VENCIDA: se informa al importe vigente, que es el que se cobra hoy.")
                                        .FontSize(7.5f).Italic().FontColor(Colors.Grey.Darken2);
                            });
                        });
                    });
                })
                .WithMetadata(new DocumentMetadata { Title = "Deudas de socios" })
                .GeneratePdf();
        }

        private void Encabezado(IContainer contenedor, DeudasInformeDto informe)
        {
            contenedor.Column(col =>
            {
                Institucion(col, _opciones);

                col.Item().PaddingTop(3, Unit.Millimetre).Row(fila =>
                {
                    fila.RelativeItem().Text("Deudas de socios").Bold().FontSize(15);
                    fila.AutoItem().AlignBottom().Text($"Al {Fecha(informe.FechaCorte)}").Bold().FontSize(10);
                });

                col.Item().PaddingTop(1, Unit.Millimetre).Text(texto =>
                {
                    texto.Span("Socio: ").FontColor(Colors.Grey.Darken2);
                    texto.Span(informe.Socio is null
                        ? "Todos"
                        : $"{NombreCompleto(informe.Socio.Apellido, informe.Socio.Nombre)} (N° {informe.Socio.IdSocio})").Bold();
                    texto.Span("    Vencimientos: ").FontColor(Colors.Grey.Darken2);
                    texto.Span(informe.SinFechas
                        ? "Sin filtro de fechas"
                        : $"{Fecha(informe.FechaDesde)} al {Fecha(informe.FechaHasta)}").Bold();
                    texto.Span("    Estado de cuota: ").FontColor(Colors.Grey.Darken2);
                    texto.Span(informe.EstadoCuota switch
                    {
                        DeudasFiltroDto.Vencidas => "Solo vencidas",
                        DeudasFiltroDto.Pendientes => "Solo pendientes",
                        _ => "Todas"
                    }).Bold();
                });

                col.Item().PaddingTop(2, Unit.Millimetre).LineHorizontal(0.8f).LineColor(Colors.Black);
            });
        }

        /// <summary>Recuadros: total adeudado, cuotas (vencidas) y socios con deuda.</summary>
        private static void Indicadores(IContainer contenedor, DeudasInformeDto informe)
        {
            contenedor.Column(col =>
            {
                col.Spacing(2.5f, Unit.Millimetre);
                Indicador(col.Item(), "Total adeudado", Importe(informe.Total),
                    $"{Importe(informe.TotalVencido)} ya vencido", ColorDeuda);
                col.Item().Row(fila =>
                {
                    Indicador(fila.RelativeItem(), "Cuotas adeudadas", informe.Cantidad.ToString(),
                        $"{informe.CantidadVencidas} vencida{(informe.CantidadVencidas == 1 ? "" : "s")}", ColorVencido);
                    fila.ConstantItem(2.5f, Unit.Millimetre);
                    Indicador(fila.RelativeItem(), "Socios con deuda", informe.CantidadSocios.ToString(), null, Colors.Grey.Darken1);
                });
            });

            static void Indicador(IContainer c, string titulo, string valor, string? aclaracion, string color) =>
                c.Border(0.6f).BorderColor(Colors.Grey.Lighten2).CornerRadius(2, Unit.Millimetre).Row(r =>
                {
                    r.ConstantItem(1.6f, Unit.Millimetre).Background(color);
                    r.RelativeItem().PaddingVertical(2.5f, Unit.Millimetre).PaddingHorizontal(3, Unit.Millimetre).Column(t =>
                    {
                        t.Item().Text(titulo).FontSize(8).FontColor(Colors.Grey.Darken2);
                        t.Item().Text(valor).Bold().FontSize(15);
                        if (aclaracion is not null)
                            t.Item().Text(aclaracion).FontSize(7.5f).FontColor(Colors.Grey.Darken1);
                    });
                });
        }

        /// <summary>Deuda por tipo de cuota: barra de participación sobre el total, porcentaje e importe.</summary>
        private static void PorTipo(IContainer contenedor, DeudasInformeDto informe)
        {
            var total = informe.Total;

            contenedor.Border(0.6f).BorderColor(Colors.Grey.Lighten2).CornerRadius(2, Unit.Millimetre)
                .Padding(3.5f, Unit.Millimetre).Column(col =>
                {
                    col.Item().Text("Deuda por tipo de cuota").Bold().FontSize(10);
                    col.Item().PaddingTop(1.5f, Unit.Millimetre).Column(lista =>
                    {
                        foreach (var t in informe.PorTipo)
                        {
                            var proporcion = total == 0 ? 0f : (float)(t.Total / total);
                            lista.Item().PaddingVertical(1.6f, Unit.Millimetre).Row(fila =>
                            {
                                fila.ConstantItem(24, Unit.Millimetre).AlignMiddle().Column(a =>
                                {
                                    a.Item().Text(t.Descripcion).SemiBold().FontSize(9);
                                    a.Item().Text($"{t.Cantidad} cuota{(t.Cantidad == 1 ? "" : "s")}").FontSize(7).FontColor(Colors.Grey.Darken1);
                                });
                                fila.RelativeItem().AlignMiddle().Height(2.6f, Unit.Millimetre).Background(ColorPista)
                                    .CornerRadius(1.3f, Unit.Millimetre).Row(pista =>
                                    {
                                        var largo = Math.Clamp(proporcion, 0.01f, 1f);
                                        pista.RelativeItem(largo).Background(ColorDeuda).CornerRadius(1.3f, Unit.Millimetre);
                                        if (largo < 1f) pista.RelativeItem(1f - largo);
                                    });
                                fila.ConstantItem(13, Unit.Millimetre).AlignMiddle().AlignRight()
                                    .Text(Porcentaje((decimal)proporcion)).FontSize(8).FontColor(Colors.Grey.Darken2);
                                fila.ConstantItem(30, Unit.Millimetre).AlignMiddle().AlignRight()
                                    .Text(Importe(t.Total)).FontSize(9.5f).SemiBold();
                            });
                        }
                    });
                });
        }

        /// <summary>
        /// Detalle agrupado por socio: un bloque por socio con su encabezado (nombre, N° de socio,
        /// DNI), una tabla con sus cuotas y su subtotal. EnsureSpace evita que un bloque empiece al
        /// pie de una hoja (encabezado del socio solo); si un socio tiene muchas cuotas, su tabla
        /// sigue en la hoja siguiente repitiendo los títulos de las columnas.
        /// </summary>
        private static void TablaDetalle(IContainer contenedor, DeudasInformeDto informe)
        {
            contenedor.Column(col =>
            {
                col.Spacing(4, Unit.Millimetre);

                foreach (var socio in informe.Cuotas.GroupBy(c => c.IdEntidad))
                {
                    var cuotas = socio.ToList();
                    // Alto aproximado del bloque (encabezados + filas + subtotal), con tope para socios con muchas cuotas.
                    var altoMinimo = Math.Min(22 + cuotas.Count * 6.5f, 60);
                    col.Item().EnsureSpace((int)Math.Ceiling(altoMinimo * 2.835f)).Element(x => BloqueSocio(x, cuotas));
                }

                // Total general.
                col.Item().Background(Colors.Grey.Lighten2).BorderTop(0.8f).BorderColor(Colors.Black)
                    .PaddingVertical(2, Unit.Millimetre).PaddingHorizontal(1.5f, Unit.Millimetre).Row(fila =>
                    {
                        fila.RelativeItem().Text("TOTAL ADEUDADO").Bold().FontSize(10);
                        fila.AutoItem().Text($"{informe.Cantidad} cuotas de {informe.CantidadSocios} socio{(informe.CantidadSocios == 1 ? "" : "s")}")
                            .FontSize(8.5f).FontColor(Colors.Grey.Darken2);
                        fila.ConstantItem(40, Unit.Millimetre).AlignRight().Text(Importe(informe.Total)).Bold().FontSize(11);
                    });
            });
        }

        private static void BloqueSocio(IContainer contenedor, List<DeudaCuotaDto> cuotas)
        {
            var primera = cuotas[0];

            contenedor.Column(bloque =>
            {
                // Encabezado del socio.
                bloque.Item().Background(Colors.Grey.Lighten3).BorderLeft(1.6f).BorderColor(ColorDeuda)
                    .PaddingVertical(1.6f, Unit.Millimetre).PaddingHorizontal(2, Unit.Millimetre).Row(fila =>
                    {
                        fila.RelativeItem().Text(t =>
                        {
                            t.Span(NombreCompleto(primera.Apellido, primera.Nombre)).Bold().FontSize(9.5f);
                            t.Span($"    N° de socio {primera.IdSocio}    DNI {Valor(primera.Dni)}")
                                .FontSize(8).FontColor(Colors.Grey.Darken2);
                        });
                        fila.AutoItem().Text($"{cuotas.Count} cuota{(cuotas.Count == 1 ? "" : "s")}  ·  {Importe(cuotas.Sum(c => c.Monto))}")
                            .SemiBold().FontSize(9);
                    });

                bloque.Item().Table(tabla =>
                {
                    tabla.ColumnsDefinition(c =>
                    {
                        c.RelativeColumn(12); // Mes
                        c.RelativeColumn(6);  // Año
                        c.RelativeColumn(10); // Tipo de cuota
                        c.RelativeColumn(12); // Monto
                        c.RelativeColumn(11); // Vencimiento
                        c.RelativeColumn(11); // Estado
                        c.RelativeColumn(11); // Atraso
                    });

                    // Títulos de columna (se repiten si la tabla del socio sigue en otra hoja).
                    tabla.Header(h =>
                    {
                        foreach (var (titulo, alineacion) in new[] { ("Mes", 0), ("Año", 1), ("Tipo cuota", 0), ("Monto", 2),
                                                                     ("Vencimiento", 1), ("Estado", 0), ("Atraso", 2) })
                        {
                            var celda = h.Cell().BorderBottom(0.6f).BorderColor(Colors.Grey.Medium)
                                .PaddingVertical(1.2f, Unit.Millimetre).PaddingHorizontal(1.2f, Unit.Millimetre);
                            celda = alineacion switch { 1 => celda.AlignCenter(), 2 => celda.AlignRight(), _ => celda };
                            celda.Text(titulo).FontSize(7.5f).SemiBold().FontColor(Colors.Grey.Darken2);
                        }
                    });

                    for (var i = 0; i < cuotas.Count; i++)
                    {
                        var c = cuotas[i];
                        var vencida = c.DiasAtraso > 0;

                        Celda(tabla.Cell(), i).Text(NombreMes(c.Mes));
                        Celda(tabla.Cell(), i).AlignCenter().Text(c.Anio.ToString());
                        Celda(tabla.Cell(), i).Text(c.TipoCuota);
                        Celda(tabla.Cell(), i).AlignRight().Text(Importe(c.Monto) + (c.MontoActualizado ? " *" : ""));
                        Celda(tabla.Cell(), i).AlignCenter().Text(Fecha(c.FechaVencimiento));
                        Celda(tabla.Cell(), i).Text(c.Estado);

                        // El atraso va en texto (no depende solo del color).
                        var atraso = Celda(tabla.Cell(), i).AlignRight()
                            .Text(vencida ? $"{c.DiasAtraso} día{(c.DiasAtraso == 1 ? "" : "s")}" : "Por vencer");
                        if (vencida) atraso.SemiBold().FontColor(ColorVencido);
                        else atraso.FontColor(Colors.Grey.Darken1);
                    }
                });
            });
        }

        private static string NombreMes(int mes) => mes is >= 1 and <= 12 ? Meses[mes - 1] : mes.ToString();

        /// <summary>Participación legible: un decimal fuera del rango 10 %–99 % para no redondear a 0 % o 100 %.</summary>
        private static string Porcentaje(decimal proporcion)
        {
            if (proporcion <= 0) return "0 %";
            if (proporcion < 0.001m) return "< 0,1 %";
            return proporcion is >= 0.1m and < 0.99m
                ? proporcion.ToString("P0", CulturaAr)
                : proporcion.ToString("P1", CulturaAr);
        }

        private static string Importe(decimal monto) =>
            monto < 0 ? $"-{PdfComun.Importe(-monto)}" : PdfComun.Importe(monto);
    }
}
