namespace my_life_api.Models.Requests.Category
{
    public class CategoryCreateRequestDTO
    {
        public string nome { get; set; }

        public string? iconeBase64 { get; set; }

        public int idTipoConteudo { get; set; }

        public CategoryCreateRequestDTO BuildFromObj(dynamic dynamicObject)
        {
            nome = dynamicObject.nome;
            idTipoConteudo = dynamicObject.idTipoConteudo;

            iconeBase64 = dynamicObject.iconeBase64;

            return this;
        }
    }
}
