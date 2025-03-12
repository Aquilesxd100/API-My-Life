using my_life_api.Database.Managers;
using my_life_api.Models;
using my_life_api.Models.Content;
using my_life_api.Resources;
using my_life_api.Shared.ContentResources;

namespace my_life_api.Services;

public class ContentService {
    private ContentDBManager dbManager = new ContentDBManager();

    public async Task<IEnumerable<object>> GetItems(
        ContentTypesEnum contentType,
        ContentFilters filters
    ) {
        ContentTypeData contentTypeData = ContentUtils.GetContentTypeData(contentType);

        IEnumerable<ContentDTO> items = await dbManager.GetItems(
            contentTypeData,
            filters
        );

        // Filtro fixo para idsCategoria
        if (filters.categoriesIds.Any()) {
            items = items.Where(i => 
                i.categorias.Any(itemCategory =>
                    filters.categoriesIds.Any(filterCategoryId => 
                        itemCategory.id == filterCategoryId
                    )
                )
            );
        }

        return items;
    }

    public async Task CreateItem<T>(
        ContentTypesEnum contentType,
        T item,
        IEnumerable<int> categoriesIds,
        IFormFile? itemImg = null
    ) {
        ContentTypeData contentTypeData = ContentUtils.GetContentTypeData(contentType);

        int itemId = await dbManager.CreateItem(
            item,
            contentTypeData.dbTableName
        );

        if (itemImg != null) {
            string imageUrl = await FtpManager.UploadContentPicture(
                contentTypeData,
                itemId, 
                itemImg
            );

            await dbManager.UpdateItemImageUrlByIdAndTableName(
                itemId, 
                contentTypeData.dbTableName,
                imageUrl
            );
        }

        if (categoriesIds.Any()) {
            ContentDBManager contentDbManager = new ContentDBManager();

            await contentDbManager.CreateItemCategoryRelations(
                contentTypeData,
                itemId, 
                categoriesIds
            );
        }
    }

    public async Task UpdateItem<T>(
        ContentTypesEnum contentType,
        int itemId,
        T item,
        IEnumerable<int> categoriesIds,
        IFormFile? itemImg = null
    ) {
        ContentTypeData contentTypeData = ContentUtils.GetContentTypeData(contentType);

        await dbManager.UpdateItem(
            itemId,
            item,
            contentTypeData.dbTableName
        );

        if (itemImg != null) {
            string imageUrl = await FtpManager.UploadContentPicture(
                contentTypeData,
                itemId, 
                itemImg
            );

            await dbManager.UpdateItemImageUrlByIdAndTableName(
                itemId, 
                contentTypeData.dbTableName,
                imageUrl
            );
        }

        ContentDBManager contentDbManager = new ContentDBManager();

        await contentDbManager.UpdateItemCategoryRelations(
            contentTypeData,
            itemId, 
            categoriesIds
        );
    }

    public async Task DeleteItemByTypeAndId( 
        ContentTypesEnum contentType,
        int contentId,
        string? urlImagem = null
    ) {
        ContentTypeData contentTypeData = ContentUtils.GetContentTypeData(contentType);

        await dbManager.DeleteItemCategoryRelations(contentTypeData, contentId);
        await dbManager.DeleteItemByIdAndTableName(contentId, contentTypeData.dbTableName);

        if (!string.IsNullOrEmpty(urlImagem)) {
            string imgName = FtpManager.GetImageNameFromUrl(urlImagem);

            await FtpManager.DeleteFile(imgName, contentTypeData.storageFolder);
        }
    }
}
