using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
