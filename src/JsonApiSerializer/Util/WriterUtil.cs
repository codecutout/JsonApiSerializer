using Newtonsoft.Json;
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
    }
}
