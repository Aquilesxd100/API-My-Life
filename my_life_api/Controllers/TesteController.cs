using Microsoft.AspNetCore.Mvc;

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
        public string Get()
        {
            return "oi";
        }
    }
}
