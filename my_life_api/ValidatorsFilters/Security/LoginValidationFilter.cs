using Microsoft.AspNetCore.Mvc.Filters;
using my_life_api.Models;
using my_life_api.Models.Requests;
using my_life_api.Resources;

namespace my_life_api.ValidatorsFilters.Security
{
    public class LoginValidationFilter : ICustomActionFilter
    {
        public override async Task OnActionExecutionAsync(
            ActionExecutingContext context,
            ActionExecutionDelegate next)
        {
            var body = await GetBodyContent<LoginRequest>(context);

            if (string.IsNullOrEmpty(body.senha))
            {

                throw new CustomException(400, "Informe sua senha pelo corpo da requisição para efetuar login.");
            }

            await next();
        }
    }
}
