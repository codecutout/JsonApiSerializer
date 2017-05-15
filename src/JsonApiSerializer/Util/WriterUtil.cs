using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

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
