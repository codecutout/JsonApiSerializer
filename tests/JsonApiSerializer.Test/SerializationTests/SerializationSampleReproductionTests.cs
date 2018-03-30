using JsonApiSerializer.JsonApi;
using JsonApiSerializer.Test.Models.Articles;
using JsonApiSerializer.Test.TestUtils;
using Newtonsoft.Json;
using System.Collections.Generic;
using Xunit;

namespace JsonApiSerializer.Test.SerializationTests
{
    public class SerializationSampleReproductionTests
    {
        public JsonApiSerializerSettings settings = new JsonApiSerializerSettings()
        {
            Formatting = Formatting.Indented //pretty print makes it easier to debug
        };

        [Fact]
        public void Should_be_possible_to_define_sample_document()
        {
            var author = new Person
            {
                Id = "9",
                FirstName = "Dan",
                LastName = "Gebhardt",
                Twitter = "dgeb",
                Links = new Links()
                {
                    {"self", new Link {Href = "http://example.com/people/9" } }
                }
            };

            var root = new DocumentRoot<ArticleWithRelationship[]>
            {
                Links = new Links
                    {
                        {"self", new Link { Href = "http://example.com/articles" } },
                        {"next", new Link { Href = "http://example.com/articles?page[offset]=2" } },
                        {"last", new Link { Href = "http://example.com/articles?page[offset]=10" } }
                    },
                Data = new[] {
                    new ArticleWithRelationship
                    {
                        Id = "1",
                        Type = "articles",
                        Title = "JSON API paints my bikeshed!",
                        Links = new Links
                        {
                            { "self", new Link {Href = "http://example.com/articles/1" } }
                        },
                        Author = new Relationship<Person>
                        {
                            Data = author,
                            Links = new Links
                                {
                                    { "self" , new Link {  Href = "http://example.com/articles/1/relationships/author" } },
                                    { "related", new Link {  Href = "http://example.com/articles/1/author" } }
                                },
                        },
                        Comments = new Relationship<List<Comment>>
                        {
                            Data = new List<Comment>
                            {
                                new Comment
                                {
                                    Type = "comments",
                                    Id = "5",
                                    Body = "First!",
                                    Author = new Person
                                    {
                                        Type = "people",
                                        Id = "2"
                                    },
                                    Links = new Links
                                    {
                                        { "self", new Link {Href = "http://example.com/comments/5" } }
                                    },
                                },
                                new Comment
                                {
                                    Type = "comments",
                                    Id = "12",
                                    Body = "I like XML better",
                                    Author = author,
                                    Links = new Links
                                    {
                                        { "self", new Link {Href = "http://example.com/comments/12" } }
                                    },
                                }
                            },
                            Links = new Links
                            {
                                    {"self", new Link {Href = "http://example.com/articles/1/relationships/comments" } },
                                    { "related", new Link {Href = "http://example.com/articles/1/comments" } }
                            }
                        }
                    }
                }
            };


            var json = JsonConvert.SerializeObject(root, settings);
            var expectedjson = EmbeddedResource.Read("Data.Articles.sample.json");
            Assert.Equal(expectedjson, json, JsonStringEqualityComparer.Instance);
        }
    }
}
