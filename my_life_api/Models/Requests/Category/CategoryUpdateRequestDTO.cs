namespace my_life_api.Models.Requests.Category
{
    public class CategoryUpdateRequestDTO
    {
        public int id { get; set; }

        public string? nome { get; set; }

        public string? iconeBase64 { get; set; }

        public CategoryUpdateRequestDTO BuildFromObj(dynamic dynamicObject)
        {
            id = dynamicObject.id;

            nome = dynamicObject.nome;
            iconeBase64 = dynamicObject.iconeBase64;

            return this;
        }
    }
}
