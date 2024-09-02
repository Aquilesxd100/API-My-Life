using System.Data;
using my_life_api.Models;
using MySql.Data.MySqlClient;

namespace my_life_api.Resources
{
    public static class DataBase
    {
        static private MySqlConnection connection;

        public static async Task OpenConnectionIfClosed()
        {
            if (connection.State == ConnectionState.Closed)
            {
                await connection.OpenAsync();
            }
        }

        public static async Task CloseConnection()
        {
            await connection.CloseAsync();
        }

        public static async Task ConnectToDataBase(string connectionString)
        {
            connection = new MySqlConnection(connectionString);
            await connection.OpenAsync();
        }

        public static async Task<List<int>> GetContentTypesId()
        {
            await OpenConnectionIfClosed();

            MySqlCommand myCommand = new MySqlCommand();
            myCommand.Connection = connection;
            myCommand.CommandText = @" Select id from ContentTypes; ";

            List<int> contentTypesIds = new List<int>();
            using var myReader = await myCommand.ExecuteReaderAsync();

            while (myReader.Read())
            {
                contentTypesIds.Add(myReader.GetInt32("id"));
            }

            await CloseConnection();

            return contentTypesIds;
        }

        public static async Task CreateAuthor(AuthorDTO author)
        {
            await OpenConnectionIfClosed();

            string treatedUrlImage = author.urlImagem != null 
                ? $"'{author.urlImagem}'"
                : "NULL";

            // TBM PRECISA ATUALIZAR ESSA URL DE IMAGEM PARA POSTAR NO STORAGE A IMAGEM E RETONAR O LINK DELA
            // CASO FALHE A INCLUSAO NO BANCO ABAIXO DEVE-SE DELETAR A IMAGEM CRIADA LÁ NO STORAGE

            MySqlCommand myCommand = new MySqlCommand();
            myCommand.Connection = connection;

            myCommand.CommandText = 
                "Insert Into Authors" +
                    "(name, imageUrl, content_type_id)" +
                    "Values" +
                        $"('{author.nome}', {treatedUrlImage}, {author.idTipoConteudo})";

            await myCommand.ExecuteReaderAsync();

            await CloseConnection();
        }
    }
}
