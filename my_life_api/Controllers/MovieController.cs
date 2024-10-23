using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using my_life_api.Validators;
using my_life_api.Resources;
using my_life_api.Services;
using my_life_api.Models.Requests;
using my_life_api.Validators.Security;
using my_life_api.Models;
using my_life_api.Models.Requests.Author;
using my_life_api.Validators.Author;

namespace my_life_api.Controllers
{
    [ApiController]
    public class MovieController : CustomControllerBase
    {

        private readonly ILogger<MovieController> _logger;

        public MovieController(ILogger<MovieController> logger)
        {
            _logger = logger;
        }

        [HttpGet("filmes", Name = "filmes")]
        [ServiceFilter(typeof(TokenValidationFilter))]
        [ServiceFilter(typeof(ContentFiltersParamValidationFilter))]
        public async Task<IActionResult> Get(
            [FromQuery] string fragmentoAlma,
            [FromQuery] string idAutor,
            [FromQuery] IEnumerable<string> idsCategorias,
            [FromQuery] string notaMaiorIgualQue,
            [FromQuery] string notaMenorIgualQue,
            [FromQuery] string nome,
            [FromQuery] string dublado
        ) {
            ContentFilters filters = new ContentFilters()
            {
                soulFragment = fragmentoAlma != null ? (fragmentoAlma == "true") : null,
                authorId = idAutor != null ? Int32.Parse(idAutor) : null,
                categoriesIds = idsCategorias != null ? idsCategorias.Select(id => Int32.Parse(id)) : null,
                ratingGreaterEqualTo = notaMaiorIgualQue != null ? float.Parse(notaMaiorIgualQue) : null,
                ratingLesserEqualTo = notaMenorIgualQue != null ? float.Parse(notaMenorIgualQue) : null,
                name = nome,
                dubbed = dublado != null ? (dublado == "true") : null,
            };
            MovieService service = new MovieService();

            IEnumerable<MovieDTO> movies = await service.GetMovies(filters);

            return Ok(ApiResponse.CreateBody(
                200, 
                "Lista de filmes recebida com sucesso!",
                new { filmes = movies }
            ));
        }

        [HttpPost("filme", Name = "filme")]
        [ServiceFilter(typeof(TokenValidationFilter))]
        [ServiceFilter(typeof(CreateMovieValidationFilter))]
        public async Task<IActionResult> Post(
            [FromForm] string nome,
            [FromForm] IFormFile imagem,
            [FromForm] float nota,
            [FromForm] bool dublado,
            [FromForm] bool fragmentoAlma,
            [FromForm] int idAutor,
            [FromForm] IEnumerable<int> idsCategorias
        ) {
            MovieService service = new MovieService();

            MovieCreateRequestDTO movieReq = new MovieCreateRequestDTO
            {
                nome = nome,
                imagem = imagem,
                nota = nota,
                dublado = dublado,
                fragmentoAlma = fragmentoAlma,
                idAutor = idAutor,
                idsCategorias = idsCategorias,
            };

            await service.CreateMovie(movieReq);

            return Ok(ApiResponse.CreateBody(201, "Filme registrado com sucesso!"));
        }

        [HttpPut("filme", Name = "filme")]
        [ServiceFilter(typeof(TokenValidationFilter))]
        [ServiceFilter(typeof(UpdateMovieValidationFilter))]
        public async Task<IActionResult> Put(
            [FromForm] int id,
            [FromForm] string? nome,
            [FromForm] IFormFile? imagem,
            [FromForm] float? nota,
            [FromForm] bool? dublado,
            [FromForm] bool? fragmentoAlma,
            [FromForm] IEnumerable<int>? idsCategorias
        ) {
            idsCategorias = GetNullIfValueNotInformed(idsCategorias, "idsCategorias");

            MovieService service = new MovieService();

            MovieUpdateRequestDTO movieReq = new MovieUpdateRequestDTO
            {
                id = id,
                nome = nome,
                imagem = imagem,
                nota = nota,
                dublado = dublado,
                fragmentoAlma = fragmentoAlma,
                idsCategorias = idsCategorias,
            };

            MovieDTO dbAuthor = JsonConvert.DeserializeObject<MovieDTO>(
                HttpContext.Request.Headers["requestedItem"]
            );

            await service.UpdateMovie(movieReq, dbAuthor);

            return Ok(ApiResponse.CreateBody(200, "Filme atualizado com sucesso!"));
        }

        [HttpDelete("filme", Name = "filme")]
        [ServiceFilter(typeof(TokenValidationFilter))]
        [ServiceFilter(typeof(DeleteMovieValidationFilter))]
        public async Task<IActionResult> Delete(
            [FromQuery] string idFilme
        ) {
            ContentService service = new ContentService();

            MovieDTO dbMovie = JsonConvert.DeserializeObject<MovieDTO>(
                HttpContext.Request.Headers["requestedItem"]
            );

            await service.DeleteContentById(        
                ContentTypesEnum.Cinema,
                Int32.Parse(idFilme),
                dbMovie.urlImagem
            );

            return Ok(ApiResponse.CreateBody(200, "O filme foi excluído com sucesso!"));
        }
    }
}
