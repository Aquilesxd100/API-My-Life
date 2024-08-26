using my_life_api.Models;
using my_life_api.Resources;
using System.Net;

namespace my_life_api.Middlewares
{
    public class GlobalExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;

        public GlobalExceptionHandlerMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            Console.WriteLine("entrou no middleware");
            try
            {
                await _next(context);
            }
            catch (CustomException exception)
            {
                Console.WriteLine("middleware pegou a excecao");
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = exception.StatusCode;

                await context.Response.WriteAsJsonAsync(
                    ApiResponse.CreateBody(
                        exception.StatusCode,
                        exception.Message,
                        exception.Content
                    )
                );
            }
            catch (Exception exception)
            {
                Console.WriteLine("middleware pegou a excecao2");
                Console.WriteLine(exception);

                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (int) HttpStatusCode.InternalServerError;

                await context.Response.WriteAsJsonAsync(
                    ApiResponse.CreateBody(500, "Ocorreu um erro interno no servidor.")
                );
            }
        }
    }
}