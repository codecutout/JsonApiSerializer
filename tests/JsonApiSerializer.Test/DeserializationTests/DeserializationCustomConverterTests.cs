using JsonApiSerializer.Test.Models.Timer;
using Newtonsoft.Json;
using System;
using Xunit;

namespace JsonApiSerializer.Test.SerializationTests
{
    public class DeserializationCustomConverterTests
    {
        [Fact]
        public void When_custom_convertor_should_use_on_attributes()
        {
            var json = @"{
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
}";

            var settings = new JsonApiSerializerSettings();
            var timer = JsonConvert.DeserializeObject<Timer>(json, settings);
            
            Assert.Equal(TimeSpan.FromSeconds(42), timer.Duration);
            Assert.Single(timer.SubTimer);
            Assert.Equal(TimeSpan.FromSeconds(142), timer.SubTimer[0].Duration);
        }
    }
}

