namespace my_life_api.Models;

public class InvalidFieldError {
    public string campo { get; set; }
    public string detalhes { get; set; }
    public InvalidFieldError(string _campo, string _detalhes) {
        campo = _campo;
        detalhes = _detalhes;
    }
}
