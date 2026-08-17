namespace Socios.Application.DTOs
{
    /// <summary>
    /// DTO de salida para el listado de proveedores (pantalla "Proveedores").
    /// Trae los datos del proveedor junto con los de su entidad, el servicio que
    /// presta y el estado (que vive en la fila de EntidadTipo de tipo "Proveedor").
    /// </summary>
    public class ProveedorListadoDto
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

        // Domicilio
        public int IdCiudad { get; set; }
        public string? Ciudad { get; set; }   // nombre, para mostrar
        public string? Calle { get; set; }
        public int? Altura { get; set; }
        public string? Observacion { get; set; }

        // Servicio prestado
        public int IdPrestacion { get; set; }
        public string? ServicioPrestado { get; set; }   // nombre de la prestación, para mostrar

        // Estado (vive en EntidadTipo)
        public string Estado { get; set; } = null!;
    }
}
