namespace my_life_api.Models.Content.Entities;


// NECESSARIO CRIAR A ENTITY E O DTO DO CONTEUDO COM PROPRIEDADES/COLUNAS
// NA EXATA MESMA ORDEM,
// O 'AUTOR' DO DTO MARCA O FIM DOS DADOS DO DTO QUE SERAO PREENCHIDOS

public class MovieEntity {
    public string name { get; set; }
    public string imageUrl { get; set; }
    public float rating { get; set; }
    public byte dubbed { get; set; }
    public byte soulFragment { get; set; }
    public int id { get; set; }
    public int authorId { get; set; }
}