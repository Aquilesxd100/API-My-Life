using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using my_life_api.Database.Managers;
using my_life_api.Models;
using my_life_api.Resources;
using my_life_api.Models.Requests.Record;
using my_life_api.Shared;

namespace my_life_api.ValidatorsFilters.Record;

public class AddRecordSecondaryImgValidationFilter : ICustomActionFilter {
    public override async Task OnActionExecutionAsync(
        ActionExecutingContext context,
        ActionExecutionDelegate next
    ) {
        var requestObj = await GetFormDataContent<RecordAddSecondaryImgDTO>(context);

        RecordAddSecondaryImgDTO secondaryImgRequest = new RecordAddSecondaryImgDTO().BuildFromObj(requestObj);

        var validator = new ContentValidator();
        if (secondaryImgRequest.imagemSecundaria != null) { 
            validator.ValidateOptionalImgFile(secondaryImgRequest.imagemSecundaria);            
        } else {
            throw new CustomException(
                400, 
                "A 'imagemSecundaria' é obrigatória e não foi informada."
            );
        }

        RecordDTO? record = null;

        RecordDBManager recordDbManager = new RecordDBManager();
        record = await recordDbManager.GetRecordById(secondaryImgRequest.idRegistro);

        if (record == null) {
            throw new CustomException(404, "Nenhum registro com esse id foi encontrado.");
        }

        if (record.imagensSecundarias.Count() >= 8) {
            throw new CustomException(
                400, 
                "Esse registro já alcançou o limite de imagens secundárias."
            );
        }

        context.HttpContext.Request.Headers.Add(
            "requestedItem", 
            JsonConvert.SerializeObject(record)
        );

        await next();
    }
}
