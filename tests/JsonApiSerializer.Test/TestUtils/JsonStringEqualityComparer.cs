using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace JsonApiSerializer.Test.TestUtils
{
    class JsonStringEqualityComparer : IEqualityComparer<string>
    {
        public static JsonStringEqualityComparer Instance = new JsonStringEqualityComparer();

        public JTokenEqualityComparer jtokenComparer = new JTokenEqualityComparer();

        public bool Equals(string x, string y)
        {
            var jx = JToken.Parse(x);
            var jy = JToken.Parse(y);
            return jtokenComparer.Equals(jx, jy);

        }

        public int GetHashCode(string obj)
        {
            return jtokenComparer.GetHashCode(JToken.Parse(obj));
        }
    }
}
