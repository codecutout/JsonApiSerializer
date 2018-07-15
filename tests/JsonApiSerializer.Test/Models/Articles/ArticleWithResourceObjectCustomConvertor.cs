using JsonApiSerializer.JsonApi;
using JsonApiSerializer.Test.TestUtils;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace JsonApiSerializer.Test.Models.Articles
{
    public class ArticleWithResourceObjectCustomConvertor
    {
        public string Type { get; set; } = "articles";

        public string Id { get; set; }

        public string Title { get; set; }

        [JsonConverter(typeof(SubclassResourceObjectConverter<Person>),
            new object[] { "people-admin", typeof(PersonAdmin) })]
        public Person Author { get; set; }

        public List<Comment> Comments { get; set; }

        public Links Links { get; set; }
    }
}
