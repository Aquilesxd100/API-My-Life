using Microsoft.AspNetCore.Mvc.Filters;
using my_life_api.Models;
using my_life_api.Models.Requests;
using my_life_api.Resources;
using my_life_api.Shared;

namespace my_life_api.Validators.Author
{
    public class CreateAuthorValidationFilter : ICustomActionFilter
    {
        public override async Task OnActionExecutionAsync(
            ActionExecutingContext context,
            ActionExecutionDelegate next)
        {
            var authorObj = await GetFormDataContent<AuthorCreateRequestDTO>(context);

            AuthorCreateRequestDTO author = new AuthorCreateRequestDTO().BuildFromObj(authorObj);

            if (string.IsNullOrEmpty(author.nome) || author.nome.Trim().Length == 0) {
                throw new CustomException(400, "O nome do autor é obrigatório e não pode ficar vazio.");
            }

            if (author.nome.Length > 50) {
                throw new CustomException(400, "O nome do autor deve ter no máximo 50 caracteres.");
            }

            if (Validator.HasInvalidCharacters(author.nome)) {
                throw new CustomException(400, "O nome do autor contém caracteres inválidos.");
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

            if (Validator.IsContentTypeIdInvalid(author.idTipoConteudo)) {
                throw new CustomException(400, "O idTipoConteudo informado não é valido.");
            }

            await next();
        }
    }
}
