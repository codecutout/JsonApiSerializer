using System;
using System.Collections.Generic;
using JsonApiSerializer.JsonConverters;
using JsonApiSerializer.Test.Models.Products;
using JsonApiSerializer.Test.Models.Timer;
using JsonApiSerializer.Test.TestUtils;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Xunit;

namespace JsonApiSerializer.Test.DeserializationTests
{
    public class DeserializationCustomConverterTests
    {
        private class ProductContentConverter : ResourceObjectConverter
        {
            protected override object CreateDefault(JsonObjectContract contract, string type)
            {
                switch (type)
                {
                    case "videos":
                        contract.DefaultCreator = () => new Video();
                        break;
                    case "images":
                        contract.DefaultCreator = () => new Image();
                        break;
                }

                return contract.DefaultCreator();
            }
        }

        public static IEnumerable<object[]> ProductsTestData
        {
            get
            {
                yield return new object[]
                    {EmbeddedResource.Read("Data.Products.sample-product-with-images.json"), typeof(Image)};
                yield return new object[]
                    {EmbeddedResource.Read("Data.Products.sample-product-with-videos.json"), typeof(Video)};
            }
        }

        [Theory]
        [MemberData(nameof(ProductsTestData))]
        public void When_resource_type_is_generic_should_deserialize(string json, Type type)
        {
            var product = JsonConvert.DeserializeObject<Product>(json, GetSerializerSettings());
            Assert.IsType(type, product.Content.Data);
        }

        private static JsonSerializerSettings GetSerializerSettings()
        {
            return new JsonApiSerializerSettings(new ProductContentConverter());
        }

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