using my_life_api.Database.Managers;
using my_life_api.Resources;
using my_life_api.Shared.ContentResources;

namespace my_life_api.Services;

public class DeleteImgService {
    public async Task DeleteContentImg(ContentTypesEnum contentType, dynamic requestedItem) {
        int contentId = (int)requestedItem.id;
        string contentImageUrl = (string)requestedItem.urlImagem;

        switch (contentType) {
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
                await movieDbManager.UpdateMovieImageUrlById(contentId, null);

                string reqContentExt = contentImageUrl.Split(".")[^1];
                await FtpManager.DeleteFile(
                    $"movie-{contentId}.{reqContentExt}", 
                    FtpManager.moviePicturesFolder
                );
            break;
        }
    }
}
