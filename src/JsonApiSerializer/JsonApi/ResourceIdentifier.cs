using JsonApiSerializer.JsonApi.WellKnown;
using Newtonsoft.Json;

namespace JsonApiSerializer.JsonApi
{
    public class ResourceIdentifier
    {
        /// <summary>
        /// Creates a Relationship for a given type.
        /// </summary>
        /// <typeparam name="TData">The type of the data.</typeparam>
        public static ResourceIdentifier<TResourceObject> Create<TResourceObject>(TResourceObject resourceObject)
        {
            return new ResourceIdentifier<TResourceObject>
            {
                Value = resourceObject
            };
        }

        [JsonProperty("meta", NullValueHandling = NullValueHandling.Ignore)]
        public Meta Meta { get; set; }
    }

    public class ResourceIdentifier<TResourceObject> : ResourceIdentifier, IResourceIdentifier<TResourceObject>
    {
        public TResourceObject Value { get; set; }
    }
}
