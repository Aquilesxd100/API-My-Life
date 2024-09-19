using System.Data;
using my_life_api.Models;
using my_life_api.Models.Requests;
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

        public static void ConfigureDataBase(string connectionString)
        {
            connection = new MySqlConnection(connectionString);
        }

        public static async Task<IEnumerable<AuthorDTO>> GetAuthorsByContentTypeId(int contentTypeId)
        {
            await OpenConnectionIfClosed();

            MySqlCommand myCommand = new MySqlCommand();
            myCommand.Connection = connection;
            myCommand.CommandText = @" Select id, name, imageUrl, contentTypeId " +
                "From Authors " +
                $"Where contentTypeId = {contentTypeId};";

            List<AuthorDTO> authors = new List<AuthorDTO>();
            using var myReader = await myCommand.ExecuteReaderAsync();

            while (myReader.Read())
            {
                AuthorDTO authorToAdd = new AuthorDTO()
                {
                    id = myReader.GetInt32("id"),
                    nome = myReader.GetString("name"),
                    urlImagem = myReader.IsDBNull("imageUrl") ? null : myReader.GetString("imageUrl"),
                    idTipoConteudo = (ContentTypesEnum)contentTypeId,
                };
                authors.Add(authorToAdd);
            }

            await CloseConnection();

            return authors;
        }

        public static async Task<int> CreateAuthor(AuthorDTO author)
        {
            await OpenConnectionIfClosed();

            string treatedUrlImage = author.urlImagem != null
                ? $"'{author.urlImagem}'"
                : "NULL";

            MySqlCommand myCommand = new MySqlCommand();
            myCommand.Connection = connection;

            myCommand.CommandText =
                "Insert Into Authors" +
                    "(name, imageUrl, contentTypeId)" +
                    "Values" +
                        $"('{author.nome}', {treatedUrlImage}, {(int)author.idTipoConteudo});"
                + "Select Last_Insert_Id();";

            var result = await myCommand.ExecuteScalarAsync();
            int authorId = Int32.Parse(result.ToString());

            await CloseConnection();

            return authorId;
        }

        public static async Task UpdateAuthor(AuthorDTO author)
        {
            await OpenConnectionIfClosed();

            string treatedUrlImage = author.urlImagem != null
                ? $"'{author.urlImagem}'"
                : "NULL";

            MySqlCommand myCommand = new MySqlCommand();
            myCommand.Connection = connection;

            myCommand.CommandText =
                "Update Authors " +
                    "Set " +
                        $"name = '{author.nome}'," +
                        $"imageUrl = {treatedUrlImage}," +
                        $"contentTypeId = {(int)author.idTipoConteudo} " +
                $"Where id = {author.id};";

            await myCommand.ExecuteScalarAsync();

            await CloseConnection();
        }
    }
}
