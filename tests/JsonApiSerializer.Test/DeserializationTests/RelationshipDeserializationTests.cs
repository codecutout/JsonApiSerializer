using JsonApiSerializer.Test.Models.Articles;
using JsonApiSerializer.Test.TestUtils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace JsonApiSerializer.Test.DeserializationTests
{
    public class RelationshipDeserializationTests
    {
        [Fact]
        public void When_reference_same_object_should_have_deserialize_same_object()
        {
            var json = EmbeddedResource.Read("Data.Articles.sample.json");

            var articles = JsonConvert.DeserializeObject<Article[]>(json, new JsonApiSerializerSettings());

            Assert.Equal(articles[0].Author, articles[0].Comments[1].Author);
        }

        [Fact]
        public void When_reference_relationship_should_deserialize_links()
        {
            var json = EmbeddedResource.Read("Data.Articles.sample.json");

            var articles = JsonConvert.DeserializeObject<ArticleWithRelationship[]>(json, new JsonApiSerializerSettings());

            var article = articles[0];

            Assert.Equal("http://example.com/articles/1/relationships/author", article.Author.Links["self"].Href);
            Assert.Equal("http://example.com/articles/1/author", article.Author.Links["related"].Href);
            Assert.Equal("9", article.Author.Data.Id);
            Assert.Equal("Dan", article.Author.Data.FirstName);
            Assert.Equal("Gebhardt", article.Author.Data.LastName);
            Assert.Equal("dgeb", article.Author.Data.Twitter);

            Assert.Equal("http://example.com/articles/1/relationships/comments", article.Comments.Links["self"].Href);
            Assert.Equal("http://example.com/articles/1/comments", article.Comments.Links["related"].Href);
            var comments = article.Comments.Data;
            Assert.Equal(2, comments.Count);
            Assert.Equal("First!", comments[0].Body);
            Assert.Equal("I like XML better", comments[1].Body);
        }
    }
}
