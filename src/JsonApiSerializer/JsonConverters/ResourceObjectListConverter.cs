using JsonApiSerializer.JsonApi;
using JsonApiSerializer.JsonApi.WellKnown;
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
    internal class ResourceObjectListConverter : JsonConverter
    {
        private static readonly Regex DataPathRegex = new Regex($@"{PropertyNames.Data}$");

        public readonly JsonConverter ResourceObjectConverter;

        public ResourceObjectListConverter(JsonConverter resourceObjectConverter)
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
            if (DocumentRootConverter.TryResolveAsRootData(reader, objectType, serializer, out list))
                return list;

            //read into the 'Data' path
            var preDataPath = ReaderUtil.ReadUntilStart(reader, DataPathRegex);
            
            //we should be dealing with list types, but we also want the element type
            Type elementType;
            if (!ListUtil.IsList(objectType, out elementType))
                throw new ArgumentException($"{typeof(ResourceObjectListConverter)} can only read json lists", nameof(objectType));

            var itemsIterator = ReaderUtil.IterateList(reader).Select(x => serializer.Deserialize(reader, elementType));
            list = ListUtil.CreateList(objectType, itemsIterator);

            //read out of the 'Data' path
            ReaderUtil.ReadUntilEnd(reader, preDataPath);

            return list;
            
           
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (DocumentRootConverter.TryResolveAsRootData(writer, value, serializer))
                return;

            WriterUtil.WriteIntoElement(writer, DataPathRegex, PropertyNames.Data, () =>
            {
                var enumerable = value as IEnumerable<object> ?? Enumerable.Empty<object>();
                writer.WriteStartArray();
                foreach (var valueElement in enumerable)
                {
                    serializer.Serialize(writer, valueElement);
                }
                writer.WriteEndArray();
            });
        }
    }
}
