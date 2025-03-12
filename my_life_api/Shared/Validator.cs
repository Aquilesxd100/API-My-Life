using System.Collections.Immutable;
using System.Text.RegularExpressions;
using my_life_api.Models;
using my_life_api.Shared.ContentResources;

namespace my_life_api.Shared;

public static class Validator {
    public static readonly ImmutableArray<int> validContentTypesIds = ImmutableArray.ToImmutableArray(
        ContentUtils.contentTypesData.Select(ctd => 
            (int)ctd.contentType
        )
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

    public static bool IsRatingInvalid(float value) {
        return value < 0 || value > 10;
    }

    public static void ValidateText(
        string text, 
        bool isRequired = false,
        string customName = "nome",
        int customMaxLength = 50
    ) {
        if (isRequired) {
            if (string.IsNullOrEmpty(text) || text.Trim().Length == 0) {
                throw new CustomException(400, $"O(a) {customName} é obrigatório(a) e não pode ficar vazio(a).");
            }
        }

        if (!string.IsNullOrEmpty(text)) {
            if (text.Trim().Length == 0) {
                throw new CustomException(400, $"O(a) {customName} não pode ser vazio(a).");
            }

            if (text.Length > customMaxLength) {
                throw new CustomException(400, $"O(a) {customName} deve ter no máximo {customMaxLength} caracteres.");
            }

            if (Validator.HasInvalidCharacters(text)) {
                throw new CustomException(400, $"O(a) {customName} contém caracteres inválidos.");
            }
        }
    }

    public static void ValidateOptionalImgFile(IFormFile? file) {
        if (file != null) {
            if (Validator.IsInvalidImageFormat(file)) {
                throw new CustomException(
                    400,
                    "A imagem enviada está em formato incorreto, só são permitidas imagens png, jpg e jpeg."
                );
            }

            if (Validator.IsInvalidImageSize(file)) {
                throw new CustomException(
                    400, 
                    "A imagem enviada excede o tamanho máximo de 12mb."
                );
            }
        }
    }
}
