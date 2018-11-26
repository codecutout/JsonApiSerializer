using JsonApiSerializer.JsonApi;
using JsonApiSerializer.Test.Models.Articles;
using JsonApiSerializer.Test.TestUtils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace JsonApiSerializer.Test.DeserializationTests
{
    public class DeserializationResourceIdentifierTests
    {
        public class Lover
        {
            public string Id { get; set; }
            public ResourceIdentifier<Lover> Loves { get; set; }
        }

        [Fact]
        public void When_resource_identifer_used_should_deserialize_object()
        {
            var json = EmbeddedResource.Read("Data.Articles.sample.json");

            var articles = JsonConvert.DeserializeObject<ArticleWithResourceIdentifier[]>(json, new JsonApiSerializerSettings());

            var article = articles[0];

            var author = article.Author.Value;
            Assert.Equal("9", author.Id);
            Assert.Equal("Dan", author.FirstName);
            Assert.Equal("Gebhardt", author.LastName);
            Assert.Equal("dgeb", author.Twitter);

            var comments = article.Comments;
            Assert.Equal(2, comments.Count);
            Assert.Equal("5", comments[0].Value.Id);
            Assert.Equal("First!", comments[0].Value.Body);

            Assert.Equal("12", comments[1].Value.Id);
            Assert.Equal("I like XML better", comments[1].Value.Body);
        }

        [Fact]
        public void When_resource_identifer_not_reference_included_object_should_deserialize_null()
        {
            var json = EmbeddedResource.Read("Data.Articles.sample-without-included.json");

            var articles = JsonConvert.DeserializeObject<ArticleWithResourceIdentifier[]>(json, new JsonApiSerializerSettings());

            var article = articles[0];
           
            Assert.True(article.Author.Meta["external"].ToObject<bool>());
            Assert.Null(article.Author.Value);

            var comments = article.Comments;
            Assert.Equal(2, comments.Count);

            Assert.Equal("validated", comments[0].Meta["status"].ToObject<string>());
            Assert.Null(comments[0].Value);

            Assert.Null(comments[1].Meta);
            Assert.Null(comments[1].Value);
        }

        [Fact]
        public void When_resource_identifer_used_and_included_first_should_deserialize_object()
        {
            var json = EmbeddedResource.Read("Data.Articles.sample-out-of-order.json");

            var articles = JsonConvert.DeserializeObject<ArticleWithResourceIdentifier[]>(json, new JsonApiSerializerSettings());

            var article = articles[0];

            var author = article.Author.Value;
            Assert.Equal("9", author.Id);
            Assert.Equal("Dan", author.FirstName);
            Assert.Equal("Gebhardt", author.LastName);
            Assert.Equal("dgeb", author.Twitter);

            var comments = article.Comments;
            Assert.Equal(2, comments.Count);
            Assert.Equal("5", comments[0].Value.Id);
            Assert.Equal("First!", comments[0].Value.Body);

            Assert.Equal("12", comments[1].Value.Id);
            Assert.Equal("I like XML better", comments[1].Value.Body);
        }

        [Fact]
        public void When_resource_identfier_with_circular_reference_should_deserialize()
        {
            var json = @"
{
    ""data"": {
        ""type"": ""lover"",
        ""id"": ""alice"",
        ""relationships"": {
            ""loves"": {
                ""data"": {
                    ""id"": ""bob"",
                    ""type"": ""lover""
                }
            }
        }
    },
    ""included"": [
    {
        ""type"": ""lover"",
        ""id"": ""bob"",
        ""relationships"": {
            ""loves"": {
                ""data"": {
                    ""id"": ""cedric"",
                    ""type"": ""lover""
                }
            }
        }
    },
    {
        ""type"": ""lover"",
        ""id"": ""cedric"",
        ""relationships"": {
            ""loves"": {
                ""data"": {
                    ""id"": ""alice"",
                    ""type"": ""lover""
                }
            }
        }
    }]
}";
            var lover = JsonConvert.DeserializeObject<Lover>(json, new JsonApiSerializerSettings());

            Assert.Equal(lover, lover.Loves.Value.Loves.Value.Loves.Value);

        }

    }
}
