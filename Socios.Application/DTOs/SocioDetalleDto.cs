namespace Socios.Application.DTOs
{
    /// <summary>
    /// DTO de salida para visualizar un socio: trae TODOS los datos necesarios para
    /// volver a cargar la pantalla de alta (son los mismos campos).
    ///
    /// Reúne datos de varias tablas: Entidad (datos personales y domicilio), Socio
    /// (plan, sepelio, etc.), EntidadTipo (estado y fecha de alta), EntidadBaja (fecha de
    /// baja si corresponde) y Contactos (teléfonos y emails).
    /// </summary>
    public class SocioDetalleDto
    {
        public int IdSocio { get; set; }

        // Documento / identidad
        public string? TipoDocumento { get; set; }
        public string? Dni { get; set; }

        // Datos personales
        public string? Nombre { get; set; }
        public string? Apellido { get; set; }
        public DateTime? FechaNacimiento { get; set; }
        public string? Sexo { get; set; }

        // Domicilio
        public int IdCiudad { get; set; }
        public string? Ciudad { get; set; }   // nombre, para mostrar
        public string? Calle { get; set; }
        public int? Altura { get; set; }
        public string? Observaciones { get; set; }

        // Estado (solo lectura en la pantalla)
        public string Estado { get; set; } = null!;
        public DateTime FechaAlta { get; set; }
        public DateTime? FechaBaja { get; set; }

        // Datos de socio
        public int? IdObraSocial { get; set; }
        public string? ObraSocial { get; set; }   // nombre, para mostrar
        public string? NumeroAfiliado { get; set; }
        public string? Plan { get; set; }
        public string? Sepelio { get; set; }
        public string? Cobrador { get; set; }

        // Contactos
        public List<string> Telefonos { get; set; } = new();
        public List<string> Emails { get; set; } = new();
    }
}
