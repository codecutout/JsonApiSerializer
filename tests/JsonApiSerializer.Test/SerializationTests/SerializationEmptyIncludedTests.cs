using JsonApiSerializer.JsonApi;
using JsonApiSerializer.Test.Models.Articles;
using JsonApiSerializer.Test.TestUtils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace JsonApiSerializer.Test.SerializationTests
{

    public class SerializationEmptyIncludedTests
    {
        public class Foo
        {
            public int Id { get; set; }
            public Bar Bar { get; set; }  
        }

        public class Bar
        {
            public int Id { get; set; }
        }

        public JsonApiSerializerSettings settings = new JsonApiSerializerSettings()
        {
            Formatting = Formatting.Indented //pretty print makes it easier to debug
        };

        [Fact]
        public void When_reference_object_has_no_new_data_should_not_create_included_object()
        {
            var json = JsonConvert.SerializeObject(new Foo
            {
                Id = 1,
                Bar = new Bar()
                {
                    Id = 1
                }
            }, settings);


            var expectedjson = @"{
              ""data"": {
                        ""type"": ""foo"",
                        ""id"": 1,
                        ""relationships"": {
                            ""bar"": {
                                ""data"": {
                                    ""id"": 1,
                                    ""type"": ""bar""
                                }
                            }
                        }
                    }
                }";
            Assert.Equal(expectedjson, json, JsonStringEqualityComparer.Instance);
        }
    }
}
