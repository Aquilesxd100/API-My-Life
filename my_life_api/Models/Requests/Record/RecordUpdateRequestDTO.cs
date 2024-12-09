namespace my_life_api.Models.Requests.Record
{
    public class RecordUpdateRequestDTO
    {
        public int id { get; set; }

        public string nome { get; set; }

        public string ano { get; set; }

        public string conteudo { get; set; }

        public IFormFile? imagemPrincipal { get; set; }

        public RecordUpdateRequestDTO BuildFromObj(dynamic dynamicObject)
        {
            id = dynamicObject.id;
            nome = dynamicObject.nome;
            ano = dynamicObject.ano;
            conteudo = dynamicObject.conteudo;

            imagemPrincipal = dynamicObject.imagemPrincipal;

            return this;
        }
    }
}
