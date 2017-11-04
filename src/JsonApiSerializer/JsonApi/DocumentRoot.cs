using JsonApiSerializer.JsonApi.WellKnown;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace JsonApiSerializer.JsonApi
{
    /// <summary>
    /// Represents the root of a JsonApi document
    /// </summary>
    /// <typeparam name="TData">The type of the data.</typeparam>
    /// <seealso cref="JsonApiSerializer.JsonApi.WellKnown.IDocumentRoot{TData}" />
    public class DocumentRoot<TData> : IDocumentRoot<TData>
    {
        [JsonProperty(propertyName: "jsonapi", NullValueHandling = NullValueHandling.Ignore)]
        public VersionInfo JsonApi { get; set; }

        public TData Data { get; set; }

        public List<JObject> Included { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<Error> Errors { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public Links Links { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public Meta Meta { get; set; }
    }
}
