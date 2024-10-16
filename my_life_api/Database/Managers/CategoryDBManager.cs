using System.Data;
using MySql.Data.MySqlClient;
using my_life_api.Models.Requests;
using my_life_api.Models;
using my_life_api.Resources;

namespace my_life_api.Database.Managers
{
    public class CategoryDBManager : BaseDBManager
    {
        public async Task CreateCategory(CategoryDTO category)
        {
            await DataBase.OpenConnectionIfClosed();

            string treatedBase64Icon = !string.IsNullOrEmpty(category.iconeBase64)
                ? $"'{category.iconeBase64}'"
                : "NULL";

            MySqlCommand myCommand = new MySqlCommand();
            myCommand.Connection = DataBase.connection;

            myCommand.CommandText =
                "Insert Into Categories" +
                    "(name, iconBase64, contentTypeId)" +
                    "Values" +
                        $"('{category.nome}', {treatedBase64Icon}, {(int)category.idTipoConteudo});";

            await myCommand.ExecuteReaderAsync();

            await DataBase.CloseConnection();
        }

        public async Task UpdateCategory(CategoryDTO category)
        {
            await DataBase.OpenConnectionIfClosed();

            string treatedBase64Icon = !string.IsNullOrEmpty(category.iconeBase64)
                ? $"'{category.iconeBase64}'"
                : "NULL";

            MySqlCommand myCommand = new MySqlCommand();
            myCommand.Connection = DataBase.connection;

            myCommand.CommandText =
                "Update Categories " +
                    "Set " +
                        $"name = '{category.nome}'," +
                        $"iconBase64 = {treatedBase64Icon} " +
                $"Where id = {category.id};";

            await myCommand.ExecuteReaderAsync();

            await DataBase.CloseConnection();
        }

        public async Task DeleteCategoryById(int categoryId)
        {
            await DataBase.OpenConnectionIfClosed();

            MySqlCommand myCommand = new MySqlCommand();
            myCommand.Connection = DataBase.connection;
            myCommand.CommandText =
                "Delete From Categories " +
                    $"Where id = {categoryId};";

            await myCommand.ExecuteReaderAsync();

            await DataBase.CloseConnection();
        }

        public async Task DeleteCategoryRelations(
            int categoryId, 
            ContentTypesEnum contentType
        ) {
            string contentName = GetContentNameByContentType(contentType);

            await DataBase.OpenConnectionIfClosed();

            MySqlCommand myCommand = new MySqlCommand();
            myCommand.Connection = DataBase.connection;
            myCommand.CommandText =
                $"Delete From {contentName}_x_Category " +
                    $"Where categoryId = {categoryId};";

            await myCommand.ExecuteReaderAsync();

            await DataBase.CloseConnection();
        }

        public async Task<CategoryDTO?> GetCategoryById(int categoryId)
        {
            await DataBase.OpenConnectionIfClosed();

            MySqlCommand myCommand = new MySqlCommand();
            myCommand.Connection = DataBase.connection;
            myCommand.CommandText = @"Select id, name, iconBase64, contentTypeId " +
                "From Categories " +
                $"Where id = {categoryId};";

            CategoryDTO category = null;
            using var myReader = await myCommand.ExecuteReaderAsync();

            while (myReader.Read())
            {
                category = new CategoryDTO()
                {
                    id = myReader.GetInt32("id"),
                    nome = myReader.GetString("name"),
                    iconeBase64 = myReader.IsDBNull("iconBase64") ? null : myReader.GetString("iconBase64"),
                    idTipoConteudo = (ContentTypesEnum)myReader.GetInt32("contentTypeId"),
                };
            }

            await DataBase.CloseConnection();

            return category;
        }

        public async Task<IEnumerable<CategoryDTO>> GetCategoriesByContentTypeId(int contentTypeId)
        {
            await DataBase.OpenConnectionIfClosed();

            MySqlCommand myCommand = new MySqlCommand();
            myCommand.Connection = DataBase.connection;
            myCommand.CommandText = @"Select id, name, iconBase64, contentTypeId " +
                "From Categories " +
                $"Where contentTypeId = {contentTypeId};";

            List<CategoryDTO> categories = new List<CategoryDTO>();
            using var myReader = await myCommand.ExecuteReaderAsync();

            while (myReader.Read())
            {
                CategoryDTO categoryToAdd = new CategoryDTO()
                {
                    id = myReader.GetInt32("id"),
                    nome = myReader.GetString("name"),
                    iconeBase64 = myReader.IsDBNull("iconBase64") ? null : myReader.GetString("iconBase64"),
                    idTipoConteudo = (ContentTypesEnum)contentTypeId,
                };
                categories.Add(categoryToAdd);
            }

            await DataBase.CloseConnection();

            return categories;
        }
    }
}
