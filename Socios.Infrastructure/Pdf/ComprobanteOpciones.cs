namespace Socios.Infrastructure.Pdf
{
    /// <summary>
    /// Datos fijos que se imprimen en los comprobantes (encabezado de la institución y
    /// leyenda al pie). Se pueden cambiar desde la sección "Comprobante" del appsettings;
    /// lo que no venga configurado toma los datos del Centro (ver <see cref="ConValoresPorDefecto"/>).
    /// El appsettings.json no se versiona (tiene la contraseña de la base), así que en otra
    /// máquina o en el servidor la sección puede no existir: por eso los valores por defecto
    /// viven en el código y el PDF nunca sale sin encabezado.
    /// </summary>
    public class ComprobanteOpciones
    {
        public string Institucion { get; set; } = string.Empty;
        public string Domicilio { get; set; } = string.Empty;
        public string PersoneriaJuridica { get; set; } = string.Empty;
        public string Telefono { get; set; } = string.Empty;

        /// <summary>Líneas de la leyenda al pie del comprobante, en orden.</summary>
        public string[] Leyenda { get; set; } = Array.Empty<string>();

        /// <summary>
        /// Devuelve una copia con los datos del Centro en los campos que no vengan configurados.
        /// Los valores por defecto se aplican acá y no como valor inicial de las propiedades:
        /// al leer el appsettings, .NET agrega los elementos de un arreglo a los que ya tiene en
        /// vez de reemplazarlos, y la leyenda saldría duplicada.
        /// </summary>
        public ComprobanteOpciones ConValoresPorDefecto() => new()
        {
            Institucion = Valor(Institucion, "Centro de Jubilados y Pensionados de Roldán"),
            Domicilio = Valor(Domicilio, "Independencia 176"),
            PersoneriaJuridica = Valor(PersoneriaJuridica, "383"),
            Telefono = Valor(Telefono, "496-1270"),
            Leyenda = Leyenda.Length > 0
                ? Leyenda
                : new[]
                {
                    "Sr. asociado mantenga su cuota al día",
                    "En caso de atraso en tres meses se le dará de baja en forma automática",
                    "de acuerdo a los estatutos en vigencia."
                }
        };

        private static string Valor(string configurado, string porDefecto) =>
            string.IsNullOrWhiteSpace(configurado) ? porDefecto : configurado;
    }
}
