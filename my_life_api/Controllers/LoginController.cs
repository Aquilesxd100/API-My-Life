using Microsoft.AspNetCore.Mvc;
using my_life_api.Services;
using my_life_api.Resources;
using my_life_api.Models.Requests;
using my_life_api.Validators.Security;

namespace my_life_api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class LoginController : ControllerBase
    {
        private readonly ILogger<LoginController> _logger;

        public LoginController(ILogger<LoginController> logger)
        {
            _logger = logger;
        }

        [HttpPost(Name = "login")]
        [ServiceFilter(typeof(LoginValidationFilter))]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            AuthorizationService authorizationService = new AuthorizationService();
            
            string token = authorizationService.Login(request.senha);
            return Ok(ApiResponse.CreateBody(200, "Login efetuado com sucesso.", new { token } ));
        }
    }
}
