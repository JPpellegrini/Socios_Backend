namespace Socios.Application.DTOs
{
    public class EmpleadoModificarDto
    {
        public int IdEntidad { get; set; }              // Identificador del empleado a modificar
        public string? Dni { get; set; }
        public string? CuitCuil { get; set; }
        public string? Nombre { get; set; }
        public string? Apellido { get; set; }
        public string? Sexo { get; set; }
        public DateTime? Nacimiento { get; set; }
        public int? IdCiudad { get; set; }              // Nueva ciudad
        public string Calle { get; set; } = null!;      // Nueva calle
        public int? Altura { get; set; }                // Nueva altura
        public string? Observaciones { get; set; }      // Observaciones

        // Datos propios del empleado
        public int? IdObraSocial { get; set; }          // Obra social
        public string? NumeroAfiliado { get; set; }     // Número de afiliado
        public string? Cargo { get; set; }              // Cargo del empleado
        public decimal? Sueldo { get; set; }            // Sueldo (si lo manejás en la entidad)

        // Contactos
        public List<string> Telefonos { get; set; } = new();
        public List<string>? Emails { get; set; }
    }
}
