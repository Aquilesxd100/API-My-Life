using Microsoft.AspNetCore.Mvc;
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
        public async Task<string> Get()
        {
            string response = await DataBase.TesteConexao();
            return response;
        }
    }
}
