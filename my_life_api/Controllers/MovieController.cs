using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using my_life_api.Resources;
using my_life_api.Services;
using my_life_api.ValidationFilters.Security;
using my_life_api.Models;
using my_life_api.ValidationFilters.Content;
using my_life_api.Shared.ContentResources;
using my_life_api.Models.Content;
using my_life_api.Models.Content.Entities;
using my_life_api.Shared;

namespace my_life_api.Controllers;

[ApiController]
public class MovieController : ControllerBase {

    private readonly ILogger<MovieController> _logger;

    public MovieController(ILogger<MovieController> logger) {
        _logger = logger;
    }

    [HttpGet("filmes", Name = "filmes")]
    [ServiceFilter(typeof(TokenValidationFilter))]
    [ServiceFilter(typeof(ContentFiltersParamValidationFilter))]
    public async Task<IActionResult> Get(
        [FromQuery] string pesquisa,
        [FromQuery] bool? fragmentoAlma,
        [FromQuery] int? idAutor,
        [FromQuery] IEnumerable<string> idsCategorias,
        [FromQuery] string notaMaiorIgualQue,
        [FromQuery] string notaMenorIgualQue,
        [FromQuery] bool? dublado
    ) {
        ContentFilters filters = new ContentFilters(
            pesquisa,
            fragmentoAlma,
            idAutor,
            idsCategorias,
            notaMaiorIgualQue,
            notaMenorIgualQue,
            null,
            dublado
        );
        ContentService service = new ContentService();

        IEnumerable<object> items = await service.GetItems(
            ContentTypesEnum.Cinema, 
            filters
        );

        return Ok(ApiResponse.CreateBody(
            200, 
            "Lista de filmes recebida com sucesso!",
            new { filmes = items }
        ));
    }

    [HttpPost("filme", Name = "filme")]
    [ServiceFilter(typeof(TokenValidationFilter))]
    [ServiceFilter(typeof(CreateUpdateContentValidationFilter))]
    public async Task<IActionResult> Post(
        [FromForm] string nome,
        [FromForm] IFormFile? imagem,
        [FromForm] float nota,
        [FromForm] bool? dublado,
        [FromForm] bool? fragmentoAlma,
        [FromForm] int idAutor,
        [FromForm] IEnumerable<int> idsCategorias
    ) {
        ContentService service = new ContentService();

        MovieEntity item = new MovieEntity() {
            name = nome,
            rating = nota,
            dubbed = Format.GetByteValueOfBool(dublado ?? false),
            soulFragment = Format.GetByteValueOfBool(fragmentoAlma ?? false),
            authorId = idAutor
        };

        await service.CreateItem(
            ContentTypesEnum.Cinema, 
            item,
            idsCategorias,
            imagem
        );

        return Ok(ApiResponse.CreateBody(201, "Filme registrado com sucesso!"));
    }

    [HttpPut("filme", Name = "filme")]
    [ServiceFilter(typeof(TokenValidationFilter))]
    [ServiceFilter(typeof(CreateUpdateContentValidationFilter))]
    public async Task<IActionResult> Put(
        [FromForm] int id,
        [FromForm] string? nome,
        [FromForm] IFormFile? imagem,
        [FromForm] float? nota,
        [FromForm] bool? dublado,
        [FromForm] bool? fragmentoAlma,
        // Espera sempre valor, caso nao enviado
        // considera que o conteudo nao deve ter categorias
        [FromForm] IEnumerable<int> idsCategorias
    ) {
        MovieDTO dbMovie = JsonConvert.DeserializeObject<MovieDTO>(
            HttpContext.Request.Headers["requestedItem"]
        );

        ContentService service = new ContentService();

        MovieEntity item = new MovieEntity() {
            name = nome ?? dbMovie.nome,
            rating = nota ?? dbMovie.nota,
            dubbed = Format.GetByteValueOfBool(dublado ?? dbMovie.dublado),
            soulFragment = Format.GetByteValueOfBool(fragmentoAlma ?? dbMovie.fragmentoAlma),
            authorId = (int)dbMovie.autor.id
        };

        await service.UpdateItem(
            ContentTypesEnum.Cinema,
            id,
            item,
            idsCategorias,
            imagem
        );

        return Ok(ApiResponse.CreateBody(200, "Filme atualizado com sucesso!"));
    }

    [HttpDelete("filme", Name = "filme")]
    [ServiceFilter(typeof(TokenValidationFilter))]
    [ServiceFilter(typeof(DeleteContentValidationFilter))]
    public async Task<IActionResult> Delete(
        [FromQuery] string idFilme
    ) {
        ContentService service = new ContentService();

        MovieDTO dbMovie = JsonConvert.DeserializeObject<MovieDTO>(
            HttpContext.Request.Headers["requestedItem"]
        );

        await service.DeleteItemByTypeAndId(        
            ContentTypesEnum.Cinema,
            Int32.Parse(idFilme),
            dbMovie.urlImagem
        );

        return Ok(ApiResponse.CreateBody(200, "O filme foi excluído com sucesso!"));
    }
}
