using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Socios.Application.DTOs;
using Socios.Application.Interfaces;
using Socios.Application.UseCases.Socios;

namespace Socios.Api.Controllers
{
    [ApiController]
    [Route("api/v1/socios")]
    [Authorize]
    public class SociosController : ControllerBase
    {
        private readonly ISocioRepository _socioRepository;
        private readonly ICrearSocioUseCase _crearSocioUseCase;

        public SociosController(ISocioRepository socioRepository, ICrearSocioUseCase crearSocioUseCase)
        {
            _socioRepository = socioRepository;
            _crearSocioUseCase = crearSocioUseCase;
        }

        /// <summary>
        /// Lista los socios para la pantalla "Socios".
        /// Filtros (todos opcionales, combinados con AND):
        ///   - busqueda: coincidencia parcial por Nombre o Apellido.
        ///   - dni: coincidencia parcial por DNI.
        ///   - incluirInactivos: false (default) solo ACTIVOS; true incluye INACTIVOS.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> BuscarSociosAsync([FromBody] SocioFiltroDto filtro)
        {
            var socios = await _socioRepository.BuscarAsync(filtro);

            return Ok(socios);
        }

        /// <summary>
        /// Da de alta un socio. La orquestación (crear o reutilizar la entidad, evitar
        /// duplicados, armar el socio + tipo + contactos y confirmar todo en una sola
        /// transacción) vive en el caso de uso, no acá.
        ///
        /// El controlador solo recibe la solicitud y devuelve la respuesta:
        ///   - Las validaciones de formato las aplica [ApiController] → 400 automático.
        ///   - Las reglas de negocio (ej: "ya es socio") las maneja el ExceptionMiddleware → 409.
        /// </summary>
        [HttpPost("crear")]
        public async Task<IActionResult> CrearSocioAsync([FromBody] SocioCrearDto dto)
        {
            var idSocio = await _crearSocioUseCase.EjecutarAsync(dto);

            return Ok(new { idSocio });
        }

        /// <summary>
        /// Da de baja un socio: cambia su estado a INACTIVO en EntidadTipo
        /// y registra la baja en EntidadBajas con fecha y motivo.
        /// </summary>
        [HttpPost("baja")]
        public async Task<IActionResult> DarDeBajaSocioAsync([FromBody] SocioBajaDto dto)
        {
            await _socioRepository.DarDeBajaAsync(dto);

            return Ok(new { mensaje = "El socio fue dado de baja correctamente." });
        }

    }
}
