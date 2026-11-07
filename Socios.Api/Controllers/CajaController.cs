using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Socios.Application.DTOs;
using Socios.Application.UseCases.CajaDiaria;

namespace Socios.Api.Controllers
{
    [ApiController]
    [Route("api/v1/caja")]
    [Authorize]
    public class CajaController : ControllerBase
    {
        private readonly ICajaDiariaUseCase _cajaDiariaUseCase;

        public CajaController(ICajaDiariaUseCase cajaDiariaUseCase)
        {
            _cajaDiariaUseCase = cajaDiariaUseCase;
        }

        /// <summary>
        /// Abre la caja diaria del usuario autenticado con el efectivo que declara. Body:
        /// { saldoInicial }. El usuario sale del login y la fecha/hora es la del servidor
        /// (se devuelve en FechaHoraApertura). El número de caja lo asigna el sistema
        /// (correlativo) y se devuelve junto con el saldo del último cierre del usuario y la
        /// diferencia (informativa, no bloquea).
        ///   - Saldo inicial ausente, negativo o con más de 2 decimales → 400.
        ///   - El usuario ya tiene una caja abierta                     → 409.
        /// </summary>
        [HttpPost("apertura")]
        public async Task<IActionResult> AbrirAsync([FromBody] CajaAperturaDto dto)
        {
            // La caja se abre a nombre del usuario autenticado.
            var idUsuario = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var resultado = await _cajaDiariaUseCase.AbrirAsync(dto, idUsuario);

            return Ok(resultado);
        }
    }
}
