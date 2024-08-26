using System.Reflection;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using my_life_api.Models;

namespace my_life_api.Resources
{
    public class InvalidFieldError
    {
        public string campo { get; set; }
        public string detalhes { get; set; }
        public InvalidFieldError(string _campo, string _detalhes)
        {
            campo = _campo;
            detalhes = _detalhes;
        }
    }

    public class ICustomActionFilter : IAsyncActionFilter
    {
        public async Task<T> GetBodyContent<T>(ActionExecutingContext context) {
            context.HttpContext.Request.Body.Position = 0;

            using var reader = new StreamReader(context.HttpContext.Request.Body, encoding: System.Text.Encoding.UTF8, detectEncodingFromByteOrderMarks: false, bufferSize: 1024, leaveOpen: true);
            var bodyJson = await reader.ReadToEndAsync();

            try
            {
                // Caso exista uma ou mais propriedades não nulas com tipo diferente do esperado
                // as formata para mostrar em retorno de erro
                if (context.HttpContext.Request.Headers["Has-Invalid-Field"] == "true")
                {
                    // Body deserializado com type padrao da biblioteca, mantendo type original das propriedades
                    JObject rawBody = JsonConvert.DeserializeObject<JObject>(bodyJson);

                    Type genericType = typeof(T);
                    PropertyInfo[] properties = genericType.GetProperties();

                    List<InvalidFieldError> invalidFields = new List<InvalidFieldError>();
                    foreach (PropertyInfo property in properties)
                    {
                        // Obtém o valor do JSON para a propriedade atual
                        JToken rawPropertyValue = rawBody.GetValue(property.Name);
                        if (rawPropertyValue == null) continue;

                        Type expectedType = property.PropertyType;
                        Type receivedType = (MatchedJTokenTypesList.Get()
                                .FirstOrDefault(mt =>
                                    mt.jTokenType == rawPropertyValue.Type
                                )
                                ?? MatchedJTokenTypesList.Get()[0]
                            ).systemType;

                        if (expectedType != receivedType)
                        {
                            invalidFields.Add(new InvalidFieldError(
                                property.Name,
                                $"O tipo de dado recebido é diferente de {expectedType.Name}."
                            ));
                        }
                    }

                    if (invalidFields.Count() > 0)
                    {
                        throw new CustomException(400, "Um ou mais campos estão no formato incorreto.", invalidFields);
                    }
                }

                // Reinicia a posicao de leitura do body a fim de ser lido corretamente pelo controller
                context.HttpContext.Request.Body.Position = 0;

                var body = JsonConvert.DeserializeObject<T>(bodyJson);
                return body;
            }
            catch (CustomException exception) { 
                throw exception; 
            } catch (Exception exception) {
                throw new CustomException(400, "Formato do corpo da requisição incorreto, verifique e tente novamente.");
            }
        }

        public virtual async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate del)
        {
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
        }
    }
}
