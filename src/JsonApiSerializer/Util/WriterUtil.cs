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
        internal static IDisposable WritePath(JsonWriter writer, Regex pathCondition, string element)
        {
            if (!pathCondition.IsMatch(writer.Path))
            {
                writer.WriteStartObject();
                writer.WritePropertyName(element);
                return new ActionDisposable(writer.WriteEndObject);
            }
            return null;
        }
    }
}
