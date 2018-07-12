using JsonApiSerializer.Exceptions;
using JsonApiSerializer.Test.Models.Articles;
using JsonApiSerializer.Test.TestUtils;
using Newtonsoft.Json;
using Xunit;

namespace JsonApiSerializer.Test.DeserializationTests
{
    public class DeserializationExceptionTests
    {
        [Fact]
        public void When_type_not_a_string_should_throw()
        {
            var json = EmbeddedResource.Read("Data.Articles.sample-error-type-not-string.json");
            var settings = new JsonApiSerializerSettings();

            var exception = Assert.Throws<JsonApiFormatException>(() => JsonConvert.DeserializeObject<Article[]>(
                json,
                settings));

            Assert.Equal("data[0].type", exception.Path);
        }

        [Fact]
        public void When_id_not_a_string_should_throw()
        {
            var json = EmbeddedResource.Read("Data.Articles.sample-error-id-not-string.json");
            var settings = new JsonApiSerializerSettings();

            var exception = Assert.Throws<JsonApiFormatException>(() => JsonConvert.DeserializeObject<Article[]>(
                json,
                settings));

            Assert.Equal("data[0].relationships.comments.data[1].id", exception.Path);
        }

        [Fact]
        public void When_data_element_missing_should_throw()
        {
            var json = EmbeddedResource.Read("Data.Articles.sample-error-missing-data-element.json");
            var settings = new JsonApiSerializerSettings();

            var exception = Assert.Throws<JsonApiFormatException>(() => JsonConvert.DeserializeObject<Article[]>(
                json,
                settings));

            Assert.Equal("data[0].relationships.author", exception.Path);
        }

        [Fact]
        public void When_relationships_not_an_object_should_throw()
        {
            var json = EmbeddedResource.Read("Data.Articles.sample-error-relationship-not-object.json");
            var settings = new JsonApiSerializerSettings();

            var exception = Assert.Throws<JsonApiFormatException>(() => JsonConvert.DeserializeObject<Article[]>(
                json,
                settings));

            Assert.Equal("data[0].relationships", exception.Path);
        }


        [Fact]
        public void When_attributes_not_object_should_throw()
        {
            var json = EmbeddedResource.Read("Data.Articles.sample-error-attributes-not-object.json");
            var settings = new JsonApiSerializerSettings();

            var exception = Assert.Throws<JsonApiFormatException>(() => JsonConvert.DeserializeObject<Article[]>(
                json,
                settings));

            Assert.Equal("data[0].attributes", exception.Path);
        }

        [Fact]
        public void When_model_does_not_match_json_should_throw_serialization_exception()
        {
            var json = EmbeddedResource.Read("Data.Articles.sample-error-model-not-match-values.json");
            var settings = new JsonApiSerializerSettings();

            var exception = Assert.Throws<Newtonsoft.Json.JsonSerializationException>(() => JsonConvert.DeserializeObject<Article[]>(
                json,
                settings));
        }

        [Fact]
        public void When_model_references_same_object_withdifferent_type_shoud_throw_exception()
        {
            var json = EmbeddedResource.Read("Data.Articles.sample-error-two-class-single-include.json");
            var settings = new JsonApiSerializerSettings();

            var exception = Assert.Throws<Newtonsoft.Json.JsonSerializationException>(() => JsonConvert.DeserializeObject<Article[]>(
                json,
                settings));
        }

        [Fact]
        public void When_array_deserize_as_object_should_throw_exception()
        {
            var json = EmbeddedResource.Read("Data.Articles.sample.json");
            var settings = new JsonApiSerializerSettings();

            var exception = Assert.Throws<JsonApiFormatException>(() => JsonConvert.DeserializeObject<Article>(
                json,
                settings));

            Assert.Equal("data", exception.Path);
        }
    }
}
