using System.Data;
using MySql.Data.MySqlClient;
using my_life_api.Models;
using my_life_api.Resources;

namespace my_life_api.Database.Managers;

public class MovieDBManager : BaseDBManager {
    public async Task<int> CreateMovie(MovieDTO movie) {
        await DataBase.OpenConnectionIfClosed();

        string treatedUrlImage = movie.urlImagem != null
            ? $"'{movie.urlImagem}'"
            : "NULL";

        int dubbedInBytes = movie.dublado ? 1 : 0;
        int soulFragmentInBytes = movie.fragmentoAlma ? 1 : 0;
        string treatedRatingValue = GetTreatedRatingValue(movie.nota);

        MySqlCommand myCommand = new MySqlCommand();
        myCommand.Connection = DataBase.connection;

        myCommand.CommandText =
            "Insert Into Movies" +
                "(name, imageUrl, rating, dubbed, soulFragment, authorId)" +
                "Values" +
                    $"('{movie.nome}', {treatedUrlImage}, {treatedRatingValue}, " +
                    $"{dubbedInBytes}, {soulFragmentInBytes}, {movie.autor.id});"
            + "Select Last_Insert_Id();";

        var result = await myCommand.ExecuteScalarAsync();
        int movieId = int.Parse(result.ToString());

        await DataBase.CloseConnection();

        return movieId;
    }

    public async Task UpdateMovie(MovieDTO movie) {
        await DataBase.OpenConnectionIfClosed();

        string treatedUrlImage = movie.urlImagem != null
            ? $"'{movie.urlImagem}'"
            : "NULL";

        int dubbedInBytes = movie.dublado ? 1 : 0;
        int soulFragmentInBytes = movie.fragmentoAlma ? 1 : 0;
        string treatedRatingValue = GetTreatedRatingValue(movie.nota);

        MySqlCommand myCommand = new MySqlCommand();
        myCommand.Connection = DataBase.connection;

        myCommand.CommandText =
            "Update Movies " +
                "Set " +
                    $"name = '{movie.nome}'," +
                    $"imageUrl = {treatedUrlImage}," +
                    $"rating = {treatedRatingValue}," +
                    $"dubbed = {dubbedInBytes}," +
                    $"soulFragment = {soulFragmentInBytes}," +
                    $"authorId = {movie.autor.id} " +
            $"Where id = {movie.id};";

        await myCommand.ExecuteReaderAsync();

        await DataBase.CloseConnection();
    }

    public async Task<IEnumerable<MovieDTO>> GetMovies(ContentFilters filters) {
        await DataBase.OpenConnectionIfClosed();

        MySqlCommand myCommand = new MySqlCommand();
        myCommand.Connection = DataBase.connection;
        myCommand.CommandText =
            "Select " +
                "Movies.id As movie_id, Movies.name As movie_name, Movies.imageUrl As movie_imageUrl, " +
                "rating, dubbed, soulFragment, authorId, " +
                "a.id as author_id, a.name As author_name, a.imageUrl As author_imageUrl " +
            "From Movies " +
            "Left Join Authors a " +
                "On authorId = a.id " +
            MountConditionalQueryPartByFilters(filters, "Movie_x_Category", "Movies") + ";";

        List<MovieDTO> movies = new List<MovieDTO>();
        using var myReader = await myCommand.ExecuteReaderAsync();

        while (myReader.Read()) {
            MovieDTO movieToAdd = new MovieDTO() {
                id = myReader.GetInt32("movie_id"),
                nome = myReader.GetString("movie_name"),
                urlImagem = myReader.IsDBNull("movie_imageUrl") ? null : myReader.GetString("movie_imageUrl"),
                dublado = myReader.GetBoolean("dubbed"),
                fragmentoAlma = myReader.GetBoolean("soulFragment"),
                autor = new AuthorDTO() {
                    id = myReader.GetInt32("author_id"),
                    nome = myReader.GetString("author_name"),
                    urlImagem = myReader.IsDBNull("author_imageUrl") ? null : myReader.GetString("author_imageUrl")
                },
                nota = myReader.GetFloat("rating")
            };

            movies.Add(movieToAdd);
        }

        await DataBase.CloseConnection();

        foreach (MovieDTO movie in movies) {
            IEnumerable<CategoryDTO> categories = await GetCategoriesByContentId(
                (int)movie.id, ContentTypesEnum.Cinema
            );

            movie.categorias = categories;
        }

        return movies;
    }

    public async Task<MovieDTO?> GetMovieById(int movieId) {
        await DataBase.OpenConnectionIfClosed();

        MySqlCommand myCommand = new MySqlCommand();
        myCommand.Connection = DataBase.connection;
        myCommand.CommandText = "Select " + 
            "Movies.id As movie_id, Movies.name As movie_name, Movies.imageUrl As movie_imageUrl, " + 
            "rating, dubbed, soulFragment, authorId, " +
            "Authors.id As author_id, Authors.name As author_name, Authors.imageUrl As author_imageUrl " +
            "From Movies " +
            "Inner Join Authors " +
                "On authorId = Authors.id " +
            $"Where Movies.id = {movieId};";

        MovieDTO movie = null;
        using var myReader = await myCommand.ExecuteReaderAsync();

        while (myReader.Read()) {
            movie = new MovieDTO() {
                id = myReader.GetInt32("movie_id"),
                nome = myReader.GetString("movie_name"),
                urlImagem = myReader.IsDBNull("movie_imageUrl") ? null : myReader.GetString("movie_imageUrl"),
                nota = myReader.GetFloat("rating"),
                dublado = myReader.GetBoolean("dubbed"),
                fragmentoAlma = myReader.GetBoolean("soulFragment"),
                autor = new AuthorDTO() {
                    id = myReader.GetInt32("author_id"),
                    nome = myReader.GetString("author_name"),
                    urlImagem = myReader.IsDBNull("author_imageUrl") ? null : myReader.GetString("author_imageUrl")
                },
            };
        }

        await DataBase.CloseConnection();

        return movie;
    }

    public async Task UpdateMovieImageUrlById(int movieId, string? imageUrl) {
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
