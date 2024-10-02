using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using my_life_api.Validators;
using my_life_api.Resources;
using my_life_api.Services;
using my_life_api.Models;
using my_life_api.Validators.Security;

namespace my_life_api.Controllers
{
    [ApiController]
    public class DeleteImgController : ControllerBase
    {

        private readonly ILogger<DeleteImgController> _logger;

        public DeleteImgController(ILogger<DeleteImgController> logger)
        {
            _logger = logger;
        }

        [HttpDelete("deletarimg/recurso", Name = "deletarimg/recurso")]
        [ServiceFilter(typeof(TokenValidationFilter))]
        [ServiceFilter(typeof(DeleteResourceImgValidationFilter))]
        public async Task<IActionResult> DeleteResourceImg(
            [FromQuery] string idTipoConteudo,
            [FromQuery] string idRecurso
        )
        {
            ContentTypesEnum contentTypeId = (ContentTypesEnum)Int32.Parse(idTipoConteudo);
            int resourceId = Int32.Parse(idRecurso);
            dynamic requestedItem = JsonConvert.DeserializeObject(HttpContext.Request.Headers["requestedItem"]);

            DeleteImgService service = new DeleteImgService();
            await service.DeleteResourceImg(contentTypeId, requestedItem);

            return Ok(ApiResponse.CreateBody(
                200,
                "A imagem foi removida com sucesso!"
            ));
        }
    }
}
