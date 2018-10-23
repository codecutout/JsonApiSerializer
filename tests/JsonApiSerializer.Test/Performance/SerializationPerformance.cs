using JsonApiSerializer.JsonApi;
using JsonApiSerializer.Test.Models.Articles;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace JsonApiSerializer.Test.Performance
{
    public class SerializationPerformance
    {
        public JsonApiSerializerSettings settings = new JsonApiSerializerSettings
        {
            Formatting = Formatting.Indented
        };

        [Fact]
        public void When()
        {
            var data = Enumerable.Range(0, 10000).Select(i => new SimpleObject
            {
                Id = i.ToString(),
                Title = "My Article"
            });

            var root = new DocumentRoot<List<SimpleObject>>
            {
                Data = data.ToList()
            };

            for (var j = 0; j < 100; ++j)
            {
                JsonConvert.SerializeObject(root, settings);
            }
        }

        [Fact]
        public void WhenHasRelationships()
        {
            var data = Enumerable.Range(0, 10000).Select(i => new Article
            {
                Id = i.ToString(),
                Title = "My Article",
                Author = new Person
                {
                    Id = i.ToString(),
                    FirstName = "John",
                    LastName = "Smith",
                    Twitter = "jsmi"
                }
            });

            var root = new DocumentRoot<List<Article>>
            {
                Data = data.ToList()
            };

            for (var j = 0; j < 100; ++j)
            {
                JsonConvert.SerializeObject(root, settings);
            }
        }

        public class SimpleObject
        {
            public string Id { get; set; }

            public string Title { get; set; }
        }
    }
}
