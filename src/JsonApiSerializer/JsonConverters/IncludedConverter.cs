using JsonApiSerializer.Util;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

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
            var reference = ReaderUtil.ReadAheadToIdentifyObject(ref reader);
            var existingObject = serializer.ReferenceResolver.ResolveReference(null, reference.ToString());

            if(existingObject == null)
            {
                //we dont know what type this object should be so we will just save it as a JObject
                var unknownObject = serializer.Deserialize<JObject>(reader);
                serializer.ReferenceResolver.AddReference(null, reference.ToString(), unknownObject);
                return unknownObject;
            }
            else if(existingObject is JObject)
            {
                //we already have a resolved object that we dont know what hte type is, we will keep the first one
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
