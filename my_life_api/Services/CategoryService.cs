using my_life_api.Database.Managers;
using my_life_api.Models;
using my_life_api.Models.Requests.Category;

namespace my_life_api.Services;

public class CategoryService {
    private CategoryDBManager dbManager = new CategoryDBManager();

    public async Task<IEnumerable<CategoryDTO>> GetCategoriesByContentTypeId(
        ContentTypesEnum contentType
    ) {
        IEnumerable<CategoryDTO> categories = await dbManager.GetCategoriesByContentTypeId(
            (int)contentType
        );

        return categories;
    }

    public async Task CreateCategory(CategoryCreateRequestDTO categoryReq) {
        CategoryDTO category = new CategoryDTO {
            idTipoConteudo = (ContentTypesEnum)categoryReq.idTipoConteudo,
            nome = categoryReq.nome,
            iconeBase64 = categoryReq.iconeBase64
        };

        await dbManager.CreateCategory(category);
    }

    public async Task UpdateCategory(
        CategoryUpdateRequestDTO categoryReq, 
        CategoryDTO dbCategory
    ) {
        CategoryDTO category = new CategoryDTO {
            id = dbCategory.id,
            nome = dbCategory.nome,
            iconeBase64 = dbCategory.iconeBase64
        };

        if (!string.IsNullOrEmpty(categoryReq.iconeBase64)) {
            category.iconeBase64 = categoryReq.iconeBase64;
        }

        if (!string.IsNullOrEmpty(categoryReq.nome)) {
            category.nome = categoryReq.nome;
        }

        await dbManager.UpdateCategory(category);
    }

    public async Task DeleteCategory(CategoryDTO category) {
        await dbManager.DeleteCategoryRelations(category.id, category.idTipoConteudo);
        await dbManager.DeleteCategoryById(category.id);
    }

    public async Task DeleteCategoryIcon(CategoryDTO category) {
        category.iconeBase64 = null;
        await dbManager.UpdateCategory(category);
    }
}
