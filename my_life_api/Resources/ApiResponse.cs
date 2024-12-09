namespace my_life_api.Resources;

public static class ApiResponse {
    public static object CreateBody(
        int code, 
        string message, 
        object details = null
    ) {
        if (details == null) {
            return new {
                codigo = code,
                mensagem = message
            };
        }

        return new {
            codigo = code,
            mensagem = message,
            conteudo = details
        };
    }
}
