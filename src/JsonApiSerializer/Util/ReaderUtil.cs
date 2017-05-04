using JsonApiSerializer.JsonApi.WellKnown;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace JsonApiSerializer.Util
{
    internal static class ReaderUtil
    {
        public static ResourceObjectReference ReadAheadToIdentifyObject(ref JsonReader reader)
        {
            var lookAheadReader = ForkableJsonReader.LookAhead(ref reader);
            var properties = ReaderUtil.EnumerateProperties(lookAheadReader);
            var reference = new ResourceObjectReference();

            while (properties.MoveNext())
            {
                var propName = properties.Current;

                if (propName == PropertyNames.Id)
                    reference.Id = lookAheadReader.Value.ToString();
                else if (propName == PropertyNames.Type)
                    reference.Type = lookAheadReader.Value.ToString();

                if (reference.Id != null && reference.Type != null)
                    break;
            }

            return reference;
        }

        public static IEnumerator<string> EnumerateProperties(JsonReader reader)
        {
            return IterateProperties(reader).GetEnumerator();
        }

        /// <summary>
        /// Iterates over the properties in the reader. Assumes reader is at the start of an object
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentException">When reader is not at the start of an object</exception>
        /// <exception cref="System.Exception"></exception>
        public static IEnumerable<string> IterateProperties(JsonReader reader)
        {
            
            if (reader.TokenType == JsonToken.Null)
                yield break;
            if (reader.TokenType != JsonToken.StartObject)
                throw new ArgumentException($"Expected {JsonToken.StartObject} in reader", nameof(reader));

            reader.Read();
            while (reader.TokenType != JsonToken.EndObject)
            {
                if (reader.TokenType != JsonToken.PropertyName)
                    throw new Exception($"Expected {JsonToken.PropertyName} in reader");
                var jsonPropName = reader.Value;
                var startPath = reader.Path;
                reader.Read();
                yield return jsonPropName.ToString();

                if (startPath == reader.Path)
                    reader.Skip();

                reader.Read();
            }
        }

        public static bool TryPopulateProperty(JsonSerializer serializer, object obj, JsonProperty property, JsonReader value)
        {
            if (property == null)
            {
                return false;
            }

            var valueObj = serializer.Deserialize(value, property.PropertyType);
            property.ValueProvider.SetValue(obj, valueObj);
            return true;
        }

        /// <summary>
        /// Iterates the elements in a list.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentException">When reader is not at the start of an array</exception>
        public static IEnumerable<object> IterateList(JsonReader reader)
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
            else if (reader.TokenType == JsonToken.None || reader.TokenType == JsonToken.Null || reader.TokenType == JsonToken.Undefined)
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

        public static IDisposable ReadUntilPath(JsonReader reader, Regex pathRegex)
        {
            var startPath = reader.Path;

            do
            {
                switch (reader.TokenType)
                {
                    case JsonToken.StartObject:
                    case JsonToken.StartArray:
                    case JsonToken.Null:
                        if (pathRegex.IsMatch(reader.Path))
                            return new ReadUntilEnd(reader, startPath);
                        break;
                    case JsonToken.EndObject:
                    case JsonToken.EndArray:
                        if (reader.Path == startPath)
                            throw new Exception();
                        break;
                    default:
                        break;

                }
            } while (reader.Read());

            throw new Exception();
        }

        private struct ReadUntilEnd : IDisposable
        {
            public readonly JsonReader Reader;
            public readonly string Path;

            public ReadUntilEnd(JsonReader reader, string path)
            {
                this.Reader = reader;
                this.Path = path;
            }

            public void Dispose()
            {
                do
                {
                    if ((Reader.TokenType == JsonToken.EndObject || Reader.TokenType == JsonToken.EndArray) && Reader.Path == Path)
                        return;
                }
                while (Reader.Read());
            }
        }
    }
}
