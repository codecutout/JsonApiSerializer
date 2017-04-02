using JsonApiSerializer.JsonApi;
using JsonApiSerializer.Util;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace JsonApiSerializer.JsonConverters
{
    internal class ResourceListWrapConverter : JsonConverter
    {
        public readonly JsonConverter ResourceObjectConverter;

        public ResourceListWrapConverter(JsonConverter resourceObjectConverter)
        {
            ResourceObjectConverter = resourceObjectConverter;
        }

        public override bool CanConvert(Type objectType)
        {
            Type elementType;
            return ListUtil.IsList(objectType, out elementType) && ResourceObjectConverter.CanConvert(elementType);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            object list;
            if (ResourceWrapConverter.TryResolveAsRoot(reader, objectType, serializer, out list))
                return list;

            using (ResourceWrapConverter.MoveToDataElement(reader))
            {
                //we should be dealing with list types, but we also want the element type
                Type elementType;
                if (!ListUtil.IsList(objectType, out elementType))
                    throw new ArgumentException($"{typeof(ResourceListWrapConverter)} can only read json lists", nameof(objectType));


                var itemsIterator = reader.IterateList().Select(x => serializer.Deserialize(reader, elementType));
                list = ListUtil.CreateList(objectType, itemsIterator);

                return list;
            }
           
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (ResourceWrapConverter.TryResolveAsRoot(writer, value, serializer))
                return;

            using (ResourceWrapConverter.MoveToDataElement(writer))
            {
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
}
