using System.Collections.Generic;
using JsonApiSerializer.Test.TestUtils;
using Newtonsoft.Json;
using Xunit;

namespace JsonApiSerializer.Test.SerializationTests
{
    public class AnonTests
    {
        private JsonApiSerializerSettings settings = new JsonApiSerializerSettings
        {
            Formatting = Formatting.Indented
        };

        [Fact]
        public void When_anonymous_types_should_serialize()
        {
            var root = new
            {
                Id = "hq",
                Type = "Theives-Guild",
                Leader = new
                {
                    Id = "bobby",
                    Type = "Bandit",
                    Hobbies = new[] { "croquet" }
                }
            };

            var json = JsonConvert.SerializeObject(root, settings);
            var expectedjson = @"
{
  ""data"": {
    ""id"": ""hq"",
    ""type"": ""Theives-Guild"",
    ""relationships"": {
      ""leader"": {
        ""data"": {
          ""id"": ""bobby"",
          ""type"": ""Bandit""
        }
      }
    }
  },
  ""included"": [
    {
      ""id"": ""bobby"",
      ""type"": ""Bandit"",
      ""attributes"": {
        ""hobbies"": [
          ""croquet""
        ]
      }
    }
  ]
}";
            Assert.Equal(json, expectedjson, JsonStringEqualityComparer.Instance);
        }

        [Fact]
        public void When_anonymous_types_array_should_serialize()
        {
            var root = new List<object>
            {
                (object)new
                {
                    id = "1",
                    type = "bears",
                    name = "yogi"
                },
                (object)new
                {
                    id = "1",
                    type = "bears",
                    name = "booboo"
                }
            };

            var json = JsonConvert.SerializeObject(root, settings);
            var expectedjson = @"{
  ""data"": [
    {
      ""id"": ""1"",
      ""type"": ""bears"",
      ""attributes"": {
        ""name"": ""yogi""
      }
    },
    {
      ""id"": ""1"",
      ""type"": ""bears"",
      ""attributes"": {
        ""name"": ""booboo""
      }
    }
  ]
}";
            Assert.Equal(json, expectedjson, JsonStringEqualityComparer.Instance);
        }

    }
}
