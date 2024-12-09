namespace my_life_api.Models;

public class CustomException : Exception {
    public int StatusCode { get; set; }

    public object Content { get; set; }

    public CustomException(
        int ErrorStatusCode, 
        string Message, 
        object ErrorContent = null
    ) : base(Message) {
        StatusCode = ErrorStatusCode;
        Content = ErrorContent;
    }
}
