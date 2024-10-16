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
    }
}
