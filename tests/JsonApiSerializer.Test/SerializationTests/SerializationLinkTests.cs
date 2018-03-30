using JsonApiSerializer.JsonApi;
using JsonApiSerializer.Test.Models.Articles;
using JsonApiSerializer.Test.TestUtils;
using Newtonsoft.Json;
using Xunit;

namespace JsonApiSerializer.Test.SerializationTests
{
    public class SerializationLinkTests
    {
        public JsonApiSerializerSettings settings = new JsonApiSerializerSettings()
        {
            Formatting = Formatting.Indented //pretty print makes it easier to debug
        };

        [Fact]
        public void When_link_has_only_href_should_serialize_as_string()
        {
            var root = new Article
            {
                Id = "1234",
                Links = new Links
                {
                    {"self", new Link {Href= "http://articles/1234" } }
                }
            };


            var json = JsonConvert.SerializeObject(root, settings);
            var expectedjson = @"{
                ""data"": {
                    ""id"": ""1234"",
                    ""type"": ""articles"",
                    ""links"": {
                        ""self"": ""http://articles/1234""
                    }
                },
            }";
            Assert.Equal(expectedjson, json, JsonStringEqualityComparer.Instance);
        }

        [Fact]
        public void When_link_has_meta_should_serialize_as_object()
        {
            var root = new Article
            {
                Id = "1234",
                Links = new Links
                {
                    {"self", new Link {

                        Href = "http://articles/1234",
                        Meta = new Meta
                        {
                            {"resolved", true }
                        }
                    }}
                }
            };


            var json = JsonConvert.SerializeObject(root, settings);
            var expectedjson = @"{
                ""data"": {
                    ""id"": ""1234"",
                    ""type"": ""articles"",
                    ""links"": {
                        ""self"": { 
                            ""href"":""http://articles/1234"",
                            ""meta"":{
                                ""resolved"":true
                            }
                        }
                    }
                },
            }";
            Assert.Equal(expectedjson, json, JsonStringEqualityComparer.Instance);
        }
    }
}
