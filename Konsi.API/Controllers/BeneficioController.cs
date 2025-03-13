using Konsi.API.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Konsi.API.Controllers
{
    [ApiController]
    [Route("api/v1/inss")]
    public class BeneficioController : ControllerBase
    {
        private readonly BeneficioService _service;

        public BeneficioController(BeneficioService beneficioService)
        {
            _service = beneficioService;
        }

        [HttpGet("consulta-beneficios")]
        public async Task<IActionResult> GetBeneficio([FromQuery] string cpf)
        {
            if (string.IsNullOrWhiteSpace(cpf))
            {
                return BadRequest("O CPF é obrigatório.");
            }

            var beneficios = await _service.GetBenefitsAsync(cpf); // Busca dados da API externa.
            return Ok(beneficios);
        }
    }

}
