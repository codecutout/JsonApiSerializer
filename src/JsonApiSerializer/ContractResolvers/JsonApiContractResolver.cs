using JsonApiSerializer.JsonApi;
using JsonApiSerializer.JsonConverters;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace JsonApiSerializer
{
    internal class JsonApiContractResolver : DefaultContractResolver
    {
        public readonly JsonConverter ResourceObjectConverter;

        private readonly JsonConverter _resourceWrapConverter;

        private readonly JsonConverter _resourceListWrapConverter;

        public JsonApiContractResolver(JsonConverter resourceObjectConverter)
        {
            ResourceObjectConverter = resourceObjectConverter;
            _resourceWrapConverter = new ResourceWrapConverter(ResourceObjectConverter);
            _resourceListWrapConverter = new ResourceListWrapConverter(ResourceObjectConverter);

            this.NamingStrategy = new CamelCaseNamingStrategy();
        }

        public JsonApiContractResolver()
            : this(new ResourceObjectConverter())
        {
        }

        protected override JsonConverter ResolveContractConverter(Type objectType)
        {
            if (_resourceWrapConverter.CanConvert(objectType))
                return _resourceWrapConverter;

            if (_resourceListWrapConverter.CanConvert(objectType))
                return _resourceListWrapConverter;

            if (LinkConverter.CanConvertStatic(objectType))
                return new LinkConverter();

            if (DocumentRootConverter.CanConvertStatic(objectType))
                return new DocumentRootConverter();

            return base.ResolveContractConverter(objectType);
        }
    }
}
