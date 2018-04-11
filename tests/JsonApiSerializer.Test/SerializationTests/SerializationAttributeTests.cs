using JsonApiSerializer.JsonApi;
using JsonApiSerializer.Test.Models.Articles;
using JsonApiSerializer.Test.TestUtils;
using Newtonsoft.Json;
using Xunit;

namespace JsonApiSerializer.Test.SerializationTests
{
    public class SerializationAttributeTests
    {
        public JsonApiSerializerSettings settings = new JsonApiSerializerSettings
        {
            Formatting = Formatting.Indented, //pretty print makes it easier to debug
        };

        [Fact]
        public void When_fields_controlled_by_jsonnet_attributes_should_respect_attributes()
        {
            var root = new DocumentRoot<PersonWithAttributes>
            {
                Data = new PersonWithAttributes
                {
                    Id = "1234", //before type
                    FirstName = "John", //after lastname
                    LastName = "Smith", //before firstname
                    Twitter = "jsmi" //ignored
                }
            };

            var json = JsonConvert.SerializeObject(root, settings);

            Assert.Equal(@"
                {
                  ""data"": {
                    ""id"": ""1234"",
                    ""type"": ""people"",
                    ""attributes"": {
                      ""last-name"": ""Smith"",
                      ""first-name"": ""John""
                    }
                  }
                }".Trim(), json, JsonStringEqualityComparer.Instance);
        }

        [Fact]
        public void When_fields_controlled_by_jsonnet_nullinclude_attributes_should_include_null()
        {
            var root = new DocumentRoot<ArticleWithNullIncludeProperties>
            {
                Data = new ArticleWithNullIncludeProperties
                {
                    Id = "1234",
                    Title = null, //default NullValueHandling
                    Author = null, //NullValueHandling.Ignore
                    Comments = null //NullValueHandling.Include
                }
            };

            var json = JsonConvert.SerializeObject(root, settings);

            Assert.Equal(@"
            {
                ""data"": {
                ""type"": ""articles"",
                ""id"": ""1234"",
                ""relationships"": {
                    ""comments"": {
                    ""data"": []
                    }
                }
                }
            }".Trim(), json, JsonStringEqualityComparer.Instance);
        }

        [Fact]
        public void When_fields_controlled_by_jsonnet_nullignore_attributes_should_ignore_null()
        {
            var root = new DocumentRoot<ArticleWithNullIncludeProperties>
            {
                Data = new ArticleWithNullIncludeProperties
                {
                    Id = "1234",
                    Title = null, //default NullValueHandling
                    Author = null, //NullValueHandling.Ignore
                    Comments = null //NullValueHandling.Include
                }
            };

            var newSettings = new JsonApiSerializerSettings()
            {
                Formatting = Formatting.Indented, //pretty print makes it easier to debug
                NullValueHandling = NullValueHandling.Include
            };
            var json = JsonConvert.SerializeObject(root, newSettings);

            Assert.Equal(@"
            {
                ""data"": {
                ""type"": ""articles"",
                ""id"": ""1234"",
                ""links"": null,
                ""attributes"": {
                    ""title"": null
                },
                ""relationships"": {
                    ""comments"": {
                    ""data"": []
                    }
                }
                }
            }".Trim(), json, JsonStringEqualityComparer.Instance);
        }
    }
}
