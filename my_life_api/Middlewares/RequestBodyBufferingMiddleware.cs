using System.Collections.Immutable;

public class RequestBodyBufferingMiddleware
{
    private readonly RequestDelegate _next;
    public static readonly ImmutableList<string> methodsWithBody = ImmutableList.Create("POST", "PUT");

    public RequestBodyBufferingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (methodsWithBody.Contains(context.Request.Method))
        {
            // Habilita o bufferamento para ler o corpo da requisição múltiplas vezes
            context.Request.EnableBuffering();
        }

        await _next(context);
    }
}