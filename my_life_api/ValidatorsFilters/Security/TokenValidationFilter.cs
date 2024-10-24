using Microsoft.AspNetCore.Mvc.Filters;
using my_life_api.Services;

namespace my_life_api.Validators.Security
{
    public class TokenValidationFilter : IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext context)
        {
            var token = context.HttpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

            AuthorizationService authorizationService = new AuthorizationService();
            authorizationService.ValidateToken(token);

        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
        }
    }
}
