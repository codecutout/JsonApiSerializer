using JsonApiSerializer.JsonApi;
using JsonApiSerializer.Test.Models.Articles;
using JsonApiSerializer.Test.Models.Locations;
using JsonApiSerializer.Test.TestUtils;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JsonApiSerializer.JsonConverters;
using Xunit;

namespace JsonApiSerializer.Test.SerializationTests
{

    public class SerializationEmptyRelationshipBehaviourTests
    {

        [Fact]
        public void When_relationships_empty_should_not_be_in_includes()
        {
            var root = new LocationWithId()
            {
                Id = "Willesdon Green",
                Parents = new[]
                {
                    new LocationWithId()
                    {
                        Id = $"London_1"
                    },
                    new LocationWithId()
                    {
                        Id = $"London_2"
                    },
                    new LocationWithId()
                    {
                        Id = $"London_3"
                    }
                }
            };

            var json = JsonConvert.SerializeObject(root, new JsonApiSerializerSettings()
            {
                Formatting = Formatting.Indented //pretty print makes it easier to debug
            });

            Assert.Equal(@"{
            ""data"": {
                ""type"": ""locationwithid"",
                ""id"": ""Willesdon Green"",
                ""relationships"": {
                    ""parents"": {
                        ""data"": [
                            {
                                ""id"": ""London_1"",
                                ""type"": ""locationwithid""
                            },
                            {
                                ""id"": ""London_2"",
                                ""type"": ""locationwithid""
                            },
                            {
                                ""id"": ""London_3"",
                                ""type"": ""locationwithid""
                            }
                        ]
                    }
                }
            }
        }", json, JsonStringEqualityComparer.Instance);
        }

        [Fact]
        public void When_relationships_empty_configured_to_be_included_should_be_in_includes()
        {
            var root = new LocationWithId()
            {
                Id = "Willesdon Green",
                Parents = new[]
                {
                    new LocationWithId()
                    {
                        Id = $"London_1"
                    },
                    new LocationWithId()
                    {
                        Id = $"London_2"
                    },
                    new LocationWithId()
                    {
                        Id = $"London_3"
                    }
                }
            };

            var json = JsonConvert.SerializeObject(root, new JsonApiSerializerSettings(new ResourceObjectConverter()
            {
                EmptyResourceObjectRelationshipSerializationBehaviour = EmptyResourceObjectRelationshipSerializationBehaviour.AppendToIncludes
            })
            {
                Formatting = Formatting.Indented //pretty print makes it easier to debug
            });

            Assert.Equal(@"{
  ""data"": {
    ""type"": ""locationwithid"",
    ""id"": ""Willesdon Green"",
    ""relationships"": {
      ""parents"": {
        ""data"": [
          {
            ""id"": ""London_1"",
            ""type"": ""locationwithid""
          },
          {
            ""id"": ""London_2"",
            ""type"": ""locationwithid""
          },
          {
            ""id"": ""London_3"",
            ""type"": ""locationwithid""
          }
        ]
      }
    }
  },
  ""included"": [
    {
      ""type"": ""locationwithid"",
      ""id"": ""London_1""
    },
    {
      ""type"": ""locationwithid"",
      ""id"": ""London_2""
    },
    {
      ""type"": ""locationwithid"",
      ""id"": ""London_3""
    }
  ]
}", json, JsonStringEqualityComparer.Instance);
        }
    }
}
