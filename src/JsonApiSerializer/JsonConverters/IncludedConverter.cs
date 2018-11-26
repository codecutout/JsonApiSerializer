using JsonApiSerializer.SerializationState;
using JsonApiSerializer.Util;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace JsonApiSerializer.JsonConverters
{
    internal class IncludedConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(object);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var forkableReader = reader as ForkableJsonReader ?? new ForkableJsonReader(reader);
            var reference = ReaderUtil.ReadAheadToIdentifyObject(forkableReader);
            var serializationData = SerializationData.GetSerializationData(forkableReader);
            
            if (!serializationData.Included.TryGetValue(reference, out var existingObject))
            {
                //we dont know what type this object should be so we will just save it as a JObject
                var unknownObject = serializer.Deserialize<JObject>(forkableReader);
                serializationData.Included.Add(reference, unknownObject);
                return unknownObject;
            }
            else if(existingObject is JObject)
            {
                //we already have a resolved object that we dont know what the type is, we will keep the first one
                return existingObject;
            }
            else
            {
                //We have an existing object, its likely our included data has more detail than what
                //is currently on the object so we will pass the reader so it can be deserialized again
                var type = existingObject.GetType();
                var existingObjectContract = serializer.ContractResolver.ResolveContract(type);
                return existingObjectContract.Converter.ReadJson(reader, type, existingObject, serializer);
            }
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}
