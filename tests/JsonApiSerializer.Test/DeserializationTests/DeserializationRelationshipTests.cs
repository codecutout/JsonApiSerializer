using JsonApiSerializer.Exceptions;
using JsonApiSerializer.JsonApi;
using JsonApiSerializer.Test.Models.Articles;
using JsonApiSerializer.Test.TestUtils;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using Xunit;

namespace JsonApiSerializer.Test.DeserializationTests
{
    public class DeserializationRelationshipTests
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

        [Fact]
        public void When_null_should_return_as_or_empty_list()
        {
            var json = EmbeddedResource.Read("Data.Articles.author-comments-null.json");

            var articles = JsonConvert.DeserializeObject<Article[]>(json, new JsonApiSerializerSettings());
            var article = articles[0];
            Assert.Null(article.Author);
            Assert.Empty(article.Comments);
        }

        [Fact]
        public void When_relationship_dictionary_should_deserialize()
        {
            var json = EmbeddedResource.Read("Data.Articles.sample.json");

            var articlesRoot = JsonConvert.DeserializeObject<DocumentRoot<ArticleWithRelationshipDictionary[]>>(
                json,
                new JsonApiSerializerSettings());


            var comments = (dynamic)articlesRoot.Data[0].Relationships["comments"].Data;
            Assert.Equal("5", comments[0].id.ToString());
            Assert.Equal("comments", comments[0].type.ToString());
            Assert.Equal("12", comments[1].id.ToString());
            Assert.Equal("comments", comments[1].type.ToString());

            var author = (dynamic)articlesRoot.Data[0].Relationships["author"].Data;
            Assert.Equal("9", author.id.ToString());
            Assert.Equal("people", author.type.ToString());

            Assert.Equal(3, articlesRoot.Included.Count);
            Assert.Contains(articlesRoot.Included, x => x["id"].ToString() == "5" && x["type"].ToString() == "comments");
            Assert.Contains(articlesRoot.Included, x => x["id"].ToString() == "12" && x["type"].ToString() == "comments");
            Assert.Contains(articlesRoot.Included, x => x["id"].ToString() == "9" && x["type"].ToString() == "people");
        }

        [Fact]
        public void When_single_serializer_should_reference_relationships()
        {
            var json = EmbeddedResource.Read("Data.Articles.sample.json");

            var serializer = JsonSerializer.Create(new JsonApiSerializerSettings());

            var articles1 = serializer.Deserialize<Article[]>(new JsonTextReader(new StringReader(json)));
            var article1 = articles1[0];

            var articles2 = serializer.Deserialize<Article[]>(new JsonTextReader(new StringReader(json)));
            var article2 = articles2[0];

            //Check article1 is deserialized correctly
            Assert.Equal("9", article1.Author.Id);
            Assert.Equal("Dan", article1.Author.FirstName);
            Assert.Equal("Gebhardt", article1.Author.LastName);
            Assert.Equal("dgeb", article1.Author.Twitter);

            var comments1 = article1.Comments;
            Assert.Equal(2, comments1.Count);
            Assert.Equal("First!", comments1[0].Body);
            Assert.Equal("I like XML better", comments1[1].Body);


            //Check article2 is deserialized correctly
            Assert.Equal("9", article2.Author.Id);
            Assert.Equal("Dan", article2.Author.FirstName);
            Assert.Equal("Gebhardt", article2.Author.LastName);
            Assert.Equal("dgeb", article2.Author.Twitter);

            var comments2 = article2.Comments;
            Assert.Equal(2, comments2.Count);
            Assert.Equal("First!", comments2[0].Body);
            Assert.Equal("I like XML better", comments2[1].Body);
        }

        [Fact]
        public void When_dataless_relationship_should_deserialize()
        {
            var json = @"
{  
    ""data"": {
    ""attributes"":{
        ""title"":""Test"",
    },
    ""relationships"":{
        ""comments"":{
            ""links"":{
                ""self"":""https://localhost/api/articles/1/comments"",
            }
        }
    },
    ""type"":""articles"",
    ""id"":""1""
    }
}";
            var obj = JsonConvert.DeserializeObject<Article>(json, new JsonApiSerializerSettings());

            Assert.Equal("Test", obj.Title);
            Assert.Null(obj.Comments);
        }

        [Fact]
        public void When_empty_relationship_should_throw()
        {
            var json = @"
{  
    ""data"": {
    ""attributes"":{
        ""title"":""Test"",
    },
    ""relationships"":{
        ""comments"":{
           
        }
    },
    ""type"":""articles"",
    ""id"":""1""
    }
}";
            Assert.Throws<JsonApiFormatException>(() => JsonConvert.DeserializeObject<Article>(json, new JsonApiSerializerSettings()));
        }
    }
   
}
