namespace Socios.Application.DTOs
{
    /// <summary>
    /// DTO de salida para visualizar un proveedor: trae TODOS los datos necesarios para
    /// cargar la pantalla de detalle/edición.
    ///
    /// Reúne datos de varias tablas: Entidad (identidad y domicilio), Proveedor + Prestacion
    /// (servicio prestado), EntidadTipo (estado y fecha de alta), EntidadBaja (fecha de baja
    /// si corresponde) y Contactos (teléfonos y emails).
    /// </summary>
    public class ProveedorDetalleDto
    {
        public int IdProveedor { get; set; }
        public int IdEntidad { get; set; }

        // Documento / identidad
        public string? TipoDocumento { get; set; }
        public string? Dni { get; set; }
        public string? CuitCuil { get; set; }

        // Datos (persona o empresa)
        public string? Nombre { get; set; }
        public string? Apellido { get; set; }
        public string? RazonSocial { get; set; }
        public DateTime? FechaNacimiento { get; set; }   // nacimiento / constitución

        // Domicilio
        public int IdCiudad { get; set; }
        public string? Ciudad { get; set; }   // nombre, para mostrar
        public string? Calle { get; set; }
        public int? Altura { get; set; }
        public string? Observaciones { get; set; }

        // Servicio prestado
        public int IdPrestacion { get; set; }
        public string? ServicioPrestado { get; set; }   // nombre de la prestación, para mostrar

        // Estado (solo lectura en la pantalla)
        public string Estado { get; set; } = null!;
        public DateTime FechaAlta { get; set; }
        public DateTime? FechaBaja { get; set; }

        // Contactos
        public List<string> Telefonos { get; set; } = new();
        public List<string> Emails { get; set; } = new();
    }
}
