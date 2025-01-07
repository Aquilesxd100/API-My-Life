using my_life_api.Resources.ContentAttributes;

namespace my_life_api.Models.Content;

// Essas propriedades sao ordenadas automaticamente para serem lidas
// depois das da classe que a herda
public abstract class ContentDTO {
    [RequiredFieldOnUpdate()]
    [CustomField("id", typeof(int))]
    public int? id { get; set; }

    [RequiredFieldOnCreation()]
    [CustomField("idAutor")]
    public AuthorDTO? autor { get; set; }

    [CustomField("idsCategorias", typeof(IEnumerable<int>))]
    public IEnumerable<CategoryDTO>? categorias { get; set; }
}
