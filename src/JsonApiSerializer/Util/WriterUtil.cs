using JsonApiSerializer.ContractResolvers;
using JsonApiSerializer.ContractResolvers.Contracts;
using JsonApiSerializer.Exceptions;
using JsonApiSerializer.JsonApi.WellKnown;
using JsonApiSerializer.JsonConverters;
using JsonApiSerializer.SerializationState;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace JsonApiSerializer.Util
{
    internal static class WriterUtil
    {
        private static readonly List<Type> AllowedIdTypes = new List<Type> {
            typeof(string),
            typeof(int),
            typeof(long),
            typeof(Guid),
            typeof(int?),
            typeof(long?),
            typeof(Guid?),
            typeof(uint),
            typeof(ulong),
            typeof(uint?),
            typeof(ulong?),
        };

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool ShouldWriteProperty<T>(object value, JsonProperty prop, JsonSerializer serializer, out T propValue)
        {
            if (prop == null)
            {
                propValue = default(T);
                return false;
            }
            propValue = (T)prop.ValueProvider.GetValue(value);
            var shouldSerialize = prop.ShouldSerialize == null || prop.ShouldSerialize(value) == true;
            return shouldSerialize && (propValue != null || (prop.NullValueHandling ?? serializer.NullValueHandling) == NullValueHandling.Include);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool TryConvertIdToString(object objId, out string stringId)
        {
            if (objId is string str)
            {
                stringId = str;
                return true;
            }
            else if (objId == null)
            {
                stringId = null;
                return false;
            }
            else if (AllowedIdTypes.Contains(objId.GetType())) {
                //we will allow some non-string properties if it is trival to convert to a string
                stringId = objId?.ToString();
                return true;
            }
            else
            {
                stringId = default(string);
                return false;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteResourceObjectId(JsonWriter writer, object id, out string writtenId)
        {
            if (!WriterUtil.TryConvertIdToString(id, out writtenId))
                throw new JsonApiFormatException(
                    writer.Path,
                    $"Expected Id property to be a string or primitive but found it to be '{id?.GetType()}'",
                    "The values of the id member MUST be a string");
            writer.WritePropertyName(PropertyNames.Id);
            writer.WriteValue(writtenId);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void WriteResourceObjectType(
            JsonWriter writer, 
            string type, 
            object resourceObject, 
            SerializationData serializationData, 
            JsonSerializer serializer,
            out string writtenType)
        {
            writtenType = type ?? WriterUtil.CalculateDefaultJsonApiType(resourceObject, serializationData, serializer);
            writer.WritePropertyName(PropertyNames.Type);
            writer.WriteValue(writtenType);
        }

        public static bool TryUseCustomConvertor(JsonWriter writer, object value, JsonSerializer serializer, JsonConverter excludeConverter)
        {
            // if they have custom convertors registered, we will respect them
            for (var index = 0; index < serializer.Converters.Count; index++)
            {
                var converter = serializer.Converters[index];

                if (converter != excludeConverter && converter.CanWrite && converter.CanConvert(value.GetType()))
                {
                    converter.WriteJson(writer, value, serializer);
                    return true;
                }
            }

            return false;
        }

        public static string CalculateDefaultJsonApiType(object obj, SerializationData serializationData, JsonSerializer serializer)
        {
            if (serializationData.ReferenceTypeNames.TryGetValue(obj, out string typeName))
                return typeName;

            var jsonApiType = CalculateDefaultJsonApiTypeFromObjectType(obj.GetType(), serializationData, serializer);
            serializationData.ReferenceTypeNames[obj] = jsonApiType;

            return jsonApiType;
        }

        private static string CalculateDefaultJsonApiTypeFromObjectType(Type objectType, SerializationData serializationData, JsonSerializer serializer)
        {
            // Hack: To keep backward compatability we are not sure what resouceObjectConverter to use
            // we need to check if either one was defined as a serializer, or if one was defined as
            // furher up the stack (i.e. a member converter)

            for (var i = 0; i < serializer.Converters.Count; i++)
            {
                var converter = serializer.Converters[i];
                if (converter is ResourceObjectConverter roc && converter.CanWrite && converter.CanConvert(objectType))
                {
                    return roc.GenerateDefaultTypeNameInternal(objectType);
                }
            }

            var contractResolver = (JsonApiContractResolver)serializer.ContractResolver;
            foreach (var converter in serializationData.ConverterStack)
            {
                if (converter == contractResolver.ResourceObjectConverter
                    && contractResolver.ResolveContract(objectType) is ResourceObjectContract defaultRoc)
                {
                    return defaultRoc.DefaultType;
                }
                else if (converter is ResourceObjectConverter roc)
                {
                    return roc.GenerateDefaultTypeNameInternal(objectType);
                }
            }

            return objectType.Name.ToLower();
        }
    }
}
