using System.Reflection;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using my_life_api.Models;
using System.Dynamic;
using Microsoft.Extensions.Primitives;
using System.Collections.Immutable;

namespace my_life_api.Resources
{
    static public class MatchedJTokenTypesArray
    {
        public class MatchedJTokenType
        {
            public JTokenType jTokenType { get; set; }
            public Type systemType { get; set; }
            public MatchedJTokenType(JTokenType _jTokenType, Type _systemType)
            {
                jTokenType = _jTokenType;
                systemType = _systemType;
            }
        }

        private static readonly ImmutableArray<MatchedJTokenType> array = ImmutableArray.Create(
            new MatchedJTokenType(JTokenType.Null, null),
            new MatchedJTokenType(JTokenType.Boolean, typeof(bool)),
            new MatchedJTokenType(JTokenType.Integer, typeof(int)),
            new MatchedJTokenType(JTokenType.String, typeof(string)),
            new MatchedJTokenType(JTokenType.Array, typeof(Array))
        );

        public static ImmutableArray<MatchedJTokenType> Get()
        {
            return array;
        }
    }

    public enum FormDataFieldType
    {
        Bool = 1,
        String = 2,
        Int = 3,
        Float = 4,
        File = 5
    }

    static public class MatchedFormDataFieldTypesArray
    {
        public class MatchedFormDataFieldType
        {
            public FormDataFieldType fieldTypeEnum { get; set; }
            public Type type { get; set; }
            public MatchedFormDataFieldType(FormDataFieldType _fieldTypeEnum, Type _type)
            {
                fieldTypeEnum = _fieldTypeEnum;
                type = _type;
            }
        }

        private static readonly ImmutableArray<MatchedFormDataFieldType> array = ImmutableArray.Create(
            new MatchedFormDataFieldType(FormDataFieldType.String, typeof(string)),
            new MatchedFormDataFieldType(FormDataFieldType.Bool, typeof(bool)),
            new MatchedFormDataFieldType(FormDataFieldType.Int, typeof(int)),
            new MatchedFormDataFieldType(FormDataFieldType.Float, typeof(float)),
            new MatchedFormDataFieldType(FormDataFieldType.File, typeof(IFormFile))
        );

        public static ImmutableArray<MatchedFormDataFieldType> Get()
        {
            return array;
        }
    }

    public class ICustomActionFilter : IAsyncActionFilter
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

        public string GetParamValue(
            string paramName,
            ActionExecutingContext context
        ) {
            IQueryCollection reqParams = context.HttpContext.Request.Query;

            int paramsVerified = 0;
            var param = reqParams.FirstOrDefault((p) =>
            {
                if (paramsVerified > 5) {
                    throw new CustomException(400, "Os params enviados são inválidos.");
                }
                paramsVerified++;

                return p.Key == paramName;
            });

            return param.Value;
        }

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
                        Type receivedType = (MatchedJTokenTypesArray.Get()
                                .FirstOrDefault(mt =>
                                    mt.jTokenType == rawPropertyValue.Type
                                )
                                ?? MatchedJTokenTypesArray.Get()[0]
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

        /// <summary>
        ///     Monta um objeto dinâmico, usando como base o Type genérico informado,
        ///     também valida obrigatóriedade de campos que não sejam String ou FormData
        /// </summary>
        ///     <param name="context">
        ///         Contexto da requisição
        ///     </param>
        /// <returns></returns>
        public async Task<dynamic> GetFormDataContent<T>(ActionExecutingContext context)
        {
            try
            {
                IFormCollection formData = context.HttpContext.Request.Form;

                Type genericType = typeof(T);
                PropertyInfo[] properties = genericType.GetProperties();

                ExpandoObject exObj = new ExpandoObject();
                var registeredProperties = (ICollection<KeyValuePair<string, object>>)exObj;

                List<InvalidFieldError> invalidFields = new List<InvalidFieldError>();

                foreach (PropertyInfo property in properties)
                {
                    Type expectedType = property.PropertyType;

                    try {
                        // Nome da propriedade esperada
                        string propertyName = property.Name;

                        // Obtém o valor da propriedade esperada da requisição
                        StringValues reqPropertyValue = formData[propertyName];

                        FormDataFieldType? expectedTypeEnum = (MatchedFormDataFieldTypesArray.Get()
                            .FirstOrDefault(ft =>
                                ft.type == expectedType
                            )
                        )?.fieldTypeEnum;

                        // Função conversora do valor recebido para o esperado
                        // se for um valor opcional e vazio roda a função abaixo, retornando null
                        Func<object> reqValueConverter = () => null;

                        // Se for esperado arquivo
                        // ou valor do tipo String não aplica regra de obrigatoriedade
                        bool isFileExpected = expectedTypeEnum == FormDataFieldType.File;
                        bool isTextExpected = expectedTypeEnum == FormDataFieldType.String;

                        // Só tenta converter o valor caso ele tenha sido informado
                        if (!string.IsNullOrEmpty(reqPropertyValue) || isFileExpected || isTextExpected) {
                            switch (expectedTypeEnum) {
                                case FormDataFieldType.String:
                                    reqValueConverter = () => reqPropertyValue.ToString();
                                break;
                                case FormDataFieldType.Int:
                                    reqValueConverter = () => Int32.Parse(reqPropertyValue);
                                break;
                                case FormDataFieldType.Float:
                                    reqValueConverter = () => float.Parse(reqPropertyValue);
                                break;
                                case FormDataFieldType.Bool:
                                    reqValueConverter = () => bool.Parse(reqPropertyValue);
                                break;
                                case FormDataFieldType.File:
                                    Console.WriteLine(formData.Files[propertyName].FileName);
                                    reqValueConverter = () => formData.Files[propertyName];
                                break;
                            }
                        } else {
                            // Verifica se a propriedade esperada é opcional/nullable
                            // ou seja, foi typada com "?" Ex: int?
                            bool isPropertyRequired = !expectedType.IsGenericType || expectedType.GetGenericTypeDefinition() != typeof(Nullable<>);

                            if (isPropertyRequired) {
                                invalidFields.Add(new InvalidFieldError(
                                    property.Name,
                                    "Esse campo é obrigatório e não foi informado."
                                ));
                                continue;
                            }
                        }

                        object convertedValue = reqValueConverter();

                        var propertyToRegister = new KeyValuePair<string, object>(
                            propertyName,
                            convertedValue
                        );

                        registeredProperties.Add(propertyToRegister);

                    } catch (Exception exception) {
                        invalidFields.Add(new InvalidFieldError(
                            property.Name,
                            $"O tipo de dado recebido é diferente de {expectedType.Name}."
                        ));
                    }
                }

                if (invalidFields.Count() > 0) {
                    throw new CustomException(
                        400, 
                        "Um ou mais campos estão no formato incorreto ou não foram informados.", 
                        invalidFields
                    );
                }

                dynamic resultObj = exObj;

                return resultObj;

            }
            catch (CustomException exception) {
                throw exception;
            } catch (Exception exception) {
                throw new CustomException(
                    400,
                    "Formato do corpo da requisição incorreto, ele deve ser enviado como FormData."
                );
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
