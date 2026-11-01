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
    /// Genera el PDF del informe de socios con QuestPDF.
    /// Diseño: A4 apaisada, encabezado con la institución y los filtros aplicados, una tabla
    /// con una fila por socio (el encabezado de la tabla se repite en cada hoja) y pie con
    /// la fecha de emisión y el número de página.
    /// </summary>
    public class InformeSociosPdf : IInformeSociosPdf
    {
        // Misma fuente que los comprobantes: viene incluida en QuestPDF (ver ComprobanteCuotaPdf).
        private const string Fuente = "Lato";
        private const string SinDato = "-";

        private static readonly CultureInfo CulturaAr = new("es-AR");

        private readonly ComprobanteOpciones _opciones;

        static InformeSociosPdf()
        {
            // Misma declaración de licencia que en ComprobanteCuotaPdf: se repite acá porque el
            // informe puede ser el primer documento que se genere desde que arrancó la API.
            QuestPDF.Settings.License = LicenseType.Community;
        }

        public InformeSociosPdf(IOptions<ComprobanteOpciones> opciones)
        {
            // Se reutilizan los datos de la institución que ya se imprimen en los comprobantes.
            _opciones = opciones.Value.ConValoresPorDefecto();
        }

        public byte[] Generar(IReadOnlyList<SocioInformeDto> socios, SocioInformeFiltroDto filtro)
        {
            var emision = DateTime.Now;

            return Document.Create(documento =>
                {
                    documento.Page(pagina =>
                    {
                        pagina.Size(PageSizes.A4.Landscape());
                        pagina.Margin(10, Unit.Millimetre);
                        pagina.DefaultTextStyle(estilo => estilo.FontFamily(Fuente).FontSize(8.5f));

                        pagina.Header().Element(x => Encabezado(x, filtro, socios.Count));
                        pagina.Content().PaddingTop(4, Unit.Millimetre).Element(x => Tabla(x, socios));
                        pagina.Footer().Element(x => Pie(x, emision));
                    });
                })
                .WithMetadata(new DocumentMetadata { Title = "Informe de socios" })
                .GeneratePdf();
        }

        private void Encabezado(IContainer contenedor, SocioInformeFiltroDto filtro, int cantidad)
        {
            contenedor.Column(col =>
            {
                col.Item().Text(_opciones.Institucion).Bold().FontSize(12);
                col.Item().Text($"{_opciones.Domicilio}    Pers. Jurídica {_opciones.PersoneriaJuridica}    Tel.: {_opciones.Telefono}")
                    .FontSize(8).FontColor(Colors.Grey.Darken2);

                col.Item().PaddingTop(3, Unit.Millimetre).Row(fila =>
                {
                    fila.RelativeItem().Text("Informe de socios").Bold().FontSize(15);
                    fila.AutoItem().AlignBottom().Text($"Total: {cantidad} socio{(cantidad == 1 ? "" : "s")}").Bold().FontSize(10);
                });

                col.Item().PaddingTop(1, Unit.Millimetre).Text(texto =>
                {
                    texto.Span("Estado: ").FontColor(Colors.Grey.Darken2);
                    texto.Span(DescripcionEstado(filtro.Estado)).Bold();
                    texto.Span("    Categorías: ").FontColor(Colors.Grey.Darken2);
                    texto.Span(DescripcionCategorias(filtro)).Bold();
                });

                col.Item().PaddingTop(2, Unit.Millimetre).LineHorizontal(0.8f).LineColor(Colors.Black);
            });
        }

        private static void Tabla(IContainer contenedor, IReadOnlyList<SocioInformeDto> socios)
        {
            contenedor.Table(tabla =>
            {
                // Anchos relativos pensados para A4 apaisada (277 mm útiles).
                tabla.ColumnsDefinition(columnas =>
                {
                    columnas.RelativeColumn(17); // Nombre y apellido
                    columnas.RelativeColumn(13); // Domicilio
                    columnas.RelativeColumn(8);  // Ciudad
                    columnas.RelativeColumn(8);  // Teléfono
                    columnas.RelativeColumn(21); // Mail
                    columnas.RelativeColumn(10); // Obra social
                    columnas.RelativeColumn(5);  // Edad
                    columnas.RelativeColumn(6);  // Sepelio
                    columnas.RelativeColumn(5);  // Nicho
                    columnas.RelativeColumn(7);  // Estado
                });

                // El encabezado de la tabla se repite automáticamente en cada hoja.
                tabla.Header(encabezado =>
                {
                    foreach (var titulo in new[] { "Apellido y nombre", "Domicilio", "Ciudad", "Teléfono", "Mail", "Obra social" })
                        CeldaTitulo(encabezado.Cell()).Text(titulo);

                    foreach (var titulo in new[] { "Edad", "Sepelio", "Nicho", "Estado" })
                        CeldaTitulo(encabezado.Cell()).AlignCenter().Text(titulo);
                });

                for (var i = 0; i < socios.Count; i++)
                {
                    var s = socios[i];
                    // Filas alternadas en gris claro para seguir la línea con la vista.
                    var fondo = i % 2 == 0 ? Colors.White : Colors.Grey.Lighten4;

                    Celda(tabla.Cell(), fondo).Text(NombreCompleto(s)).Bold();
                    Celda(tabla.Cell(), fondo).Text(Domicilio(s));
                    Celda(tabla.Cell(), fondo).Text(Valor(s.Ciudad));
                    Celda(tabla.Cell(), fondo).Text(Valor(s.Telefono));
                    Celda(tabla.Cell(), fondo).Text(Valor(s.Mail));
                    Celda(tabla.Cell(), fondo).Text(Valor(s.ObraSocial));
                    Celda(tabla.Cell(), fondo).AlignCenter().Text(s.Edad?.ToString() ?? SinDato);
                    Celda(tabla.Cell(), fondo).AlignCenter().Text(s.Sepelio ? "SI" : "NO");
                    Celda(tabla.Cell(), fondo).AlignCenter().Text(s.Nicho ? "SI" : "NO");
                    Celda(tabla.Cell(), fondo).AlignCenter().Text(Valor(s.Estado));
                }
            });
        }

        private static void Pie(IContainer contenedor, DateTime emision)
        {
            contenedor.PaddingTop(2, Unit.Millimetre).Row(fila =>
            {
                fila.RelativeItem().Text($"Emitido el {emision.ToString("dd/MM/yyyy HH:mm", CulturaAr)}")
                    .FontSize(7.5f).FontColor(Colors.Grey.Darken1);
                fila.RelativeItem().AlignRight().Text(texto =>
                {
                    texto.DefaultTextStyle(estilo => estilo.FontSize(7.5f).FontColor(Colors.Grey.Darken1));
                    texto.Span("Página ");
                    texto.CurrentPageNumber();
                    texto.Span(" de ");
                    texto.TotalPages();
                });
            });
        }

        private static IContainer CeldaTitulo(IContainer celda) =>
            celda.Background(Colors.Grey.Lighten2)
                .BorderBottom(0.8f).BorderColor(Colors.Black)
                .PaddingVertical(1.5f, Unit.Millimetre).PaddingHorizontal(1.2f, Unit.Millimetre)
                .DefaultTextStyle(estilo => estilo.Bold().FontSize(8));

        private static IContainer Celda(IContainer celda, string fondo) =>
            celda.Background(fondo)
                .BorderBottom(0.3f).BorderColor(Colors.Grey.Lighten1)
                .PaddingVertical(1.2f, Unit.Millimetre).PaddingHorizontal(1.2f, Unit.Millimetre);

        private static string DescripcionEstado(string estado) => estado switch
        {
            "ACTIVO" => "Activos",
            "INACTIVO" => "Inactivos",
            _ => "Todos"
        };

        private static string DescripcionCategorias(SocioInformeFiltroDto filtro)
        {
            var categorias = new List<string>();
            if (filtro.IncluirAdherentes) categorias.Add("Adherente");
            if (filtro.IncluirSepelio) categorias.Add("Sepelio");
            if (filtro.IncluirNicho) categorias.Add("Nicho");
            return categorias.Count == 3 ? "Todas" : string.Join(", ", categorias);
        }

        private static string NombreCompleto(SocioInformeDto s)
        {
            var apellido = (s.Apellido ?? string.Empty).ToUpper(CulturaAr).Trim();
            var nombre = (s.Nombre ?? string.Empty).Trim();
            return apellido.Length > 0 && nombre.Length > 0 ? $"{apellido}, {nombre}" : Valor(apellido + nombre);
        }

        private static string Domicilio(SocioInformeDto s) => Valor($"{s.Calle} {s.Altura}".Trim());

        private static string Valor(string? texto) =>
            string.IsNullOrWhiteSpace(texto) ? SinDato : texto.Trim();
    }
}
