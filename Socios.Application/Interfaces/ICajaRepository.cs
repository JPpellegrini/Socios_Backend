using Socios.Application.DTOs;
using Socios.Domain.Entities;

namespace Socios.Application.Interfaces
{
    public interface ICajaRepository
    {
        /// <summary>Marca el movimiento de caja para ser insertado. NO guarda: eso lo hace la unidad de trabajo.</summary>
        void Agregar(Caja movimiento);

        /// <summary>
        /// Trae los datos para imprimir el comprobante de pago de un movimiento de caja que
        /// corresponde al cobro de una cuota. Null si el movimiento no existe o no es el pago
        /// de una cuota. Solo lectura.
        /// </summary>
        Task<ComprobantePagoDto?> ObtenerComprobantePagoAsync(int idMovimiento);
    }
}
