using JsonApiSerializer.Test.Models.Articles;
using JsonApiSerializer.Test.TestUtils;
using Newtonsoft.Json;
using Xunit;

namespace JsonApiSerializer.Test.DeserializationTests
{
    public class DeserializationAttributeTests
    {
        [Fact]
        public void When_fields_controlled_by_jsonnet_ignore_attribute_should_ignore()
        {
            var json = EmbeddedResource.Read("Data.Articles.single-item.json");

            var settings = new JsonApiSerializerSettings();
            var article = JsonConvert.DeserializeObject<ArticleWithIgnoredProperties>(
                json,
                new JsonApiSerializerSettings());

            Assert.Equal("1", article.Id);
            Assert.Null(article.Title); //ignored

            var author = article.Author;
            Assert.Equal("9", author.Id);
            Assert.Equal("Dan", author.FirstName);
            Assert.Equal("Gebhardt", author.LastName);
            Assert.Equal("dgeb", author.Twitter);

            Assert.Null(article.Comments); //ignored
        }

        [Fact]
        public void When_fields_controlled_by_jsonapi_id_attribute_parse_correct()
        {
            var json = EmbeddedResource.Read("Data.Articles.single-item-mapped-attributes.json");

            var settings = new JsonApiSerializerSettings();
            var article = JsonConvert.DeserializeObject<ArticleWithJsonApiAttributes>(
                json,
                new JsonApiSerializerSettings());

            Assert.Equal("1", article.InternalId);
            Assert.Equal("JSON API paints my bikeshed!", article.Title);
            Assert.Equal(123, article.Id);
            Assert.Equal("blog", article.Type);

            Assert.Null(article.Comments); //ignored
        }

        [Fact]
        public void When_fields_controlled_by_jsonapi_id_attribute_parse_correct_included()
        {
            var json = EmbeddedResource.Read("Data.Articles.sample-with-mapped-attributes.json");

            var settings = new JsonApiSerializerSettings();
            var article = JsonConvert.DeserializeObject<ArticleWithIgnoredPropertiesPersonWithJsonApiAttributes>(
                json,
                new JsonApiSerializerSettings());

            Assert.Equal("1", article.Id);
            Assert.Null(article.Title); //ignored

            var author = article.Author;
            Assert.Equal(123, author.Id);
            Assert.Equal("dan@test.com", author.Email);
            Assert.Equal("sample", author.Type);

            Assert.Null(article.Comments); //ignored
        }

        [Fact]
        public void When_fields_are_only_getters_should_ignore()
        {
            var json = EmbeddedResource.Read("Data.Articles.single-item.json");

            var settings = new JsonApiSerializerSettings();
            var article = JsonConvert.DeserializeObject<ArticleWithGetters>(
                json,
                new JsonApiSerializerSettings());

            Assert.Equal("1", article.Id);
            Assert.Null(article.Title); //ignored

            var author = article.Author;
            Assert.Equal("9", author.Id);
            Assert.Equal("Dan", author.FirstName);
            Assert.Equal("Gebhardt", author.LastName);
            Assert.Equal("dgeb", author.Twitter);

            Assert.Null(article.Comments); //ignored
        }

    }
}
