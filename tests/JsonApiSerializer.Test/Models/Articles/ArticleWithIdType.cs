using JsonApiSerializer.JsonApi;
using System.Collections.Generic;

namespace JsonApiSerializer.Test.Models.Articles
{
    public class ArticleWithIdType<T>
    {
        public string Type { get; set; } = "articles";

        public T Id { get; set; }

        public string Title { get; set; }

        public Person Author { get; set; }

        public List<Comment> Comments { get; set; }

        public Links Links { get; set; }
    }
}
