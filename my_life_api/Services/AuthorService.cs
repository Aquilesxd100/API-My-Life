using my_life_api.Models;
using my_life_api.Resources;

namespace my_life_api.Services
{
    public class AuthorService
    {
        public async Task CreateAuthor(AuthorDTO author)
        {
            await DataBase.CreateAuthor(author);
        }

        //public async Task UpdateAuthor(AuthorDTO author)
        //{
        //    await DataBase.UpdateAuthor(author);
        //}
    }
}
