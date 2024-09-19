using FluentFTP;

namespace my_life_api.Resources
{
    public static class FtpManager
    {
        static private AsyncFtpClient client;
        static private string storageBaseUrl;

        static readonly string authorPicturesFolder = "author_pictures";

        public static async Task OpenConnectionIfClosed()
        {
            if (!client.IsConnected)
            {
                await client.AutoConnect();
            }
        }

        public static async Task CloseConnection()
        {
            await client.Disconnect();
        }

        public static void ConfigureFtpServer(string server, string userName, string password, string _storageBaseUrl)
        {
            client = new AsyncFtpClient(server, userName, password);
            storageBaseUrl = _storageBaseUrl;
        }

        public static async Task UploadFile(string fileName, IFormFile file, string uploadPath)
        {
            var tempFilePath = Path.Combine(Path.GetTempPath(), fileName);

            // Salvar o arquivo IFormFile em um local temporário
            using (var stream = new FileStream(tempFilePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // Upload do arquivo
            await client.UploadFile(tempFilePath, $"{uploadPath}/{fileName}");

            // Deleta o arquivo temporario
            if (File.Exists(tempFilePath)) {
                File.Delete(tempFilePath);
            }
        }

        public static async Task DeleteFile(string fileName, string uploadPath)
        {
            bool fileExists = await client.FileExists($"{uploadPath}/{fileName}");

            if (fileExists) {
                await client.DeleteFile($"{uploadPath}/{fileName}");
            }
        }

        public static async Task<string> UploadAuthorPicture(int authorId, IFormFile image)
        {
            await OpenConnectionIfClosed();

            string fileName = GetImageName(authorId, image, "author-");

            await UploadFile(fileName, image, $"{authorPicturesFolder}");

            await CloseConnection();

            string pictureUrl = $"{storageBaseUrl}/{authorPicturesFolder}/{fileName}";
            return pictureUrl;
        }

        private static string GetImageName(int id, IFormFile img, string prefix = null)
        {
            string mimeType = img.ContentType;
            string imgExtension = "." + mimeType.Substring(mimeType.IndexOf("/") + 1);
            string fileName =  (prefix ?? "") + id + imgExtension;

            return fileName;
        }
    }
}
