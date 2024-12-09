using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using my_life_api.Database.Managers;
using my_life_api.Models;
using my_life_api.Resources;
using my_life_api.Services;
using my_life_api.Shared;
using my_life_api.Models.Requests.Movie;

namespace my_life_api.ValidatorsFilters.Movie;

public class UpdateMovieValidationFilter : ICustomActionFilter {
    public override async Task OnActionExecutionAsync(
        ActionExecutingContext context,
        ActionExecutionDelegate next
    ) {
        var movieObj = await GetFormDataContent<MovieUpdateRequestDTO>(context);

        MovieUpdateRequestDTO movie = new MovieUpdateRequestDTO().BuildFromObj(movieObj);
        ContentValidator validator = new ContentValidator();

        validator.ValidateName(movie.nome);
        validator.ValidateOptionalImgFile(movie.imagem);
        validator.ValidateRating(movie.nota);

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
            IEnumerable<int> validCategoriesIds = (
                await categoryService.GetCategoriesByContentTypeId(ContentTypesEnum.Cinema)
            ).Select(c => c.id);

            foreach (int idCategory in movie.idsCategorias) {
                if (!validCategoriesIds.Contains(idCategory)) {
                    throw new CustomException(
                        404,
                        "Um ou mais idsCategorias não existem."
                    );
                }
            }
        }

        context.HttpContext.Request.Headers.Add(
            "requestedItem", 
            JsonConvert.SerializeObject(movieDbData)
        );

        await next();
    }
}
