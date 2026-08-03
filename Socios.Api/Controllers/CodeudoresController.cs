using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Socios.Application.DTOs;
using Socios.Application.Interfaces;

namespace Socios.Api.Controllers
{
    [ApiController]
    [Route("api/v1/codeudores")]
    [Authorize]
    public class CodeudoresController : ControllerBase
    {
        private readonly ICodeudorRepository _codeudorRepository;

        public CodeudoresController(ICodeudorRepository codeudorRepository)
        {
            _codeudorRepository = codeudorRepository;
        }
    }
}
