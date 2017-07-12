using JsonApiSerializer.JsonConverters;
using JsonApiSerializer.ReferenceResolvers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JsonApiSerializer.ContractResolvers;

namespace JsonApiSerializer
{
    /// <summary>
    /// Provides a set of default settings to allow the deserialization of JsonApi formatted json.
    /// </summary>
    /// <seealso cref="Newtonsoft.Json.JsonSerializerSettings" />
    public class JsonApiSerializerSettings : JsonSerializerSettings
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="JsonApiSerializerSettings"/> class.
        /// </summary>
        /// <param name="resourceObjectConverter">The converter to use when serializing/deserializing a JsonApi resource object</param>
        public JsonApiSerializerSettings(JsonConverter resourceObjectConverter) : base()
        {
            this.NullValueHandling = NullValueHandling.Ignore;
            this.ReferenceLoopHandling = ReferenceLoopHandling.Serialize;
            this.ReferenceResolverProvider = () => new IncludedReferenceResolver();
            this.ContractResolver = new JsonApiContractResolver(resourceObjectConverter);
            this.DateParseHandling = DateParseHandling.None;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonApiSerializerSettings"/> class.
        /// </summary>
        public JsonApiSerializerSettings() : this(new ResourceObjectConverter())
        {
            
        }


    }
}
