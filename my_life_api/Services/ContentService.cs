using my_life_api.Database.Managers;
using my_life_api.Models;
using my_life_api.Resources;

namespace my_life_api.Services
{
    public class ContentService
    {
        public async Task DeleteContentById( 
            ContentTypesEnum contentType,
            int contentId,
            string? urlImagem
        ) {
            ContentDBManager contentDbManager = new ContentDBManager();

            await contentDbManager.DeleteContentCategoryRelations(contentType, contentId);
            await contentDbManager.DeleteContent(contentId, contentType);

            string imgFolder = 
                contentDbManager.GetContentNameByContentType(contentType).ToLower()
                + "_pictures";

            if (!string.IsNullOrEmpty(urlImagem)) {
                string imgName = FtpManager.GetImageNameFromUrl(urlImagem);

                await FtpManager.DeleteFile(imgName, imgFolder);
            }
        }
    }
}
