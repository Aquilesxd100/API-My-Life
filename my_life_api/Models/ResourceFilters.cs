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
        public bool? completed { get; set; }

        // Filtros de filmes e séries
        public bool? dubbed { get; set; }

        public string MountConditionalQueryPart(string? categoryRelationTableAlias) {
            string conditionalQueryPart = "";
            List<string> conditionals = new List<string>();

            if (authorId != null) 
                conditionals.Add($"authorId = {authorId}");

            if (
                categoriesIds != null
                && !string.IsNullOrEmpty(categoryRelationTableAlias)
            ) {
                string formatedCategoriesIds = String.Join(", ", categoriesIds);
                conditionals.Add($"{categoryRelationTableAlias}.categoryId In ({formatedCategoriesIds})");
            }

            if (soulFragment != null) {
                int soulFragmentInByte = soulFragment == true ? 1 : 0;
                conditionals.Add($"soulFragment = {soulFragmentInByte}");
            }

            if (dubbed != null) {
                int dubbedInByte = dubbed == true ? 1 : 0;
                conditionals.Add($"dubbed = {dubbedInByte}");
            }

            if (completed != null) {
                int completedInByte = completed == true ? 1 : 0;
                conditionals.Add($"completed = {completedInByte}");
            }

            if (ratingGreaterEqualTo != null) 
                conditionals.Add($"rating >= {ratingGreaterEqualTo}");

            if (ratingLesserEqualTo != null)
                conditionals.Add($"rating <= {ratingLesserEqualTo}");

            if (!string.IsNullOrEmpty(name))
                conditionals.Add($"name Like '{name}'");

            if (conditionals.Count > 0) {
                conditionalQueryPart = "Where " + String.Join(" And ", conditionals);
            }

            return conditionalQueryPart;
        }
    }
}
