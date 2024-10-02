using Microsoft.AspNetCore.Mvc.Filters;
using my_life_api.Models;
using my_life_api.Models.Requests.Category;
using my_life_api.Resources;
using my_life_api.Shared;

namespace my_life_api.Validators.Author
{
    public class CreateCategoryValidationFilter : ICustomActionFilter
    {
        public override async Task OnActionExecutionAsync(
            ActionExecutingContext context,
            ActionExecutionDelegate next)
        {
            var authorObj = await GetFormDataContent<CategoryCreateRequestDTO>(context);

            CategoryCreateRequestDTO category = new CategoryCreateRequestDTO().BuildFromObj(authorObj);

            if (string.IsNullOrEmpty(category.nome) || category.nome.Trim().Length == 0) {
                throw new CustomException(400, "O nome da categoria é obrigatório e não pode ficar vazio.");
            }

            if (category.nome.Length > 50) {
                throw new CustomException(400, "O nome da categoria deve ter no máximo 50 caracteres.");
            }

            if (Validator.HasInvalidCharacters(category.nome)) {
                throw new CustomException(400, "O nome da categoria contém caracteres inválidos.");
            }

            if (Validator.IsContentTypeIdInvalid(category.idTipoConteudo)) {
                throw new CustomException(400, "O idTipoConteudo informado não é valido.");
            }

            // VERIFICAR SE ISSO FUNCIONA
            // TESTAR TD O RESTO TBM
            if (!string.IsNullOrEmpty(category.iconeBase64)) {
                if (category.iconeBase64.Length > 20000) {
                    throw new CustomException(400, "O iconeBase64 é grande demais.");
                }

                if (category.iconeBase64.IndexOf("data:image/") != 0) {
                    throw new CustomException(400, "O iconeBase64 informado não é valido.");
                }

                if (Validator.HasInvalidCharacters(category.iconeBase64)) {
                    throw new CustomException(400, "O iconeBase64 tem caracteres inválidos.");
                }
            }

            await next();
        }
    }
}
