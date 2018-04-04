using Newtonsoft.Json;

namespace JsonApiSerializer.JsonApi
{
    public class Relationship
    {
        /// <summary>
        /// Creates a Relationship for a given type.
        /// </summary>
        /// <typeparam name="TData">The type of the data.</typeparam>
        public static Relationship<TData> Create<TData>(TData data)
        {
            return new Relationship<TData>
            {
                Data = data
            };
        }

        [JsonProperty("links", NullValueHandling = NullValueHandling.Ignore)]
        public Links Links { get; set; }

        [JsonProperty("meta", NullValueHandling = NullValueHandling.Ignore)]
        public Meta Meta { get; set; }
    }

    /// <summary>
    /// Represents a Relationship.
    /// </summary>
    /// <typeparam name="TData">The type of the data.</typeparam>
    public class Relationship<TData> : Relationship
    {
        [JsonProperty("data", NullValueHandling = NullValueHandling.Include)]
        public TData Data { get; set; }
    }
}
