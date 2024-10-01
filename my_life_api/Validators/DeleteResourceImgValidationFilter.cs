using Microsoft.AspNetCore.Mvc.Filters;
using my_life_api.Database.Managers;
using my_life_api.Models;
using my_life_api.Resources;
using Newtonsoft.Json;
using System.Collections.Immutable;

namespace my_life_api.Validators
{
    public class DeleteResourceImgValidationFilter : ICustomActionFilter
    {
        public static readonly ImmutableArray<int> validContentTypesIds = ImmutableArray.Create(
            (int)ContentTypesEnum.Mangas,
            (int)ContentTypesEnum.Animes,
            (int)ContentTypesEnum.Seriado,
            (int)ContentTypesEnum.Livros,
            (int)ContentTypesEnum.Jogos,
            (int)ContentTypesEnum.Cinema
        );

        public override async Task OnActionExecutionAsync(
            ActionExecutingContext context,
            ActionExecutionDelegate next
        ){
            string contentTypeId = GetParamValue("idTipoConteudo", context);
            string resourceId = GetParamValue("idRecurso", context);

            if (string.IsNullOrEmpty(contentTypeId)) {
                throw new CustomException(400, "O param 'idTipoConteudo' é obrigatório e não foi informado.");
            }

            if (string.IsNullOrEmpty(resourceId)) {
                throw new CustomException(400, "O param 'idRecurso' é obrigatório e não foi informado.");
            }

            int convertedContentTypeId = 0;
            int convertedResourceId = 0;

            try {
                convertedContentTypeId = Int32.Parse(contentTypeId);
                convertedResourceId = Int32.Parse(resourceId);
            } catch (Exception exception) { }

            if (!validContentTypesIds.Contains(convertedContentTypeId)) {
                throw new CustomException(
                    400, 
                    "O param 'idTipoConteudo' informado é inválido, informe um tipo de conteúdo que aceite imagens."
                );
            }

            dynamic? content = null;

            switch ((ContentTypesEnum)convertedContentTypeId)
            {
                case ContentTypesEnum.Animes:

                break;
                case ContentTypesEnum.Mangas:

                break;
                case ContentTypesEnum.Seriado:

                break;
                case ContentTypesEnum.Livros:

                break;
                case ContentTypesEnum.Jogos:

                break;
                case ContentTypesEnum.Cinema:
                    MovieDBManager movieDbManager = new MovieDBManager();
                    content = await movieDbManager.GetMovieById(convertedResourceId);
                break;
            }

            if (content == null) {
                throw new CustomException(404, "Nenhum recurso com esse id foi encontrado.");
            }

            if (String.IsNullOrEmpty(content.urlImagem)) {
                throw new CustomException(400, "Esse recurso não tem imagens registradas.");
            }

            context.HttpContext.Request.Headers.Add("requestedItem", JsonConvert.SerializeObject(content));

            await next();
        }
    }
}
