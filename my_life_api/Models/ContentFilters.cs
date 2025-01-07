namespace my_life_api.Models;

public class ContentFilters {
    // Filtros globais
    public string? search { get; set; }

    public bool? soulFragment { get; set; }

    public int? authorId { get; set; }

    public IEnumerable<int>? categoriesIds { get; set; }

    public float? ratingGreaterEqualTo { get; set; }

    public float? ratingLesserEqualTo { get; set; }

    // Filtros de séries, animes, mangás, jogos e livros
    public bool? finished { get; set; }

    // Filtros de filmes e séries
    public bool? dubbed { get; set; }

    public ContentFilters(
        string? search = null,
        bool? soulFragment = null,
        int? authorId = null,
        IEnumerable<string>? categoriesIds = null,
        string? ratingGreaterEqualTo = null,
        string? ratingLesserEqualTo = null,
        bool? finished = null,
        bool? dubbed = null
    ) { 
        this.search = search;
        this.soulFragment = soulFragment;
        this.authorId = authorId;
        this.categoriesIds = 
            categoriesIds != null 
                ? categoriesIds.Select(id => Int32.Parse(id)) 
                : null;
        this.ratingGreaterEqualTo = ConvertStringToFloat(ratingGreaterEqualTo);
        this.ratingLesserEqualTo = ConvertStringToFloat(ratingLesserEqualTo);
        this.finished = finished;
        this.dubbed = dubbed;
    }

    private float? ConvertStringToFloat (string? value) { 
        return value != null 
            ? float.Parse(value) 
            : null;
    }
}
