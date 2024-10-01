namespace my_life_api.Models.Requests
{
    public class AuthorUpdateRequestDTO
    {
        public int id { get; set; }

        public string? nome { get; set; }

        public IFormFile? imagem { get; set; }

        public AuthorUpdateRequestDTO BuildFromObj(dynamic dynamicObject)
        {
            nome = dynamicObject.nome;

            imagem = dynamicObject.imagem;
            id = dynamicObject.id;

            return this;
        }
    }
}
