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
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace JsonApiSerializer.JsonConverters
{
    internal class ResourceWrapConverter : JsonConverter
    {
        public readonly JsonConverter ResourceObjectConverter;

        public ResourceWrapConverter(JsonConverter resourceObjectConverter)
        {
            ResourceObjectConverter = resourceObjectConverter;
        }

        public override bool CanConvert(Type objectType)
        {
            return ResourceObjectConverter.CanConvert(objectType);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            object obj;
            if (ResourceWrapConverter.TryResolveAsRoot(reader, objectType, serializer, out obj))
                return obj;
            var startPath = reader.Path;

            using (ResourceWrapConverter.MoveToDataElement(reader))
            {
                var jobj = serializer.Deserialize<JToken>(reader) as JObject;
                if (jobj == null)
                    return null;

                var key = IncludedReferenceResolver.GetReferenceValue(jobj);
                var resolved = serializer.ReferenceResolver.ResolveReference(null, key);

                //we may have already resolved this to a full object, if we have we dont need to do any more
                if (resolved != null && objectType.GetTypeInfo().IsAssignableFrom(resolved.GetType().GetTypeInfo()))
                    return resolved;

                //we may have this object as a JObject, if so we will use that as our data and resolve it fully
                if (resolved != null && resolved is JObject)
                    jobj = (JObject)resolved;

                //delegate the actual conversion of the object out
                var subreader = jobj.CreateReader();
                subreader.Read();
                obj = ResourceObjectConverter.ReadJson(subreader, objectType, existingValue, serializer);

                //add our deserialized object so we can reslove it again in the future
                serializer.ReferenceResolver.AddReference(null, key, obj);

                return obj;
            }
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (ResourceWrapConverter.TryResolveAsRoot(writer, value, serializer))
                return;

            using (MoveToDataElement(writer))
            {
                var probe = writer as AttributeOrRelationshipProbe;
                if (probe != null)
                {
                    //if someone is sending a probe its because we are in a relationship property.
                    //let the probe know we are in a relationship and write the reference element
                    probe.PropertyType = AttributeOrRelationshipProbe.Type.Relationship;
                    ResourceWrapConverter.WriteReference(writer, value, serializer);
                }
                else
                {
                    ResourceObjectConverter.WriteJson(writer, value, serializer);
                }
            }
        }

        internal static void WriteReference(JsonWriter writer, object value, JsonSerializer serializer, JsonObjectContract contract = null)
        {
            contract = contract ?? (JsonObjectContract)serializer.ContractResolver.ResolveContract(value.GetType());

            writer.WriteStartObject();

            //A “resource identifier object” MUST contain type and id members.
            writer.WritePropertyName(PropertyNames.Id);
            var idProp = contract.Properties.GetClosestMatchProperty(PropertyNames.Id);
            var idVal = idProp?.ValueProvider?.GetValue(value) ?? string.Empty;
            serializer.Serialize(writer, idVal);

            writer.WritePropertyName(PropertyNames.Type);
            var typeProp = contract.Properties.GetClosestMatchProperty(PropertyNames.Type);
            var typeVal = typeProp?.ValueProvider?.GetValue(value) ?? value.GetType().Name.ToLowerInvariant();
            serializer.Serialize(writer, typeVal);

            //we will only write the object to included if there are properties that have have data
            //that we cant include wihtin the reference
            var willWriteObjectToIncluded = contract.Properties.Any(prop =>
            {
                //ignore id, type, meta and ignored properties
                if (prop.PropertyName == PropertyNames.Id 
                    || prop.PropertyName == PropertyNames.Type
                    || prop.PropertyName == PropertyNames.Meta
                    || prop.Ignored)
                    return false;

                //ignore null properties
                var propValue = prop.ValueProvider.GetValue(value);
                if (propValue == null && serializer.NullValueHandling == NullValueHandling.Ignore)
                    return false;

                //we have another property with a value
                return true;
            });

            //typeically we would just write the meta in the included. But if we are not going to
            //have something in included we will write the meta inline here
            if (!willWriteObjectToIncluded)
            {
                var metaProp = contract.Properties.GetClosestMatchProperty(PropertyNames.Meta);
                var metaVal = metaProp?.ValueProvider?.GetValue(value);
                if(metaVal != null)
                {
                    writer.WritePropertyName(PropertyNames.Meta);
                    serializer.Serialize(writer, metaVal);
                }
            }
            

            writer.WriteEndObject();


            if (willWriteObjectToIncluded)
            {
                var reference = IncludedReferenceResolver.GetReferenceValue(idVal.ToString(), typeVal.ToString());
                serializer.ReferenceResolver.AddReference(null, reference, value);
            }
        }

        private static Regex _dataReadPath = new Regex($@"(^$)|(^\[\d\]$)|({PropertyNames.Data}(\[\d\])?$)");
        internal static IDisposable MoveToDataElement(JsonReader reader)
        {
            var startPath = reader.Path;
            reader.ReadUntil(r => _dataReadPath.IsMatch(r.Path) && r.TokenType != JsonToken.PropertyName);

            return new ActionDisposable(() => reader.ReadUntil(r => r.Path == startPath
                && (r.TokenType == JsonToken.EndObject || r.TokenType == JsonToken.EndArray)));
        }

        private static Regex _dataWritePath = new Regex($@"({PropertyNames.Included}(\[\d\])?$)|({PropertyNames.Data}(\[\d\])?$)");
        internal static IDisposable MoveToDataElement(JsonWriter writer)
        {
            if (!_dataWritePath.IsMatch(writer.Path))
            {
                writer.WriteStartObject();
                writer.WritePropertyName(PropertyNames.Data);
                return new ActionDisposable(writer.WriteEndObject);
            }
            return null;
        }

       

        internal static bool TryResolveAsRoot(JsonReader reader, Type objectType, JsonSerializer serializer, out object obj)
        {
            //if we already have a root object then we dont need to resolve the root object
            if (serializer.ReferenceResolver.ResolveReference(null, IncludedReferenceResolver.RootReference) != null)
            {
                obj = null;
                return false;
            }

            //we do not have a root object, so this is probably the entry point, so we will resolve
            //a document root and return the data object
            var documentRootType = typeof(MinimalDocumentRoot<>).MakeGenericType(objectType);
            var objContract = (JsonObjectContract)serializer.ContractResolver.ResolveContract(documentRootType);
            var dataProp = objContract.Properties.GetClosestMatchProperty("data");

            var root = serializer.Deserialize(reader, documentRootType);
            obj = dataProp.ValueProvider.GetValue(root);
            return true;
        }

        internal static bool TryResolveAsRoot(JsonWriter writer, object value, JsonSerializer serializer)
        {
            //if we already have a root object then we dont need to resolve the root object
            if (serializer.ReferenceResolver.ResolveReference(null, IncludedReferenceResolver.RootReference) != null)
            {
                return false;
            }

            //we do not have a root object, so this is probably the entry point, so we will resolve
            //it as a document root
            var documentRootType = typeof(MinimalDocumentRoot<>).MakeGenericType(value.GetType());
            var objContract = (JsonObjectContract)serializer.ContractResolver.ResolveContract(documentRootType);
            var rootObj = objContract.DefaultCreator();

            //set the data property to be our current object
            var dataProp = objContract.Properties.GetClosestMatchProperty("data");
            dataProp.ValueProvider.SetValue(rootObj, value);
           
            serializer.Serialize(writer, rootObj);
            return true;
        }

        private class MinimalDocumentRoot<T> : IDocumentRoot<T>
        {
            public T Data { get; set; }
        }

    }
}
