using my_life_api.Database.Managers;
using my_life_api.Resources;
using my_life_api.Shared.ContentResources;

namespace my_life_api.Services;

public class ContentService {
    private ContentDBManager dbManager = new ContentDBManager();

    public async Task DeleteContentById( 
        ContentTypesEnum contentType,
        int contentId,
        string? urlImagem
    ) {
        await dbManager.DeleteContentCategoryRelations(contentType, contentId);
        await dbManager.DeleteContent(contentId, contentType);

        string imgFolder = 
            dbManager.GetContentNameByContentType(contentType).ToLower()
            + "_pictures";

        if (!string.IsNullOrEmpty(urlImagem)) {
            string imgName = FtpManager.GetImageNameFromUrl(urlImagem);

            await FtpManager.DeleteFile(imgName, imgFolder);
        }
    }
}
