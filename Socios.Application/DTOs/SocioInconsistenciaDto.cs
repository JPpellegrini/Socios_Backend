namespace Socios.Application.DTOs
{
    /// <summary>
    /// Socio al que no se le pudo generar alguna cuota por un dato mal cargado (por ejemplo,
    /// tiene sepelio pero no tiene plan o fecha de nacimiento). Se informa para corregirlo.
    /// </summary>
    public class SocioInconsistenciaDto
    {
        public int IdSocio { get; set; }
        public string Motivo { get; set; } = null!;
    }
}
