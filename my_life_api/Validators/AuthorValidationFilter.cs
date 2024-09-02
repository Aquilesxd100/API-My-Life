using Microsoft.AspNetCore.Mvc.Filters;
using my_life_api.Models;
using my_life_api.Resources;
using System.Collections.Immutable;

namespace my_life_api.Validators
{
    public class AuthorValidationFilter : ICustomActionFilter
    {
        public override async Task OnActionExecutionAsync(
            ActionExecutingContext context, 
            ActionExecutionDelegate next)
        {
            var body = await GetBodyContent<AuthorDTO>(context);

            if (string.IsNullOrEmpty(body.nome))
            {
                throw new CustomException(400, "Informe o nome do autor pelo corpo da requisição.");
            }

            if (body.idTipoConteudo == null)
            {
                throw new CustomException(400, "Informe o idTipoConteudo do autor pelo corpo da requisição.");
            }

            ImmutableArray<ContentTypesEnum> validContentTypesId = ImmutableArray.Create(
                ContentTypesEnum.Cinema,
                ContentTypesEnum.Mangas,
                ContentTypesEnum.Seriado,
                ContentTypesEnum.Jogos,
                ContentTypesEnum.Livros,
                ContentTypesEnum.Frases,
                ContentTypesEnum.Musical
            );

            if (!validContentTypesId.Any(ct => (int)ct == body.idTipoConteudo))
            {
                throw new CustomException(400, "O idTipoConteudo informado não é valido.");
            }

            await next();
        }
    }
}
