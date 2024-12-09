namespace my_life_api.Models.Requests.Record;

public class RecordAddSecondaryImgDTO {
    public int idRegistro { get; set; }

    public IFormFile imagemSecundaria { get; set; }

    public RecordAddSecondaryImgDTO BuildFromObj(dynamic dynamicObject)
    {
        idRegistro = dynamicObject.idRegistro;
        imagemSecundaria = dynamicObject.imagemSecundaria;

        return this;
    }
}
