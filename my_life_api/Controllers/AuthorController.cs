using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using my_life_api.Validators;
using my_life_api.Resources;
using my_life_api.Services;
using my_life_api.Models;
using my_life_api.Models.Requests;
using my_life_api.Validators.Author;
using my_life_api.Validators.Security;

namespace my_life_api.Controllers
{
    [ApiController]
    public class AuthorController : ControllerBase
    {

        private readonly ILogger<AuthorController> _logger;

        public AuthorController(ILogger<AuthorController> logger)
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

            AuthorCreateRequestDTO authorReq = new AuthorCreateRequestDTO
            {
                nome = nome,
                idTipoConteudo = idTipoConteudo,
                imagem = imagem
            };

            await service.CreateAuthor(authorReq);

            return Ok(ApiResponse.CreateBody(201, "Autor criado com sucesso!"));
        }

        [HttpPut("autor", Name = "autor")]
        [ServiceFilter(typeof(TokenValidationFilter))]
        [ServiceFilter(typeof(UpdateAuthorValidationFilter))]
        public async Task<IActionResult> Put(
            [FromForm] int id,
            [FromForm] string nome,
            [FromForm] IFormFile imagem
        )
        {
            AuthorService service = new AuthorService();

            AuthorUpdateRequestDTO authorReq = new AuthorUpdateRequestDTO
            {
                id = id,
                nome = nome,
                imagem = imagem
            };

            AuthorDTO dbAuthor = JsonConvert.DeserializeObject<AuthorDTO>(
                HttpContext.Request.Headers["requestedItem"]
            );

            await service.UpdateAuthor(authorReq, dbAuthor);

            return Ok(ApiResponse.CreateBody(201, "Autor atualizado com sucesso!"));
        }
    }
}
