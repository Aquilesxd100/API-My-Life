using my_life_api.Models;
using System.Collections.Immutable;
using System.Text.RegularExpressions;

namespace my_life_api.Shared
{
    public static class Validator
    {
        public static readonly ImmutableArray<int> validContentTypesIds = ImmutableArray.Create(
            (int)ContentTypesEnum.Musical,
            (int)ContentTypesEnum.Mangas,
            (int)ContentTypesEnum.Animes,
            (int)ContentTypesEnum.Seriado,
            (int)ContentTypesEnum.Livros,
            (int)ContentTypesEnum.Frases,
            (int)ContentTypesEnum.Jogos,
            (int)ContentTypesEnum.Cinema
        );

        public static readonly ImmutableArray<string> validImgTypes = ImmutableArray.Create(
            "image/jpeg", 
            "image/png", 
            "image/jpg" 
        );

        public static bool IsContentTypeIdInvalid(int? value) {
            return !validContentTypesIds.Contains(value ?? 0);
        }

        public static bool IsInvalidImageFormat(IFormFile file) {
            string mimeType = file.ContentType;

            if (string.IsNullOrEmpty(mimeType)) return true;

            if (!validImgTypes.Contains(mimeType)) return true;

            return false;
        }

        public static bool IsInvalidImageSize(IFormFile file) {
            long imageSizeInMegabytes = file.Length / 1000000;
            short maxImageSizeInMegabytes = 12;

            if (imageSizeInMegabytes > maxImageSizeInMegabytes) return true;

            return false;
        }

        public static bool HasInvalidCharacters(string value) {
            var regex = @"^[^<>""'{}$]*$";
            Match match = Regex.Match(value, regex, RegexOptions.IgnoreCase);

            return !match.Success;
        }
    }
}
