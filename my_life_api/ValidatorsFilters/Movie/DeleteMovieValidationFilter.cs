using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using my_life_api.Database.Managers;
using my_life_api.Models;
using my_life_api.Resources;

namespace my_life_api.ValidatorsFilters.Movie
{
    public class DeleteMovieValidationFilter : ICustomActionFilter
    {
        public override async Task OnActionExecutionAsync(
            ActionExecutingContext context,
            ActionExecutionDelegate next
        ) {
            string authorId = GetParamValue("idFilme", context);

            if (string.IsNullOrEmpty(authorId)) {
                throw new CustomException(400, "O param 'idFilme' é obrigatório e não foi informado.");
            }

            int convertedMovieId = 0;

            try {
                convertedMovieId = Int32.Parse(authorId);
            } catch (Exception exception) {
                throw new CustomException(400, "O id informado é inválido.");
            }

            if (convertedMovieId < 0) {
                throw new CustomException(400, "O id informado é inválido.");
            }

            MovieDTO? movie = null;

            MovieDBManager movieDbManager = new MovieDBManager();
            movie = await movieDbManager.GetMovieById(convertedMovieId);

            if (movie == null) {
                throw new CustomException(404, "Nenhum filme com esse id foi encontrado.");
            }

            context.HttpContext.Request.Headers.Add("requestedItem", JsonConvert.SerializeObject(movie));

            await next();
        }
    }
}
