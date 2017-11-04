using JsonApiSerializer.JsonApi;
using JsonApiSerializer.JsonApi.WellKnown;
using JsonApiSerializer.Util;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace JsonApiSerializer.JsonConverters
{
    internal class LinkConverter : JsonConverter
    {
        public static bool CanConvertStatic(Type objectType)
        {
            return typeof(ILink).GetTypeInfo().IsAssignableFrom(objectType.GetTypeInfo());
        }

        public override bool CanConvert(Type objectType)
        {
            return CanConvertStatic(objectType);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var link = (ILink)serializer.ContractResolver.ResolveContract(objectType).DefaultCreator();
            if (reader.TokenType == JsonToken.String)
            {
                link.Href = (string)reader.Value;
            }
            else
            {
                serializer.Populate(reader, link);
            }
            return link;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var contract = (JsonObjectContract)serializer.ContractResolver.ResolveContract(value.GetType());
                       
            //gather all the properties for the link
            var outputProperties = new List<KeyValuePair<string, object>>();
            foreach (var prop in contract.Properties.Where(x => !x.Ignored))
            {
                var propValue = prop.ValueProvider.GetValue(value);
                if (propValue == null && (prop.NullValueHandling ?? serializer.NullValueHandling) == NullValueHandling.Ignore)
                    continue;
                outputProperties.Add(new KeyValuePair<string, object>(prop.PropertyName, propValue));
              
            }

            //A link MUST be represented as either: a string, a link object
            if (outputProperties.Count == 1 && outputProperties[0].Key == PropertyNames.Href)
            {
                //if the link just has an href we will output it as a string
                writer.WriteValue(outputProperties[0].Value);
            }
            else
            {
                //if the link has other properties (i.e. meta) then we will output all the other properties
                writer.WriteStartObject();
                foreach (var outputProperty in outputProperties)
                {
                    writer.WritePropertyName(outputProperty.Key);
                    serializer.Serialize(writer, outputProperty.Value);
                }

                writer.WriteEndObject();
            }

           
        }
    }
}
