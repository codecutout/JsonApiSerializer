using JsonApiSerializer.JsonApi;
using Newtonsoft.Json;

namespace JsonApiSerializer.Test.Models.Articles.VanillaJson
{
    public class CommentVanillaJson
    {
        public class ObjectAttributes
        {
            [JsonProperty(PropertyName = "body")]
            public string Body { get; set; }
        }

        public class ObjectRelationships
        {
            [JsonProperty(PropertyName = "author")]
            public Relationship<Reference> Author { get; set; }
        }

        [JsonProperty(PropertyName = "type")]
        public string Type { get; set; }

        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "attributes")]
        public ObjectAttributes Attributes { get; set; }

        [JsonProperty(PropertyName = "relationships")]
        public ObjectRelationships Relationships { get; set; }

        [JsonProperty(PropertyName = "links", NullValueHandling = NullValueHandling.Ignore)]
        public Links Links { get; set; }
    }
}
