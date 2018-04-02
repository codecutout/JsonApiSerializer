using JsonApiSerializer.JsonApi.WellKnown;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace JsonApiSerializer.JsonApi
{
    public static class DocumentRoot
    {
        /// <summary>
        /// Creates a new DocumentRoot for a given type.
        /// </summary>
        /// <typeparam name="TData">The type of the data.</typeparam>
        public static DocumentRoot<TData> Create<TData>(TData data)
        {
            return new DocumentRoot<TData>
            {
                Data = data
            };
        }
    }

    /// <summary>
    /// Represents the root of a JsonApi document
    /// </summary>
    /// <typeparam name="TData">The type of the data.</typeparam>
    /// <seealso cref="WellKnown.IDocumentRoot{TData}" />
    public class DocumentRoot<TData> : IDocumentRoot<TData>
    {
        [JsonProperty("jsonapi", NullValueHandling = NullValueHandling.Ignore)]
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
