namespace Socios.Domain.Entities
{
    public class Prestacion
    {
        public const string TipoColaboracion = "COLABORACION";
        public const string TipoPrestacion = "PRESTACION";
        public const string EstadoActiva = "ACTIVA";
        public const string EstadoInactiva = "INACTIVA";

        public int Id_Prestacion { get; set; }
        public string NombrePrestacion { get; set; } = null!;

        /// <summary>COLABORACION o PRESTACION.</summary>
        public string Tipo { get; set; } = null!;

        /// <summary>
        /// ACTIVA o INACTIVA. Una prestación INACTIVA no se ofrece para asignar a proveedores ni
        /// colaboradores nuevos, pero los que ya la tienen la conservan (y sus movimientos también).
        /// </summary>
        public string Estado { get; set; } = EstadoActiva;
    }
}
