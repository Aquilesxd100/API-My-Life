using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using my_life_api.Database.Managers;
using my_life_api.Models;
using my_life_api.Models.Requests.Author;
using my_life_api.Resources;
using my_life_api.Services;
using my_life_api.Shared;

namespace my_life_api.Validators.Author
{
    public class UpdateMovieValidationFilter : ICustomActionFilter
    {
        public override async Task OnActionExecutionAsync(
            ActionExecutingContext context,
            ActionExecutionDelegate next)
        {
            var movieObj = await GetFormDataContent<MovieUpdateRequestDTO>(context);

            MovieUpdateRequestDTO movie = new MovieUpdateRequestDTO().BuildFromObj(movieObj);

            if (!string.IsNullOrEmpty(movie.nome)) {
                if (movie.nome.Trim().Length == 0) {
                    throw new CustomException(400, "O nome do filme não pode ser vazio.");
                }

                if (movie.nome.Length > 50) {
                    throw new CustomException(400, "O nome do filme deve ter no máximo 50 caracteres.");
                }

                if (Validator.HasInvalidCharacters(movie.nome)) {
                    throw new CustomException(400, "O nome do filme contém caracteres inválidos.");
                }
            }

            if (movie.imagem != null) {
                if (Validator.IsInvalidImageFormat(movie.imagem)) {
                    throw new CustomException(
                        400,
                        "A imagem enviada está em formato incorreto, só são permitidas imagens png, jpg e jpeg."
                    );
                }

                if (Validator.IsInvalidImageSize(movie.imagem)) {
                    throw new CustomException(400, "A imagem enviada excede o tamanho máximo de 12mb.");
                }
            }

            if (movie.nota != null) {
                if (Validator.IsRatingInvalid((float)movie.nota)) {
                    throw new CustomException(400, "A nota informada é inválida, o valor deve ser entre 0 e 10.");
                }
            }

            dynamic? movieDbData = null;
            if (movie.id > 0) {
                MovieDBManager movieDbManager = new MovieDBManager();
                movieDbData = await movieDbManager.GetMovieById((int)movie.id);

                if (movieDbData == null) {
                    throw new CustomException(404, "Não existe nenhum filme com esse id.");
                }
            } else {
                throw new CustomException(
                    400,
                    "O id do filme informado é inválido."
                );
            }

            if (movie.idsCategorias.Count() > 0) {
                CategoryService categoryService = new CategoryService();
                IEnumerable<int> validCategoriesIds = (await categoryService.GetCategoriesByContentTypeId(
                    ContentTypesEnum.Cinema)
                ).Select(c => c.id);

                foreach(int idCategory in movie.idsCategorias) {
                    if (!validCategoriesIds.Contains(idCategory)) {
                        throw new CustomException(
                            404,
                            "Um ou mais idsCategorias não existem."
                        );
                    }
                }
            }

            context.HttpContext.Request.Headers.Add("requestedItem", JsonConvert.SerializeObject(movieDbData));

            await next();
        }
    }
}
