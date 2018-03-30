using JsonApiSerializer.Test.Models.Locations;
using JsonApiSerializer.Test.TestUtils;
using Newtonsoft.Json;
using System.Linq;
using Xunit;

namespace JsonApiSerializer.Test.SerializationTests
{

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

        [Fact]
        public void When_linqed_list_should_serialize()
        {
            int selectCount = 0;
            var root = new LocationWithId()
            {
                Id = "Willesdon Green",
                Parents = Enumerable.Range(0, 3).Select(i => new LocationWithId()
                {
                    Id = $"London_{i}_{selectCount++}"
                })
            };

            var json = JsonConvert.SerializeObject(root, settings);
            Assert.Equal(3, selectCount); //should evaluate IEnumerable only once
            Assert.Equal(@"{
            ""data"": {
                ""type"": ""locationwithid"",
                ""id"": ""Willesdon Green"",
                ""relationships"": {
                    ""parents"": {
                        ""data"": [
                            {
                                ""id"": ""London_0_0"",
                                ""type"": ""locationwithid""
                            },
                            {
                                ""id"": ""London_1_1"",
                                ""type"": ""locationwithid""
                            },
                            {
                                ""id"": ""London_2_2"",
                                ""type"": ""locationwithid""
                            }
                        ]
                    }
                }
            }
        }", json, JsonStringEqualityComparer.Instance);
        }
    }
}
