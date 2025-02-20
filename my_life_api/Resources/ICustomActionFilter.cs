﻿using System.Dynamic;
using System.Reflection;
using System.Collections.Immutable;
using System.Collections;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using my_life_api.Models;
using my_life_api.Shared;

namespace my_life_api.Resources;

static public class MatchedJTokenTypesArray {
    public class MatchedJTokenType {
        public JTokenType jTokenType { get; set; }
        public Type systemType { get; set; }
        public MatchedJTokenType(JTokenType _jTokenType, Type _systemType) {
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

    public static ImmutableArray<MatchedJTokenType> Get() {
        return array;
    }
}

public class ICustomActionFilter : IAsyncActionFilter {
    /// <summary>
    ///     Tenta buscar o valor de um Param da requisição
    ///     retornando o valor do primeiro que encontrar,
    ///     cria uma exceção caso mais de trinta e cinco parâmetros estejam presentes
    /// </summary>
    ///     <param name="paramName">
    ///         Nome do parâmetro buscado
    ///     </param>
    ///     <param name="context">
    ///         Contexto da requisição
    ///     </param>
    /// <returns></returns>
    public string GetParamValue(
        string paramName,
        ActionExecutingContext context
    ) {
        IQueryCollection reqParams = context.HttpContext.Request.Query;

        int paramsVerified = 0;
        var param = reqParams.FirstOrDefault((p) => {
            if (paramsVerified > 35) {
                throw new CustomException(400, "Os params enviados são inválidos.");
            }
            paramsVerified++;

            return p.Key == paramName;
        });

        return param.Value;
    }

    /// <summary>
    ///     Monta uma instância do Type genérico informado
    ///     fazendo uso dos campos enviados pelo body da requisição,
    ///     também valida cada campo e acusa erro caso algum dos dados recebidos
    ///     seja diferente do esperado
    /// </summary>
    ///     <param name="context">
    ///         Contexto da requisição
    ///     </param>
    /// <returns></returns>
    public async Task<T> GetBodyContent<T>(ActionExecutingContext context) {
        context.HttpContext.Request.Body.Position = 0;

        using var reader = new StreamReader(
            context.HttpContext.Request.Body, 
            encoding: System.Text.Encoding.UTF8, 
            detectEncodingFromByteOrderMarks: false, 
            bufferSize: 1024, 
            leaveOpen: true
        );
        var bodyJson = await reader.ReadToEndAsync();

        try {
            // Caso exista uma ou mais propriedades não nulas com tipo diferente do esperado
            // as formata para mostrar em retorno de erro
            if (context.HttpContext.Request.Headers["Has-Invalid-Field"] == "true") {
                // Body deserializado com type padrao da biblioteca, mantendo type original das propriedades
                JObject rawBody = JsonConvert.DeserializeObject<JObject>(bodyJson);

                Type genericType = typeof(T);
                PropertyInfo[] properties = genericType.GetProperties();

                List<InvalidFieldError> invalidFields = new List<InvalidFieldError>();
                foreach (PropertyInfo property in properties) {
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

                    if (expectedType != receivedType) {
                        invalidFields.Add(new InvalidFieldError(
                            property.Name,
                            $"O tipo de dado recebido é diferente de {expectedType.Name}."
                        ));
                    }
                }

                if (invalidFields.Count() > 0) {
                    throw new CustomException(
                        400, 
                        "Um ou mais campos estão no formato incorreto.", 
                        invalidFields
                    );
                }
            }

            // Reinicia a posicao de leitura do body a fim de ser lido corretamente pelo controller
            context.HttpContext.Request.Body.Position = 0;

            var body = JsonConvert.DeserializeObject<T>(bodyJson);
            return body;
        } catch (CustomException exception) { 
            throw exception; 
        } catch (Exception exception) {
            throw new CustomException(
                400, 
                "Formato do corpo da requisição incorreto, verifique e tente novamente."
            );
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
    public async Task<dynamic> GetFormDataContent<T>(ActionExecutingContext context) {
        try {
            IFormCollection formData = context.HttpContext.Request.Form;

            Type genericType = typeof(T);
            PropertyInfo[] properties = genericType.GetProperties();

            ExpandoObject exObj = new ExpandoObject();
            var registeredProperties = (ICollection<KeyValuePair<string, object>>)exObj;

            List<InvalidFieldError> invalidFields = new List<InvalidFieldError>();

            foreach (PropertyInfo property in properties) {
                Type expectedType = property.PropertyType;

                // Nome da propriedade esperada
                string propertyName = property.Name;

                // Obtém o valor da propriedade esperada da requisição
                StringValues reqPropertyValue = formData[propertyName];

                DataTypesEnum? expectedTypeEnum = 
                    DataType.GetMatchedTypeByType(expectedType)?.fieldTypeEnum;

                // Função conversora do valor recebido para o esperado
                // se for um valor opcional e vazio roda a função abaixo, retornando null
                Func<object> reqValueConverter = () => null;

                // Se for esperado arquivo, valor tipo string ou Enumerable
                // não aplica regra de obrigatóriedade
                bool isFileExpected = expectedTypeEnum == DataTypesEnum.File;
                bool isTextExpected = expectedTypeEnum == DataTypesEnum.String;
                bool isEnumerableExpected = typeof(IEnumerable).IsAssignableFrom(expectedType) 
                    && !isTextExpected;

                // Só tenta converter o valor caso ele tenha sido informado
                if (
                    !string.IsNullOrEmpty(reqPropertyValue) 
                    || isFileExpected 
                    || isTextExpected
                    || isEnumerableExpected
                ) {
                    switch (expectedTypeEnum) {
                        case DataTypesEnum.String:
                            reqValueConverter = () => reqPropertyValue.ToString();
                        break;
                        case DataTypesEnum.Int:
                            reqValueConverter = () => Int32.Parse(reqPropertyValue);
                        break;
                        case DataTypesEnum.Float:
                            reqValueConverter = () => float.Parse(reqPropertyValue);
                        break;
                        case DataTypesEnum.Bool:
                            reqValueConverter = () => bool.Parse(reqPropertyValue);
                        break;
                        case DataTypesEnum.IntEnumerable:
                            reqValueConverter = () => {
                                // Se informado o campo, mas vazio, retorna vazio
                                if (reqPropertyValue == "") return Array.Empty<int>();

                                string[] rawArray =  reqPropertyValue.ToArray();

                                // Valida por aqui pois o método de conversão direto não cai 
                                // no catch local (??)
                                rawArray.FirstOrDefault(i => {
                                    bool result = !int.TryParse(i, out _);
                                    if (result) throw new Exception();

                                    return result;
                                });

                                IEnumerable<int> convertedValues = rawArray
                                    .Select(i => Int32.Parse(i));                                 

                                return convertedValues;
                            };
                        break;
                        case DataTypesEnum.StringEnumerable:
                            reqValueConverter = () => {
                                // Se informado o campo, mas vazio, retorna vazio
                                if (reqPropertyValue == "") return Array.Empty<string>();

                                return reqPropertyValue.ToArray(); 
                            };
                        break;
                        case DataTypesEnum.File:
                            reqValueConverter = () => formData.Files[propertyName];
                        break;
                        case DataTypesEnum.FileEnumerable:
                            reqValueConverter = () => {
                                // Se informado o campo, mas vazio, retorna vazio
                                if (formData.Files[propertyName] == null) return Array.Empty<IFormFile>();

                                // Retorna todos os arquivos do respectivo campo
                                return formData.Files.GetFiles(propertyName); 
                            };
                        break;
                    }
                } else {
                    // Verifica se a propriedade esperada é opcional/nullable
                    // ou seja, foi typada com "?" Ex: int?
                    bool isPropertyRequired = 
                        !expectedType.IsGenericType 
                        || expectedType.GetGenericTypeDefinition() != typeof(Nullable<>);

                    if (isPropertyRequired) {
                        invalidFields.Add(new InvalidFieldError(
                            property.Name,
                            "Esse campo é obrigatório e não foi informado."
                        ));
                        continue;
                    }
                }

                try {
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

        } catch (CustomException exception) {
            throw exception;
        } catch (Exception exception) {
            throw new CustomException(
                400,
                "Formato do corpo da requisição incorreto, ele deve ser enviado como FormData."
            );
        }
    }

    public virtual async Task OnActionExecutionAsync(
        ActionExecutingContext context, 
        ActionExecutionDelegate del
    ) {
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
    }
}
