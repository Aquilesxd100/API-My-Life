﻿using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using my_life_api.Resources;
using my_life_api.Services;
using my_life_api.ValidationFilters.Content;
using my_life_api.ValidationFilters.Security;
using my_life_api.Shared.ContentResources;

namespace my_life_api.Controllers;

[ApiController]
public class DeleteImgController : ControllerBase {

    private readonly ILogger<DeleteImgController> _logger;

    public DeleteImgController(ILogger<DeleteImgController> logger) {
        _logger = logger;
    }

    [HttpDelete("deletarimg/conteudo", Name = "deletarimg/conteudo")]
    [ServiceFilter(typeof(TokenValidationFilter))]
    [ServiceFilter(typeof(DeleteContentImgValidationFilter))]
    public async Task<IActionResult> DeleteContentImg(
        [FromQuery] string idTipoConteudo,
        [FromQuery] string idConteudo
    ) {
        ContentTypesEnum contentTypeId = (ContentTypesEnum)Int32.Parse(idTipoConteudo);
        int contentId = Int32.Parse(idConteudo);
        dynamic requestedItem = JsonConvert.DeserializeObject(
            HttpContext.Request.Headers["requestedItem"]
        );

        DeleteImgService service = new DeleteImgService();
        await service.DeleteContentImg(contentTypeId, requestedItem);

        return Ok(ApiResponse.CreateBody(
            200,
            "A imagem foi excluída com sucesso!"
        ));
    }
}
