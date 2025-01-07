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

    public static void ValidateName(string name, bool isRequired = false) {
        if (isRequired) {
            if (string.IsNullOrEmpty(name) || name.Trim().Length == 0) {
                throw new CustomException(400, "O nome é obrigatório e não pode ficar vazio.");
            }
        }

        if (!string.IsNullOrEmpty(name)) {
            if (name.Trim().Length == 0) {
                throw new CustomException(400, "O nome não pode ser vazio.");
            }

            if (name.Length > 50) {
                throw new CustomException(400, "O nome deve ter no máximo 50 caracteres.");
            }

            if (Validator.HasInvalidCharacters(name)) {
                throw new CustomException(400, "O nome contém caracteres inválidos.");
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
