using Microsoft.AspNetCore.Mvc.Filters;
using my_life_api.Services;

namespace my_life_api.ValidationFilters.Security;

public class TokenValidationFilter : IActionFilter {
    public void OnActionExecuting(ActionExecutingContext context) {
        var authenticationCookie = context.HttpContext.Request.Cookies
            .FirstOrDefault(cookie => 
                cookie.Key == "tokenAutenticacao"
            );

        // Antiga autenticacao feita utilizando somente localStorage
        //var token = context.HttpContext.Request.Headers["Authorization"]
        //    .FirstOrDefault()?.Split(" ").Last();

        AuthorizationService authorizationService = new AuthorizationService();
        authorizationService.ValidateToken(authenticationCookie.Value);

    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
    }
}
