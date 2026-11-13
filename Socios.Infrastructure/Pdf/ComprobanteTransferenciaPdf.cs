using Microsoft.Extensions.Options;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using Socios.Application.DTOs;
using Socios.Application.Interfaces;

namespace Socios.Infrastructure.Pdf
{
    /// <summary>
    /// Genera el PDF del comprobante de una transferencia entre cuentas del Centro (ej. depósito
    /// del efectivo en el banco): un documento interno, individual (una hoja A4), con el mismo
    /// estilo que el comprobante de pago. Una transferencia tiene UN comprobante, numerado con el
    /// movimiento de EGRESO. Lleva dos firmas (quien la registró y quien la controla) para que se
    /// pueda adjuntar a la boleta del banco y conciliar con el extracto.
    /// </summary>
    public class ComprobanteTransferenciaPdf : IComprobanteTransferenciaPdf
    {
        private static readonly string GrisSuave = Colors.Grey.Lighten3;
        private static readonly string GrisTexto = Colors.Grey.Darken2;

        private readonly ComprobanteOpciones _opciones;

        static ComprobanteTransferenciaPdf()
        {
            // La licencia de QuestPDF se declara en un solo lugar (ver PdfComun).
            PdfComun.ConfigurarLicencia();
        }

        public ComprobanteTransferenciaPdf(IOptions<ComprobanteOpciones> opciones)
        {
            // Lo que no venga en el appsettings toma los datos del Centro.
            _opciones = opciones.Value.ConValoresPorDefecto();
        }

        public byte[] Generar(ComprobanteTransferenciaDto comprobante)
        {
            return Document.Create(documento => documento.Page(pagina =>
                {
                    pagina.Size(PageSizes.A4);
                    pagina.Margin(18, Unit.Millimetre);
                    pagina.DefaultTextStyle(estilo => estilo.FontFamily(PdfComun.Fuente).FontSize(10));

                    pagina.Header().Element(x => Encabezado(x, comprobante));
                    pagina.Content().PaddingTop(6, Unit.Millimetre).Element(x => Cuerpo(x, comprobante));
                    pagina.Footer().AlignCenter()
                        .Text("Comprobante interno. Emitido por el sistema de gestión de socios.")
                        .FontSize(8).FontColor(GrisTexto);
                }))
                .WithMetadata(new DocumentMetadata { Title = $"Comprobante de transferencia N° {comprobante.IdMovimientoEgreso:D8}" })
                .GeneratePdf();
        }

        /// <summary>Institución a la izquierda; número, fecha y hora en un recuadro a la derecha.</summary>
        private void Encabezado(IContainer contenedor, ComprobanteTransferenciaDto c)
        {
            contenedor.Column(col =>
            {
                col.Item().Row(fila =>
                {
                    fila.RelativeItem().PaddingRight(6, Unit.Millimetre).Column(inst =>
                    {
                        // En una sola línea: si el nombre no entra, se achica en vez de partirse.
                        inst.Item().Height(7, Unit.Millimetre).ScaleToFit()
                            .Text(_opciones.Institucion).Bold().FontSize(15);
                        inst.Item().PaddingTop(1, Unit.Millimetre).Text(_opciones.Domicilio);
                        inst.Item().Text($"Personería Jurídica N° {_opciones.PersoneriaJuridica}");
                        inst.Item().Text($"Tel.: {_opciones.Telefono}");
                    });

                    fila.ConstantItem(68, Unit.Millimetre).Border(1).Padding(3, Unit.Millimetre).Column(caja =>
                    {
                        caja.Item().AlignCenter().Text("COMPROBANTE DE TRANSFERENCIA").Bold().FontSize(11);
                        caja.Item().AlignCenter().Text($"N° {c.IdMovimientoEgreso:D8}").Bold().FontSize(14);
                        caja.Item().PaddingTop(1, Unit.Millimetre).AlignCenter()
                            .Text($"Fecha: {c.FechaHora.ToString("dd/MM/yyyy", PdfComun.CulturaAr)}   Hora: {c.FechaHora.ToString("HH:mm", PdfComun.CulturaAr)}");
                    });
                });

                col.Item().PaddingTop(4, Unit.Millimetre).LineHorizontal(1);
                col.Item().PaddingTop(2, Unit.Millimetre).AlignCenter()
                    .Text("MOVIMIENTO INTERNO DE FONDOS ENTRE CUENTAS DEL CENTRO").Bold().FontSize(10);
            });
        }

        private static void Cuerpo(IContainer contenedor, ComprobanteTransferenciaDto c)
        {
            var importe = PdfComun.Importe(c.Importe);
            var importeEnLetras = NumeroEnLetras.Importe(c.Importe).ToUpper(PdfComun.CulturaAr);

            contenedor.Column(col =>
            {
                col.Spacing(6, Unit.Millimetre);

                // Texto con el importe en letras (evita adulteraciones del número).
                col.Item().Text(texto =>
                {
                    texto.Span("Se registra la transferencia de la suma de ");
                    texto.Span($"PESOS {importeEnLetras}").Bold();
                    texto.Span($" ({importe}) entre las siguientes cuentas:");
                });

                // Origen → destino.
                col.Item().Table(tabla =>
                {
                    tabla.ColumnsDefinition(columnas =>
                    {
                        columnas.RelativeColumn(4);
                        columnas.RelativeColumn(4);
                        columnas.RelativeColumn(3);
                    });

                    tabla.Header(encabezado =>
                    {
                        encabezado.Cell().Element(CeldaEncabezado).Text("Cuenta de origen").Bold();
                        encabezado.Cell().Element(CeldaEncabezado).Text("Cuenta de destino").Bold();
                        encabezado.Cell().Element(CeldaEncabezado).AlignRight().Text("Importe").Bold();
                    });

                    tabla.Cell().Element(Celda).Text(DescripcionCuenta(c.CuentaOrigen, c.NroCuentaOrigen));
                    tabla.Cell().Element(Celda).Text(DescripcionCuenta(c.CuentaDestino, c.NroCuentaDestino));
                    tabla.Cell().Element(Celda).AlignRight().Text(importe);

                    tabla.Cell().ColumnSpan(2).Element(CeldaTotal).AlignRight().Text("TOTAL").Bold();
                    tabla.Cell().Element(CeldaTotal).AlignRight().Text(importe).Bold().FontSize(12);
                });

                // Detalle de la operación.
                col.Item().Column(detalle =>
                {
                    detalle.Item().Element(TituloSeccion).Text("DETALLE DE LA OPERACIÓN").Bold();
                    detalle.Item().PaddingTop(2, Unit.Millimetre).Element(x => Dato(x, "Medio", Capitalizar(c.MetodoPago)));
                    detalle.Item().Element(x => Dato(x, "Comprobante bancario", c.ComprobanteBancario ?? "-"));
                    detalle.Item().Element(x => Dato(x, "Movimientos de caja",
                        $"Egreso N° {c.IdMovimientoEgreso:D8} / Ingreso N° {c.IdMovimientoIngreso:D8}"));
                });

                // Quién la registró y firmas.
                col.Item().PaddingTop(4, Unit.Millimetre).Column(registro =>
                {
                    registro.Item().Text($"Registró: {c.Cajero}");
                    registro.Item().Text($"Caja N° {c.NroCaja}");
                });

                col.Item().PaddingTop(16, Unit.Millimetre).Row(fila =>
                {
                    fila.RelativeItem().Element(x => Firma(x, "Firma de quien registró"));
                    fila.ConstantItem(20, Unit.Millimetre);
                    fila.RelativeItem().Element(x => Firma(x, "Firma de quien controló"));
                });
            });
        }

        private static void Firma(IContainer contenedor, string leyenda)
        {
            contenedor.Column(firma =>
            {
                firma.Item().LineHorizontal(0.8f);
                firma.Item().PaddingTop(1, Unit.Millimetre).AlignCenter().Text(leyenda).FontSize(9);
            });
        }

        /// <summary>Fila de datos: etiqueta a la izquierda, valor en negrita.</summary>
        private static void Dato(IContainer contenedor, string etiqueta, string valor)
        {
            contenedor.PaddingVertical(0.8f, Unit.Millimetre).Row(fila =>
            {
                fila.ConstantItem(45, Unit.Millimetre).Text($"{etiqueta}:").FontColor(GrisTexto);
                fila.RelativeItem().Text(valor).Bold();
            });
        }

        /// <summary>"BANCO MACRO" + 789012 → "Banco Macro (N° 789012)"; EFECTIVO sin número.</summary>
        private static string DescripcionCuenta(string nombre, int numero) =>
            numero > 0 ? $"{Capitalizar(nombre)} (N° {numero})" : Capitalizar(nombre);

        private static IContainer TituloSeccion(IContainer contenedor) =>
            contenedor.Background(GrisSuave).PaddingVertical(1.5f, Unit.Millimetre).PaddingHorizontal(2, Unit.Millimetre);

        private static IContainer CeldaEncabezado(IContainer contenedor) =>
            contenedor.Background(GrisSuave).BorderBottom(1).Padding(2, Unit.Millimetre);

        private static IContainer Celda(IContainer contenedor) =>
            contenedor.BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten1).Padding(2, Unit.Millimetre);

        private static IContainer CeldaTotal(IContainer contenedor) =>
            contenedor.Padding(2, Unit.Millimetre);

        /// <summary>"TRANSFERENCIA BANCARIA" → "Transferencia Bancaria".</summary>
        private static string Capitalizar(string texto) =>
            PdfComun.CulturaAr.TextInfo.ToTitleCase(texto.ToLower(PdfComun.CulturaAr));
    }
}
