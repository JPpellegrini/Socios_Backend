namespace Socios.Application.DTOs
{
    /// <summary>Cuenta activa para los desplegables (ej. origen/destino de una transferencia).</summary>
    public class CuentaListadoDto
    {
        public int IdCuenta { get; set; }

        public string NombreCuenta { get; set; } = null!;

        public int NroCuenta { get; set; }
    }
}
