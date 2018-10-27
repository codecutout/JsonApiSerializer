using JsonApiSerializer.JsonApi;
using JsonApiSerializer.Test.Models.Articles.VanillaJson;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace JsonApiSerializer.Test.Models.Articles
{
    public class ArticleVanillaJson
    {
        public class ObjectAttributes
        {
            [JsonProperty(PropertyName = "title")]
            public string Title { get; set; }
        }

        public class ObjectRelationships
        {
            [JsonProperty(PropertyName = "author")]
            public Relationship<Reference> Author { get; set; }

            [JsonProperty(PropertyName = "comments")]
            public Relationship<List<Reference>> Comments { get; set; }

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
