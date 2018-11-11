using JsonApiSerializer.JsonApi;
using System.Collections.Generic;

namespace JsonApiSerializer.Test.Models.Articles
{
    public class ArticleWithResourceIdentifier
    {
        public string Type { get; set; } = "articles";

        public string Id { get; set; }

        public string Title { get; set; }

        public ResourceIdentifier<Person> Author { get; set; }

        public List<ResourceIdentifier<Comment>> Comments { get; set; }

        public Links Links { get; set; }
    }
}
