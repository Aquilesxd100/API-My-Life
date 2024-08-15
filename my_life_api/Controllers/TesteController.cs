using Microsoft.AspNetCore.Mvc;
using my_life_api.Filters;
using my_life_api.Models;

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
        public CustomResult Get()
        {
            return new CustomResult(200, "Tudo certo!");
        }
    }
}
