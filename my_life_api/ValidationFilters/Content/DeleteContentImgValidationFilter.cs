using System.Collections.Immutable;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using my_life_api.Database.Managers;
using my_life_api.Models;
using my_life_api.Resources;
using my_life_api.Shared.ContentResources;

namespace my_life_api.ValidationFilters.Content;

public class DeleteContentImgValidationFilter : ICustomActionFilter {
    // Array com ids de 'contentType' de somente os conteudos
    // que tem imagem, ou seja, a coluna 'imageUrl'
    public static readonly ImmutableArray<int> validContentTypesIds = ContentUtils.contentTypesData
        .Where(
            ctd => ctd.entityType.GetProperties()
                    .Any(p => p.Name == "imageUrl")
        )
        .Select(ctd => 
            (int)ctd.contentType
        )
        .ToImmutableArray<int>();

    public override async Task OnActionExecutionAsync(
        ActionExecutingContext context,
        ActionExecutionDelegate next
    ) {
        string contentTypeId = GetParamValue("idTipoConteudo", context);
        string contentId = GetParamValue("idConteudo", context);

        if (string.IsNullOrEmpty(contentTypeId)) {
            throw new CustomException(
                400, 
                "O param 'idTipoConteudo' é obrigatório e não foi informado."
            );
        }

        if (string.IsNullOrEmpty(contentId)) {
            throw new CustomException(
                400, 
                "O param 'idConteudo' é obrigatório e não foi informado."
            );
        }

        int convertedContentTypeId = 0;
        int convertedContentId = 0;

        try {
            convertedContentTypeId = Int32.Parse(contentTypeId);
            convertedContentId = Int32.Parse(contentId);
        } catch (Exception exception) { }

        if (!validContentTypesIds.Contains(convertedContentTypeId)) {
            throw new CustomException(
                400, 
                "O param 'idTipoConteudo' informado é inválido, informe um tipo de conteúdo que aceite imagens."
            );
        }

        ContentTypeData contentTypeData = ContentUtils.GetContentTypeData(
            (ContentTypesEnum)convertedContentTypeId
        );

        ContentDBManager contentDbManager = new ContentDBManager();
        dynamic? contentItem = await contentDbManager.GetItemByIdAndTypeData(
            contentTypeData,
            convertedContentId
        );

        if (contentItem == null) {
            throw new CustomException(404, "Nenhum conteúdo com esse id foi encontrado.");
        }

        if (String.IsNullOrEmpty(contentItem.urlImagem)) {
            throw new CustomException(400, "Esse conteudo não tem imagem registrada.");
        }

        context.HttpContext.Request.Headers.Add(
            "requestedItem", 
            JsonConvert.SerializeObject(contentItem)
        );

        await next();
    }
}
