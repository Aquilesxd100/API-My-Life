namespace my_life_api.Models.Requests
{
    public class CreateAuthorRequestDTO
    {
        public int? id { get; set; }

        public string nome { get; set; }

        public IFormFile imagem { get; set; }

        public int idTipoConteudo { get; set; }

        public CreateAuthorRequestDTO BuildFromObj(dynamic dynamicObject)
        {
            nome = dynamicObject.nome;
            idTipoConteudo = dynamicObject.idTipoConteudo;

            id = dynamicObject.id;
            imagem = dynamicObject.imagem;

            return this;
        }
    }
}
