using JsonApiSerializer.Exceptions;
using JsonApiSerializer.JsonApi;
using JsonApiSerializer.Test.Models.Articles;
using JsonApiSerializer.Test.TestUtils;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;
using Xunit;

namespace JsonApiSerializer.Test.SerializationTests
{
    public class SerializationIdTests
    {
        public JsonApiSerializerSettings settings = new JsonApiSerializerSettings()
        {
            Formatting = Formatting.Indented //pretty print makes it easier to debug
        };

        [Fact]
        public void When_id_not_on_root_serialize_without_id()
        {
            var root = new DocumentRoot<Article>
            {
                Data = new Article
                {
                    Title = "My title"
                }
            };


            var json = JsonConvert.SerializeObject(root, settings);
            var expectedjson = @"{
                ""data"": {
                    ""type"": ""articles"",
                    ""attributes"": {
                        title: ""My title""
                    }
                },
            }";
            Assert.Equal(expectedjson, json, JsonStringEqualityComparer.Instance);
        }

        [Fact]
        public void When_id_int_should_serialize_as_string()
        {
            var root = new DocumentRoot<ArticleWithIdType<int>>
            {
                Data = new ArticleWithIdType<int>
                {
                    Id = 7357,
                    Title = "My title"
                }
            };

            var json = JsonConvert.SerializeObject(root, settings);
            var expectedjson = @"{
                ""data"": {
                    ""id"": ""7357"",
                    ""type"": ""articles"",
                    ""attributes"": {
                        title: ""My title""
                    }
                },
            }";
            Assert.Equal(expectedjson, json, JsonStringEqualityComparer.Instance);
        }

        [Fact]
        public void When_id_object_should_throw()
        {
            var root = new DocumentRoot<ArticleWithIdType<Tuple<string,string>>>
            {
                Data = new ArticleWithIdType<Tuple<string, string>>
                {
                    Id = new Tuple<string, string>("1", "2"),
                    Title = "My title"
                }
            };

            Assert.Throws<JsonApiFormatException>(() => JsonConvert.SerializeObject(root, settings));
        }

        [Fact]
        public void When_id_invalid_type_and_null_and_show_nulls_should_throw()
        {
            var root = new DocumentRoot<ArticleWithIdType<Task>>
            {
                Data = new ArticleWithIdType<Task>
                {
                    Id = null,
                    Title = "My title"
                }
            };

            var showNullSettings = new JsonApiSerializerSettings { NullValueHandling = NullValueHandling.Include };
            Assert.Throws<JsonApiFormatException>(() => JsonConvert.SerializeObject(root, showNullSettings));
        }
    }
}
