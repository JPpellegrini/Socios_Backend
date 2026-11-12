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

        public Task<List<PrestacionDto>> ListarAsync() => _prestaciones.ListarAsync();

        /// <summary>
        /// Alta de una prestación. Reglas:
        ///   - El nombre se guarda en mayúsculas (como el resto de los datos del sistema), sin
        ///     espacios al principio ni al final y con un solo espacio entre palabras.
        ///   - No puede repetirse: se compara sin distinguir mayúsculas ni tildes, para que
        ///     "Podologia" no conviva con "PODOLOGÍA".
        /// </summary>
        public async Task<PrestacionDto> CrearAsync(PrestacionCrearDto dto)
        {
            var nombre = Regex.Replace(dto.Nombre.Trim(), @"\s+", " ").ToUpper(new CultureInfo("es-AR"));

            // El catálogo es chico: se compara en memoria para poder ignorar tildes.
            var existentes = await _prestaciones.ListarAsync();
            var repetida = existentes.FirstOrDefault(p => ClaveComparacion(p.Nombre) == ClaveComparacion(nombre));
            if (repetida is not null)
                throw new ReglaNegocioException($"Ya existe la prestación '{repetida.Nombre}'.");

            var prestacion = new Prestacion { NombrePrestacion = nombre };
            _prestaciones.Agregar(prestacion);
            await _unitOfWork.GuardarCambiosAsync();

            return new PrestacionDto { IdPrestacion = prestacion.Id_Prestacion, Nombre = prestacion.NombrePrestacion };
        }

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
