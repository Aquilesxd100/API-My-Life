namespace my_life_api.Models;

public class CategoryDTO {
    public int id { get; set; }

    public string nome { get; set; }

    public string? iconeBase64 { get; set; }

    public ContentTypesEnum idTipoConteudo { get; set; }
}
