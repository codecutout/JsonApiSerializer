using System;
using JsonApiSerializer.JsonConverters;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace JsonApiSerializer.ContractResolvers
{
    public class JsonApiContractResolver : DefaultContractResolver
    {
        public readonly JsonConverter ResourceObjectConverter;

        private readonly JsonConverter _resourceObjectListConverter;

        public JsonApiContractResolver(JsonConverter resourceObjectConverter)
        {
            ResourceObjectConverter = resourceObjectConverter;
            _resourceObjectListConverter = new ResourceObjectListConverter(ResourceObjectConverter);

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

            if (_resourceObjectListConverter.CanConvert(objectType))
                return _resourceObjectListConverter;

            if (LinkConverter.CanConvertStatic(objectType))
                return new LinkConverter();

            if (DocumentRootConverter.CanConvertStatic(objectType))
                return new DocumentRootConverter();

            return base.ResolveContractConverter(objectType);
        }
    }
}
