﻿using System.Data;
using MySql.Data.MySqlClient;
using my_life_api.Models.Requests;

namespace my_life_api.Database.Managers
{
    public class MovieDBManager
    {
        public async Task<MovieDTO?> GetMovieById(int movieId)
        {
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

            while (myReader.Read())
            {
                movie = new MovieDTO()
                {
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
