using JsonApiSerializer.JsonApi;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonApiSerializer.Test.Models.Articles
{
    public class ArticleWithRelationshipDictionary
    {
        public string Type { get; set; } = "articles";

        public string Id { get; set; }

        public string Title { get; set; }

        public Dictionary<string, Relationship<JToken>> Relationships { get; set; }

        public Links Links { get; set; }
    }
}
