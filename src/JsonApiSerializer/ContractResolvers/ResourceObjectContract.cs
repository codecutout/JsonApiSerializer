using JsonApiSerializer.JsonApi;
using JsonApiSerializer.JsonApi.WellKnown;
using JsonApiSerializer.JsonConverters;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JsonApiSerializer.ContractResolvers
{
    internal class ResourceObjectContract : JsonObjectContract
    {
        public readonly string DefaultType;
        public readonly JsonProperty IdProperty;
        public readonly JsonProperty TypeProperty;
        public readonly JsonProperty LinksProperty;
        public readonly JsonProperty MetaProperty;
        public readonly JsonProperty[] Attributes;
        public readonly KeyValuePair<JsonProperty, Func<object, object>>[] RelationshipTransformations;

        public ResourceObjectContract(JsonObjectContract jsonObjectContract)
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

            //populate ResourceObjectContract fields
            if (jsonObjectContract.Converter is ResourceObjectConverter resourceObjectConverter)
                DefaultType = resourceObjectConverter.GenerateDefaultTypeNameInternal(jsonObjectContract.UnderlyingType);

            var attributes = new List<JsonProperty>();
            var relationshipTransformations = new List<KeyValuePair<JsonProperty, Func<object, object>>>();
            foreach (var prop in jsonObjectContract.Properties.Where(x => !x.Ignored))
            {
                switch (prop.PropertyName)
                {
                    //In addition, a resource object MAY contain any of these top - level members: links, meta, attributes, relationships
                    case PropertyNames.Id: //Id is optional on base objects
                        IdProperty = prop;
                        break;
                    case PropertyNames.Links:
                        LinksProperty = prop;
                        break;
                    case PropertyNames.Meta:
                        MetaProperty = prop;
                        break;
                    case PropertyNames.Type:
                        TypeProperty = prop;
                        break;
                    case var _ when TryCreateRelationshipFactory(jsonObjectContract, prop.PropertyType, out var relationshipTranformation):
                        relationshipTransformations.Add(new KeyValuePair<JsonProperty, Func<object, object>>(prop, relationshipTranformation));
                        break;
                    default:
                        attributes.Add(prop);
                        break;
                }
            }

            this.Attributes = attributes.ToArray();
            this.RelationshipTransformations = relationshipTransformations.ToArray();
        }

        public static bool TryCreateRelationshipFactory(JsonObjectContract contract, Type propType, out Func<object, object> relationshipFactory)
        {
            if (contract.Converter.CanConvert(propType))
            {
                //it is a direct link to a resoruce object
                //Will create the joining relationship object
                relationshipFactory = value => Relationship.Create(value);
                return true;
            }
            else if (ResourceObjectListConverter.CanConvertStatic(propType, contract.Converter))
            {
                //it is a direct link to a list of reosurce objects
                //Will crete the joining relationship object
                relationshipFactory = value => Relationship.Create(value as IEnumerable<object>);
                return true;
            }
            else if (ResourceRelationshipConverter.CanConvertStatic(propType))
            {
                //it is a link to a relationship, no need to create relationship object
                relationshipFactory = value => value;
                return true;
            }
            else
            {
                relationshipFactory = null;
                return false;
            }
        }
    }
}
