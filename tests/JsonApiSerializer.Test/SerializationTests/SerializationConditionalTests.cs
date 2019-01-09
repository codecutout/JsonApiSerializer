using JsonApiSerializer.Exceptions;
using JsonApiSerializer.JsonApi;
using JsonApiSerializer.Test.Models.Articles;
using JsonApiSerializer.Test.Models.Locations;
using JsonApiSerializer.Test.TestUtils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using Xunit;

namespace JsonApiSerializer.Test.SerializationTests
{
    public class SerializationConditionalTests
    {
        public JsonApiSerializerSettings settings = new JsonApiSerializerSettings
        {
            Formatting = Formatting.Indented //pretty print makes it easier to debug
        };

        [Fact]
        public void When_serialization_condition_nothing_should_serialize_mandatory_fields()
        {
            var root = new DocumentRoot<ArticleWithSerializationConditions>
            {
                Data = new ArticleWithSerializationConditions
                {
                    Id = "1234",
                    Title = "My Article",
                    Author = new PersonWithSerializationConditions
                    {
                        Id = "333",
                        FirstName = "John",
                        LastName = "Smith",
                        Twitter = "jsmi"
                    },
                    SerializeProperties = new List<string> {  }

                }
            };

            var json = JsonConvert.SerializeObject(root, settings);
            var expectedjson = @"{
                ""data"": {
                    ""type"": ""articles""
                }
            }";
            Assert.Equal(expectedjson, json, JsonStringEqualityComparer.Instance);
        }

        [Fact]
        public void When_only_attribute_serialization_should_serialize()
        {
            var root = new DocumentRoot<ArticleWithSerializationConditions>
            {
                Data = new ArticleWithSerializationConditions
                {
                    Id = "1234",
                    Title = "My Article",
                    Author = new PersonWithSerializationConditions
                    {
                        Id = "333",
                        FirstName = "John",
                        LastName = "Smith",
                        Twitter = "jsmi"
                    },
                    SerializeProperties = new List<string> { "Title" }
                    
                }
            };

            var json = JsonConvert.SerializeObject(root, settings);
            var expectedjson = @"{
                ""data"": {
                    ""type"": ""articles"",
                    ""attributes"": {
                        ""title"": ""My Article""
                    }
                }
            }";
            Assert.Equal(expectedjson, json, JsonStringEqualityComparer.Instance);
        }

        [Fact]
        public void When_attribute_serialization_and_null_should_serialize()
        {
            var root = new DocumentRoot<ArticleWithSerializationConditions>
            {
                Data = new ArticleWithSerializationConditions
                {
                    Id = "1234",
                    Title = null,
                    Author = new PersonWithSerializationConditions
                    {
                        Id = "333",
                        FirstName = "John",
                        LastName = "Smith",
                        Twitter = "jsmi"
                    },
                    SerializeProperties = new List<string> { "Title" }

                }
            };

            var json = JsonConvert.SerializeObject(root, new JsonApiSerializerSettings
            {
                NullValueHandling = NullValueHandling.Include,
                Formatting = Formatting.Indented
            });
            var expectedjson = @"{
                ""data"": {
                    ""type"": ""articles"",
                    ""attributes"": {
                        ""title"": null
                    }
                }
            }";
            Assert.Equal(expectedjson, json, JsonStringEqualityComparer.Instance);
        }

        [Fact]
        public void When_only_relationship_serialization_should_serialize()
        {
            var root = new DocumentRoot<ArticleWithSerializationConditions>
            {
                Data = new ArticleWithSerializationConditions
                {
                    Id = "1234",
                    Title = "My Article",
                    Author = new PersonWithSerializationConditions
                    {
                        Id = "333",
                        FirstName = "John",
                        LastName = "Smith",
                        Twitter = "jsmi"
                    },
                    SerializeProperties = new List<string> {  "Author" }

                }
            };

            var json = JsonConvert.SerializeObject(root, settings);
            var expectedjson = @"{
                ""data"": {
                    ""type"": ""articles"",
                    ""relationships"": {
                        ""author"": {
                            ""data"": { 
                                ""id"":""333"", 
                                ""type"":""people""
                            }
                        }
                    }
                },
                ""included"" : [
                    {
                        ""id"": ""333"",
                        ""type"": ""people"",
                        ""attributes"":{
                            ""first-name"": ""John"",
                            ""last-name"": ""Smith"",
                            ""twitter"": ""jsmi""
                        }

                    }
                ]
            }";
            Assert.Equal(expectedjson, json, JsonStringEqualityComparer.Instance);
        }

        [Fact]
        public void When_relationslhip_serialization_and_null_should_serialize()
        {
            var root = new DocumentRoot<ArticleWithSerializationConditions>
            {
                Data = new ArticleWithSerializationConditions
                {
                    Id = "1234",
                    Title = "My Article",
                    Author = null,
                    SerializeProperties = new List<string> { "Author" }

                }
            };

            var json = JsonConvert.SerializeObject(root, new JsonApiSerializerSettings
            {
                NullValueHandling = NullValueHandling.Include,
                Formatting = Formatting.Indented
            });
            var expectedjson = @"{
                ""data"": {
                    ""type"": ""articles"",
                    ""relationships"": {
                        ""author"": {
                            ""data"": null
                        }
                    }
                }
            }";
            Assert.Equal(expectedjson, json, JsonStringEqualityComparer.Instance);
        }

        [Fact]
        public void When_only_id_serialization_should_serialize()
        {
            var root = new DocumentRoot<ArticleWithSerializationConditions>
            {
                Data = new ArticleWithSerializationConditions
                {
                    Id = "1234",
                    Title = "My Article",
                    Author = new PersonWithSerializationConditions
                    {
                        Id = "333",
                        FirstName = "John",
                        LastName = "Smith",
                        Twitter = "jsmi"
                    },
                    SerializeProperties = new List<string> { "Id" }

                }
            };

            var json = JsonConvert.SerializeObject(root, settings);
            var expectedjson = @"{
                ""data"": {
                    ""id"": ""1234"",
                    ""type"": ""articles"",
                }
            }";
            Assert.Equal(expectedjson, json, JsonStringEqualityComparer.Instance);
        }

        [Fact]
        public void When_nested_relationship_has_condition_should_serialize()
        {
            var root = new DocumentRoot<ArticleWithSerializationConditions>
            {
                Data = new ArticleWithSerializationConditions
                {
                    Id = "1234",
                    Title = "My Article",
                    Author = new PersonWithSerializationConditions
                    {
                        Id = "333",
                        FirstName = "John",
                        LastName = "Smith",
                        Twitter = "jsmi",
                        SerializeProperties = new List<string> { "LastName", "Id" }
                        
                    },
                    SerializeProperties = new List<string> { "Author" }

                }
            };

            var json = JsonConvert.SerializeObject(root, settings);
            var expectedjson = @"{
                ""data"": {
                    ""type"": ""articles"",
                    ""relationships"": {
                        ""author"": {
                            ""data"": { 
                                ""id"":""333"", 
                                ""type"":""people""
                            }
                        }
                    }
                },
                ""included"" : [
                    {
                        ""id"": ""333"",
                        ""type"": ""people"",
                        ""attributes"":{
                            ""last-name"": ""Smith""
                        }

                    }
                ]
            }";
            Assert.Equal(expectedjson, json, JsonStringEqualityComparer.Instance);
        }
    }
}
