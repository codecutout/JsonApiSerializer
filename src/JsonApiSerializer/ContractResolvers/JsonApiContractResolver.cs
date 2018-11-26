using System;
using JsonApiSerializer.ContractResolvers.Contracts;
using JsonApiSerializer.JsonConverters;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace JsonApiSerializer.ContractResolvers
{
    public class JsonApiContractResolver : DefaultContractResolver
    {
        public readonly JsonConverter ResourceObjectConverter;

        internal readonly ResourceObjectListConverter ResourceObjectListConverter;

        internal readonly ResourceRelationshipConverter ResourceRelationshipConverter;

        internal readonly ResourceIdentifierConverter ResourceIdentifierConverter;


        public JsonApiContractResolver(JsonConverter resourceObjectConverter)
        {
            ResourceObjectConverter = resourceObjectConverter;

            ResourceObjectListConverter = new ResourceObjectListConverter(ResourceObjectConverter);
            ResourceIdentifierConverter = new ResourceIdentifierConverter(ResourceObjectConverter.CanConvert);
            ResourceRelationshipConverter = new ResourceRelationshipConverter(ResourceIdentifierConverter.CanConvert);
            
            this.NamingStrategy = new CamelCaseNamingStrategy();
        }

        public JsonApiContractResolver()
            : this(new ResourceObjectConverter())
        {
        }

        protected override JsonConverter ResolveContractConverter(Type objectType)
        {
            if (ErrorConverter.CanConvertStatic(objectType))
                return new ErrorConverter();

            if (ErrorListConverter.CanConvertStatic(objectType))
                return new ErrorListConverter();

            if (ResourceObjectConverter.CanConvert(objectType))
                return ResourceObjectConverter;

            if (ResourceObjectListConverter.CanConvert(objectType))
                return ResourceObjectListConverter;

            if (LinkConverter.CanConvertStatic(objectType))
                return new LinkConverter();

            if (DocumentRootConverter.CanConvertStatic(objectType))
                return new DocumentRootConverter();

            if (ResourceRelationshipConverter.CanConvert(objectType))
                return ResourceRelationshipConverter;

            return base.ResolveContractConverter(objectType);
        }

        protected override JsonObjectContract CreateObjectContract(Type objectType)
        {
            var contract = base.CreateObjectContract(objectType);
            if (ResourceIdentifierConverter.IsExplicitResourceIdentifier(objectType))
                return new ResourceIdentifierContract(contract);
            if (ResourceObjectConverter.CanConvert(objectType))
                return new ResourceObjectContract(contract, ResourceRelationshipConverter.CanConvert);
            if (ResourceRelationshipConverter.IsExplicitRelationship(objectType))
                return new ResourceRelationshipContract(contract);
            return contract;
        }

    }
}
