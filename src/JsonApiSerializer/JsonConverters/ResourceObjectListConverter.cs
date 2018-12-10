using JsonApiSerializer.ContractResolvers;
using JsonApiSerializer.ContractResolvers.Contracts;
using JsonApiSerializer.Exceptions;
using JsonApiSerializer.JsonApi.WellKnown;
using JsonApiSerializer.SerializationState;
using JsonApiSerializer.Util;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace JsonApiSerializer.JsonConverters
{
    internal class ResourceObjectListConverter : JsonConverter
    {
        public readonly JsonConverter ResourceObjectConverter;

        public ResourceObjectListConverter(JsonConverter resourceObjectConverter)
        {
            ResourceObjectConverter = resourceObjectConverter;
        }

        public static bool CanConvertStatic(Type objectType, JsonConverter elementConvertor)
        {
            return ListUtil.IsList(objectType, out Type elementType) 
                && elementConvertor.CanConvert(elementType);
        }

        public override bool CanConvert(Type objectType)
        {
            return CanConvertStatic(objectType, ResourceObjectConverter);
        }
        
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var serializationData = SerializationData.GetSerializationData(reader);
            if (!serializationData.HasProcessedDocumentRoot)
                return DocumentRootConverter.ResolveAsRootData(reader, objectType, serializer);

            //we should be dealing with list types, but we also want the element type
            if (!ListUtil.IsList(objectType, out Type elementType))
                throw new ArgumentException($"{typeof(ResourceObjectListConverter)} can only read json lists", nameof(objectType));

            var itemsIterator = ReaderUtil.IterateList(reader).Select(x => serializer.Deserialize(reader, elementType));
            var list = ListUtil.CreateList(objectType, itemsIterator);

            return list;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var serializationData = SerializationData.GetSerializationData(writer);
            if (!serializationData.HasProcessedDocumentRoot)
            {
                //treat this value as a document root
                DocumentRootConverter.ResolveAsRootData(writer, value, serializer);
                return;
            }

            var contractResolver = serializer.ContractResolver;

            var enumerable = value as IEnumerable<object> ?? Enumerable.Empty<object>();
            writer.WriteStartArray();
            foreach (var valueElement in enumerable)
            {
                if (valueElement == null || !(contractResolver.ResolveContract(valueElement.GetType()) is ResourceObjectContract))
                    throw new JsonApiFormatException(writer.Path,
                        $"Expected to find to find resource objects within lists, but found '{valueElement}'",
                        "Resource identifier objects MUST contain 'id' members");
                serializer.Serialize(writer, valueElement);
            }
            writer.WriteEndArray();
        }
    }
}
