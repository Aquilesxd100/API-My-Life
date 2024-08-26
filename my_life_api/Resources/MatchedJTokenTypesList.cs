using System.Collections.Immutable;
using Newtonsoft.Json.Linq;

namespace my_life_api.Resources
{
    public class MatchedJTokenType
    {
        public JTokenType jTokenType { get; set; }
        public Type systemType { get; set; }
        public MatchedJTokenType(JTokenType _jTokenType, Type _systemType)
        {
            jTokenType = _jTokenType;
            systemType = _systemType;
        }
    }

    static public class MatchedJTokenTypesList
    {
        public static readonly ImmutableList<MatchedJTokenType> list = ImmutableList.Create(
            new MatchedJTokenType(JTokenType.Null, null),
            new MatchedJTokenType(JTokenType.Boolean, typeof(bool)),
            new MatchedJTokenType(JTokenType.Integer, typeof(int)),
            new MatchedJTokenType(JTokenType.String, typeof(string)),
            new MatchedJTokenType(JTokenType.Array, typeof(Array))
        );

        public static ImmutableList<MatchedJTokenType> Get()
        {
            return list;
        }
    }
}
