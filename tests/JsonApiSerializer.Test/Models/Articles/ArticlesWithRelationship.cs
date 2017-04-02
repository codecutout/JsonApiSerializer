using JsonApiSerializer.JsonApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonApiSerializer.Test.Models.Articles
{
    public class ArticleWithRelationship
    {
        public string Type { get; set; } = "articles";

        public string Id { get; set; }

        public string Title { get; set; }

        public Relationship<Person> Author { get; set; }

        public Relationship<List<Comment>> Comments { get; set; }

        public Links Links { get; set; }
    }
}
