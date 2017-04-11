using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonApiSerializer.Util
{
    internal static class JsonSerializerExtensions
    {
        /// <summary>
        /// If a reader is at a property will populate the associated property in the object
        /// </summary>
        /// <param name="serializer"></param>
        /// <param name="reader"></param>
        /// <param name="obj"></param>
        /// <param name="contract"></param>
        /// <returns></returns>
        public static object PopulateProperty(this JsonSerializer serializer, JsonReader reader, object obj, JsonObjectContract contract)
        {
            if (reader.TokenType != JsonToken.PropertyName)
                throw new Exception($"Expected {JsonToken.PropertyName} in reader");

            var jsonPropString = reader.Value.ToString();
            var prop = contract.Properties.GetClosestMatchProperty(jsonPropString);
            if (prop == null)
                return null;

            reader.Read(); //move to property value

            var value = serializer.Deserialize(reader, prop.PropertyType);
            prop.ValueProvider.SetValue(obj, value);
            return value;
        }

        public static void Populate(this JsonSerializer serializer, JToken jtoken, object obj)
        {
            using (var sr = jtoken.CreateReader())
            {
                sr.Read();
                serializer.Populate(sr, obj);
            }
        }

        public static object Deserialize(this JsonSerializer serializer, JToken jtoken, Type type)
        {
            using (var sr = jtoken.CreateReader())
            {
                sr.Read();
                return serializer.Deserialize(sr, type);
            }
        }

        public static T Deserialize<T>(this JsonSerializer serializer, JToken jtoken)
        {
            using (var sr = jtoken.CreateReader())
            {
                sr.Read();
                return serializer.Deserialize<T>(sr);
            }
        }
    }
}
