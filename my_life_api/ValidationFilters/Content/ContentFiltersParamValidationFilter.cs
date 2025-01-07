using Microsoft.AspNetCore.Mvc.Filters;
using my_life_api.Models;
using my_life_api.Resources;
using my_life_api.Shared;

namespace my_life_api.ValidationFilters.Content;

public class ContentFiltersParamValidationFilter : ICustomActionFilter {
    public override async Task OnActionExecutionAsync(
        ActionExecutingContext context,
        ActionExecutionDelegate next
    ) {
        string search = GetParamValue("pesquisa", context);

        string soulFragment = GetParamValue("fragmentoAlma", context);
        string dubbed = GetParamValue("dublado", context);
        string finished = GetParamValue("finalizado", context);

        string authorId = GetParamValue("idAutor", context);

        string ratingGreaterEqualTo = GetParamValue("notaMaiorIgualQue", context);
        string ratingLesserEqualTo = GetParamValue("notaMenorIgualQue", context);

        string categoriesIds = GetParamValue("idsCategorias", context);

        IEnumerable<string> allowedBoolTypes = new string[] { "true", "false" };

        if (soulFragment != null) {
            if (!allowedBoolTypes.Any(abt => abt == soulFragment)) {
                throw new CustomException(
                    400, 
                    "O param 'fragmentoAlma' só pode ter o valor 'true' ou 'false'."
                );
            }
        }

        if (dubbed != null) {
            if (!allowedBoolTypes.Any(abt => abt == dubbed)) {
                throw new CustomException(
                    400,
                    "O param 'dublado' só pode ter o valor 'true' ou 'false'."
                );
            }
        }

        if (finished != null) {
            if (!allowedBoolTypes.Any(abt => abt == finished)) {
                throw new CustomException(
                    400,
                    "O param 'finalizado' só pode ter o valor 'true' ou 'false'."
                );
            }
        }

        if (authorId != null) {
            try {
                Int32.Parse(authorId);
            } catch {
                throw new CustomException(
                    400,
                    "O param 'idAutor' informado não é valido."
                );
            }
        }

        if (ratingGreaterEqualTo != null) {
            float convertedRatingGreaterEqualTo;
            try {
                convertedRatingGreaterEqualTo = float.Parse(ratingGreaterEqualTo);
            } catch {
                throw new CustomException(
                    400,
                    "O param 'notaMaiorIgualQue' informado não é valido."
                );
            }

            if (Validator.IsRatingInvalid(convertedRatingGreaterEqualTo)) {
                throw new CustomException(
                    400,
                    "O param 'notaMaiorIgualQue' informado não é valido, informe um valor dentre 0 e 10."
                );
            }
        }

        if (ratingLesserEqualTo != null) {
            float convertedRatingLesserEqualTo;
            try {
                convertedRatingLesserEqualTo = float.Parse(ratingLesserEqualTo);
            } catch {
                throw new CustomException(
                    400,
                    "O param 'notaMenorIgualQue' informado não é valido."
                );
            }

            if (Validator.IsRatingInvalid(convertedRatingLesserEqualTo)) {
                throw new CustomException(
                    400,
                    "O param 'notaMenorIgualQue' informado não é valido, informe um valor dentre 0 e 10."
                );
            }
        }

        if (categoriesIds != null) {
            IEnumerable<string> categoriesIdsList = categoriesIds.Split(',');

            if (categoriesIdsList.Count() > 40) {
                throw new CustomException(
                    400,
                    "Não é permitido informar mais de 40 'idsCategorias'."
                );
            }

            try {

                foreach (string categoryId in categoriesIdsList) {
                    Int32.Parse(categoryId);
                }
            } catch {
                throw new CustomException(
                    400,
                    "Um ou mais params 'idsCategorias' não são validos."
                );
            }
        }

        if (search != null) {
            if (search.Length > 30) {
                throw new CustomException(
                    400,
                    "O param 'pesquisa' não pode ter mais que 30 caracteres."
                );
            }

            if (Validator.HasInvalidCharacters(search)) {
                throw new CustomException(
                    400,
                    "O param 'pesquisa' contém caracteres inválidos."
                );
            }
        }

        await next();
    }
}
