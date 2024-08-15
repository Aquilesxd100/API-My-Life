using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using my_life_api.Models;
using my_life_api.Services;

namespace my_life_api.Filters
{
    public class TokenValidationFilter : IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext context)
        {
            var token = context.HttpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

            AuthorizationService authorizationService = new AuthorizationService();

            try
            {
                authorizationService.ValidateToken(token);
            }
            catch (CustomException ex)
            {
                context.Result = new CustomResult(ex.StatusCode, ex.Message);
            }
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
        }
    }
}
