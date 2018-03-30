using System.Collections.Generic;
using System.Linq;
using JsonApiSerializer.JsonApi;
using JsonApiSerializer.Test.TestUtils;
using Newtonsoft.Json;
using Xunit;

namespace JsonApiSerializer.Test.SerializationTests
{
    public class SerializationErrorTests
    {
        [Fact]
        public void When_errors_should_serialize_in_errors_element()
        {
            var json = JsonConvert.SerializeObject(
                new []
                {
                    new Error()
                    {
                        Title = "Invalid Attribute",
                        Detail = "First name must contain at least three characters.",
                        Source = new ErrorSource()
                        {
                            Pointer = "/data/attributes/first-name"
                        }
                    }
                },
                new JsonApiSerializerSettings());

            Assert.Equal(@"
{
  ""errors"": [
    {
      ""source"": { ""pointer"": ""/data/attributes/first-name"" },
      ""title"": ""Invalid Attribute"",
      ""detail"": ""First name must contain at least three characters.""
    }
  ]
}", json, JsonStringEqualityComparer.Instance);
        }

        [Fact]
        public void When_error_should_serialize_in_errors_element()
        {
            var json = JsonConvert.SerializeObject(
                new Error()
                {
                    Title = "Invalid Attribute",
                    Detail = "First name must contain at least three characters.",
                    Source = new ErrorSource()
                    {
                        Pointer = "/data/attributes/first-name"
                    }
                },
                new JsonApiSerializerSettings());

            Assert.Equal(@"
{
  ""errors"": [
    {
      ""source"": { ""pointer"": ""/data/attributes/first-name"" },
      ""title"": ""Invalid Attribute"",
      ""detail"": ""First name must contain at least three characters.""
    }
  ]
}", json, JsonStringEqualityComparer.Instance);
        }

        [Fact]
        public void When_error_null_should_deserialize_empty_object()
        {
            var json = JsonConvert.SerializeObject(
                Enumerable.Empty<Error>(),
                new JsonApiSerializerSettings());

            Assert.Equal(@"
{
  ""errors"": []
}", json, JsonStringEqualityComparer.Instance);

        }

        [Fact]
        public void When_errors_should_serialize_all_errors_object()
        {
            var json = JsonConvert.SerializeObject(
                new[]
                {
                    new Error()
                    {
                        Title = "Value is too short",
                        Detail = "First name must contain at least three characters.",
                        Source = new ErrorSource()
                        {
                            Pointer = "/data/attributes/first-name"
                        },
                        Code = "123"
                    },
                    new Error()
                    {
                        Title = "Passwords must contain a letter, number, and punctuation character.",
                        Detail = "The password provided is missing a punctuation character.",
                        Source = new ErrorSource()
                        {
                            Pointer = "/data/attributes/password"
                        },
                        Code = "225"
                    },
                    new Error()
                    {
                        Title = "Password and password confirmation do not match.",
                        Source = new ErrorSource()
                        {
                            Pointer = "/data/attributes/password"
                        },
                        Code = "226"
                    }
                },
                new JsonApiSerializerSettings());

            Assert.Equal(@"
{
  ""errors"": [
    {
      ""code"":   ""123"",
      ""source"": { ""pointer"": ""/data/attributes/first-name"" },
      ""title"":  ""Value is too short"",
      ""detail"": ""First name must contain at least three characters.""
    },
    {
      ""code"": ""225"",
      ""source"": { ""pointer"": ""/data/attributes/password"" },
      ""title"": ""Passwords must contain a letter, number, and punctuation character."",
      ""detail"": ""The password provided is missing a punctuation character.""
    },
    {
      ""code"":   ""226"",
      ""source"": { ""pointer"": ""/data/attributes/password"" },
      ""title"": ""Password and password confirmation do not match.""
    }
  ]
}", json, JsonStringEqualityComparer.Instance);

        }

        [Fact]
        public void When_document_root_with_error_should_serialize_errors_object()
        {
            var json = JsonConvert.SerializeObject(
                new DocumentRoot<object>()
                {
                    JsonApi = new VersionInfo()
                    {
                        Version = "1.0"
                    },
                    Errors = new List<Error>()
                    {
                        new Error()
                        {
                            Title = "Value is too short",
                            Detail = "First name must contain at least three characters.",
                            Source = new ErrorSource()
                            {
                                Pointer = "/data/attributes/first-name"
                            },
                            Code = "123"
                        },
                        new Error()
                        {
                            Title = "Passwords must contain a letter, number, and punctuation character.",
                            Detail = "The password provided is missing a punctuation character.",
                            Source = new ErrorSource()
                            {
                                Pointer = "/data/attributes/password"
                            },
                            Code = "225"
                        },
                        new Error()
                        {
                            Title = "Password and password confirmation do not match.",
                            Source = new ErrorSource()
                            {
                                Pointer = "/data/attributes/password"
                            },
                            Code = "226"
                        }
                    }
                }
                ,
                new JsonApiSerializerSettings());

            Assert.Equal(@"

{
  ""jsonapi"": { ""version"": ""1.0"" },
  ""errors"": [
    {
      ""code"":   ""123"",
      ""source"": { ""pointer"": ""/data/attributes/first-name"" },
      ""title"":  ""Value is too short"",
      ""detail"": ""First name must contain at least three characters.""
    },
    {
      ""code"": ""225"",
      ""source"": { ""pointer"": ""/data/attributes/password"" },
      ""title"": ""Passwords must contain a letter, number, and punctuation character."",
      ""detail"": ""The password provided is missing a punctuation character.""
    },
    {
      ""code"":   ""226"",
      ""source"": { ""pointer"": ""/data/attributes/password"" },
      ""title"": ""Password and password confirmation do not match.""
    }
  ]
}", json, JsonStringEqualityComparer.Instance);

        }
    }
}
