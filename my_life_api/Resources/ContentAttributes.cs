using my_life_api.Shared.ContentResources;
using my_life_api.Shared;
using my_life_api.Models;

namespace my_life_api.Resources.ContentAttributes;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public class CustomField : Attribute {
    public string name { get; }
    public Type? type { get; }

    public CustomField(
        string name, 
        Type? type = null
    ) {
        this.name = name;
        this.type = type;
    }
}

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public class RequiredFieldOnCreation : Attribute {}

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public class RequiredFieldOnUpdate : Attribute {}

public enum ValidationTypeEnum {
    Text = 1,
    Rating = 2,
    ImgFile = 3,
    Phrase = 4
}

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public class CustomValidation : Attribute {
    public ValidationTypeEnum validationType;

    public CustomValidation(ValidationTypeEnum validationType) {
        this.validationType = validationType;
    }

    public async Task<InvalidFieldError?> Validate(
        ContentTypeData contentTypeData,
        string fieldName,
        IFormCollection formData
    ) {
        switch (this.validationType) {
            case ValidationTypeEnum.Text:
                Validator.ValidateText(formData[fieldName], true);
            break;
            case ValidationTypeEnum.Phrase:
                Validator.ValidateText(formData[fieldName], true, "frase", 255);
            break;
            case ValidationTypeEnum.Rating:
                float convertedRating = 0;

                try {
                    convertedRating = float.Parse(formData[fieldName]);
                } catch(Exception excep) { 
                    return new InvalidFieldError(
                        fieldName, 
                        "A nota informada é inválida."
                    );
                }

                if (Validator.IsRatingInvalid(convertedRating)) {
                    return new InvalidFieldError(
                        fieldName, 
                        "A nota informada é inválida, o valor deve ser entre 0 e 10."
                    );
                }
            break;
            case ValidationTypeEnum.ImgFile:
                IFormFile? receivedImg = formData.Files[fieldName];
                if (Validator.IsInvalidImageFormat(receivedImg)) {
                    return new InvalidFieldError(
                        fieldName,
                        "A imagem enviada está em formato incorreto, só são permitidas imagens png, jpg e jpeg."
                    );
                }

                if (Validator.IsInvalidImageSize(receivedImg)) {
                    return new InvalidFieldError(
                        fieldName, 
                        "A imagem enviada excede o tamanho máximo de 12mb."
                    );
                }            
            break;
        }

        return null;
    }
}
