using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using my_life_api.Database.Managers;
using my_life_api.Models;
using my_life_api.Resources;
using my_life_api.Shared;
using my_life_api.Models.Requests.Category;

namespace my_life_api.ValidationFilters.Category;

public class UpdateCategoryValidationFilter : ICustomActionFilter {
    public override async Task OnActionExecutionAsync(
        ActionExecutingContext context,
        ActionExecutionDelegate next
    ) {
        var categoryObj = await GetFormDataContent<CategoryUpdateRequestDTO>(context);

        CategoryUpdateRequestDTO category = new CategoryUpdateRequestDTO().BuildFromObj(
            categoryObj
        );

        if (!string.IsNullOrEmpty(category.nome)) {
            if (category.nome.Trim().Length == 0) {
                throw new CustomException(400, "Não é permitido nome vazio.");
            }

            if (category.nome.Length > 50) {
                throw new CustomException(
                    400, 
                    "O nome da categoria deve ter no máximo 50 caracteres."
                );
            }

            if (Validator.HasInvalidCharacters(category.nome)) {
                throw new CustomException(
                    400, 
                    "O nome da categoria contém caracteres inválidos."
                );
            }
        }

        if (!string.IsNullOrEmpty(category.iconeBase64)) {
            if (category.iconeBase64.Length > 400000) {
                throw new CustomException(400, "O iconeBase64 é grande demais.");
            }

            if (category.iconeBase64.IndexOf("data:image/") != 0) {
                throw new CustomException(400, "O iconeBase64 informado não é valido.");
            }

            if (Validator.HasInvalidCharacters(category.iconeBase64)) {
                throw new CustomException(400, "O iconeBase64 tem caracteres inválidos.");
            }
        }

        if (
            string.IsNullOrEmpty(category.nome)
            && string.IsNullOrEmpty(category.iconeBase64)
        ) {
            throw new CustomException(400, "Informe ao menos um campo para atualizar.");
        }

        dynamic? categoryDbData = null;
        if (category.id > 0) {
            CategoryDBManager categoryDbManager = new CategoryDBManager();
            categoryDbData = await categoryDbManager.GetCategoryById((int)category.id);

            if (categoryDbData == null) {
                throw new CustomException(404, "Não existe nenhuma categoria com esse id.");
            }
        } else {
            throw new CustomException(
                400,
                "O id de categoria informado é inválido."
            );
        }

        context.HttpContext.Request.Headers.Add(
            "requestedItem", 
            JsonConvert.SerializeObject(categoryDbData)
        );

        await next();
    }
}
