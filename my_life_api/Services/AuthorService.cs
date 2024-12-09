using my_life_api.Database.Managers;
using my_life_api.Models;
using my_life_api.Models.Requests.Author;
using my_life_api.Resources;

namespace my_life_api.Services;

public class AuthorService {
    private AuthorDBManager dbManager = new AuthorDBManager();
    public async Task<IEnumerable<AuthorDTO>> GetAuthorsByContentTypeId(
        ContentTypesEnum contentType
    ) {
        AuthorDBManager authorDbManager = new AuthorDBManager();
        IEnumerable<AuthorDTO> authors = await authorDbManager.GetAuthorsByContentTypeId(
            (int)contentType
        );

        return authors;
    }

    public async Task CreateAuthor(AuthorCreateRequestDTO authorReq) {
        AuthorDTO author = new AuthorDTO {
            idTipoConteudo = (ContentTypesEnum)authorReq.idTipoConteudo,
            nome = authorReq.nome,      
        };

        int authorId = await dbManager.CreateAuthor(author);
        author.id = authorId;

        if (authorReq.imagem != null) {
            string imageUrl = await FtpManager.UploadAuthorPicture(authorId, authorReq.imagem);
            author.urlImagem = imageUrl;

            await dbManager.UpdateAuthor(author);
        }
    }

    public async Task UpdateAuthor(AuthorUpdateRequestDTO authorReq, AuthorDTO dbAuthor) {
        AuthorDTO author = new AuthorDTO {
            id = dbAuthor.id,
            nome = dbAuthor.nome,
            urlImagem = dbAuthor.urlImagem
        };

        if (authorReq.imagem != null) {
            string imageUrl = await FtpManager.UploadAuthorPicture(
                (int)authorReq.id, 
                authorReq.imagem
            );
            author.urlImagem = imageUrl;
        }

        if (!string.IsNullOrEmpty(authorReq.nome)) {           
            author.nome = authorReq.nome;
        }

        await dbManager.UpdateAuthor(author);
    }

    public async Task DeleteAuthorById(int idAuthor) {
        await dbManager.DeleteAuthorById(idAuthor);
    }

    public async Task DeleteAuthorImg(AuthorDTO dbAuthor) {
        await FtpManager.DeleteFile(
            FtpManager.GetImageNameFromUrl(dbAuthor.urlImagem), 
            FtpManager.authorPicturesFolder
        );

        dbAuthor.urlImagem = null;
        await dbManager.UpdateAuthor(dbAuthor);
    }
}
