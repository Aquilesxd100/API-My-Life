using Microsoft.AspNetCore.Mvc;

namespace my_life_api.Resources
{
    public class CustomControllerBase: ControllerBase
    {
        public T? GetNullIfValueNotInformed<T>(T receivedValue, string fieldName) where T : class
        {

            IFormCollection formData = HttpContext.Request.Form;
            string originalValue = formData[fieldName];

            if (originalValue == null) return null;

            // No caso de array, caso informado e o valor é string vazia, retorna array vazio
            return receivedValue;
        }
    }
}
