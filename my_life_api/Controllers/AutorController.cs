using Microsoft.AspNetCore.Mvc;
using my_life_api.Validators;
using my_life_api.Resources;
using my_life_api.Services;
using my_life_api.Models;
using my_life_api.Models.Requests;

namespace my_life_api.Controllers
{
    [ApiController]
    public class AutorController : ControllerBase
    {

        private readonly ILogger<AutorController> _logger;

        public AutorController(ILogger<AutorController> logger)
        {
            _logger = logger;
        }

        [HttpGet("autores", Name = "autores")]
        [ServiceFilter(typeof(TokenValidationFilter))]
        [ServiceFilter(typeof(ContentTypeParamValidationFilter))]
        public async Task<IActionResult> Get([FromQuery] string idTipoConteudo){
            AuthorService service = new AuthorService();

            ContentTypesEnum contentTypeId = (ContentTypesEnum)Int32.Parse(idTipoConteudo);
            IEnumerable<AuthorDTO> authors = await service.GetAuthorsByContentTypeId(contentTypeId);

            return Ok(ApiResponse.CreateBody(
                200, 
                "Lista de autores recebida com sucesso.",
                new { autores = authors }
            ));
        }

        [HttpPost("autor", Name = "autor")]
        [ServiceFilter(typeof(TokenValidationFilter))]
        [ServiceFilter(typeof(CreateAuthorValidationFilter))]
        public async Task<IActionResult> Post(
            [FromForm] string nome,
            [FromForm] short idTipoConteudo,
            [FromForm] IFormFile imagem
        ){
            AuthorService service = new AuthorService();

            CreateAuthorRequestDTO authorReq = new CreateAuthorRequestDTO
            {
                nome = nome,
                idTipoConteudo = idTipoConteudo,
                imagem = imagem
            };

            await service.CreateAuthor(authorReq);

            return Ok(ApiResponse.CreateBody(201, "Autor criado com sucesso!"));
        }
    }
}
