namespace my_life_api.Models
{
    public class ResourceFilters
    {
        // Filtros globais
        public bool? soulFragment { get; set; }

        public int? authorId { get; set; }

        public IEnumerable<int>? categoriesIds { get; set; }

        public float? ratingGreaterEqualTo { get; set; }

        public float? ratingLesserEqualTo { get; set; }

        // Filtros globais, com exceção de Frases
        public string? name { get; set; }

        // Filtros de séries, animes, mangás, jogos e livros
        public bool? finished { get; set; }

        // Filtros de filmes e séries
        public bool? dubbed { get; set; }
    }
}
