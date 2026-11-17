using Socios.Application.Exceptions;
using Socios.Application.Interfaces;
using Socios.Domain.Entities;

namespace Socios.Application.UseCases.Prestaciones
{
    /// <summary>
    /// Regla común para asignar una prestación (servicio prestado) a un proveedor o colaborador.
    /// </summary>
    public static class PrestacionAsignacion
    {
        /// <summary>
        /// La prestación tiene que existir, estar ACTIVA y ser del tipo que corresponde a quien
        /// la recibe: COLABORACION para un colaborador, PRESTACION para un proveedor
        /// (<paramref name="tipoRequerido"/>).
        ///
        /// Excepción: al modificar, si se deja la misma prestación que ya tenía
        /// (<paramref name="idPrestacionActual"/>), se acepta aunque después se haya dado de baja
        /// o se le haya cambiado el tipo, para poder editar los demás datos sin obligar a cambiar
        /// el servicio.
        /// </summary>
        public static async Task ValidarAsync(IPrestacionRepository prestaciones, int idPrestacion, string tipoRequerido, int? idPrestacionActual = null)
        {
            var datos = await prestaciones.ObtenerTipoYEstadoAsync(idPrestacion)
                ?? throw new ReglaNegocioException($"No existe una prestación con el Id {idPrestacion} para asignar como servicio prestado.");

            if (idPrestacion == idPrestacionActual)
                return;

            if (datos.Estado != Prestacion.EstadoActiva)
                throw new ReglaNegocioException("La prestación indicada está inactiva y no se puede asignar como servicio prestado.");

            if (datos.Tipo != tipoRequerido)
                throw new ReglaNegocioException(tipoRequerido == Prestacion.TipoColaboracion
                    ? "A un colaborador solo se le pueden asignar prestaciones de tipo COLABORACION."
                    : "A un proveedor solo se le pueden asignar prestaciones de tipo PRESTACION.");
        }
    }
}
