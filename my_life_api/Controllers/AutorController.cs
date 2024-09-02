using Microsoft.AspNetCore.Mvc;
using my_life_api.Validators;
using my_life_api.Resources;
using my_life_api.Models;
using my_life_api.Services;

namespace my_life_api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AutorController : ControllerBase
    {

        private readonly ILogger<AutorController> _logger;

        public AutorController(ILogger<AutorController> logger)
        {
            _logger = logger;
        }

        [HttpPost(Name = "autor")]
        [ServiceFilter(typeof(TokenValidationFilter))]
        [ServiceFilter(typeof(AuthorValidationFilter))]
        public async Task<IActionResult> Post([FromBody] AuthorDTO request)
        {
            AuthorService service = new AuthorService();
            await service.CreateAuthor(request);

            return Ok(ApiResponse.CreateBody(201, "Autor criado com sucesso!"));
        }
    }
}
