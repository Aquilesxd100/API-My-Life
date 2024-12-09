using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using my_life_api.Models;
using my_life_api.Resources;
using my_life_api.Services;
using my_life_api.ValidatorsFilters;
using my_life_api.ValidatorsFilters.Security;
using my_life_api.ValidatorsFilters.Category;
using my_life_api.Models.Requests.Category;

namespace my_life_api.Controllers;

[ApiController]
public class CategoryController : ControllerBase {

    private readonly ILogger<CategoryController> _logger;

    public CategoryController(ILogger<CategoryController> logger) {
        _logger = logger;
    }

    [HttpGet("categorias", Name = "categorias")]
    [ServiceFilter(typeof(TokenValidationFilter))]
    [ServiceFilter(typeof(ContentTypeParamValidationFilter))]
    public async Task<IActionResult> Get([FromQuery] string idTipoConteudo) {
        CategoryService service = new CategoryService();

        ContentTypesEnum contentTypeId = (ContentTypesEnum)Int32.Parse(idTipoConteudo);
        IEnumerable<CategoryDTO> categories = await service.GetCategoriesByContentTypeId(contentTypeId);

        return Ok(ApiResponse.CreateBody(
            200,
            "Lista de categorias recebida com sucesso!",
            new { categorias = categories }
        ));
    }

    [HttpPost("categoria", Name = "categoria")]
    [ServiceFilter(typeof(TokenValidationFilter))]
    [ServiceFilter(typeof(CreateCategoryValidationFilter))]
    public async Task<IActionResult> Post(
        [FromForm] string nome,
        [FromForm] string iconeBase64,
        [FromForm] int idTipoConteudo
    ) {
        CategoryService service = new CategoryService();

        CategoryCreateRequestDTO categoryReq = new CategoryCreateRequestDTO {
            nome = nome,
            idTipoConteudo = idTipoConteudo,
            iconeBase64 = iconeBase64
        };

        await service.CreateCategory(categoryReq);

        return Ok(ApiResponse.CreateBody(201, "Categoria criada com sucesso!"));
    }

    [HttpPut("categoria", Name = "categoria")]
    [ServiceFilter(typeof(TokenValidationFilter))]
    [ServiceFilter(typeof(UpdateCategoryValidationFilter))]
    public async Task<IActionResult> Put(
        [FromForm] int id,
        [FromForm] string nome,
        [FromForm] string iconeBase64
    ) {
        CategoryService service = new CategoryService();

        CategoryUpdateRequestDTO categoryReq = new CategoryUpdateRequestDTO {
            id = id,
            nome = nome,
            iconeBase64 = iconeBase64
        };

        CategoryDTO dbCategory = JsonConvert.DeserializeObject<CategoryDTO>(
            HttpContext.Request.Headers["requestedItem"]
        );

        await service.UpdateCategory(categoryReq, dbCategory);

        return Ok(ApiResponse.CreateBody(200, "Categoria atualizada com sucesso!"));
    }

    [HttpDelete("categoria", Name = "categoria")]
    [ServiceFilter(typeof(TokenValidationFilter))]
    [ServiceFilter(typeof(DeleteCategoryValidationFilter))]
    public async Task<IActionResult> Delete([FromQuery] string idCategoria) {
        CategoryService service = new CategoryService();

        CategoryDTO dbCategory = JsonConvert.DeserializeObject<CategoryDTO>(
            HttpContext.Request.Headers["requestedItem"]
        );

        await service.DeleteCategory(dbCategory);

        return Ok(ApiResponse.CreateBody(200, "A categoria foi excluída com sucesso!"));
    }

    [HttpDelete("categoriaIcone", Name = "categoriaIcone")]
    [ServiceFilter(typeof(TokenValidationFilter))]
    [ServiceFilter(typeof(DeleteCategoryValidationFilter))]
    public async Task<IActionResult> DeleteIcone([FromQuery] string idCategoria) {
        CategoryService service = new CategoryService();

        CategoryDTO dbCategory = JsonConvert.DeserializeObject<CategoryDTO>(
            HttpContext.Request.Headers["requestedItem"]
        );

        await service.DeleteCategoryIcon(dbCategory);

        return Ok(ApiResponse.CreateBody(200, "O ícone da categoria foi excluído com sucesso!"));
    }
}
