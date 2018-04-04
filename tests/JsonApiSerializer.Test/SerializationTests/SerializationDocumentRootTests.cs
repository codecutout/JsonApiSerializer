using JsonApiSerializer.Exceptions;
using JsonApiSerializer.JsonApi;
using JsonApiSerializer.Test.Models.Articles;
using JsonApiSerializer.Test.TestUtils;
using Newtonsoft.Json;
using Xunit;

namespace JsonApiSerializer.Test.SerializationTests
{
    public class SerializationDocumentRootTests
    {

        public JsonApiSerializerSettings settings = new JsonApiSerializerSettings()
        {
            Formatting = Formatting.Indented, //pretty print makes it easier to debug
        };

        [Fact]
        public void When_document_root_should_serialize_fields()
        {
            var root = new DocumentRoot<Person>
            {
                Data = new Person
                {
                    Id = "1234",
                    FirstName = "John",
                    LastName = "Smith",
                    Twitter = "jsmi"
                }
            };

            var json = JsonConvert.SerializeObject(root,settings);

            Assert.Equal(@"{
                data : {
                    ""id"" : ""1234"",
                    ""type"" : ""people"",
                    ""attributes"" : {
                        ""first-name"" : ""John"",
                        ""last-name"" : ""Smith"",
                        ""twitter"" : ""jsmi""
                    }
                }
            }", json, JsonStringEqualityComparer.Instance);
        }

        [Fact]
        public void When_document_root_array_should_serialize_fields()
        {
            var root = new DocumentRoot<Person[]>
            {
                Data = new[] {
                    new Person
                    {
                        Id = "1234",
                        FirstName = "John",
                        LastName = "Smith",
                        Twitter = "jsmi"
                    },
                    new Person
                    {
                        Id = "1235",
                        FirstName = "Jane",
                        LastName = "Smith",
                        Twitter = "janesmi"
                    }
                }
            };
        

            var json = JsonConvert.SerializeObject(root, settings);

            Assert.Equal(@"{
                data : [
                    {
                        ""id"" : ""1234"",
                        ""type"" : ""people"",
                        ""attributes"" : {
                            ""first-name"" : ""John"",
                            ""last-name"" : ""Smith"",
                            ""twitter"" : ""jsmi""
                        }
                    },
                    {
                        ""id"" : ""1235"",
                        ""type"" : ""people"",
                        ""attributes"" : {
                            ""first-name"" : ""Jane"",
                            ""last-name"" : ""Smith"",
                            ""twitter"" : ""janesmi""
                        }
                    }
                ]
            }", json, JsonStringEqualityComparer.Instance);
        }

        [Fact]
        public void When_document_root_array_empty_should_serialize_fields()
        {
            var root = new DocumentRoot<Person[]>
            {
                Data = new Person[0]
            };


            var json = JsonConvert.SerializeObject(root, settings);

            Assert.Equal(@"{
                data : []
            }", json, JsonStringEqualityComparer.Instance);
        }

        [Fact]
        public void When_document_root_null_should_serialize_as_data_null()
        {
            var root = new DocumentRoot<Person>
            {
                Data = null
            };


            var json = JsonConvert.SerializeObject(root, settings);

            Assert.Equal(@"{
                data : null
            }", json, JsonStringEqualityComparer.Instance);
        }

        [Fact]
        public void When_object_root_should_serialize_fields()
        {
            var root = new Person
            {
                Id = "1234",
                FirstName = "John",
                LastName = "Smith",
                Twitter = "jsmi"
            };

            var json = JsonConvert.SerializeObject(root, settings);

            Assert.Equal(@"{
                data : {
                    ""id"" : ""1234"",
                    ""type"" : ""people"",
                    ""attributes"" : {
                        ""first-name"" : ""John"",
                        ""last-name"" : ""Smith"",
                        ""twitter"" : ""jsmi""
                    }
                }
            }", json, JsonStringEqualityComparer.Instance);
        }

        [Fact]
        public void When_object_root_array_should_serialize_fields()
        {
            var root = new[] {
                new Person
                {
                    Id = "1234",
                    FirstName = "John",
                    LastName = "Smith",
                    Twitter = "jsmi"
                },
                new Person
                {
                    Id = "1235",
                    FirstName = "Jane",
                    LastName = "Smith",
                    Twitter = "janesmi"
                }
            };

            var json = JsonConvert.SerializeObject(root, settings);

            Assert.Equal(@"{
                data : [
                    {
                        ""id"" : ""1234"",
                        ""type"" : ""people"",
                        ""attributes"" : {
                            ""first-name"" : ""John"",
                            ""last-name"" : ""Smith"",
                            ""twitter"" : ""jsmi""
                        }
                    },
                    {
                        ""id"" : ""1235"",
                        ""type"" : ""people"",
                        ""attributes"" : {
                            ""first-name"" : ""Jane"",
                            ""last-name"" : ""Smith"",
                            ""twitter"" : ""janesmi""
                        }
                    }
                ]
            }", json, JsonStringEqualityComparer.Instance);
        }

        [Fact]
        public void When_object_root_array_empty_should_serialize_fields()
        {
            var root = new Person[0];
          
            var json = JsonConvert.SerializeObject(root, settings);

            Assert.Equal(@"{
                data : []
            }", json, JsonStringEqualityComparer.Instance);
        }

        [Fact]
        public void When_explicit_document_root_data_is_of_type_object_list_and_contains_resources_should_serialize()
        {
            var root = new DocumentRoot<object[]>
            {
                Data = new object[] {
                    new
                    {
                        Id = "p1",
                        Type = "people",
                        FirstName = "John",
                        LastName = "Smith"
                    },
                    new
                    {
                        Id = "c1",
                        Type = "company",
                        CompanyName = "John Smith Inc"
                    }
                }
            };


            var json = JsonConvert.SerializeObject(root, settings);

            Assert.Equal(@"{
                data : [
                    {
                        ""id"" : ""p1"",
                        ""type"" : ""people"",
                        ""attributes"" : {
                            ""firstName"" : ""John"",
                            ""lastName"" : ""Smith""
                        }
                    },
                    {
                        ""id"" : ""c1"",
                        ""type"" : ""company"",
                        ""attributes"" : {
                            ""companyName"" : ""John Smith Inc""
                        }
                    }
                ]
            }", json, JsonStringEqualityComparer.Instance);
        }

        [Fact]
        public void When_explicit_document_root_data_is_of_type_object_list_and_contains_non_resources_should_error()
        {
            var root = new DocumentRoot<object[]>
            {
                Data = new object[] {
                    new
                    {
                        Id = "p1",
                        Type = "people",
                        FirstName = "John",
                        LastName = "Smith"
                    },
                    "not-a-resource-object"
                }
            };

            Assert.Throws<JsonApiFormatException>(() => JsonConvert.SerializeObject(root, settings));
        }

        [Fact]
        public void When_explicit_document_root_data_is_of_type_object_and_contains_non_resources_should_error()
        {
            var root = new DocumentRoot<object>
            {
                Data = "not-a-resource-object"
            };

            Assert.Throws<JsonApiFormatException>(() => JsonConvert.SerializeObject(root, settings));
        }
    }
}
