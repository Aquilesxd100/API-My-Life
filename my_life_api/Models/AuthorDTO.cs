using my_life_api.Shared.ContentResources;

namespace my_life_api.Models;

public class AuthorDTO {
    public int? id { get; set; }

    public string nome { get; set; }

    public string urlImagem { get; set; }

    public ContentTypesEnum? idTipoConteudo { get; set; }
}
