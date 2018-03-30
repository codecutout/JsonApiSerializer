using JsonApiSerializer.JsonApi;
using JsonApiSerializer.Test.Models.Articles;
using JsonApiSerializer.Test.TestUtils;
using Newtonsoft.Json;
using Xunit;

namespace JsonApiSerializer.Test.DeserializationTests
{
    public class DeserializationErrorTests
    {
        [Fact]
        public void When_error_should_error_list_deserialize()
        {
            var json = EmbeddedResource.Read("Data.Errors.single.json");

            var errors = JsonConvert.DeserializeObject<Error[]>(
                json,
                new JsonApiSerializerSettings());

            Assert.Single(errors);
            var error = errors[0];
            Assert.Equal("Invalid Attribute", error.Title);
            Assert.Equal("First name must contain at least three characters.", error.Detail);
            Assert.Equal("/data/attributes/first-name", error.Source.Pointer);
        }

        [Fact]
        public void When_error_should_error_single_deserialize_first_error()
        {
            var json = EmbeddedResource.Read("Data.Errors.single.json");

            var error = JsonConvert.DeserializeObject<Error>(
                json,
                new JsonApiSerializerSettings());

            Assert.Equal("Invalid Attribute", error.Title);
            Assert.Equal("First name must contain at least three characters.", error.Detail);
            Assert.Equal("/data/attributes/first-name", error.Source.Pointer);
        }

        [Fact]
        public void When_error_should_deserialize_null_object()
        {
            var json = EmbeddedResource.Read("Data.Errors.single.json");

            var articles = JsonConvert.DeserializeObject<Article[]>(
                json,
                new JsonApiSerializerSettings());

            Assert.Null(articles);
           
        }

        [Fact]
        public void When_error_should_deserialize_all_errors_object()
        {
            var json = EmbeddedResource.Read("Data.Errors.multiple.json");

            var errors = JsonConvert.DeserializeObject<Error[]>(
                json,
                new JsonApiSerializerSettings());

            Assert.Equal(3, errors.Length);

            var error1 = errors[0];
            Assert.Equal("Value is too short", error1.Title);
            Assert.Equal("First name must contain at least three characters.", error1.Detail);
            Assert.Equal("/data/attributes/first-name", error1.Source.Pointer);
            Assert.Equal("123", error1.Code);

            var error2 = errors[1];
            Assert.Equal("Passwords must contain a letter, number, and punctuation character.", error2.Title);
            Assert.Equal("The password provided is missing a punctuation character.", error2.Detail);
            Assert.Equal("/data/attributes/password", error2.Source.Pointer);
            Assert.Equal("225", error2.Code);

            var error3 = errors[2];
            Assert.Equal("Password and password confirmation do not match.", error3.Title);
            Assert.Equal("/data/attributes/password", error3.Source.Pointer);
            Assert.Equal("226", error3.Code);

        }

        [Fact]
        public void When_error_should_deserialize_document_root()
        {
            var json = EmbeddedResource.Read("Data.Errors.multiple.json");

            var doc = JsonConvert.DeserializeObject<DocumentRoot<Article>>(
                json,
                new JsonApiSerializerSettings());

            Assert.Equal(3, doc.Errors.Count);

            var error1 = doc.Errors[0];
            Assert.Equal("Value is too short", error1.Title);
            Assert.Equal("First name must contain at least three characters.", error1.Detail);
            Assert.Equal("/data/attributes/first-name", error1.Source.Pointer);
            Assert.Equal("123", error1.Code);

            var error2 = doc.Errors[1];
            Assert.Equal("Passwords must contain a letter, number, and punctuation character.", error2.Title);
            Assert.Equal("The password provided is missing a punctuation character.", error2.Detail);
            Assert.Equal("/data/attributes/password", error2.Source.Pointer);
            Assert.Equal("225", error2.Code);

            var error3 = doc.Errors[2];
            Assert.Equal("Password and password confirmation do not match.", error3.Title);
            Assert.Equal("/data/attributes/password", error3.Source.Pointer);
            Assert.Equal("226", error3.Code);

        }
    }
}
