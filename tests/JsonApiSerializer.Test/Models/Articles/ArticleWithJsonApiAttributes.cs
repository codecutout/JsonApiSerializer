using JsonApiSerializer.JsonApi;
using System.Collections.Generic;

namespace JsonApiSerializer.Test.Models.Articles
{
    using ContractResolvers.Attributes;
    using Newtonsoft.Json;

    [JsonApiProperties(Id = "internalId", Type = "resourceType")]
    public class ArticleWithJsonApiAttributes
    {
        public string Type { get; set; } = "articles";

        public string InternalId { get; set; }

        public int Id { get; set; }

        public string Title { get; set; }

        public Person Author { get; set; }

        [JsonIgnore]
        public List<Comment> Comments { get; set; }

        public Links Links { get; set; }

        public string ResourceType { get; set; } = "articles";
    }
}
