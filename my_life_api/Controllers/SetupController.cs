using Microsoft.AspNetCore.Mvc;
using my_life_api.Resources;
using my_life_api.Services;
using my_life_api.ValidationFilters.Security;

namespace my_life_api.Controllers;

[ApiController]
public class SetupController : ControllerBase {
    private readonly ILogger<SetupController> _logger;

    public SetupController(ILogger<SetupController> logger) {
        _logger = logger;
    }

    [HttpPost("montarEstrutura", Name = "montarEstrutura")]
    [ServiceFilter(typeof(TokenValidationFilter))]
    public async Task<IActionResult> Post() {
        SetupService service = new SetupService();
        await service.Setup();
        
        return Ok(ApiResponse.CreateBody(201, "A estrutura da aplicação foi criada com sucesso!"));
    }
}
