using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using my_life_api.Models;

namespace my_life_api.Validators
{
    public class LoginValidationFilter : IActionFilter
    {
        public async void OnActionExecuting(ActionExecutingContext context)
        {
            context.HttpContext.Request.Body.Position = 0;

            using var reader = new StreamReader(context.HttpContext.Request.Body, encoding: System.Text.Encoding.UTF8, detectEncodingFromByteOrderMarks: false, bufferSize: 1024, leaveOpen: true);
            var body = await reader.ReadToEndAsync();

            // Reinicia a posicao de leitura do body para ser lido corretamente pelo controller
            context.HttpContext.Request.Body.Position = 0;

            var request = JsonConvert.DeserializeObject<LoginRequest>(body);
            if (request == null || string.IsNullOrEmpty(request.senha))
            {
                context.Result = new CustomResult(400, "Informe sua senha pelo corpo da requisição para efetuar login.");
            }
        }
        public void OnActionExecuted(ActionExecutedContext context)
        {
        }
    }
}
