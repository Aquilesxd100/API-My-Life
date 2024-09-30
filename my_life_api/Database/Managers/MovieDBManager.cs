using my_life_api.Models.Requests;
using my_life_api.Models;
using MySql.Data.MySqlClient;
using System.Data;

namespace my_life_api.Database.Managers
{
    public class MovieDBManager
    {
        public async Task<MovieDTO?> GetMovieById(int movieId)
        {
            await DataBase.OpenConnectionIfClosed();

            MySqlCommand myCommand = new MySqlCommand();
            myCommand.Connection = DataBase.connection;
            myCommand.CommandText = @" Select id, name, imageUrl, rating, dubbed, soulFragment, authorId " +
                "From Movies " +
                $"Where id = {movieId};";

            MovieDTO movie = null;
            using var myReader = await myCommand.ExecuteReaderAsync();

            while (myReader.Read())
            {
                movie = new MovieDTO()
                {
                    id = myReader.GetInt32("id"),
                    nome = myReader.GetString("name"),
                    urlImagem = myReader.IsDBNull("imageUrl") ? null : myReader.GetString("imageUrl"),
                    nota = myReader.GetFloat("rating"),
                    dublado = myReader.GetBoolean("dubbed"),
                    fragmentoAlma = myReader.GetBoolean("soulFragment"),
                    idAutor = myReader.GetInt32("authorId"),
                };
            }

            await DataBase.CloseConnection();

            return movie;
        }

        public async Task UpdateMovieImageUrlById(int movieId, string? imageUrl)
        {
            await DataBase.OpenConnectionIfClosed();

            string newImageUrl = String.IsNullOrEmpty(imageUrl) ? "NULL" : $"'{imageUrl}'";

            MySqlCommand myCommand = new MySqlCommand();
            myCommand.Connection = DataBase.connection;
            myCommand.CommandText = 
                @"Update Movies " +
                    "Set " +
                        $"imageUrl = {newImageUrl} " +
                    $"Where id = {movieId};";

            await myCommand.ExecuteReaderAsync();

            await DataBase.CloseConnection();
        }
    }
}
