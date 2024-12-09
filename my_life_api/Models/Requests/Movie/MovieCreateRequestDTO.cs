namespace my_life_api.Models.Requests.Movie;

public class MovieCreateRequestDTO {
    public string nome { get; set; }

    public IFormFile? imagem { get; set; }

    public float nota { get; set; }

    public bool? dublado { get; set; }

    public bool? fragmentoAlma { get; set; }

    public int idAutor { get; set; }

    public IEnumerable<int>? idsCategorias { get; set; }

    public MovieCreateRequestDTO BuildFromObj(dynamic dynamicObject) {
        nome = dynamicObject.nome;
        nota = dynamicObject.nota;
        idAutor = dynamicObject.idAutor;

        idsCategorias = dynamicObject.idsCategorias;
        fragmentoAlma = dynamicObject.fragmentoAlma;
        dublado = dynamicObject.dublado;
        imagem = dynamicObject.imagem;

        return this;
    }
}
