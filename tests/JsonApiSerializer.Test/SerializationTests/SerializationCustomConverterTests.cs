using JsonApiSerializer.Test.Models.Timer;
using JsonApiSerializer.Test.TestUtils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using JsonApiSerializer.JsonApi;
using JsonApiSerializer.JsonConverters;
using JsonApiSerializer.Test.Models.Articles;
using Xunit;

namespace JsonApiSerializer.Test.SerializationTests
{
    public class SerializationCustomConverterTests
    {
        [Fact]
        public void When_custom_convertor_should_use_on_attributes()
        {
            var settings = new JsonApiSerializerSettings()
            {
                Formatting = Formatting.Indented, //pretty print makes it easier to debug
            };

            var root = new Timer
            {
                Id = "root",
                Duration = TimeSpan.FromSeconds(42),
                SubTimer = new List<Timer>
                {
                    new Timer
                    {
                        Id="sub1",
                        Duration = TimeSpan.FromSeconds(142)
                    }
                }
            };
            

            var json = JsonConvert.SerializeObject(root, settings);

            Assert.Equal(@"{
  ""data"": {
    ""type"": ""timer"",
    ""id"": ""root"",
    ""attributes"": {
                ""duration"": 42
    },
    ""relationships"": {
                ""subTimer"": {
                    ""data"": [
                      {
            ""id"": ""sub1"",
                        ""type"": ""timer""
          }
        ]
      }
    }
  },
  ""included"": [
    {
      ""type"": ""timer"",
      ""id"": ""sub1"",
      ""attributes"": {
        ""duration"": 142
      }
    }
  ]
}", json, JsonStringEqualityComparer.Instance);
        }
    }
}

