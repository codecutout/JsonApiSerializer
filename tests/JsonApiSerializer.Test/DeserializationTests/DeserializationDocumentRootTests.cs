using JsonApiSerializer.JsonApi;
using JsonApiSerializer.Test.Models.Articles;
using JsonApiSerializer.Test.TestUtils;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using JsonApiSerializer.Test.Models.Products;
using Xunit;

namespace JsonApiSerializer.Test.DeserializationTests
{
    public class DeserializationDocumentRootTests
    {
        [Fact]
        public void When_object_root_with_array_should_deserialize()
        {
            var json = EmbeddedResource.Read("Data.Articles.sample.json");

            var settings = new JsonApiSerializerSettings();
            var articles = JsonConvert.DeserializeObject<Article[]>(
                json,
                new JsonApiSerializerSettings());

            AssertArticlesMatchData(articles);
        }

        [Fact]
        public void When_included_before_data__with_recursive_references_should_deserialize()
        {
            var json = EmbeddedResource.Read("Data.Products.sample-included-first-recursive.json");

            var category = JsonConvert.DeserializeObject<Category>(
                json,
                new JsonApiSerializerSettings());

            Assert.NotNull(category);
            Assert.Equal("MyCategory", category.Name);
            Assert.Single(category.Products);
            Assert.Equal("MyCategory", category.Products[0].Category.Name);
        }

        [Fact]
        public void When_object_root_with_list_should_deserialize()
        {
            var json = EmbeddedResource.Read("Data.Articles.sample.json");

            var articles = JsonConvert.DeserializeObject<List<Article>>(
                json,
                new JsonApiSerializerSettings());

            AssertArticlesMatchData(articles);
        }

        [Fact]
        public void When_object_root_with_enumerable_should_deserialize()
        {
            var json = EmbeddedResource.Read("Data.Articles.sample.json");

            var articles = JsonConvert.DeserializeObject<IEnumerable<Article>>(
                json,
                new JsonApiSerializerSettings());

            AssertArticlesMatchData(articles);
        }

        [Fact]
        public void When_document_root_with_array_should_deserialize()
        {
            var json = EmbeddedResource.Read("Data.Articles.sample.json");

            var articlesRoot = JsonConvert.DeserializeObject<DocumentRoot<Article[]>>(
                json, 
                new JsonApiSerializerSettings());

            AssertArticlesMatchData(articlesRoot.Data);
        }

        [Fact]
        public void When_document_root_with_null_link_should_deserialize()
        {
            var json = EmbeddedResource.Read("Data.Articles.sample-with-link-null.json");

            var articlesRoot = JsonConvert.DeserializeObject<DocumentRoot<Article[]>>(
                json,
                new JsonApiSerializerSettings());

            Assert.Equal(4, articlesRoot.Links.Count);
            Assert.NotNull(articlesRoot.Links["self"]);
            Assert.NotNull(articlesRoot.Links["next"]);
            Assert.NotNull(articlesRoot.Links["last"]);
            Assert.NotNull(articlesRoot.Links["prev"]);
            Assert.Null(articlesRoot.Links["prev"].Href);
        }

        [Fact]
        public void When_document_root_with_list_should_deserialize()
        {
            var json = EmbeddedResource.Read("Data.Articles.sample.json");

            var articlesRoot = JsonConvert.DeserializeObject<DocumentRoot<List<Article>>>(
                json,
                new JsonApiSerializerSettings());

            AssertArticlesMatchData(articlesRoot.Data);
        }

        [Fact]
        public void When_document_root_with_enumerable_should_deserialize()
        {
            var json = EmbeddedResource.Read("Data.Articles.sample.json");

            var articlesRoot = JsonConvert.DeserializeObject<DocumentRoot<IEnumerable<Article>>>(
                json,
                new JsonApiSerializerSettings());

            AssertArticlesMatchData(articlesRoot.Data);
        }

        [Fact]
        public void When_single_item_should_deserialize()
        {
            var json = EmbeddedResource.Read("Data.Articles.single-item.json");

            var article = JsonConvert.DeserializeObject<Article>(json, new JsonApiSerializerSettings());

            AssertArticlesMatchData(new[] {article });
        }

        [Fact]
        public void When_single_item_treated_as_array_should_deserialize_as_single_element_array()
        {
            var json = EmbeddedResource.Read("Data.Articles.single-item.json");

            var articles = JsonConvert.DeserializeObject<Article[]>(json, new JsonApiSerializerSettings());

            AssertArticlesMatchData(articles);
        }

        [Fact]
        public void When_json_order_unconventional_should_deserialize()
        {
            var json = EmbeddedResource.Read("Data.Articles.sample-out-of-order.json");

            var articles = JsonConvert.DeserializeObject<Article[]>(
                json,
                new JsonApiSerializerSettings());

            AssertArticlesMatchData(articles);
        }

        [Fact]
        public void When_json_null_should_deserialize()
        {
            var article = JsonConvert.DeserializeObject<Article>(
                @"{""data"": null}",
                new JsonApiSerializerSettings());

            Assert.Null(article);
        }

        [Fact]
        public void When_list_json_null_should_deserialize_as_empty()
        {
            var articles = JsonConvert.DeserializeObject<Article[]>(
                @"{""data"": null}",
                new JsonApiSerializerSettings());

            Assert.Empty(articles);
        }



        private void AssertArticlesMatchData<T>(DocumentRoot<T> articleRoot) where T : IEnumerable<Article>
        {
            Assert.Equal("http://example.com/articles", articleRoot.Links["self"].Href);
            Assert.Equal("http://example.com/articles?page[offset]=2", articleRoot.Links["next"].Href);
            Assert.Equal("http://example.com/articles?page[offset]=10", articleRoot.Links["last"].Href);
            AssertArticlesMatchData(articleRoot.Data);
        }

        private void AssertArticlesMatchData(IEnumerable<Article> articles)
        {
            var articlesList = articles as List<Article> ?? articles.ToList();
            Assert.Single(articlesList);

            var article = articlesList[0];
            Assert.Equal("1", article.Id);
            Assert.Equal("JSON API paints my bikeshed!", article.Title);

            var author = article.Author;
            Assert.Equal("9", author.Id);
            Assert.Equal("Dan", author.FirstName);
            Assert.Equal("Gebhardt", author.LastName);
            Assert.Equal("dgeb", author.Twitter);

            var comments = article.Comments;
            Assert.Equal(2, comments.Count);
            Assert.Equal("First!", comments[0].Body);
            Assert.Equal("I like XML better", comments[1].Body);
        }
    }
}
