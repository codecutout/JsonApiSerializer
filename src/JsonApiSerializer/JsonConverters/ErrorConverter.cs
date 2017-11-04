using JsonApiSerializer.JsonApi;
using JsonApiSerializer.JsonApi.WellKnown;
using JsonApiSerializer.Util;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using JsonApiSerializer.Exceptions;

namespace JsonApiSerializer.JsonConverters
{
    internal class ErrorConverter : JsonConverter
    {
        public static bool CanConvertStatic(Type objectType)
        {
            return typeof(IError).GetTypeInfo().IsAssignableFrom(objectType.GetTypeInfo());
        }

        public override bool CanConvert(Type objectType)
        {
            return CanConvertStatic(objectType);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            //we may be starting the deserialization here, if thats the case we need to resolve this object as the root
            IEnumerable<IError> errors;
            if (DocumentRootConverter.TryResolveAsRootError(reader, objectType, serializer, out errors))
            {
                //not sure if this is the correct thing to do. We are deserializing a single
                //error but json:api always gives us a list of errors. We just return the first
                //error
                return errors.First();
            }

            var contract = (JsonObjectContract)serializer.ContractResolver.ResolveContract(objectType);
            var error = (IError)serializer.ContractResolver.ResolveContract(objectType).DefaultCreator();
            foreach (var propName in ReaderUtil.IterateProperties(reader))
            {
                ReaderUtil.TryPopulateProperty(serializer, error, contract.Properties.GetClosestMatchProperty(propName), reader);
            }
            return error;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (DocumentRootConverter.TryResolveAsRootError(writer, value, serializer))
            {
                return;
            }

            var contract = (JsonObjectContract)serializer.ContractResolver.ResolveContract(value.GetType());

            writer.WriteStartObject();
            foreach (var prop in contract.Properties.Where(x => !x.Ignored))
            {
                var propValue = prop.ValueProvider.GetValue(value);
                if (propValue == null && (prop.NullValueHandling ?? serializer.NullValueHandling) == NullValueHandling.Ignore)
                    continue;

                writer.WritePropertyName(prop.PropertyName);
                serializer.Serialize(writer, propValue);
            }
            writer.WriteEndObject();
        }
    }
}
