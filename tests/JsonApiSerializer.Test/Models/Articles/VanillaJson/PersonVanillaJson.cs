using JsonApiSerializer.JsonApi;
using Newtonsoft.Json;

namespace JsonApiSerializer.Test.Models.Articles.VanillaJson
{
    public class PersonVanillaJson
    {
        public class ObjectAttributes
        {
            [JsonProperty(propertyName: "first-name")]
            public string FirstName { get; set; }

            [JsonProperty(propertyName: "last-name")]
            public string LastName { get; set; }

            [JsonProperty(propertyName: "twitter")]
            public string Twitter { get; set; }
        }

        [JsonProperty(PropertyName = "type")]
        public string Type { get; set; }

        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "attributes")]
        public ObjectAttributes Attributes { get; set; }

        [JsonProperty(PropertyName = "links", NullValueHandling = NullValueHandling.Ignore)]
        public Links Links { get; set; }

        [JsonProperty(PropertyName = "meta", NullValueHandling = NullValueHandling.Ignore)]
        public Meta Meta { get; set; }
    }
}
