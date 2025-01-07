using my_life_api.Resources.ContentAttributes;

namespace my_life_api.Models.Content;

public class MovieDTO : ContentDTO {
    [RequiredFieldOnCreation()]
    [CustomValidation(ValidationTypeEnum.Text)]
    public string nome { get; set; }

    [CustomField("imagem", typeof(IFormFile))]
    [CustomValidation(ValidationTypeEnum.ImgFile)]
    public string urlImagem { get; set; }

    [RequiredFieldOnCreation()]
    [CustomValidation(ValidationTypeEnum.Rating)]
    public float nota { get; set; }

    public bool dublado { get; set; }

    public bool fragmentoAlma { get; set; }
}
