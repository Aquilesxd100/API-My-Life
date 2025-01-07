using FluentFTP;
using my_life_api.Shared.ContentResources;

namespace my_life_api.Resources;

public static class FtpManager {
    static private AsyncFtpClient client;
    static private string storageBaseUrl;

    public static readonly string recordPicturesFolder = "record_pictures";
    public static readonly string authorPicturesFolder = "author_pictures";

    public static async Task OpenConnectionIfClosed() {
        if (!client.IsConnected) {
            await client.AutoConnect();
        }
    }

    public static async Task CloseConnection() {
        await client.Disconnect();
    }

    public static void ConfigureFtpServer(
        string server, 
        string userName, 
        string password, 
        string _storageBaseUrl
    ) {
        client = new AsyncFtpClient(server, userName, password);
        storageBaseUrl = _storageBaseUrl;
    }

    public static async Task UploadFile(
        string fileName, 
        IFormFile file, 
        string uploadPath
    ) {
        var tempFilePath = Path.Combine(Path.GetTempPath(), fileName);

        // Salva o arquivo IFormFile em um local temporario
        using (var stream = new FileStream(tempFilePath, FileMode.Create)) {
            await file.CopyToAsync(stream);
        }

        // Upload do arquivo
        await client.UploadFile(tempFilePath, $"{uploadPath}/{fileName}");

        // Deleta o arquivo temporario
        if (File.Exists(tempFilePath)) {
            File.Delete(tempFilePath);
        }
    }

    public static async Task DeleteFile(string fileName, string uploadPath) {
        bool fileExists = await client.FileExists($"{uploadPath}/{fileName}");

        if (fileExists) {
            await client.DeleteFile($"{uploadPath}/{fileName}");
        }
    }

    public static async Task<string> UploadRecordMainPicture(
        int recordId, 
        IFormFile image
    ) {
        await OpenConnectionIfClosed();

        string fileName = GenerateImageName(recordId, image, "record-main-img-");

        string folderPath = $"{recordPicturesFolder}/{recordId}";
        await UploadFile(fileName, image, folderPath);

        await CloseConnection();

        string pictureUrl = $"{storageBaseUrl}/{recordPicturesFolder}/{recordId}/{fileName}";
        return pictureUrl;
    }

    public static async Task<string> UploadRecordSecondaryPicture(
        int recordId, 
        string pictureId, 
        IFormFile image
    ) {
        await OpenConnectionIfClosed();

        string fileName = GenerateImageName(null, image, $"secondary-img-{pictureId}");

        await UploadFile(fileName, image, $"{recordPicturesFolder}/{recordId}");

        await CloseConnection();

        string pictureUrl = $"{storageBaseUrl}/{recordPicturesFolder}/{recordId}/{fileName}";
        return pictureUrl;
    }

    public static async Task<string> UploadAuthorPicture(
        int authorId, 
        IFormFile image
    ) {
        await OpenConnectionIfClosed();

        string fileName = GenerateImageName(authorId, image, "author-");

        await UploadFile(fileName, image, $"{authorPicturesFolder}");

        await CloseConnection();

        string pictureUrl = $"{storageBaseUrl}/{authorPicturesFolder}/{fileName}";
        return pictureUrl;
    }

    public static async Task<string> UploadContentPicture(
        ContentTypeData contentTypeData,  
        int contentId,
        IFormFile image
    ) {
        string prefixFileName = $"{contentTypeData.name}-";
        string folderName = contentTypeData.storageFolder;

        await OpenConnectionIfClosed();

        string fileName = GenerateImageName(contentId, image, prefixFileName);

        await UploadFile(fileName, image, $"{folderName}");

        await CloseConnection();

        string pictureUrl = $"{storageBaseUrl}/{folderName}/{fileName}";
        return pictureUrl;
    }

    public static async Task CreateFolder(string path) {
        await OpenConnectionIfClosed();

        await client.CreateDirectory(path);

        await CloseConnection();
    }

    public static async Task DeleteFolder(string path) {
        await OpenConnectionIfClosed();

        await client.DeleteDirectory(path);

        await CloseConnection();
    }

    private static string GenerateImageName(
        int? id, 
        IFormFile img, 
        string prefix = null
    ) {
        string mimeType = img.ContentType;
        string imgExtension = "." + mimeType.Substring(mimeType.IndexOf("/") + 1);
        string fileName =  (prefix ?? "") + (id != null ? id : "") + imgExtension;

        return fileName;
    }

    public static string GetImageNameFromUrl(string url) {
        return url.Split('/')[^1];
    }
}
