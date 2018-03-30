using System;
using JsonApiSerializer.JsonApi;
using JsonApiSerializer.JsonConverters;
using JsonApiSerializer.Test.Models.Articles;
using JsonApiSerializer.Test.TestUtils;
using Newtonsoft.Json;
using Xunit;

namespace JsonApiSerializer.Test.SerializationTests
{
    public class SerializationCustomResourceObjectConverterTests
    {
        private class ShoutingClassNameObjectConverter : ResourceObjectConverter
        {
            protected override string GenerateDefaultTypeName(Type type)
            {
                return type.Name.ToUpper();
            }
        }

        [Fact]
        public void When_override_generate_default_type_name_should_use_customer_type_name()
        {
            var settings = new JsonApiSerializerSettings(new ShoutingClassNameObjectConverter())
            {
                Formatting = Formatting.Indented, //pretty print makes it easier to debug
            };

            var root = new DocumentRoot<ArticleWithNoType>
            {
                Data = new ArticleWithNoType
                {
                    Id = "1234",
                    Author = new Person()
                    {
                        Id = "jdoe",
                        FirstName = "John",
                        LastName = "Doe"
                    }
                }
            };

            var json = JsonConvert.SerializeObject(root, settings);

            Assert.Equal(@"
{
  ""data"": {
            ""type"": ""ARTICLEWITHNOTYPE"",
            ""id"": ""1234"",
            ""relationships"": {
                ""author"": {
                    ""data"": {
                        ""id"": ""jdoe"",
                        ""type"": ""people""
                    }
                }
            }
        },
        ""included"": [
        {
            ""type"": ""people"",
            ""id"": ""jdoe"",
            ""attributes"": {
                ""first-name"": ""John"",
                ""last-name"": ""Doe""
            }
        }
        ]
    }", json, JsonStringEqualityComparer.Instance);
        }
    }
}
