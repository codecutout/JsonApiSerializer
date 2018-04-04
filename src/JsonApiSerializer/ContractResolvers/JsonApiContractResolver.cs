using System;
using JsonApiSerializer.JsonApi;
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

        public JsonApiContractResolver(JsonConverter resourceObjectConverter)
        {
            ResourceObjectConverter = resourceObjectConverter;
            ResourceObjectListConverter = new ResourceObjectListConverter(ResourceObjectConverter);
            ResourceRelationshipConverter = new ResourceRelationshipConverter();

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
    }
}
