using JsonApiSerializer.Test.Models.Articles;
using Newtonsoft.Json;
using AutoFixture;
using Xunit;

namespace JsonApiSerializer.Test.RoundTripTests
{
    public class SerializeAndDeserializeTests
    {
        public JsonApiSerializerSettings settings = new JsonApiSerializerSettings
        {
            Formatting = Formatting.Indented //pretty print makes it easier to debug
        };

        [Fact]
        public void When_serialize_should_deserialize_to_equal_objects()
        {
            var fixture = new Fixture();
            var product1 = fixture.Create<Article[]>();

            var json = JsonConvert.SerializeObject(product1, settings);
            var product2 = JsonConvert.DeserializeObject<Article[]>(json, settings);

            var equal = TestUtils.JsonObjectEqualityComparer<Article[]>.Instance.Equals(product1, product2);

            Assert.True(equal);
        }
    }
}
