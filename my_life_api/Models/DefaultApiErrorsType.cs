using my_life_api.Configurations;

namespace my_life_api.Models
{
    public class DefaultApiErrorType
    {
        public DefaultApiErrorsEnum type { get; set; }
        public string message { get; set; }

        public DefaultApiErrorType(DefaultApiErrorsEnum _type, string _message) {
            type = _type;
            message = _message;
        }
    }
}
