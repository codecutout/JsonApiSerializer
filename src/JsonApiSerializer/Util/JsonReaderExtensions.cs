using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonApiSerializer.Util
{
    internal static class JsonReaderExtensions
    {
        /// <summary>
        /// Iterates over the properties in the reader. Assumes reader is at the start of an object
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentException">When reader is not at the start of an object</exception>
        /// <exception cref="System.Exception"></exception>
        public static IEnumerable<string> IterateProperties(this JsonReader reader)
        {
            if (reader.TokenType != JsonToken.StartObject)
                throw new ArgumentException($"Expected {JsonToken.StartObject} in reader", nameof(reader));

            reader.Read();
            while (reader.TokenType != JsonToken.EndObject)
            {
                if (reader.TokenType != JsonToken.PropertyName)
                    throw new Exception($"Expected {JsonToken.PropertyName} in reader");
                var jsonPropName = reader.Value;
                yield return jsonPropName.ToString();

                if (jsonPropName == reader.Value)
                    reader.Skip();

                reader.Read();
            }
        }

        /// <summary>
        /// Iterates the elements in a list.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentException">When reader is not at the start of an array</exception>
        public static IEnumerable<object> IterateList(this JsonReader reader)
        {
         

            if (reader.TokenType == JsonToken.StartArray)
            {
                while (reader.Read())
                {
                    if (reader.TokenType == JsonToken.EndArray)
                        yield break;

                    yield return reader.Value;
                }
            }
            else if(reader.TokenType == JsonToken.None || reader.TokenType == JsonToken.Null || reader.TokenType == JsonToken.Undefined)
            {
                //we dont have a value so return empty array
                yield break;
            }
            else
            {
                //are a value, we will return the items as though its a list with just that item
                yield return reader.Value;
                yield break;
            }
        }

        public static void ReadUntil(this JsonReader reader, Func<JsonReader,bool> predicate)
        {
            while(!predicate(reader) && reader.Read())
            {

            }
        }
    }
}
