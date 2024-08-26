using Microsoft.AspNetCore.Mvc;
using my_life_api.Validators;
using my_life_api.Resources;

namespace my_life_api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TesteController : ControllerBase
    {

        private readonly ILogger<TesteController> _logger;

        public TesteController(ILogger<TesteController> logger)
        {
            _logger = logger;
        }

        [HttpGet(Name = "teste")]
        [ServiceFilter(typeof(TokenValidationFilter))]
        public IActionResult Get()
        {
            return Ok(ApiResponse.CreateBody(200, "Tudo certo!"));
        }
    }
}
