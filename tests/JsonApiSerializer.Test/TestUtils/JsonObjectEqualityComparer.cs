using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonApiSerializer.Test.TestUtils
{
    class JsonObjectEqualityComparer<T> : IEqualityComparer<T>
    {
        public static JsonObjectEqualityComparer<T> Instance = new JsonObjectEqualityComparer<T>();

        public bool Equals(T x, T y)
        {
            return JsonStringEqualityComparer.Instance.Equals(JsonConvert.SerializeObject(x), JsonConvert.SerializeObject(y));
        }

        public int GetHashCode(T obj)
        {
            return JsonStringEqualityComparer.Instance.GetHashCode(JsonConvert.SerializeObject(obj));
        }
    }
}
