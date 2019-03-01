using JsonApiSerializer.JsonApi;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace JsonApiSerializer.Test.Models.Articles
{
    public class ArticleWithIgnoredPropertiesPersonWithJsonApiAttributes
    {
        public string Type { get; set; } = "articles";

        public string Id { get; set; }

        [JsonIgnore]
        public string Title { get; set; }

        public PersonWithJsonApiAttributes Author { get; set; }

        [JsonIgnore]
        public List<Comment> Comments { get; set; }

        public Links Links { get; set; }
    }
}
