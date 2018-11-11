using System;
using System.Collections.Generic;
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

        public class CustomTypeNameResourceObjectConverter : ResourceObjectConverter
        {
            private readonly Dictionary<Type, string> _typeToName;

            public CustomTypeNameResourceObjectConverter(Dictionary<Type, string> typeToName)
            {
                _typeToName = typeToName;
            }

            public override bool CanConvert(Type objectType)
            {
                return _typeToName.ContainsKey(objectType);
            }

            protected override string GenerateDefaultTypeName(Type type)
            {
                return _typeToName[type];
            }
        }

        public class FixedTypeNameResourceObjectConverter : ResourceObjectConverter
        {
            private readonly string _typeName;

            public FixedTypeNameResourceObjectConverter(string typeName)
            {
                _typeName = typeName;
            }

            protected override string GenerateDefaultTypeName(Type type)
            {
                return _typeName;
            }
        }

        public class ArticleWithCustomConverter
        {
            public string Id { get; set; }

            [JsonConverter(typeof(FixedTypeNameResourceObjectConverter), "special-author")]
            public PersonWithNoType Author { get; set; }
            public List<Comment> Comments { get; set; }

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

        [Fact]
        public void When_customer_converter_with_default_type_name_should_use_custom_converter()
        {
            var settings = new JsonApiSerializerSettings()
            {
                Formatting = Formatting.Indented, //pretty print makes it easier to debug
            };
            settings.Converters.Add(new CustomTypeNameResourceObjectConverter(new Dictionary<Type, string>()
            {
                {typeof(ArticleWithNoType), "special-article-type" }
            }));

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
                    ""type"": ""special-article-type"",
                },
            }";
            Assert.Equal(expectedjson, json, JsonStringEqualityComparer.Instance);
        }

        [Fact]
        public void When_member_converter_with_default_type_name_should_use_custom_converter()
        {
            var settings = new JsonApiSerializerSettings()
            {
                Formatting = Formatting.Indented, //pretty print makes it easier to debug
            };


            var root = new DocumentRoot<ArticleWithCustomConverter>
            {
                Data = new ArticleWithCustomConverter
                {
                    Id = "1234",
                    Author = new PersonWithNoType()
                    {
                        Id = "person-1234",
                        FirstName = "typeless"
                    }
                }
            };

            var json = JsonConvert.SerializeObject(root, settings);
            var expectedjson = @"{
                ""data"": {
                    ""id"": ""1234"",
                    ""type"": ""articlewithcustomconverter"",
                    ""relationships"": {
                        ""author"": {
                            ""data"": { 
                                ""id"":""person-1234"", 
                                ""type"":""special-author""
                            }
                        }
                    }
                },
                ""included"" : [
                    {
                        ""id"": ""person-1234"",
                        ""type"": ""special-author"",
                        ""attributes"":{
                            ""first-name"": ""typeless""
                        }

                    }
                ]
            }";
            Assert.Equal(expectedjson, json, JsonStringEqualityComparer.Instance);
        }
    }
}
