using JsonApiSerializer.JsonApi;
using JsonApiSerializer.JsonApi.WellKnown;
using JsonApiSerializer.JsonConverters;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JsonApiSerializer.ContractResolvers.Contracts
{
    internal abstract class JsonObjectContractWrap : JsonObjectContract
    {
        public JsonObjectContractWrap(JsonObjectContract jsonObjectContract)
            : base(jsonObjectContract.UnderlyingType)
        {
            //populate JsonObjectContract fields
            MemberSerialization = jsonObjectContract.MemberSerialization;
            ItemRequired = jsonObjectContract.ItemRequired;
            foreach (var prop in jsonObjectContract.Properties)
                Properties.Add(prop);
            OverrideCreator = jsonObjectContract.OverrideCreator;
            ExtensionDataSetter = jsonObjectContract.ExtensionDataSetter;
            ExtensionDataGetter = jsonObjectContract.ExtensionDataGetter;
            ExtensionDataValueType = jsonObjectContract.ExtensionDataValueType;
            ExtensionDataNameResolver = jsonObjectContract.ExtensionDataNameResolver;

            //populate JsonContainerContract fields
            ItemConverter = jsonObjectContract.ItemConverter;
            ItemIsReference = jsonObjectContract.ItemIsReference;
            ItemReferenceLoopHandling = jsonObjectContract.ItemReferenceLoopHandling;
            ItemTypeNameHandling = jsonObjectContract.ItemTypeNameHandling;

            //poulate JsonContract fields
            CreatedType = jsonObjectContract.CreatedType;
            IsReference = jsonObjectContract.IsReference;
            Converter = jsonObjectContract.Converter;
            foreach (var callback in jsonObjectContract.OnDeserializedCallbacks)
                OnDeserializedCallbacks?.Add(callback);
            foreach (var callback in jsonObjectContract.OnDeserializingCallbacks)
                OnDeserializingCallbacks?.Add(callback);
            foreach (var callback in jsonObjectContract.OnSerializedCallbacks)
                OnSerializedCallbacks?.Add(callback);
            foreach (var callback in jsonObjectContract.OnSerializingCallbacks)
                OnSerializingCallbacks?.Add(callback);
            foreach (var callback in jsonObjectContract.OnErrorCallbacks)
                OnErrorCallbacks?.Add(callback);
            DefaultCreator = jsonObjectContract.DefaultCreator;
            DefaultCreatorNonPublic = jsonObjectContract.DefaultCreatorNonPublic;
        }
    }
}
