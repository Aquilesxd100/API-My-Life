using Microsoft.AspNetCore.Mvc.Filters;
using my_life_api.Database.Managers;
using my_life_api.Models;
using my_life_api.Models.Requests.Author;
using my_life_api.Resources;
using my_life_api.Services;
using my_life_api.Shared;

namespace my_life_api.Validators.Author
{
    public class CreateMovieValidationFilter : ICustomActionFilter
    {
        public override async Task OnActionExecutionAsync(
            ActionExecutingContext context,
            ActionExecutionDelegate next)
        {
            var movieObj = await GetFormDataContent<MovieCreateRequestDTO>(context);

            MovieCreateRequestDTO movie = new MovieCreateRequestDTO().BuildFromObj(movieObj);

            if (string.IsNullOrEmpty(movie.nome) || movie.nome.Trim().Length == 0) {
                throw new CustomException(400, "O nome do filme é obrigatório e não pode ficar vazio.");
            }

            if (movie.nome.Length > 50) {
                throw new CustomException(400, "O nome do filme deve ter no máximo 50 caracteres.");
            }

            if (Validator.HasInvalidCharacters(movie.nome)) {
                throw new CustomException(400, "O nome do filme contém caracteres inválidos.");
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

            if (Validator.IsRatingInvalid(movie.nota)) {
                throw new CustomException(400, "A nota informada é inválida, o valor deve ser entre 0 e 10.");
            }

            dynamic? authorDbData = null;
            if (movie.idAutor > 0) {
                AuthorDBManager authorDbManager = new AuthorDBManager();
                authorDbData = await authorDbManager.GetAuthorById(movie.idAutor);

                if (authorDbData == null) {
                    throw new CustomException(404, "Não existe nenhum autor com esse id.");
                }
            } else {
                throw new CustomException(
                    400,
                    "O id de autor informado é inválido."
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

            await next();
        }
    }
}
