using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using my_life_api.Database.Managers;
using my_life_api.Models;
using my_life_api.Resources;
using my_life_api.Shared.ContentResources;
using my_life_api.Shared;

namespace my_life_api.ValidationFilters.Content;

public class DeleteContentValidationFilter : ICustomActionFilter {
    public override async Task OnActionExecutionAsync(
        ActionExecutingContext context,
        ActionExecutionDelegate next
    ) {
        ContentTypeData contentTypeData = ContentUtils.GetContentTypeDataByPath(
            context.HttpContext.Request.Path
        );

        // Item correspondente ao Id, pego diretamente do banco
        object? dbItem = null;

        string reqParamName = 
            "id" + 
            Format.GetWordWithUpperCaseInitial(
                contentTypeData.nameInPtBrWithNoAccent
            );

        string reqItemId = GetParamValue(
            reqParamName, 
            context
        );

        if (string.IsNullOrEmpty(reqItemId)) {
            throw new CustomException(
                400, 
                $"O param '{reqParamName}' é obrigatório e não foi informado."
            );
        }

        int convertedId = 0;

        try {
            convertedId = Int32.Parse(reqItemId);
        } catch (Exception exception) {
            throw new CustomException(400, "O id informado é inválido.");
        }

        if (convertedId < 0) {
            throw new CustomException(400, "O id informado é inválido.");
        }

        ContentDBManager contentDbManager = new ContentDBManager();
        dbItem = await contentDbManager.GetItemByIdAndTypeData(
            contentTypeData,
            convertedId
        );

        if (dbItem == null) {
            throw new CustomException(
                404, 
                $"Nenhum(a) {contentTypeData.nameInPtBr} com esse id foi encontrado(a)."
            );
        }

        context.HttpContext.Request.Headers.Add(
            "requestedItem", 
            JsonConvert.SerializeObject(dbItem)
        );

        await next();
    }
}
