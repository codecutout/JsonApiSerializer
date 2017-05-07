using JsonApiSerializer.Exceptions;
using JsonApiSerializer.JsonApi.WellKnown;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace JsonApiSerializer.Util
{
    internal static class ReaderUtil
    {
        public static ResourceObjectReference ReadAheadToIdentifyObject(ForkableJsonReader reader)
        {
            var lookAheadReader = reader.Fork();
            var reference = new ResourceObjectReference();

            foreach (var propName in ReaderUtil.IterateProperties(lookAheadReader))
            {
                if (propName == PropertyNames.Id)
                {
                    if (lookAheadReader.TokenType != JsonToken.String)
                        throw new JsonApiFormatException(lookAheadReader.FullPath,
                            $"Expected to find string at {lookAheadReader.FullPath}",
                            "The value of 'id' MUST be a string");
                    reference.Id = (string)lookAheadReader.Value;
                }
                    
                else if (propName == PropertyNames.Type)
                {
                    if (lookAheadReader.TokenType != JsonToken.String)
                        throw new JsonApiFormatException(lookAheadReader.FullPath,
                            $"Expected to find string at {lookAheadReader.FullPath}",
                            "The value of 'type' MUST be a string");
                    reference.Type = (string)lookAheadReader.Value;
                }
                    

                if (reference.Id != null && reference.Type != null)
                    break;
            }

            return reference;
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
            {
                //check if we are an object that the json:api spec says is mandatory to be an object
                var propName = reader.Path.Split('.').LastOrDefault();
                var specInfo = new[] { "relationships", "attributes" }.Contains(propName)
                    ? $"The value of the '{propName}' key MUST be an object"
                    : null;
                throw new JsonApiFormatException(reader.Path, 
                    $"Expected to find json object at {reader.Path}", 
                    specInfo);
            }
                

            reader.Read();
            while (reader.TokenType != JsonToken.EndObject)
            {
                //this error only gets thrown if a caller puts the JsonReader in an odd state
                if (reader.TokenType != JsonToken.PropertyName)
                    throw new JsonApiFormatException(reader.Path, $"Expected {JsonToken.PropertyName} in reader");
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

        public static TResult ReadInto<TReader, TResult>(TReader reader, Regex pathRegex, Func<TReader, TResult> action) where TReader : JsonReader
        {
            var startPath = ReadUntilStart(reader, pathRegex);
            var result = action(reader);
            ReadUntilEnd(reader, startPath);
            return result;
        }

        public static string ReadUntilStart(JsonReader reader, Regex pathRegex)
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
                            return startPath;
                        break;
                    case JsonToken.EndObject:
                    case JsonToken.EndArray:
                        if (reader.Path == startPath)
                            throw new JsonApiFormatException(startPath, $"Expected to find nested object within {startPath} that matches \\{pathRegex}\\");
                        break;
                    default:
                        break;

                }
            } while (reader.Read());

            throw new JsonApiFormatException(startPath, $"Expected to find nested object matching path \\{pathRegex}\\");
        }

        public static void ReadUntilEnd(JsonReader reader, string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                return;

            do
            {
                switch (reader.TokenType)
                {
                    case JsonToken.EndObject:
                    case JsonToken.EndArray:
                        if (reader.Path == path)
                            return;
                        break;
                    default:
                        break;

                }
            } while (reader.Read());

            throw new JsonApiFormatException(path, $"Unable to find closing element element");
        }
    }
}
