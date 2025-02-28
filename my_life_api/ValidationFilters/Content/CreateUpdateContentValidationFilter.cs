using System.Reflection;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using my_life_api.Models;
using my_life_api.Resources;
using my_life_api.Services;
using my_life_api.Shared.ContentResources;
using my_life_api.Shared;
using my_life_api.Database.Managers;
using my_life_api.Resources.ContentAttributes;
using static my_life_api.Shared.DataType;

namespace my_life_api.ValidationFilters.Content;

public class CreateUpdateContentValidationFilter : ICustomActionFilter {
    public override async Task OnActionExecutionAsync(
        ActionExecutingContext context,
        ActionExecutionDelegate next
    ) {
        bool isCreation = context.HttpContext.Request.Method == "POST";
        bool hasFieldsToUpdate = false;

        ContentTypeData contentTypeData = ContentUtils.GetContentTypeDataByPath(
            context.HttpContext.Request.Path
        );

        PropertyInfo[] dtoProperties = contentTypeData.dtoType.GetProperties();
        IFormCollection formData = context.HttpContext.Request.Form;

        List<InvalidFieldError> invalidFields = new List<InvalidFieldError>();
        foreach (PropertyInfo property in dtoProperties) {
            bool isFieldOptional = 
                isCreation
                    ? property.GetCustomAttribute<RequiredFieldOnCreation>() == null
                    : property.GetCustomAttribute<RequiredFieldOnUpdate>() == null;

            CustomField? customFieldProps = property.GetCustomAttribute<CustomField>();

            string fieldName = 
                customFieldProps?.name 
                ?? property.Name;

            MatchedDataType? matchedType = 
                DataType.GetMatchedTypeByType(
                    customFieldProps?.type ?? property.PropertyType
                );

            StringValues receivedValue = formData[fieldName];

            bool isFieldEmpty = 
                matchedType?.fieldTypeEnum == DataTypesEnum.File
                    ? formData.Files[fieldName] == null
                    : String.IsNullOrEmpty(receivedValue);

            if (isFieldEmpty) {
                // Campo obrigatorio e vazio joga erro
                if (!isFieldOptional) {
                    invalidFields.Add(new InvalidFieldError(
                        fieldName,
                        "O campo é obrigatório e não foi informado."
                    ));
                }
                // Campo vazio e opcional
                // Ambos os casos pula as validacoes subsequentes
                continue;
            }

            if (fieldName != "id") {
                hasFieldsToUpdate = true;
            }

            CustomValidation? customValidation = property.GetCustomAttribute<CustomValidation>();
            if (customValidation != null) { 
                InvalidFieldError? error = await customValidation.Validate(
                    contentTypeData,
                    fieldName,
                    formData
                );

                if (error != null) { 
                    invalidFields.Add(error);
                }
                continue;
            }

            Func<object> reqValueConverter = () => null;
            switch (matchedType?.fieldTypeEnum) {
                case DataTypesEnum.String:
                default:
                    reqValueConverter = () => receivedValue.ToString();
                break;
                case DataTypesEnum.Int:
                    reqValueConverter = () => Int32.Parse(receivedValue);
                break;
                case DataTypesEnum.Float:
                    reqValueConverter = () => float.Parse(receivedValue);
                break;
                case DataTypesEnum.Bool:
                    reqValueConverter = () => bool.Parse(receivedValue);
                break;
                case DataTypesEnum.IntEnumerable:
                    reqValueConverter = () => {
                        // Se informado o campo, mas vazio, retorna vazio
                        if (receivedValue == "") return Array.Empty<int>();

                        string[] rawArray =  receivedValue.ToArray();

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
            }

            object convertedValue = null;
            try {
                convertedValue = reqValueConverter();
            } catch (Exception exception) {
                invalidFields.Add(new InvalidFieldError(
                    fieldName,
                    $"O tipo de dado recebido é diferente de {matchedType?.type.Name}."
                ));
                continue;
            }

            switch (property.Name) {
                case "id":
                    object? itemDbData = null;
                    int itemId = (int)convertedValue;

                    if (itemId > 0) {
                        ContentDBManager contentDbManager = new ContentDBManager();
                        itemDbData = await contentDbManager.GetItemByIdAndTypeData(
                            contentTypeData,
                            itemId
                        );

                        if (itemDbData == null) {
                            throw new CustomException(
                                404, 
                                "Nenhum item com esse id foi encontrado."
                            );
                        }
                    } else {
                        invalidFields.Add(new InvalidFieldError(
                            fieldName,
                            $"O {fieldName} informado é inválido."
                        ));
                    }

                    context.HttpContext.Request.Headers.Add(
                        "requestedItem", 
                        JsonConvert.SerializeObject(itemDbData)
                    );
                break;
                case "autor": 
                    AuthorDTO? authorDbData = null;
                    int authorId = (int)convertedValue;

                    if (authorId > 0) {
                        AuthorDBManager authorDbManager = new AuthorDBManager();
                        authorDbData = await authorDbManager.GetAuthorById(authorId);

                        if (authorDbData == null) {
                            throw new CustomException(
                                404, 
                                "Não existe nenhum autor com esse id."
                            );
                        }

                        if (authorDbData.idTipoConteudo != contentTypeData.contentType) {
                            invalidFields.Add(new InvalidFieldError(
                                fieldName, 
                                "Esse autor não é válido para esse tipo de conteúdo."
                            ));
                            continue;
                        }
                    } else {
                        invalidFields.Add(new InvalidFieldError(
                            fieldName, 
                            $"O {fieldName} informado é inválido."
                        ));
                    }                    
                break;
                case "categorias":
                    IEnumerable<int> categoryIds = (IEnumerable<int>)convertedValue;

                    if (categoryIds.Count() > 0) {
                        CategoryService categoryService = new CategoryService();
                        IEnumerable<int> validCategoriesIds = (
                            await categoryService.GetCategoriesByContentType(
                                contentTypeData.contentType
                            )
                        ).Select(c => c.id);

                        foreach (int idCategory in categoryIds) {
                            if (!validCategoriesIds.Contains(idCategory)) {
                                throw new CustomException(
                                    404,
                                    "Um ou mais idsCategorias não existem."
                                );
                            }
                        }
                    }                    
                break;
            }
        }

        if (invalidFields.Count() > 0) {
            throw new CustomException(
                400,
                "Um ou mais campos contém erros, verifique-os abaixo.",
                invalidFields
            );
        }

        if (!isCreation && !hasFieldsToUpdate) {
            throw new CustomException(
                400,
                "Informe ao menos um campo do item para atualizar."
            );
        }

        await next();
    }
}