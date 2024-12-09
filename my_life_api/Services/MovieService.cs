using my_life_api.Database.Managers;
using my_life_api.Models;
using my_life_api.Models.Requests.Movie;
using my_life_api.Resources;

namespace my_life_api.Services;

public class MovieService {
    private MovieDBManager dbManager = new MovieDBManager();

    public async Task<IEnumerable<MovieDTO>> GetMovies(ContentFilters filters) {
        IEnumerable<MovieDTO> movies = await dbManager.GetMovies(filters);

        return movies;
    }

    public async Task CreateMovie(MovieCreateRequestDTO movieReq) {
        MovieDTO movie = new MovieDTO {
            nome = movieReq.nome,
            dublado = movieReq.dublado ?? false,
            fragmentoAlma = movieReq.fragmentoAlma ?? false,
            autor = new AuthorDTO() { id = movieReq.idAutor },
            nota = movieReq.nota
        };

        int movieId = await dbManager.CreateMovie(movie);
        movie.id = movieId;

        if (movieReq.imagem != null) {
            string imageUrl = await FtpManager.UploadContentPicture(
                movieId, 
                ContentTypesEnum.Cinema, 
                movieReq.imagem
            );

            await dbManager.UpdateMovieImageUrlById((int)movie.id, imageUrl);
        }

        if (movieReq.idsCategorias.Count() > 0) {
            ContentDBManager contentDbManager = new ContentDBManager();

            await contentDbManager.CreateContentCategoryRelations(
                ContentTypesEnum.Cinema,
                (int)movie.id, 
                movieReq.idsCategorias
            );
        }
    }

    public async Task UpdateMovie(
        MovieUpdateRequestDTO movieReq, 
        MovieDTO dbMovie
    ) {
        MovieDTO movie = new MovieDTO {
            id = dbMovie.id,
            nome = dbMovie.nome,
            urlImagem = dbMovie.urlImagem,
            categorias = dbMovie.categorias,
            dublado = movieReq.dublado ?? dbMovie.dublado,
            fragmentoAlma = movieReq.fragmentoAlma ?? dbMovie.fragmentoAlma,
            nota = movieReq.nota ?? dbMovie.nota,
            autor = dbMovie.autor
        };

        if (movieReq.imagem != null) {
            string imageUrl = await FtpManager.UploadContentPicture(
                (int)movieReq.id, 
                ContentTypesEnum.Cinema,
                movieReq.imagem
            );
            movie.urlImagem = imageUrl;
        }

        if (!string.IsNullOrEmpty(movieReq.nome)) {
            movie.nome = movieReq.nome;
        }

        await dbManager.UpdateMovie(movie);

        if (movieReq.idsCategorias != null) {
            ContentDBManager contentDbManager = new ContentDBManager();

            await contentDbManager.UpdateContentCategoryRelations(
                ContentTypesEnum.Cinema,
                movieReq.id,
                movieReq.idsCategorias
            );
        }
    }
}
