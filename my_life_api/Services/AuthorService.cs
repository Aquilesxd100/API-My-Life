using my_life_api.Database.Managers;
using my_life_api.Models;
using my_life_api.Models.Requests;
using my_life_api.Resources;

namespace my_life_api.Services
{
    public class AuthorService
    {
        public async Task<IEnumerable<AuthorDTO>> GetAuthorsByContentTypeId(ContentTypesEnum contentType)
        {
            AuthorDBManager authorDbManager = new AuthorDBManager();
            IEnumerable<AuthorDTO> authors = await authorDbManager.GetAuthorsByContentTypeId((int)contentType);

            return authors;
        }

        public async Task CreateAuthor(AuthorCreateRequestDTO authorReq)
        {
            AuthorDTO author = new AuthorDTO
            {
                idTipoConteudo = (ContentTypesEnum)authorReq.idTipoConteudo,
                nome = authorReq.nome,      
            };

            AuthorDBManager authorDbManager = new AuthorDBManager();

            int authorId = await authorDbManager.CreateAuthor(author);
            author.id = authorId;

            if (authorReq.imagem != null) {
                string imageUrl = await FtpManager.UploadAuthorPicture(authorId, authorReq.imagem);
                author.urlImagem = imageUrl;

                await authorDbManager.UpdateAuthor(author);
            }
        }

        public async Task UpdateAuthor(AuthorUpdateRequestDTO authorReq, AuthorDTO dbAuthor)
        {
            AuthorDTO author = new AuthorDTO
            {
                id = dbAuthor.id,
                nome = dbAuthor.nome,
                urlImagem = dbAuthor.urlImagem,
                idTipoConteudo = dbAuthor.idTipoConteudo
            };

            if (authorReq.imagem != null) {
                string imageUrl = await FtpManager.UploadAuthorPicture((int)authorReq.id, authorReq.imagem);
                author.urlImagem = imageUrl;
            }

            if (!string.IsNullOrEmpty(authorReq.nome)) {           
                author.nome = authorReq.nome;
            }

            AuthorDBManager authorDbManager = new AuthorDBManager();

            await authorDbManager.UpdateAuthor(author);
        }
    }
}
