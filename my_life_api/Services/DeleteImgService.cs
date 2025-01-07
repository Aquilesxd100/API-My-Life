using my_life_api.Database.Managers;
using my_life_api.Resources;
using my_life_api.Shared.ContentResources;

namespace my_life_api.Services;

public class DeleteImgService {
    public async Task DeleteContentImg(
        ContentTypesEnum contentType, 
        dynamic requestedItem
    ) {
        ContentTypeData contentTypeData = ContentUtils.GetContentTypeData(contentType);

        int contentId = (int)requestedItem.id;
        string contentImageUrl = (string)requestedItem.urlImagem;

        ContentDBManager contentDbManager = new ContentDBManager();
        await contentDbManager.UpdateItemImageUrlByIdAndTableName(
            contentId, 
            contentTypeData.dbTableName,
            null
        );

        string reqContentExt = contentImageUrl.Split(".")[^1];
        await FtpManager.DeleteFile(
            $"{contentTypeData.prefixFileName}{contentId}.{reqContentExt}", 
            contentTypeData.storageFolder
        );
    }
}
