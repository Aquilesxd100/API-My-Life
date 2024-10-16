using FluentFTP;
using my_life_api.Models;

namespace my_life_api.Resources
{
    public static class FtpManager
    {
        static private AsyncFtpClient client;
        static private string storageBaseUrl;

        public static readonly string authorPicturesFolder = "author_pictures";
        public static readonly string moviePicturesFolder = "movie_pictures";
        public static readonly string animePicturesFolder = "anime_pictures";
        public static readonly string mangaPicturesFolder = "manga_pictures";
        public static readonly string seriesPicturesFolder = "series_pictures";
        public static readonly string bookPicturesFolder = "book_pictures";
        public static readonly string gamePicturesFolder = "game_pictures";

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

            string fileName = GenerateImageName(authorId, image, "author-");

            await UploadFile(fileName, image, $"{authorPicturesFolder}");

            await CloseConnection();

            string pictureUrl = $"{storageBaseUrl}/{authorPicturesFolder}/{fileName}";
            return pictureUrl;
        }

        public static async Task<string> UploadResourcePicture(
            int resourceId,
            ContentTypesEnum contentType,  
            IFormFile image
        ) {
            string prefixFileName = "";
            string folderName = "";

            switch (contentType) {
                case ContentTypesEnum.Animes:
                    prefixFileName = "anime-";
                    folderName = animePicturesFolder;
                break;
                case ContentTypesEnum.Mangas:
                    prefixFileName = "manga-";
                    folderName = mangaPicturesFolder;
                break;
                case ContentTypesEnum.Seriado:
                    prefixFileName = "series-";
                    folderName = seriesPicturesFolder;
                break;
                case ContentTypesEnum.Livros:
                    prefixFileName = "book-";
                    folderName = bookPicturesFolder;
                break;
                case ContentTypesEnum.Jogos:
                    prefixFileName = "game-";
                    folderName = gamePicturesFolder;
                break;
                case ContentTypesEnum.Cinema:
                    prefixFileName = "movie-";
                    folderName = moviePicturesFolder;
                break;
            }

            await OpenConnectionIfClosed();

            string fileName = GenerateImageName(resourceId, image, prefixFileName);

            await UploadFile(fileName, image, $"{folderName}");

            await CloseConnection();

            string pictureUrl = $"{storageBaseUrl}/{folderName}/{fileName}";
            return pictureUrl;
        }

        private static string GenerateImageName(int id, IFormFile img, string prefix = null)
        {
            string mimeType = img.ContentType;
            string imgExtension = "." + mimeType.Substring(mimeType.IndexOf("/") + 1);
            string fileName =  (prefix ?? "") + id + imgExtension;

            return fileName;
        }

        public static string GetImageNameFromUrl(string url)
        {
            return url.Split('/')[^1];
        }
    }
}
