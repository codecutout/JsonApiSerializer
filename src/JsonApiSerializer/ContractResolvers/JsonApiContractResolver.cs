using System;
using JsonApiSerializer.JsonConverters;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace JsonApiSerializer.ContractResolvers
{
    public class JsonApiContractResolver : DefaultContractResolver
    {
        public readonly JsonConverter ResourceObjectConverter;

        internal readonly JsonConverter ResourceObjectListConverter;

        public JsonApiContractResolver(JsonConverter resourceObjectConverter)
        {
            ResourceObjectConverter = resourceObjectConverter;
            ResourceObjectListConverter = new ResourceObjectListConverter(ResourceObjectConverter);

            this.NamingStrategy = new CamelCaseNamingStrategy();
        }

        public JsonApiContractResolver()
            : this(new ResourceObjectConverter())
        {
        }

        protected override JsonConverter ResolveContractConverter(Type objectType)
        {
            if(ErrorConverter.CanConvertStatic(objectType))
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

            return base.ResolveContractConverter(objectType);
        }
    }
}
