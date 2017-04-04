using JsonApiSerializer.JsonApi;
using JsonApiSerializer.JsonApi.WellKnown;
using JsonApiSerializer.ReferenceResolvers;
using JsonApiSerializer.Util;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace JsonApiSerializer.JsonConverters
{
    internal class DocumentRootConverter : JsonConverter
    {
        public static bool CanConvertStatic(Type objectType)
        {
            return TypeInfoShim.GetInterfaces(objectType.GetTypeInfo())
                .Select(x=>x.GetTypeInfo())
                .Any(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IDocumentRoot<>));
        }

        public override bool CanConvert(Type objectType)
        {
            return CanConvertStatic(objectType);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var contract = (JsonObjectContract)serializer.ContractResolver.ResolveContract(objectType);
            var rootObject = contract.DefaultCreator();
            serializer.ReferenceResolver.AddReference(null, IncludedReferenceResolver.RootReference, rootObject);

            JToken dataJObj = null;
            foreach (var prop in reader.IterateProperties())
            {
                switch (prop)
                {
                    case PropertyNames.Data:
                        //we cant process the data object until all the included is done, so we will
                        //store it in a JObject to process later
                        reader.Read();
                        dataJObj = serializer.Deserialize<JToken>(reader);
                        break;
                    case PropertyNames.Included:
                        var includedList = serializer.PopulateProperty(reader, rootObject, contract);
                        if(includedList == null)
                        {
                            reader.Read();
                            includedList = serializer.Deserialize<IEnumerable<JObject>>(reader);
                        }

                        foreach (JObject include in includedList as IEnumerable<JObject>  ?? Enumerable.Empty<JObject>())
                        {
                            serializer.ReferenceResolver.AddReference(null, IncludedReferenceResolver.GetReferenceValue(include), include);
                        }
                        break;
                    default:
                        serializer.PopulateProperty(reader, rootObject, contract);
                        break;
                }
            }

            //now the included are processed we can process the data
            if(dataJObj != null)
            {
                var documentRootInterfaceType = TypeInfoShim.GetInterfaces(objectType.GetTypeInfo())
                    .Select(x=>x.GetTypeInfo())
                    .FirstOrDefault(x =>
                        x.IsGenericType
                        && x.GetGenericTypeDefinition() == typeof(IDocumentRoot<>));

                var dataType = documentRootInterfaceType.GenericTypeArguments[0];
                var data = serializer.Deserialize(dataJObj, dataType);
                contract.Properties.GetClosestMatchProperty(PropertyNames.Data).ValueProvider.SetValue(rootObject, data);
            }

            return rootObject;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            serializer.ReferenceResolver.AddReference(null, IncludedReferenceResolver.RootReference, value);

            var contract = (JsonObjectContract)serializer.ContractResolver.ResolveContract(value.GetType());
            
            writer.WriteStartObject();

            var propertiesOutput = new HashSet<string>();
            foreach(var prop in contract.Properties)
            {
                //we will do includes last, so we we can ensure all the references have been added
                if (prop.PropertyName == PropertyNames.Included)
                    continue;

                //respect the serializers null handling value
                var propValue = prop.ValueProvider.GetValue(value);
                if (propValue == null && serializer.NullValueHandling == NullValueHandling.Ignore)
                    continue;

                //A document MAY contain any of these top-level members: jsonapi, links, included
                //We are also allowing everything else they happen to have on the root document
                writer.WritePropertyName(prop.PropertyName);
                serializer.Serialize(writer, propValue);
                propertiesOutput.Add(prop.PropertyName);
            }


            //A document MUST contain one of the following (data, errors, meta)
            //so if we do not have one of them we will output a null data
            if(!propertiesOutput.Contains(PropertyNames.Data)
                && !propertiesOutput.Contains(PropertyNames.Errors) 
                && !propertiesOutput.Contains(PropertyNames.Meta))
            {
                propertiesOutput.Add(PropertyNames.Data);
                writer.WritePropertyName(PropertyNames.Data);
                writer.WriteNull();
            }

            //If a document does not contain a top-level data key, the included member MUST NOT be present
            if (propertiesOutput.Contains(PropertyNames.Data))
            {
                //output the included. If we have a specified included field we will out everything in there
                //and we will also output all the references defined in our reference resolver
                var includedReferences = serializer.ReferenceResolver as IDictionary<string, object> ?? Enumerable.Empty<KeyValuePair<string, object>>();
                includedReferences = includedReferences.Where(x => x.Key != IncludedReferenceResolver.RootReference);
                var includedProperty = contract.Properties.GetClosestMatchProperty(PropertyNames.Included);
                var includedValues = includedProperty?.ValueProvider?.GetValue(value) as IEnumerable<object> ?? Enumerable.Empty<object>();

                //if we have some references we will output them
                if (includedReferences.Any() || includedReferences.Any())
                {
                    writer.WritePropertyName(PropertyNames.Included);
                    writer.WriteStartArray();

                    foreach (var includedValue in includedValues)
                        serializer.Serialize(writer, includedValue);

                    //I know we can alter the OrderedDictionary while enumerating it, otherwise this would error
                    foreach (var includedReference in includedReferences)
                        serializer.Serialize(writer, includedReference.Value);

                    writer.WriteEndArray();
                }
            }

            writer.WriteEndObject();
        }
    }
}
