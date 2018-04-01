using JsonApiSerializer.JsonApi;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace JsonApiSerializer.Test.Models.Articles
{
    public class ArticleWithGetters
    {
        public string Type { get; set; } = "articles";

        public string Id { get; set; }

        public string Title { get; }

        public Person Author { get; set; }

        [JsonIgnore]
        public List<Comment> Comments { get; }

        public Links Links { get; set; }
    }
}
