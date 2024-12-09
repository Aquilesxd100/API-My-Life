namespace my_life_api.Models.Requests.Author;

public class AuthorCreateRequestDTO {
    public string nome { get; set; }

    public IFormFile? imagem { get; set; }

    public int idTipoConteudo { get; set; }

    public AuthorCreateRequestDTO BuildFromObj(dynamic dynamicObject) {
        nome = dynamicObject.nome;
        idTipoConteudo = dynamicObject.idTipoConteudo;

        imagem = dynamicObject.imagem;

        return this;
    }
}
