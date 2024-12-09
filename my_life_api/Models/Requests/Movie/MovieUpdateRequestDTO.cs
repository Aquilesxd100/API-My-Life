namespace my_life_api.Models.Requests.Movie;

public class MovieUpdateRequestDTO {
    public int id { get; set; }

    public string? nome { get; set; }

    public IFormFile? imagem { get; set; }

    public float? nota { get; set; }

    public bool? dublado { get; set; }

    public bool? fragmentoAlma { get; set; }

    public IEnumerable<int>? idsCategorias { get; set; }

    public MovieUpdateRequestDTO BuildFromObj(dynamic dynamicObject) {
        id = dynamicObject.id;

        nome = dynamicObject.nome;
        nota = dynamicObject.nota;
        idsCategorias = dynamicObject.idsCategorias;
        fragmentoAlma = dynamicObject.fragmentoAlma;
        dublado = dynamicObject.dublado;
        imagem = dynamicObject.imagem;

        return this;
    }
}
