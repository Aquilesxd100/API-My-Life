using Microsoft.AspNetCore.Mvc.Filters;
using my_life_api.Models;
using my_life_api.Models.Requests.Movie;
using my_life_api.Resources;
using my_life_api.Services;
using my_life_api.Shared;

namespace my_life_api.ValidatorsFilters.Movie
{
    public class CreateMovieValidationFilter : ICustomActionFilter
    {
        public override async Task OnActionExecutionAsync(
            ActionExecutingContext context,
            ActionExecutionDelegate next)
        {
            var movieObj = await GetFormDataContent<MovieCreateRequestDTO>(context);

            MovieCreateRequestDTO movie = new MovieCreateRequestDTO().BuildFromObj(movieObj);
            ContentValidator validator = new ContentValidator();

            validator.ValidateName(movie.nome, true);
            validator.ValidateOptionalImgFile(movie.imagem);
            validator.ValidateRating(movie.nota);
            await validator.ValidateContentAuthor(movie.idAutor, ContentTypesEnum.Cinema);

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
