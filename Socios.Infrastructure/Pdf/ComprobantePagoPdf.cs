using Microsoft.Extensions.Options;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using Socios.Application.DTOs;
using Socios.Application.Interfaces;

namespace Socios.Infrastructure.Pdf
{
    /// <summary>
    /// Genera el PDF del comprobante de pago de una cuota con QuestPDF: un documento formal e
    /// individual (una hoja A4) que le sirve al socio de respaldo de lo que pagó. NO es válido
    /// como factura y así lo indica. Contiene: encabezado de la institución con número, fecha y
    /// hora del comprobante; datos del socio; el texto de recepción con el importe en letras;
    /// el detalle de lo pagado; la forma de pago; quién cobró y la línea de firma.
    /// </summary>
    public class ComprobantePagoPdf : IComprobantePagoPdf
    {
        private static readonly string GrisSuave = Colors.Grey.Lighten3;
        private static readonly string GrisTexto = Colors.Grey.Darken2;

        private readonly ComprobanteOpciones _opciones;

        static ComprobantePagoPdf()
        {
            // La licencia de QuestPDF se declara en un solo lugar (ver PdfComun).
            PdfComun.ConfigurarLicencia();
        }

        public ComprobantePagoPdf(IOptions<ComprobanteOpciones> opciones)
        {
            // Lo que no venga en el appsettings toma los datos del Centro.
            _opciones = opciones.Value.ConValoresPorDefecto();
        }

        public byte[] Generar(ComprobantePagoDto comprobante)
        {
            return Document.Create(documento => documento.Page(pagina =>
                {
                    pagina.Size(PageSizes.A4);
                    pagina.Margin(18, Unit.Millimetre);
                    pagina.DefaultTextStyle(estilo => estilo.FontFamily(PdfComun.Fuente).FontSize(10));

                    pagina.Header().Element(x => Encabezado(x, comprobante));
                    pagina.Content().PaddingTop(6, Unit.Millimetre).Element(x => Cuerpo(x, comprobante));
                    pagina.Footer().AlignCenter()
                        .Text("Documento no válido como factura. Emitido por el sistema de gestión de socios.")
                        .FontSize(8).FontColor(GrisTexto);
                }))
                .WithMetadata(new DocumentMetadata { Title = $"Comprobante de pago N° {comprobante.IdMovimiento:D8}" })
                .GeneratePdf();
        }

        /// <summary>Institución a la izquierda; número, fecha y hora del comprobante en un recuadro a la derecha.</summary>
        private void Encabezado(IContainer contenedor, ComprobantePagoDto c)
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
                        caja.Item().AlignCenter().Text("COMPROBANTE DE PAGO").Bold().FontSize(12);
                        caja.Item().AlignCenter().Text($"N° {c.IdMovimiento:D8}").Bold().FontSize(14);
                        caja.Item().PaddingTop(1, Unit.Millimetre).AlignCenter()
                            .Text($"Fecha: {c.FechaHoraPago.ToString("dd/MM/yyyy", PdfComun.CulturaAr)}   Hora: {c.FechaHoraPago.ToString("HH:mm", PdfComun.CulturaAr)}");
                    });
                });

                col.Item().PaddingTop(4, Unit.Millimetre).LineHorizontal(1);
                col.Item().PaddingTop(2, Unit.Millimetre).AlignCenter()
                    .Text("DOCUMENTO NO VÁLIDO COMO FACTURA").Bold().FontSize(10);
            });
        }

        private static void Cuerpo(IContainer contenedor, ComprobantePagoDto c)
        {
            var nombre = PdfComun.NombreCompleto(c.Apellido, c.Nombre);
            var importe = PdfComun.Importe(c.Monto);
            var importeEnLetras = NumeroEnLetras.Importe(c.Monto).ToUpper(PdfComun.CulturaAr);
            var periodo = $"{Capitalizar(PdfComun.CulturaAr.DateTimeFormat.GetMonthName(c.Mes))} {c.Anio}";

            contenedor.Column(col =>
            {
                col.Spacing(6, Unit.Millimetre);

                // Datos del socio.
                col.Item().Column(socio =>
                {
                    socio.Item().Element(TituloSeccion).Text("DATOS DEL SOCIO").Bold();
                    socio.Item().PaddingTop(2, Unit.Millimetre).Element(x => Dato(x, "N° de socio", c.IdSocio.ToString("D5")));
                    socio.Item().Element(x => Dato(x, "Apellido y nombre", nombre));
                    socio.Item().Element(x => Dato(x, "DNI", string.IsNullOrWhiteSpace(c.Dni) ? "-" : c.Dni));
                    socio.Item().Element(x => Dato(x, "Domicilio", Valor(PdfComun.Domicilio(c.Calle, c.Altura, c.Ciudad))));
                });

                // Texto de recepción, con el importe en letras (evita adulteraciones del número).
                col.Item().Text(texto =>
                {
                    texto.Span("Recibimos de ");
                    texto.Span(nombre).Bold();
                    texto.Span(" la suma de ");
                    texto.Span($"PESOS {importeEnLetras}").Bold();
                    texto.Span($" ({importe}) en concepto de:");
                });

                // Detalle de lo pagado.
                col.Item().Table(tabla =>
                {
                    tabla.ColumnsDefinition(columnas =>
                    {
                        columnas.RelativeColumn(5);
                        columnas.RelativeColumn(3);
                        columnas.RelativeColumn(3);
                    });

                    tabla.Header(encabezado =>
                    {
                        encabezado.Cell().Element(CeldaEncabezado).Text("Concepto").Bold();
                        encabezado.Cell().Element(CeldaEncabezado).Text("Período").Bold();
                        encabezado.Cell().Element(CeldaEncabezado).AlignRight().Text("Importe").Bold();
                    });

                    tabla.Cell().Element(Celda).Text(DescripcionConcepto(c.Concepto));
                    tabla.Cell().Element(Celda).Text(periodo);
                    tabla.Cell().Element(Celda).AlignRight().Text(importe);

                    tabla.Cell().ColumnSpan(2).Element(CeldaTotal).AlignRight().Text("TOTAL").Bold();
                    tabla.Cell().Element(CeldaTotal).AlignRight().Text(importe).Bold().FontSize(12);
                });

                // Forma de pago. Con efectivo la cuenta es la misma "EFECTIVO": no se repite.
                col.Item().Column(pago =>
                {
                    pago.Item().Element(TituloSeccion).Text("FORMA DE PAGO").Bold();
                    pago.Item().PaddingTop(2, Unit.Millimetre).Element(x => Dato(x, "Medio de pago", Capitalizar(c.MetodoPago)));
                    if (c.Cuenta != c.MetodoPago)
                        pago.Item().Element(x => Dato(x, "Cuenta", Capitalizar(c.Cuenta)));
                });

                // Quién cobró y firma.
                col.Item().PaddingTop(14, Unit.Millimetre).Row(fila =>
                {
                    fila.RelativeItem().AlignBottom().Column(cobro =>
                    {
                        cobro.Item().Text($"Recibió: {c.Cajero}");
                        cobro.Item().Text($"Caja N° {c.NroCaja}");
                    });

                    fila.ConstantItem(70, Unit.Millimetre).Column(firma =>
                    {
                        firma.Item().LineHorizontal(0.8f);
                        firma.Item().PaddingTop(1, Unit.Millimetre).AlignCenter().Text("Firma y aclaración").FontSize(9);
                    });
                });
            });
        }

        /// <summary>Fila de datos: etiqueta a la izquierda, valor en negrita.</summary>
        private static void Dato(IContainer contenedor, string etiqueta, string valor)
        {
            contenedor.PaddingVertical(0.8f, Unit.Millimetre).Row(fila =>
            {
                fila.ConstantItem(40, Unit.Millimetre).Text($"{etiqueta}:").FontColor(GrisTexto);
                fila.RelativeItem().Text(valor).Bold();
            });
        }

        private static IContainer TituloSeccion(IContainer contenedor) =>
            contenedor.Background(GrisSuave).PaddingVertical(1.5f, Unit.Millimetre).PaddingHorizontal(2, Unit.Millimetre);

        private static IContainer CeldaEncabezado(IContainer contenedor) =>
            contenedor.Background(GrisSuave).BorderBottom(1).Padding(2, Unit.Millimetre);

        private static IContainer Celda(IContainer contenedor) =>
            contenedor.BorderBottom(0.5f).BorderColor(Colors.Grey.Lighten1).Padding(2, Unit.Millimetre);

        private static IContainer CeldaTotal(IContainer contenedor) =>
            contenedor.Padding(2, Unit.Millimetre);

        /// <summary>Nombre legible del concepto para el detalle del comprobante.</summary>
        private static string DescripcionConcepto(string concepto) => concepto switch
        {
            "SOCIO" => "Cuota societaria",
            "SEPELIO" => "Cuota de sepelio",
            "NICHO" => "Cuota de nicho",
            _ => $"Cuota {Capitalizar(concepto)}"
        };

        /// <summary>"TRANSFERENCIA BANCARIA" → "Transferencia Bancaria".</summary>
        private static string Capitalizar(string texto) =>
            PdfComun.CulturaAr.TextInfo.ToTitleCase(texto.ToLower(PdfComun.CulturaAr));

        private static string Valor(string texto) => string.IsNullOrWhiteSpace(texto) ? "-" : texto;
    }
}
