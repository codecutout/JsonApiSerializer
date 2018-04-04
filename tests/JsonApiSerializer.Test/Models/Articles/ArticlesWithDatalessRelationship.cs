using JsonApiSerializer.JsonApi;
using System.Collections.Generic;

namespace JsonApiSerializer.Test.Models.Articles
{
    public class ArticleWithDatalessRelationship
    {
        public string Type { get; set; } = "articles";

        public string Id { get; set; }

        public string Title { get; set; }

        public Relationship<Person> Author { get; set; }

        public Relationship Comments { get; set; }

        public Links Links { get; set; }
    }
}
