using JWT;
using JWT.Serializers;
using JWT.Algorithms;
using JWT.Exceptions;
using my_life_api.Models;


namespace my_life_api.Services
{
    public class AuthorizationService
    {
        public string Login(string password)
        {
            string appPassword = Environment.GetEnvironmentVariable("APP_PASSWORD");
            if (password != appPassword)
            {
                throw new CustomException("A senha enviada esta incorreta.", 400);
            }

            IJwtAlgorithm algorithm = new HMACSHA256Algorithm();
            IJsonSerializer serializer = new JsonNetSerializer();
            IBase64UrlEncoder urlEncoder = new JwtBase64UrlEncoder();
            IJwtEncoder encoder = new JwtEncoder(algorithm, serializer, urlEncoder);
            string secret = Environment.GetEnvironmentVariable("JWT_SECRET");

            JwtTokenObj tokenObj = new JwtTokenObj(password);
            string token = encoder.Encode(tokenObj, secret);
            return token;
        }

        public void ValidateToken(string token)
        {
            try
            {
                if (token == null) throw new CustomException("Nenhuma credencial foi fornecida, efetue o login.", 401);

                IJsonSerializer serializer = new JsonNetSerializer();
                IDateTimeProvider provider = new UtcDateTimeProvider();
                IJwtValidator validator = new JwtValidator(serializer, provider);
                IBase64UrlEncoder urlEncoder = new JwtBase64UrlEncoder();
                IJwtAlgorithm algorithm = new HMACSHA256Algorithm();
                IJwtDecoder decoder = new JwtDecoder(serializer, validator, urlEncoder, algorithm);
                string secret = Environment.GetEnvironmentVariable("JWT_SECRET");

                string json = decoder.Decode(token, secret);
                JwtTokenObj jwtTokenObj = serializer.Deserialize<JwtTokenObj>(json);

                string appPassword = Environment.GetEnvironmentVariable("APP_PASSWORD");
                if (jwtTokenObj.password != appPassword) {
                    throw new CustomException("A senha enviada esta incorreta.", 400);
                }

            }
            catch (TokenNotYetValidException)
            {
                throw new CustomException("As credenciais ainda não são validas.", 403);
            }
            catch (TokenExpiredException)
            {
                throw new CustomException("As credenciais expiraram, faça login novamente.", 403);
            }
            catch (SignatureVerificationException)
            {
                throw new CustomException("A assinatura da credencial é inválida.", 403);
            }
            catch (CustomException exception)
            {
                throw exception;
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                throw new CustomException("Um erro ocorreu ao validar suas credenciais, verifique-as e tente novamente.", 400);
            }
        }
    }
}
