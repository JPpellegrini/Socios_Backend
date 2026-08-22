using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Socios.Application.DTOs;
using Socios.Application.Interfaces;
using Socios.Application.UseCases.Codeudores;

namespace Socios.Api.Controllers
{
    [ApiController]
    [Route("api/v1/codeudores")]
    [Authorize]
    public class CodeudoresController : ControllerBase
    {
        private readonly ICodeudorUseCase _codeudorUseCase;

        public CodeudoresController(ICodeudorUseCase codeudorUseCase)
        {
            _codeudorUseCase = codeudorUseCase;
        }

        [HttpPost("crear")]
        public async Task<IActionResult> CrearCodeudorAsync([FromBody] CodeudorCrearDto dto)
        {
            var idEntidad = await _codeudorUseCase.CrearAsync(dto);

            return Ok(new { idEntidad });
        }
    }
}
