using JsonApiSerializer.JsonApi;
using JsonApiSerializer.Test.Models.Articles;
using JsonApiSerializer.Test.TestUtils;
using Newtonsoft.Json;
using Xunit;

namespace JsonApiSerializer.Test.SerializationTests
{
    public class SerializationTypeTests
    {
        public JsonApiSerializerSettings settings = new JsonApiSerializerSettings()
        {
            Formatting = Formatting.Indented //pretty print makes it easier to debug
        };

        [Fact]
        public void When_type_not_defined_should_use_class_name()
        {
            var root = new DocumentRoot<ArticleWithNoType>
            {
                Data = new ArticleWithNoType
                {
                    Id = "1234",
                }
            };


            var json = JsonConvert.SerializeObject(root, settings);
            var expectedjson = @"{
                ""data"": {
                    ""id"": ""1234"",
                    ""type"": ""articlewithnotype"",
                },
            }";
            Assert.Equal(expectedjson, json, JsonStringEqualityComparer.Instance);
        }

        [Fact]
        public void When_type_defined_should_use_defined_type()
        {
            var root = new DocumentRoot<Article>
            {
                Data = new Article
                {
                    Id = "1234",
                    Type = "my-article-type"
                }
            };


            var json = JsonConvert.SerializeObject(root, settings);
            var expectedjson = @"{
                ""data"": {
                    ""id"": ""1234"",
                    ""type"": ""my-article-type"",
                },
            }";
            Assert.Equal(expectedjson, json, JsonStringEqualityComparer.Instance);
        }

        [Fact]
        public void When_type_readonly_should_use_readonly_type()
        {
            var root = new DocumentRoot<ArticleWithReadonlyType>
            {
                Data = new ArticleWithReadonlyType
                {
                    Id = "1234",
                }
            };


            var json = JsonConvert.SerializeObject(root, settings);
            var expectedjson = @"{
                ""data"": {
                    ""id"": ""1234"",
                    ""type"": ""readonly-article-type"",
                },
            }";
            Assert.Equal(expectedjson, json, JsonStringEqualityComparer.Instance);
        }


    }
}
