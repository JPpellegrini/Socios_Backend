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
    /// Genera el PDF del reporte de movimientos por entidad con QuestPDF.
    /// Diseño: A4 apaisada; encabezado con la institución, la entidad y el período; tabla con un
    /// movimiento por fila (el encabezado se repite en cada hoja) cuya columna de importe se titula
    /// "Ingreso" (colaborador) o "Egreso" (proveedor); fila final con el total neto. Si el período
    /// no tiene movimientos, se muestra un aviso en lugar de la tabla.
    /// </summary>
    public class InformeMovimientosEntidadPdf : IInformeMovimientosEntidadPdf
    {
        private readonly ComprobanteOpciones _opciones;

        static InformeMovimientosEntidadPdf() => DeclararLicencia();

        public InformeMovimientosEntidadPdf(IOptions<ComprobanteOpciones> opciones)
        {
            // Se reutilizan los datos de la institución que ya se imprimen en los comprobantes.
            _opciones = opciones.Value.ConValoresPorDefecto();
        }

        public byte[] Generar(MovimientosEntidadInformeDto informe)
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
                                    .Text("Sin movimientos en el período seleccionado.")
                                    .FontSize(11).FontColor(Colors.Grey.Darken2);
                                return;
                            }

                            col.Item().Element(x => Tabla(x, informe));

                            // Aclara por qué hay importes negativos (ej.: una devolución).
                            if (informe.TieneMovimientosEnContra)
                                col.Item().PaddingTop(2, Unit.Millimetre)
                                    .Text($"Los importes negativos corresponden a movimientos en sentido contrario " +
                                          $"(no {informe.TituloImporte.ToLower(CulturaAr)}s) y restan del total.")
                                    .FontSize(7.5f).Italic().FontColor(Colors.Grey.Darken2);
                        });
                    });
                })
                .WithMetadata(new DocumentMetadata { Title = $"Movimientos de {Tipo(informe).ToLower(CulturaAr)}" })
                .GeneratePdf();
        }

        private void Encabezado(IContainer contenedor, MovimientosEntidadInformeDto informe)
        {
            var e = informe.Entidad;
            var cantidad = informe.Cantidad;

            contenedor.Column(col =>
            {
                Institucion(col, _opciones);

                col.Item().PaddingTop(3, Unit.Millimetre).Row(fila =>
                {
                    fila.RelativeItem().Text($"Movimientos de {Tipo(informe).ToLower(CulturaAr)}").Bold().FontSize(15);
                    fila.AutoItem().AlignBottom()
                        .Text($"{cantidad} movimiento{(cantidad == 1 ? "" : "s")}").Bold().FontSize(10);
                });

                col.Item().PaddingTop(1, Unit.Millimetre).Text(texto =>
                {
                    texto.Span($"{Tipo(informe)}: ").FontColor(Colors.Grey.Darken2);
                    texto.Span(NombreEntidad(e)).Bold();
                    if (!string.IsNullOrWhiteSpace(e.RazonSocial) && NombreEntidad(e) != e.RazonSocial.Trim())
                    {
                        texto.Span("    Razón social: ").FontColor(Colors.Grey.Darken2);
                        texto.Span(e.RazonSocial.Trim()).Bold();
                    }
                    texto.Span("    CUIT/DNI: ").FontColor(Colors.Grey.Darken2);
                    texto.Span(Valor(e.CuitCuil ?? e.Dni)).Bold();
                    texto.Span("    Prestación: ").FontColor(Colors.Grey.Darken2);
                    texto.Span(Valor(e.Prestacion)).Bold();
                    texto.Span("    Estado: ").FontColor(Colors.Grey.Darken2);
                    texto.Span(Valor(e.Estado)).Bold();
                });

                col.Item().PaddingTop(1, Unit.Millimetre).Text(texto =>
                {
                    texto.Span("Período: ").FontColor(Colors.Grey.Darken2);
                    texto.Span($"{Fecha(informe.FechaDesde)} al {Fecha(informe.FechaHasta)}").Bold();
                });

                col.Item().PaddingTop(2, Unit.Millimetre).LineHorizontal(0.8f).LineColor(Colors.Black);
            });
        }

        private static void Tabla(IContainer contenedor, MovimientosEntidadInformeDto informe)
        {
            var movimientos = informe.Movimientos;

            contenedor.Table(tabla =>
            {
                // Anchos relativos pensados para A4 apaisada (277 mm útiles).
                tabla.ColumnsDefinition(columnas =>
                {
                    columnas.RelativeColumn(12); // Fecha y hora
                    columnas.RelativeColumn(16); // Concepto
                    columnas.RelativeColumn(12); // Ingreso / Egreso
                    columnas.RelativeColumn(13); // Método de pago
                    columnas.RelativeColumn(15); // Cuenta
                    columnas.RelativeColumn(32); // Observación
                });

                // El encabezado de la tabla se repite automáticamente en cada hoja.
                tabla.Header(encabezado =>
                {
                    CeldaTitulo(encabezado.Cell()).Text("Fecha y hora");
                    CeldaTitulo(encabezado.Cell()).Text("Concepto");
                    CeldaTitulo(encabezado.Cell()).AlignRight().Text(informe.TituloImporte);
                    CeldaTitulo(encabezado.Cell()).Text("Método de pago");
                    CeldaTitulo(encabezado.Cell()).Text("Cuenta");
                    CeldaTitulo(encabezado.Cell()).Text("Observación");
                });

                for (var i = 0; i < movimientos.Count; i++)
                {
                    var m = movimientos[i];

                    Celda(tabla.Cell(), i).Text(m.FechaHoraMovimiento.ToString("dd/MM/yyyy HH:mm", CulturaAr));
                    Celda(tabla.Cell(), i).Text(Valor(m.Concepto));
                    Celda(tabla.Cell(), i).AlignRight().Text(Importe(m.Importe));
                    Celda(tabla.Cell(), i).Text(Valor(m.MetodoPago));
                    Celda(tabla.Cell(), i).Text(Cuenta(m.Cuenta, m.NroCuenta));
                    Celda(tabla.Cell(), i).Text(Valor(m.Observacion));
                }

                // Totalizador de la columna importe.
                tabla.Cell().ColumnSpan(2).Element(CeldaTotal).AlignRight().Text($"TOTAL {informe.TituloImporte.ToUpper(CulturaAr)}");
                tabla.Cell().Element(CeldaTotal).AlignRight().Text(Importe(informe.Total));
                tabla.Cell().ColumnSpan(3).Element(CeldaTotal);
            });
        }

        private static IContainer CeldaTotal(IContainer celda) =>
            celda.Background(Colors.Grey.Lighten2)
                .BorderTop(0.8f).BorderColor(Colors.Black)
                .PaddingVertical(1.5f, Unit.Millimetre).PaddingHorizontal(1.2f, Unit.Millimetre)
                .DefaultTextStyle(estilo => estilo.Bold().FontSize(9));

        /// <summary>"$ 1.234,56" o "-$ 1.234,56".</summary>
        private static string Importe(decimal monto) =>
            monto < 0 ? $"-{PdfComun.Importe(-monto)}" : PdfComun.Importe(monto);

        private static string Tipo(MovimientosEntidadInformeDto informe) =>
            TipoEntidadMovimientos.Obtener(informe.Tipo)?.Descripcion ?? informe.Tipo;

        /// <summary>"APELLIDO, Nombre"; si no tiene nombre (empresa), la razón social.</summary>
        private static string NombreEntidad(EntidadMovimientosDto e)
        {
            var nombre = NombreCompleto(e.Apellido, e.Nombre);
            return nombre == SinDato ? Valor(e.RazonSocial) : nombre;
        }

        /// <summary>"BANCO MACRO - 789012"; sin número si la cuenta no lo tiene cargado.</summary>
        private static string Cuenta(string? nombre, int nroCuenta) =>
            nroCuenta > 0 ? $"{Valor(nombre)} - {nroCuenta}" : Valor(nombre);
    }
}
