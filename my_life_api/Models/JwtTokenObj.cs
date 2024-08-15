using JWT;

namespace my_life_api.Models
{
    public class JwtTokenObj
    {
        public string _password;
        public double exp;

        public JwtTokenObj(string password)
        {
            _password = password;
            exp = GetExpirationDate();
        }

        static private double GetExpirationDate()
        {
            int daysForTokenToExpire = 5;

            IDateTimeProvider provider = new UtcDateTimeProvider();
            return UnixEpoch.GetSecondsSince(provider.GetNow().AddDays(daysForTokenToExpire));
        }
    }
}
