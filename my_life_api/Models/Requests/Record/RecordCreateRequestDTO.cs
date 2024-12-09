namespace my_life_api.Models.Requests.Record
{
    public class RecordCreateRequestDTO
    {
        public string nome { get; set; }

        public string ano { get; set; }

        public string conteudo { get; set; }

        public IFormFile? imagemPrincipal { get; set; }

        public IEnumerable<IFormFile>? imagensSecundarias { get; set; }

        public RecordCreateRequestDTO BuildFromObj(dynamic dynamicObject)
        {
            nome = dynamicObject.nome;
            ano = dynamicObject.ano;
            conteudo = dynamicObject.conteudo;

            imagensSecundarias = dynamicObject.imagensSecundarias;
            imagemPrincipal = dynamicObject.imagemPrincipal;

            return this;
        }
    }
}
