using my_life_api.Models;
using my_life_api.Models.Requests;
using my_life_api.Resources;

namespace my_life_api.Services
{
    public class AuthorService
    {
        public async Task<IEnumerable<AuthorDTO>> GetAuthorsByContentTypeId(ContentTypesEnum contentType)
        {
            IEnumerable<AuthorDTO> authors = await DataBase.GetAuthorsByContentTypeId((int)contentType);

            return authors;
        }

        public async Task CreateAuthor(CreateAuthorRequestDTO authorReq)
        {
            AuthorDTO author = new AuthorDTO
            {
                idTipoConteudo = (ContentTypesEnum)authorReq.idTipoConteudo,
                nome = authorReq.nome,      
            };

            int authorId = await DataBase.CreateAuthor(author);
            author.id = authorId;

            if (authorReq.imagem != null) {
                string imageUrl = await FtpManager.UploadAuthorPicture(authorId, authorReq.imagem);
                author.urlImagem = imageUrl;

                await DataBase.UpdateAuthor(author);
            }
        }

        //public async Task UpdateAuthor(AuthorDTO author)
        //{
        //    await DataBase.UpdateAuthor(author);
        //}
    }
}
