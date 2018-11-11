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
    public class SerializationResourceIdentifierTests
    {
        public JsonApiSerializerSettings settings = new JsonApiSerializerSettings
        {
            Formatting = Formatting.Indented //pretty print makes it easier to debug
        };

        [Fact]
        public void When_resource_identifier_should_serialize()
        {
            var root = new DocumentRoot<ArticleWithResourceIdentifier>
            {
                Data = new ArticleWithResourceIdentifier
                {
                    Id = "1234",
                    Title = "My Article",
                    Author = new ResourceIdentifier<Person>()
                    {
                        Meta = new Meta { { "primary-author", true } },
                        Value = new Person
                        {
                            Id = "333",
                            FirstName = "John",
                            LastName = "Smith",
                            Twitter = "jsmi",
                            Meta = new Meta() { { "best-sellers", 4 } }
                           
                        }
                    },
                    Comments = new List<ResourceIdentifier<Comment>>
                    {
                        ResourceIdentifier.Create(new Comment()
                        {
                            Id = "57",
                            Body = "I give it a 5 out of 7"
                        })
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
                                ""type"":""people"",
                                ""meta"":{ ""primary-author"": true}
                            }
                        },
                        ""comments"": {
                            ""data"": [{ 
                                ""id"":""57"", 
                                ""type"":""comments"",
                            }]
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
                        },
                        ""meta"":{ ""best-sellers"": 4}

                    },
                    {
                        ""id"": ""57"",
                        ""type"": ""comments"",
                        ""attributes"":{
                            ""body"": ""I give it a 5 out of 7""
                        }
                    }
                ]
            }";
            Assert.Equal(expectedjson, json, JsonStringEqualityComparer.Instance);
        }

        [Fact]
        public void When_resource_identifier_null_should_serialize_null_relationship()
        {
            var root = new DocumentRoot<ArticleWithResourceIdentifier>
            {
                Data = new ArticleWithResourceIdentifier
                {
                    Id = "1234",
                    Title = "My Article",
                    Author = new ResourceIdentifier<Person>()
                    {
                        Value = null
                    },
                    Comments = new List<ResourceIdentifier<Comment>>
                    {
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
        public void When_relationshp_with_resource_identifier_should_serialize()
        {
            var root = DocumentRoot.Create(new
            {
                Id = "1234",
                Type = "articles",
                Title = "My Article",
                Author = new Relationship<ResourceIdentifier<Person>>(){
                    Data = new ResourceIdentifier<Person>()
                    {
                        Meta = new Meta { { "primary-author", true } },
                        Value = new Person
                        {
                            Id = "333",
                            Type = "people",
                            FirstName = "John",
                            LastName = "Smith",
                            Twitter = "jsmi",
                            Meta = new Meta() { { "best-sellers", 4 } }

                        }
                    },
                    Meta = new Meta { { "self-published", true } }
                    
                },
                Comments = Relationship.Create(new List<ResourceIdentifier<Comment>>
                    {
                        ResourceIdentifier.Create(new Comment()
                        {
                            Id = "57",
                            Type = "comments",
                            Body = "I give it a 5 out of 7"
                        })
                    })
            });

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
                                ""type"":""people"",
                                ""meta"":{ ""primary-author"": true}
                            },
                            ""meta"": {""self-published"": true},
                        },
                        ""comments"": {
                            ""data"": [{ 
                                ""id"":""57"", 
                                ""type"":""comments"",
                            }]
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
                        },
                        ""meta"":{ ""best-sellers"": 4}

                    },
                    {
                        ""id"": ""57"",
                        ""type"": ""comments"",
                        ""attributes"":{
                            ""body"": ""I give it a 5 out of 7""
                        }
                    }
                ]
            }";
            Assert.Equal(expectedjson, json, JsonStringEqualityComparer.Instance);
        }


    }
}
