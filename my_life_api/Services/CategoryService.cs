using my_life_api.Database.Managers;
using my_life_api.Models;
using my_life_api.Models.Requests;
using my_life_api.Models.Requests.Category;

namespace my_life_api.Services
{
    public class CategoryService
    {
        public async Task<IEnumerable<CategoryDTO>> GetCategoriesByContentTypeId(ContentTypesEnum contentType)
        {
            CategoryDBManager categoryDbManager = new CategoryDBManager();
            IEnumerable<CategoryDTO> categories = await categoryDbManager.GetCategoriesByContentTypeId(
                (int)contentType
            );

            return categories;
        }

        public async Task CreateCategory(CategoryCreateRequestDTO categoryReq)
        {
            CategoryDTO category = new CategoryDTO
            {
                idTipoConteudo = (ContentTypesEnum)categoryReq.idTipoConteudo,
                nome = categoryReq.nome,
                iconeBase64 = categoryReq.iconeBase64
            };

            CategoryDBManager categoryDbManager = new CategoryDBManager();
            await categoryDbManager.CreateCategory(category);
        }

        public async Task UpdateCategory(CategoryUpdateRequestDTO categoryReq, CategoryDTO dbCategory)
        {
            CategoryDTO category = new CategoryDTO
            {
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

            CategoryDBManager categoryDbManager = new CategoryDBManager();

            await categoryDbManager.UpdateCategory(category);
        }

        public async Task DeleteCategory(CategoryDTO category)
        {
            CategoryDBManager categoryDbManager = new CategoryDBManager();

            await categoryDbManager.DeleteCategoryRelations(category.id, category.idTipoConteudo);
            await categoryDbManager.DeleteCategoryById(category.id);
        }

        public async Task DeleteCategoryIcon(CategoryDTO category)
        {
            CategoryDBManager categoryDbManager = new CategoryDBManager();

            category.iconeBase64 = null;
            await categoryDbManager.UpdateCategory(category);
        }
    }
}
