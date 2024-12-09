namespace my_life_api.Models;

public class RecordDTO {
    public int? id { get; set; }

    public DateTime? dataCriacao { get; set; }

    public string nome { get; set; }

    public string ano { get; set; }

    public string? conteudo { get; set; }

    public string? urlImagemPrincipal { get; set; }

    public IEnumerable<SecondaryImgDTO>? imagensSecundarias { get; set; }
}
