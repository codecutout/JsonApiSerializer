using JsonApiSerializer.Exceptions;
using JsonApiSerializer.JsonApi;
using JsonApiSerializer.Test.Models.Articles;
using JsonApiSerializer.Test.Models.Locations;
using JsonApiSerializer.Test.TestUtils;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using Xunit;

namespace JsonApiSerializer.Test.SerializationTests
{
    public class SerializationRelationshipTests
    {
        public JsonApiSerializerSettings settings = new JsonApiSerializerSettings
        {
            Formatting = Formatting.Indented //pretty print makes it easier to debug
        };

        [Fact]
        public void When_relationship_should_add_include()
        {
            var root = new DocumentRoot<Article>
            {
                Data = new Article
                {
                    Id = "1234",
                    Title = "My Article",
                    Author = new Person
                    {
                        Id = "333",
                        FirstName = "John",
                        LastName = "Smith",
                        Twitter = "jsmi"
                    }
                }
            };

            var json = JsonConvert.SerializeObject(root, settings);
            var expectedjson = @"{
                ""data"": {
                    ""id"": ""1234"",
                    ""type"": ""articles"",
                    ""attributes"": {
                        ""title"": ""My Article""
                    },
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
        public void When_relationship_empty_array_should_output_empty_relationship()
        {
            var root = new DocumentRoot<Article>
            {
                Data = new Article
                {
                    Id = "1234",
                    Title = "My Article",
                    Comments = new List<Comment>()

                }
            };

            var json = JsonConvert.SerializeObject(root, settings);
            var expectedjson = @"{
                ""data"": {
                    ""id"": ""1234"",
                    ""type"": ""articles"",
                    ""attributes"": {
                        ""title"": ""My Article""
                    },
                    ""relationships"": {
                        ""comments"": {
                            ""data"": []
                            }
                        }
                    }
                }";
            Assert.Equal(expectedjson, json, JsonStringEqualityComparer.Instance);
        }

        [Fact]
        public void When_relationships_null_and_serializing_should_serialize_null_data()
        {
            var root = new DocumentRoot<Article>
            {
                Data = new Article
                {
                    Id = "1234",
                    Title = null,
                    Author = null,
                    Comments = null
                }
            };

            var newSettings = new JsonApiSerializerSettings
            {
                Formatting = Formatting.Indented, //pretty print makes it easier to debug
                NullValueHandling = NullValueHandling.Include
            };
            var json = JsonConvert.SerializeObject(root, newSettings);
            var expectedjson = @"{
              ""data"": {
                ""type"": ""articles"",
                ""id"": ""1234"",
                ""links"": null,
                ""attributes"": {
                    ""title"": null
                },
                ""relationships"": {
                    ""author"": {
                        ""data"": null
                    },
                    ""comments"": {
                        ""data"": []
                    }
                }
              }
            }";
            Assert.Equal(expectedjson, json, JsonStringEqualityComparer.Instance);
        }

        [Fact]
        public void When_relationship_object_explicit_with_data_respresented_as_object_list_should_serialize()
        {
            var article = new
            {
                Id = "1234",
                Type = "articles",
                Title = "My Article",
                Comments = new
                {
                    Data = new object[] {
                        new Comment() { Id = "c1" },
                        new { Id = "c2", Type = "moderator-comments" },
                   }
                }
            };

            var json = JsonConvert.SerializeObject(article, settings);
            var expectedjson = @"{
              ""data"": {
                ""type"": ""articles"",
                ""id"": ""1234"",
                ""attributes"": {
                  ""title"": ""My Article""
                },
                ""relationships"": {
                  ""comments"": {
                    ""data"": [
                      {
                        ""id"": ""c1"",
                        ""type"": ""comments""
                      },
                      {
                        ""id"": ""c2"",
                        ""type"": ""moderator-comments""
                      }
                    ]
                  }
                }
              }
            }";
            Assert.Equal(expectedjson, json, JsonStringEqualityComparer.Instance);
        }

        [Fact]
        public void When_relationship_object_explicit_with_non_resource_data_should_error()
        {
            var article = new
            {
                Id = "1234",
                Type = "articles",
                Title = "My Article",
                Comments = new
                {
                    Data = "should not be allowed to have a non-resource object here"
                }
            };

            var error = Assert.Throws<JsonApiFormatException>(() => JsonConvert.SerializeObject(article, settings));
        }

        [Fact]
        public void When_relationship_object_explicit_with_non_resource_data_list_should_error()
        {
            var article = new
            {
                Id = "1234",
                Type = "articles",
                Title = "My Article",
                Comments = new
                {
                    Data = new object[] {
                        new Comment() { Id = "c1" },
                        "should not be allowed to have a non-resource object here",
                   }
                }
            };
            var error = Assert.Throws<JsonApiFormatException>(() => JsonConvert.SerializeObject(article, settings));
        }

        [Fact]
        public void When_object_root_with_relationship_should_add_included()
        {
            var root = new Article
            {
                Id = "1234",
                Title = "My Article",
                Author = new Person
                {
                    Id = "333",
                    FirstName = "John",
                    LastName = "Smith",
                    Twitter = "jsmi"
                }
            };

            var json = JsonConvert.SerializeObject(root, settings);
            var expectedjson = @"{
                ""data"": {
                    ""id"": ""1234"",
                    ""type"": ""articles"",
                    ""attributes"": {
                        ""title"": ""My Article""
                    },
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
        public void When_relationship_object_multiple_times_should_add_singe_included()
        {
            var person = new Person
            {
                Id = "333",
                FirstName = "John",
                LastName = "Smith",
                Twitter = "jsmi"
            };
            var root = new Article
            {
                Id = "1234",
                Title = "My Article",
                Author = person,
                Comments = new List<Comment>
                {
                    new Comment
                    {
                        Id = "c1",
                        Author = person,
                        Body = "comment 1"
                    },
                    new Comment
                    {
                        Id = "c2",
                        Author = person,
                        Body = "comment 2"
                    }
                }
            };

            var json = JsonConvert.SerializeObject(root, settings);
            var expectedjson = @"{
                ""data"": {
                    ""id"": ""1234"",
                    ""type"": ""articles"",
                    ""attributes"": {
                        ""title"": ""My Article""
                    },
                    ""relationships"": {
                        ""author"": {
                            ""data"": { 
                                ""id"":""333"", 
                                ""type"":""people""
                            }
                        },
                        ""comments"": {
                            ""data"": [
                                { 
                                    ""id"":""c1"", 
                                    ""type"":""comments""
                                },
                                { 
                                    ""id"":""c2"", 
                                    ""type"":""comments""
                                }
                            ]
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
                    },
                    {
                        ""id"": ""c1"",
                        ""type"": ""comments"",
                        ""attributes"":{
                            ""body"": ""comment 1""
                        },
                        ""relationships"": {
                            ""author"": {
                                ""data"": { 
                                    ""id"":""333"", 
                                    ""type"":""people""
                                }
                            }
                        }
                    },
                    {
                        ""id"": ""c2"",
                        ""type"": ""comments"",
                        ""attributes"":{
                            ""body"": ""comment 2""
                        },
                        ""relationships"": {
                            ""author"": {
                                ""data"": { 
                                    ""id"":""333"", 
                                    ""type"":""people""
                                }
                            }
                        }
                    }
                ]
            }";
            Assert.Equal(expectedjson, json, JsonStringEqualityComparer.Instance);
        }

        [Fact]
        public void When_explicit_relationship_should_output_metaData()
        {
            var root = new ArticleWithRelationship
            {
                Id = "1234",
                Title = "My Article",
                Author = new Relationship<Person>
                {
                    Data = new Person
                    {
                        Id = "333",
                        FirstName = "John",
                        LastName = "Smith",
                        Twitter = "jsmi"
                    },
                    Links = new Links
                    {
                        { "self" , new Link {  Href = "http://example.com/articles/1/relationships/author" } },
                        { "related", new Link {  Href = "http://example.com/articles/1/author" } }
                    }
                }
            };

            var json = JsonConvert.SerializeObject(root, settings);
            var expectedjson = @"{
                ""data"": {
                    ""id"": ""1234"",
                    ""type"": ""articles"",
                    ""attributes"": {
                        ""title"": ""My Article""
                    },
                    ""relationships"": {
                        ""author"": {
                            ""data"": { 
                                ""id"":""333"", 
                                ""type"":""people""
                            },
                            ""links"": {
                                ""self"": ""http://example.com/articles/1/relationships/author"",
                                ""related"": ""http://example.com/articles/1/author""
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
        public void When_data_list_explicit_null_should_serialize_empty_list()
        {
            var root = new ArticleWithRelationship
            {
                Id = "1234",
                Title = "My Article",
                Comments = new Relationship<List<Comment>>
                {
                    Data = null,
                    Links = new Links
                    {
                        { "related", new Link {  Href = "http://example.com/articles/1/comments" } }
                    }
                }
            };

            var json = JsonConvert.SerializeObject(root, settings);
            var expectedjson = @"{
                ""data"": {
                    ""id"": ""1234"",
                    ""type"": ""articles"",
                    ""attributes"": {
                        ""title"": ""My Article""
                    },
                    ""relationships"": {
                        ""comments"": {
                            ""data"" : [],
                            ""links"": {
                                ""related"": ""http://example.com/articles/1/comments""
                            }
                        }
                    }
                }
            }";
            Assert.Equal(expectedjson, json, JsonStringEqualityComparer.Instance);
        }

        [Fact]
        public void When_data_missing_should_not_serialize_data()
        {
            var root = new ArticleWithDatalessRelationship
            {
                Id = "1234",
                Title = "My Article",
                Comments = new Relationship
                {
                    Links = new Links
                    {
                        { "related", new Link {  Href = "http://example.com/articles/1/comments" } }
                    }
                }
            };

            var json = JsonConvert.SerializeObject(root, settings);
            var expectedjson = @"{
                ""data"": {
                    ""id"": ""1234"",
                    ""type"": ""articles"",
                    ""attributes"": {
                        ""title"": ""My Article""
                    },
                    ""relationships"": {
                        ""comments"": {
                            ""links"": {
                                ""related"": ""http://example.com/articles/1/comments""
                            }
                        }
                    }
                }
            }";
            Assert.Equal(expectedjson, json, JsonStringEqualityComparer.Instance);
        }

        [Fact]
        public void When_object_reference_relationships_are_in_data_should_not_be_in_includes()
        {
            var london = new LocationWithId { Id = "London", Description = "Capital" };
            var kingsCross = new LocationWithId { Id = "Kings-Cross", Parents = new[] { london } };
            var farringdon = new LocationWithId { Id = "Farringdon", Parents = new[] { london } };

            var root = new[]
            {
                london,
                kingsCross,
                farringdon
            };

            var json = JsonConvert.SerializeObject(root, settings);
            var expectedjson = @"{
  ""data"": [
    {
      ""type"": ""locationwithid"",
      ""id"": ""London"",
      ""attributes"": {
        ""description"": ""Capital""
      }
    },
    {
      ""type"": ""locationwithid"",
      ""id"": ""Kings-Cross"",
      ""relationships"": {
        ""parents"": {
          ""data"": [
            {
              ""id"": ""London"",
              ""type"": ""locationwithid""
            }
          ]
        }
      }
    },
    {
      ""type"": ""locationwithid"",
      ""id"": ""Farringdon"",
      ""relationships"": {
        ""parents"": {
          ""data"": [
            {
              ""id"": ""London"",
              ""type"": ""locationwithid""
            }
          ]
        }
      }
    }
  ]
}";
            Assert.Equal(expectedjson, json, JsonStringEqualityComparer.Instance);
        }

        [Fact]
        public void When_id_reference_relationships_are_in_data_should_not_be_in_includes()
        {
            var root = new[]
            {
                new LocationWithId { Id = "London", Description ="Capital" },
                new LocationWithId
                {
                    Id = "Kings-Cross",
                    Parents = new[] { new LocationWithId { Id = "London", Description= "Capital" } }
                },
                new LocationWithId
                {
                    Id = "Farringdon",
                    Parents = new[] { new LocationWithId { Id = "London", Description = "Capital" } }
                },

            };

            var json = JsonConvert.SerializeObject(root, settings);
            var expectedjson = @"{
  ""data"": [
    {
      ""type"": ""locationwithid"",
      ""id"": ""London"",
      ""attributes"": {
        ""description"": ""Capital""
      }
    },
    {
      ""type"": ""locationwithid"",
      ""id"": ""Kings-Cross"",
      ""relationships"": {
        ""parents"": {
          ""data"": [
            {
              ""id"": ""London"",
              ""type"": ""locationwithid""
            }
          ]
        }
      }
    },
    {
      ""type"": ""locationwithid"",
      ""id"": ""Farringdon"",
      ""relationships"": {
        ""parents"": {
          ""data"": [
            {
              ""id"": ""London"",
              ""type"": ""locationwithid""
            }
          ]
        }
      }
    }
  ]
}";
            Assert.Equal(expectedjson, json, JsonStringEqualityComparer.Instance);
        }

        [Fact]
        public void When_shared_serializer_should_add_include()
        {
            var root = new DocumentRoot<Article>
            {
                Data = new Article
                {
                    Id = "1234",
                    Title = "My Article",
                    Author = new Person
                    {
                        Id = "333",
                        FirstName = "John",
                        LastName = "Smith",
                        Twitter = "jsmi"
                    }
                }
            };

            //Single serializer, just like MVC JsonOutputFormatters use
            var serializer = JsonSerializer.Create(settings);

            var stringWriter1 = new StringWriter();
            serializer.Serialize(stringWriter1, root);

            var stringWriter2 = new StringWriter();
            serializer.Serialize(stringWriter2, root);

            var expectedjson = @"{
                ""data"": {
                    ""id"": ""1234"",
                    ""type"": ""articles"",
                    ""attributes"": {
                        ""title"": ""My Article""
                    },
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
            Assert.Equal(stringWriter1.ToString(), expectedjson, JsonStringEqualityComparer.Instance);
            Assert.Equal(stringWriter2.ToString(), expectedjson, JsonStringEqualityComparer.Instance);
        }
    }
}
