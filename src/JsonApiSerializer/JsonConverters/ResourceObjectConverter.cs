using JsonApiSerializer.JsonApi.WellKnown;
using JsonApiSerializer.Util;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace JsonApiSerializer
{
    /// <summary>
    /// Provides functionality to convert a JsonApi resoruce object into a .NET object
    /// </summary>
    /// <seealso cref="Newtonsoft.Json.JsonConverter" />
    public class ResourceObjectConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return TypeInfoShim.GetProperty(objectType.GetTypeInfo(), "Id") != null;
            
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var contract = (JsonObjectContract)serializer.ContractResolver.ResolveContract(objectType);

            var obj = contract.DefaultCreator();

            foreach(var propName in reader.IterateProperties()){
                var successfullyPopulateProperty = serializer.PopulateProperty(reader, obj, contract) != null;

                //flatten out attributes onto the object
                if (!successfullyPopulateProperty && propName == "attributes")
                {
                    reader.Read();
                    foreach (var attributeProperties in reader.IterateProperties())
                    {
                        serializer.PopulateProperty(reader, obj, contract);
                    }
                }

                //flatten out relationships onto the object
                if (!successfullyPopulateProperty && propName == "relationships")
                {
                    reader.Read();
                    foreach (var attributeProperties in reader.IterateProperties())
                    {
                        serializer.PopulateProperty(reader, obj, contract);
                    }
                }
            }

            return obj;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var valueType = value.GetType();
            var contract = (JsonObjectContract)serializer.ContractResolver.ResolveContract(valueType);
            writer.WriteStartObject();

            //A resource object MUST contain at least the following top-level members: type
            writer.WritePropertyName("type");
            var typeProp = contract.Properties.GetClosestMatchProperty("type");
            serializer.Serialize(writer, typeProp?.ValueProvider?.GetValue(value) ?? valueType.Name.ToLowerInvariant());

            List<JsonWriterCapture> attributes = new List<JsonWriterCapture>();
            List<JsonWriterCapture> relationships = new List<JsonWriterCapture>();
            foreach (var prop in contract.Properties.Where(x=>x != typeProp && !x.Ignored))
            {
                var propValue = prop.ValueProvider.GetValue(value);
                if (propValue == null && serializer.NullValueHandling == NullValueHandling.Ignore)
                    continue;

                switch (prop.PropertyName)
                {
                    //In addition, a resource object MAY contain any of these top - level members: links, meta, attributes, relationships
                    case PropertyNames.Links:
                    case PropertyNames.Meta:
                    case PropertyNames.Id: //Id is optional on base objects, so we will put it with the other optional properties
                        writer.WritePropertyName(prop.PropertyName);
                        serializer.Serialize(writer, propValue);
                        break;
                    default:
                        //we do not know if it is an Attribute or a Relationship
                        //so we will send out a probe to determine which one it is
                        var probe = new AttributeOrRelationshipProbe();
                        probe.WritePropertyName(prop.PropertyName);
                        serializer.Serialize(probe, propValue);

                        (probe.PropertyType == AttributeOrRelationshipProbe.Type.Attribute
                            ? attributes
                            : relationships).Add(probe);
                        break;
                }
            }

            //output our attibutes in an attribute tag
            if (attributes.Count > 0)
            {
                writer.WritePropertyName(PropertyNames.Attributes);
                writer.WriteStartObject();
                foreach (var attribute in attributes)
                    attribute.ApplyCaptured(writer);
                writer.WriteEndObject();
            }

            //output our relationships in a relationship tag
            if (relationships.Count > 0)
            {
                writer.WritePropertyName(PropertyNames.Relationships);
                writer.WriteStartObject();
                foreach (var relationship in relationships)
                    relationship.ApplyCaptured(writer);
                writer.WriteEndObject();
            }

            writer.WriteEndObject();

        }
    }
}
