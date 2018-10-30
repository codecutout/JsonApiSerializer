using System;
using System.Collections.Generic;
using System.Linq;
using JsonApiSerializer.JsonApi.WellKnown;
using JsonApiSerializer.SerializationState;
using JsonApiSerializer.Util;
using Newtonsoft.Json;

namespace JsonApiSerializer.JsonConverters
{
    internal class ErrorListConverter : JsonConverter
    {
        public static bool CanConvertStatic(Type objectType)
        {
            Type elementType;
            return ListUtil.IsList(objectType, out elementType) && ErrorConverter.CanConvertStatic(elementType);
        }

        public override bool CanConvert(Type objectType)
        {
            return CanConvertStatic(objectType);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            //we may be starting the deserialization here, if thats the case we need to resolve this object as the root
            var serializationData = SerializationData.GetSerializationData(reader);
            if (!serializationData.HasProcessedDocumentRoot)
            {
                var errors = DocumentRootConverter.ResolveAsRootError(reader, objectType, serializer);
                return ListUtil.CreateList(objectType, errors);
            }

            Type elementType;
            ListUtil.IsList(objectType, out elementType);

            return ListUtil.CreateList(objectType,
                ReaderUtil.IterateList(reader)
                    .Select(x => serializer.Deserialize(reader, elementType)));
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var serializationData = SerializationData.GetSerializationData(writer);
            if (!serializationData.HasProcessedDocumentRoot)
            {
                DocumentRootConverter.ResolveAsRootError(writer, value, serializer);
                return;
            }

            var enumerable = value as IEnumerable<object> ?? Enumerable.Empty<object>();
            writer.WriteStartArray();
            foreach (var valueElement in enumerable)
            {
                serializer.Serialize(writer, valueElement);
            }
            writer.WriteEndArray();
        }
    }
}