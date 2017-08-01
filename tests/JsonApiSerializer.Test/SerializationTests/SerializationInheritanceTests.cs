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
    public interface IWithId
    {
        string Id { get; set; }
    }

    public interface ILocationWithId : IWithId
    {
    }

    public class LocationWithId : ILocationWithId
    {
        public string Id { get; set; }
        public IEnumerable<ILocationWithId> Parents { get; set; }
    }

    public class SerializationInheritanceTests
    {
        public JsonApiSerializerSettings settings = new JsonApiSerializerSettings()
        {
            Formatting = Formatting.Indented //pretty print makes it easier to debug
        };


        [Fact]
        public void When_nested_structure_should_output_correclty()
        {
            var root = new LocationWithId()
            {
                Id = "Willesdon Green",
                Parents = new ILocationWithId[]
                {
                    new LocationWithId()
                    {
                        Id = "Brent",
                        Parents = new ILocationWithId[]
                        {
                            new LocationWithId()
                            {
                                Id = "London"
                            }
                        }
                    }
                }
            };


            var json = JsonConvert.SerializeObject(root, settings);
           
            Assert.Equal(@"{
            ""data"": {
                ""type"": ""locationwithid"",
                ""id"": ""Willesdon Green"",
                ""relationships"": {
                    ""parents"": {
                        ""data"": [
                        {
                            ""id"": ""Brent"",
                            ""type"": ""locationwithid""
                        }
                        ]
                    }
                }
            },
            ""included"": [
            {
                ""type"": ""locationwithid"",
                ""id"": ""Brent"",
                ""relationships"": {
                    ""parents"": {
                        ""data"": [
                        {
                            ""id"": ""London"",
                            ""type"": ""locationwithid""
                        }
                        ]
                    }
                }
            }
            ]
        }", json, JsonStringEqualityComparer.Instance);
        }
    }
}
