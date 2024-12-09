using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using my_life_api.Database.Managers;
using my_life_api.Models;
using my_life_api.Resources;

namespace my_life_api.ValidatorsFilters.Record;

public class DeleteRecordMainImgValidationFilter : ICustomActionFilter {
    public override async Task OnActionExecutionAsync(
        ActionExecutingContext context,
        ActionExecutionDelegate next
    ) {
        string recordId = GetParamValue("idRegistro", context);

        if (string.IsNullOrEmpty(recordId)) {
            throw new CustomException(
                400, 
                "O param 'idRegistro' é obrigatório e não foi informado."
            );
        }

        int convertedRecordId = 0;

        try {
            convertedRecordId = Int32.Parse(recordId);
        } catch (Exception exception) {
            throw new CustomException(400, "O id informado é inválido.");
        }

        if (convertedRecordId < 0) {
            throw new CustomException(400, "O id informado é inválido.");
        }

        RecordDTO? record = null;

        RecordDBManager recordDbManager = new RecordDBManager();
        record = await recordDbManager.GetRecordById(convertedRecordId);

        if (record == null) {
            throw new CustomException(404, "Nenhum registro com esse id foi encontrado.");
        }

        if (String.IsNullOrEmpty(record.urlImagemPrincipal)) {
            throw new CustomException(
                400, 
                "Esse registro não tem uma imagem principal registrada."
            );
        }

        context.HttpContext.Request.Headers.Add(
            "requestedItem", 
            JsonConvert.SerializeObject(record)
        );

        await next();
    }
}
