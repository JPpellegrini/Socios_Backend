using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using Socios.Application.DTOs;
using Socios.Application.Exceptions;
using Socios.Application.Interfaces;
using Socios.Domain.Entities;

namespace Socios.Application.UseCases.Prestaciones
{
    /// <summary>
    /// Casos de uso del catálogo de prestaciones (servicios que brindan proveedores y colaboradores).
    /// </summary>
    public class PrestacionUseCase : IPrestacionUseCase
    {
        private readonly IPrestacionRepository _prestaciones;
        private readonly IUnitOfWork _unitOfWork;

        public PrestacionUseCase(IPrestacionRepository prestaciones, IUnitOfWork unitOfWork)
        {
            _prestaciones = prestaciones;
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Estado "TODOS" = sin filtrar por estado; tipo vacío = ambos tipos. El nombre se busca
        /// como parte del nombre, sin distinguir mayúsculas ni tildes. Ese filtro se aplica en
        /// memoria (el catálogo es chico) porque la base no ignora tildes sin la extensión unaccent.
        /// </summary>
        public async Task<List<PrestacionDto>> ListarAsync(PrestacionFiltroDto filtro)
        {
            var prestaciones = await _prestaciones.ListarAsync(
                string.IsNullOrWhiteSpace(filtro.Tipo) ? null : filtro.Tipo,
                filtro.Estado == "TODOS" ? null : filtro.Estado);

            if (string.IsNullOrWhiteSpace(filtro.Nombre))
                return prestaciones;

            var buscado = ClaveComparacion(filtro.Nombre);
            return prestaciones.Where(p => ClaveComparacion(p.Nombre).Contains(buscado)).ToList();
        }

        /// <summary>
        /// Alta de una prestación. Reglas:
        ///   - El nombre se guarda en mayúsculas (como el resto de los datos del sistema), sin
        ///     espacios al principio ni al final y con un solo espacio entre palabras.
        ///   - No puede repetirse: se compara sin distinguir mayúsculas ni tildes, para que
        ///     "Podologia" no conviva con "PODOLOGÍA". Se compara también contra las inactivas:
        ///     en ese caso corresponde reactivarla, no crear otra igual.
        ///   - Se crea siempre ACTIVA.
        /// </summary>
        public async Task<PrestacionDto> CrearAsync(PrestacionCrearDto dto)
        {
            var nombre = NormalizarNombre(dto.Nombre);
            await ValidarNombreLibreAsync(nombre, idExcluida: null);

            var prestacion = new Prestacion
            {
                NombrePrestacion = nombre,
                Tipo = dto.Tipo,
                Estado = Prestacion.EstadoActiva
            };
            _prestaciones.Agregar(prestacion);
            await _unitOfWork.GuardarCambiosAsync();

            return ADto(prestacion);
        }

        /// <summary>
        /// Modificación del nombre y el tipo. Mismas reglas que el alta para el nombre (sin contar
        /// a la propia prestación). Se puede modificar aunque esté inactiva. Los proveedores y
        /// colaboradores que la tienen asignada pasan a mostrar el nombre nuevo.
        /// </summary>
        public async Task<PrestacionDto> ModificarAsync(int idPrestacion, PrestacionModificarDto dto)
        {
            var prestacion = await ObtenerExistenteAsync(idPrestacion);

            var nombre = NormalizarNombre(dto.Nombre);
            await ValidarNombreLibreAsync(nombre, idExcluida: idPrestacion);

            prestacion.NombrePrestacion = nombre;
            prestacion.Tipo = dto.Tipo;
            await _unitOfWork.GuardarCambiosAsync();

            return ADto(prestacion);
        }

        /// <summary>
        /// Baja lógica: pasa a INACTIVA, deja de ofrecerse para asignar, pero no se borra. Los
        /// proveedores y colaboradores que ya la tienen la conservan, y los movimientos y
        /// reportes históricos siguen mostrando su nombre.
        /// </summary>
        public async Task<PrestacionDto> DarDeBajaAsync(int idPrestacion)
        {
            var prestacion = await ObtenerExistenteAsync(idPrestacion);
            if (prestacion.Estado == Prestacion.EstadoInactiva)
                throw new ReglaNegocioException($"La prestación '{prestacion.NombrePrestacion}' ya está inactiva.");

            prestacion.Estado = Prestacion.EstadoInactiva;
            await _unitOfWork.GuardarCambiosAsync();

            return ADto(prestacion);
        }

        /// <summary>Vuelve a dejar ACTIVA una prestación inactiva.</summary>
        public async Task<PrestacionDto> ReactivarAsync(int idPrestacion)
        {
            var prestacion = await ObtenerExistenteAsync(idPrestacion);
            if (prestacion.Estado == Prestacion.EstadoActiva)
                throw new ReglaNegocioException($"La prestación '{prestacion.NombrePrestacion}' ya está activa.");

            prestacion.Estado = Prestacion.EstadoActiva;
            await _unitOfWork.GuardarCambiosAsync();

            return ADto(prestacion);
        }

        private async Task<Prestacion> ObtenerExistenteAsync(int idPrestacion) =>
            await _prestaciones.ObtenerAsync(idPrestacion)
                ?? throw new RecursoNoEncontradoException($"No existe una prestación con el Id {idPrestacion}.");

        /// <summary>
        /// Lanza ReglaNegocioException si otra prestación (de cualquier tipo y estado) ya usa ese
        /// nombre. El catálogo es chico: se compara en memoria para poder ignorar tildes.
        /// </summary>
        private async Task ValidarNombreLibreAsync(string nombre, int? idExcluida)
        {
            var clave = ClaveComparacion(nombre);
            var existentes = await _prestaciones.ListarAsync();
            var repetida = existentes.FirstOrDefault(p => p.IdPrestacion != idExcluida && ClaveComparacion(p.Nombre) == clave);
            if (repetida is null)
                return;

            throw new ReglaNegocioException(repetida.Estado == Prestacion.EstadoActiva
                ? $"Ya existe la prestación '{repetida.Nombre}'."
                : $"Ya existe la prestación '{repetida.Nombre}', inactiva. Puede reactivarla en lugar de crear otra.");
        }

        private static string NormalizarNombre(string nombre) =>
            Regex.Replace(nombre.Trim(), @"\s+", " ").ToUpper(new CultureInfo("es-AR"));

        private static PrestacionDto ADto(Prestacion p) => new()
        {
            IdPrestacion = p.Id_Prestacion,
            Nombre = p.NombrePrestacion,
            Tipo = p.Tipo,
            Estado = p.Estado
        };

        /// <summary>Nombre sin tildes, en mayúsculas y con espacios normalizados, para comparar.</summary>
        private static string ClaveComparacion(string texto)
        {
            var descompuesto = Regex.Replace(texto.Trim(), @"\s+", " ").ToUpperInvariant().Normalize(NormalizationForm.FormD);
            var sinTildes = new StringBuilder(descompuesto.Length);
            foreach (var c in descompuesto)
                if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                    sinTildes.Append(c);
            return sinTildes.ToString().Normalize(NormalizationForm.FormC);
        }
    }
}
