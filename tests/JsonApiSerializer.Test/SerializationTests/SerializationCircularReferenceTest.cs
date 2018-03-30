using JsonApiSerializer.Test.TestUtils;
using Newtonsoft.Json;
using Xunit;

namespace JsonApiSerializer.Test.SerializationTests
{
    public class SerializationCircularReferenceTest
    {

        public class Lover
        {
            public string Id { get; set; }
            public Lover Loves { get; set; }
        }

        public JsonApiSerializerSettings settings = new JsonApiSerializerSettings()
        {
            Formatting = Formatting.Indented, //pretty print makes it easier to debug
        };

        [Fact]
        public void When_fields_controlled_by_jsonnet_attributes_should_respect_attributes()
        {
            var alice = new Lover { Id = "alice" };
            var bob = new Lover { Id = "bob" };
            var cedric = new Lover { Id = "cedric" };

            //the love triangle
            alice.Loves = bob;
            bob.Loves = cedric;
            cedric.Loves = alice;


            var json = JsonConvert.SerializeObject(alice, settings);

            Assert.Equal(@"
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
}", json, JsonStringEqualityComparer.Instance);
        }



        [Fact]
        public void When_self_reference_should_not_appear_in_includes()
        {
            var alice = new Lover { Id = "alice" };

            //the narcissist
            alice.Loves = alice;


            var json = JsonConvert.SerializeObject(alice, settings);

            Assert.Equal(@"
{
    ""data"": {
        ""type"": ""lover"",
        ""id"": ""alice"",
        ""relationships"": {
            ""loves"": {
                ""data"": {
                    ""id"": ""alice"",
                    ""type"": ""lover""
                }
            }
        }
    }
}", json, JsonStringEqualityComparer.Instance);
        }

        [Fact]
        public void When_included_self_references_should_not_have_additional_include()
        {
            var alice = new Lover { Id = "alice" };
            var bob = new Lover { Id = "bob" };

            //the unrequited love
            alice.Loves = bob;
            bob.Loves = bob;



            var json = JsonConvert.SerializeObject(alice, settings);

            Assert.Equal(@"
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
                    ""id"": ""bob"",
                    ""type"": ""lover""
                }
            }
        }
    }]
}", json, JsonStringEqualityComparer.Instance);
        }

    }
}

