using JsonApiSerializer.ContractResolvers;
using JsonApiSerializer.JsonApi.WellKnown;
using JsonApiSerializer.Util;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace JsonApiSerializer.JsonConverters
{
    internal class ResourceRelationshipConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            var typeInfo = objectType.GetTypeInfo();
            return TypeInfoShim.GetPropertyFromInhertianceChain(typeInfo, PropertyNames.Data) != null
                || TypeInfoShim.GetPropertyFromInhertianceChain(typeInfo, PropertyNames.Links) != null;
        }

        

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteStartObject();

            var valueType = value.GetType();
            var contractResolver = (JsonApiContractResolver)serializer.ContractResolver;
            var contract = (JsonObjectContract)contractResolver.ResolveContract(valueType);
            foreach (var prop in contract.Properties.Where(x => !x.Ignored))
            {
                var propValue = prop.ValueProvider.GetValue(value);
                if (propValue == null && (prop.NullValueHandling ?? serializer.NullValueHandling) == NullValueHandling.Ignore)
                    continue;
                var propType = propValue?.GetType() ?? prop.PropertyType;
                switch (prop.PropertyName)
                {
                    case PropertyNames.Data when ListUtil.IsList(propType, out var elementType):
                        writer.WritePropertyName(prop.PropertyName);
                        if (propValue == null)
                        {
                            //Resource linkage MUST be represented by an empty array ([]) for empty to-many relationships
                            writer.WriteStartArray();
                            writer.WriteEndArray();
                            break;
                        }
                        contractResolver.ResourceObjectListConverter.WriteJson(writer, propValue, serializer);
                        break;
                    case PropertyNames.Data:
                        writer.WritePropertyName(prop.PropertyName);
                        if(propValue == null)
                        {
                            writer.WriteNull();
                            break;
                        }

                        //because we are in a relationship we want to force this list to be treated as a resource object
                        contractResolver.ResourceObjectConverter.WriteJson(writer, propValue, serializer);
                        break;
                    default:
                        writer.WritePropertyName(prop.PropertyName);
                        serializer.Serialize(writer, propValue);
                        break;
                }
            }

            writer.WriteEndObject();
        }

        public override bool CanRead => false;

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotSupportedException();
        }
    }
}
