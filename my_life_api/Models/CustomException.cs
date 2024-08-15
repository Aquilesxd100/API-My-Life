namespace my_life_api.Models
{
    public class CustomException : Exception
    {
        public int StatusCode { get; set; }

        public CustomException(string Message, int errorStatusCode): base(Message)
        {
            StatusCode = errorStatusCode;
        }
    }
}
