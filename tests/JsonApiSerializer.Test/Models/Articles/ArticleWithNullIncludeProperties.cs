using JsonApiSerializer.JsonApi;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonApiSerializer.Test.Models.Articles
{
    public class ArticleWithNullIncludeProperties
    {
        [JsonProperty(Order = -20)]
        public string Type { get; set; } = "articles";

        [JsonProperty(Order = -10)]
        public string Id { get; set; }

        public string Title { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public Person Author { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Include)]
        public List<Comment> Comments { get; set; }

        public Links Links { get; set; }
    }
}
