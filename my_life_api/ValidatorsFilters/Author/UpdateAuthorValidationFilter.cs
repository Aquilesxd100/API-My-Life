using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using my_life_api.Database.Managers;
using my_life_api.Models;
using my_life_api.Resources;
using my_life_api.Shared;
using my_life_api.Models.Requests.Author;

namespace my_life_api.ValidatorsFilters.Author
{
    public class UpdateAuthorValidationFilter : ICustomActionFilter
    {
        public override async Task OnActionExecutionAsync(
            ActionExecutingContext context,
            ActionExecutionDelegate next)
        {
            var authorObj = await GetFormDataContent<AuthorUpdateRequestDTO>(context);

            AuthorUpdateRequestDTO author = new AuthorUpdateRequestDTO().BuildFromObj(authorObj);

            if (!string.IsNullOrEmpty(author.nome)) {
                if (author.nome.Trim().Length == 0) {
                    throw new CustomException(400, "Não é permitido nome vazio.");
                }

                if (author.nome.Length > 50) {
                    throw new CustomException(400, "O nome do autor deve ter no máximo 50 caracteres.");
                }

                if (Validator.HasInvalidCharacters(author.nome)) {
                    throw new CustomException(400, "O nome do autor contém caracteres inválidos.");
                }
            }

            if (author.imagem != null) {
                if (Validator.IsInvalidImageFormat(author.imagem)) {
                    throw new CustomException(
                        400,
                        "A imagem enviada está em formato incorreto, só são permitidas imagens png, jpg e jpeg."
                    );
                }

                if (Validator.IsInvalidImageSize(author.imagem)) {
                    throw new CustomException(400, "A imagem enviada excede o tamanho máximo de 12mb.");
                }
            }

            if (
                string.IsNullOrEmpty(author.nome)
                && author.imagem == null
            ) {
                throw new CustomException(400, "Informe ao menos um campo para atualizar.");
            }

            dynamic? authorDbData = null;
            if (author.id > 0) {
                AuthorDBManager authorDbManager = new AuthorDBManager();
                authorDbData = await authorDbManager.GetAuthorById((int)author.id);

                if (authorDbData == null) {
                    throw new CustomException(404, "Não existe nenhum autor com esse id.");
                }
            } else {
                throw new CustomException(
                    400,
                    "O id de autor informado é inválido."
                );
            }

            context.HttpContext.Request.Headers.Add("requestedItem", JsonConvert.SerializeObject(authorDbData));

            await next();
        }
    }
}
