using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace JsonApiSerializer.Test.TestUtils
{
    class JsonStringEqualityComparer : IEqualityComparer<string>
    {
        public static JsonStringEqualityComparer Instance = new JsonStringEqualityComparer(false);
        public static JsonStringEqualityComparer InstanceIgnoreArrayOrder = new JsonStringEqualityComparer(true);

        public JTokenEqualityComparer jtokenComparer = new JTokenEqualityComparer();
        private readonly bool ignoreArrayOrder;

        public JsonStringEqualityComparer(bool ignoreArrayOrder)
        {
            this.ignoreArrayOrder = ignoreArrayOrder;
        }

        public bool Equals(string x, string y)
        {
            var jx = JToken.Parse(x);
            var jy = JToken.Parse(y);
            if (ignoreArrayOrder)
            {
                jx = Normalize(jx);
                jy = Normalize(jy);
            }
            return jtokenComparer.Equals(jx, jy);

        }

        public int GetHashCode(string obj)
        {
            return jtokenComparer.GetHashCode(JToken.Parse(obj));
        }

        public static JToken Normalize(JToken token)
        {
            var result = token;

            switch (token.Type)
            {
                case JTokenType.Object:
                    var jObject = (JObject)token;

                    if (jObject != null && jObject.HasValues)
                    {
                        var newObject = new JObject();

                        foreach (var property in jObject.Properties().OrderBy(x => x.Name).ToList())
                        {
                            var value = property.Value as JToken;
                            if (value != null)
                            {
                                value = Normalize(value);
                            }

                            newObject.Add(property.Name, value);
                        }
                        return newObject;
                    }

                    break;

                case JTokenType.Array:

                    var jArray = (JArray)token;

                    if (jArray != null && jArray.Count > 0)
                    {
                        var normalizedArrayItems = jArray
                            .Select(x => Normalize(x))
                            .OrderBy(x => x.ToString(), StringComparer.Ordinal);

                        result = new JArray(normalizedArrayItems);
                    }

                    break;
                default:
                    break;
            }

            return result;
        }
    }
}
