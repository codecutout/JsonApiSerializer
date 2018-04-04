using JsonApiSerializer.ContractResolvers;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Text.RegularExpressions;

namespace JsonApiSerializer.Util
{
    internal static class WriterUtil
    {
        internal static void WriteIntoElement(JsonWriter writer, Regex pathCondition, string element, Action action)
        {
            if (pathCondition.IsMatch(writer.Path))
            {
                action();
            }
            else
            {
                writer.WriteStartObject();
                writer.WritePropertyName(element);
                action();
                writer.WriteEndObject();
            }
        }

        internal static void SerializeValueWithMemberConvertor(JsonSerializer serializer, JsonWriter writer, JsonProperty property, object propValue)
        {
            if (property.MemberConverter != null && property.MemberConverter.CanWrite)
            {
                property.MemberConverter.WriteJson(writer, propValue, serializer);
                return;
            }

            serializer.Serialize(writer, propValue);
        }
    }   
}
