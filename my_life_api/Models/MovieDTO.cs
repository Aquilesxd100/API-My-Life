namespace my_life_api.Models
{
    public class MovieDTO
    {
        public int? id { get; set; }

        public string nome { get; set; }

        public string urlImagem { get; set; }

        public float nota { get; set; }

        public bool dublado { get; set; }

        public bool fragmentoAlma { get; set; }

        public AuthorDTO? autor { get; set; }

        public IEnumerable<CategoryDTO>? categorias { get; set; }
    }
}
