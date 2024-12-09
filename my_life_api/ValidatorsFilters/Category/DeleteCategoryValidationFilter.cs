using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using my_life_api.Database.Managers;
using my_life_api.Models;
using my_life_api.Resources;

namespace my_life_api.ValidatorsFilters.Category
{
    public class DeleteCategoryValidationFilter : ICustomActionFilter
    {
        public override async Task OnActionExecutionAsync(
            ActionExecutingContext context,
            ActionExecutionDelegate next
        ) {
            string categoryId = GetParamValue("idCategoria", context);

            if (string.IsNullOrEmpty(categoryId)) {
                throw new CustomException(400, "O param 'idCategoria' é obrigatório e não foi informado.");
            }

            int convertedCategoryId = 0;

            try {
                convertedCategoryId = Int32.Parse(categoryId);
            } catch (Exception exception) {
                throw new CustomException(400, "O id informado é inválido.");
            }

            if (convertedCategoryId < 0) {
                throw new CustomException(400, "O id informado é inválido.");
            }

            CategoryDTO? category = null;

            CategoryDBManager categoryDbManager = new CategoryDBManager();
            category = await categoryDbManager.GetCategoryById(convertedCategoryId);

            if (category == null) {
                throw new CustomException(404, "Nenhuma categoria com esse id foi encontrada.");
            }

            context.HttpContext.Request.Headers.Add("requestedItem", JsonConvert.SerializeObject(category));

            await next();
        }
    }
}
