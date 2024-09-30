using my_life_api.Database;
using my_life_api.Database.Managers;
using my_life_api.Models;
using my_life_api.Models.Requests;
using my_life_api.Resources;

namespace my_life_api.Services
{
    public class DeleteImgService
    {
        public async Task DeleteResourceImg(ContentTypesEnum contentType, dynamic requestedItem)
        {
            int resourceId = (int)requestedItem.id;
            string resourceImageUrl = (string)requestedItem.urlImagem;

            switch (contentType)
            {
                case ContentTypesEnum.Animes:

                break;
                case ContentTypesEnum.Mangas:

                break;
                case ContentTypesEnum.Seriado:

                break;
                case ContentTypesEnum.Livros:

                break;
                case ContentTypesEnum.Jogos:

                break;
                case ContentTypesEnum.Cinema:
                    MovieDBManager movieDbManager = new MovieDBManager();
                    await movieDbManager.UpdateMovieImageUrlById(resourceId, null);

                    string reqResourceExt = resourceImageUrl.Split(".")[^1];
                    await FtpManager.DeleteFile(
                        $"movie-{resourceId}.{reqResourceExt}", 
                        FtpManager.moviePicturesFolder
                    );
                break;
            }
        }
    }
}
