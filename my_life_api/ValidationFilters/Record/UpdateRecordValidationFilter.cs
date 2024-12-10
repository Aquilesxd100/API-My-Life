using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using my_life_api.Database.Managers;
using my_life_api.Models;
using my_life_api.Resources;
using my_life_api.Shared;
using my_life_api.Shared.ContentResources;
using my_life_api.Models.Requests.Record;

namespace my_life_api.ValidationFilters.Record;

public class UpdateRecordValidationFilter : ICustomActionFilter {
    public override async Task OnActionExecutionAsync(
        ActionExecutingContext context,
        ActionExecutionDelegate next
    ) {
        var recordObj = await GetFormDataContent<RecordUpdateRequestDTO>(context);

        RecordUpdateRequestDTO record = new RecordUpdateRequestDTO().BuildFromObj(recordObj);

        if (!string.IsNullOrEmpty(record.ano)) { 
            if (record.ano.Trim().Length == 0) {
                throw new CustomException(400, "O ano do registro não pode ser vazio.");
            }

            if (record.ano.Length > 4) {
                throw new CustomException(400, "O ano do registro está em formato incorreto.");
            }

            int convertedYear = 0;
            try {
                convertedYear = int.Parse(record.ano);
            } catch (Exception ex) {
                throw new CustomException(400, "O ano do registro está em formato incorreto.");
            }

            if (convertedYear < 1800 || convertedYear > 2300) {
                throw new CustomException(400, "O ano do registro é inválido.");
            }

            if (Validator.HasInvalidCharacters(record.ano)) {
                throw new CustomException(400, "O ano do registro contém caracteres inválidos.");
            }
        }

        ContentValidator validator = new ContentValidator();

        validator.ValidateName(record.nome);

        validator.ValidateOptionalImgFile(record.imagemPrincipal);

        if (!string.IsNullOrEmpty(record.conteudo)) {
            if (record.conteudo.Trim().Length == 0) {
                throw new CustomException(400, "O conteúdo do registro não pode ser vazio.");
            }

            if (record.conteudo.Length > 3500) {
                throw new CustomException(
                    400, 
                    "O conteúdo do registro deve ter no máximo 3500 caracteres."
                );
            }

            if (Validator.HasInvalidCharacters(record.conteudo)) {
                throw new CustomException(
                    400, 
                    "O conteúdo do registro contém caracteres inválidos."
                );
            }

        }

        if (
            string.IsNullOrEmpty(record.nome)
            && string.IsNullOrEmpty(record.conteudo)
            && string.IsNullOrEmpty(record.ano)
            && record.imagemPrincipal == null
        ) {
            throw new CustomException(400, "Informe ao menos um campo para atualizar.");
        }

        dynamic? recordDbData = null;
        if (record.id > 0) {
            RecordDBManager recordDbManager = new RecordDBManager();
            recordDbData = await recordDbManager.GetRecordById(record.id);

            if (recordDbData == null) {
                throw new CustomException(404, "Não existe nenhum registro com esse id.");
            }
        } else {
            throw new CustomException(
                400,
                "O id do registro informado é inválido."
            );
        }

        context.HttpContext.Request.Headers.Add(
            "requestedItem", 
            JsonConvert.SerializeObject(recordDbData)
        );

        await next();
    }
}
