using Microsoft.AspNetCore.Mvc.Filters;
using my_life_api.Models;
using my_life_api.Resources;
using my_life_api.Shared;

namespace my_life_api.ValidatorsFilters
{
    public class ContentTypeParamValidationFilter : ICustomActionFilter
    {
        public override async Task OnActionExecutionAsync(
            ActionExecutingContext context,
            ActionExecutionDelegate next)
        {
            string contentTypeId = GetParamValue("idTipoConteudo", context);

            if (string.IsNullOrEmpty(contentTypeId)) {
                throw new CustomException(400, "O param 'idTipoConteudo' é obrigatório e não foi informado.");
            }

            int convertedContentTypeId = 0;

            try {
                convertedContentTypeId = Int32.Parse(contentTypeId);
            } catch (Exception exception) { }

            if (Validator.IsContentTypeIdInvalid(convertedContentTypeId)) {
                throw new CustomException(400, "O param 'idTipoConteudo' informado é inválido.");
            }

            await next();
        }
    }
}
