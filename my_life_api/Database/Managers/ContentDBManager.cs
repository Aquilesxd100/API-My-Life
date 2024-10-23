using MySql.Data.MySqlClient;
using my_life_api.Models;
using my_life_api.Resources;

namespace my_life_api.Database.Managers
{
    public class ContentDBManager : BaseDBManager
    {
        public async Task DeleteContent(
            int contentId,
            ContentTypesEnum contentType
        ) {
            string tableName = GetTableNameByContentType(contentType);

            await DataBase.OpenConnectionIfClosed();

            MySqlCommand myCommand = new MySqlCommand();
            myCommand.Connection = DataBase.connection;
            myCommand.CommandText =
                $"Delete From {tableName} " +
                    $"Where id = {contentId};";

            await myCommand.ExecuteReaderAsync();

            await DataBase.CloseConnection();
        }

        public async Task CreateContentCategoryRelations(
            ContentTypesEnum contentType,
            int contentId,
            IEnumerable<int> categoriesIdsToAdd
        ) {
            string contentName = GetContentNameByContentType(contentType);
            string contentColumnIdName = $"{contentName.ToLower()}Id";

            string valuesToAddQueryPart = "";
            foreach (int categoryId in categoriesIdsToAdd)
            {
                if (valuesToAddQueryPart.Length > 0)
                    valuesToAddQueryPart += ", ";

                valuesToAddQueryPart += $"({contentId}, {categoryId})";
            }
            valuesToAddQueryPart += ";";

            await DataBase.OpenConnectionIfClosed();

            MySqlCommand myCommand = new MySqlCommand();
            myCommand.Connection = DataBase.connection;
            myCommand.CommandText =
                $"Insert Into {contentName}_x_Category" +
                    $"({contentColumnIdName}, categoryId)" +
                    "Values" +
                        valuesToAddQueryPart;

            await myCommand.ExecuteReaderAsync();

            await DataBase.CloseConnection();
        }

        public async Task DeleteContentCategoryRelations(
            ContentTypesEnum contentType,
            int contentId
        ) {
            string contentName = GetContentNameByContentType(contentType);
            string contentColumnIdName = $"{contentName.ToLower()}Id";

            await DataBase.OpenConnectionIfClosed();

            MySqlCommand myCommand = new MySqlCommand();
            myCommand.Connection = DataBase.connection;
            myCommand.CommandText =
                $"Delete From {contentName}_x_Category " +
                    $"Where {contentColumnIdName} = {contentId};";

            await myCommand.ExecuteReaderAsync();

            await DataBase.CloseConnection();
        }

        public async Task UpdateContentCategoryRelations(
            ContentTypesEnum contentType,
            int contentId,
            IEnumerable<int> idsCategorias
        ) {
                await DeleteContentCategoryRelations(
                    contentType,
                    contentId
                );

                if (idsCategorias.Count() > 0) {
                    await CreateContentCategoryRelations(
                        contentType,
                        contentId,
                        idsCategorias
                    );
                }
        }
    }
}
