using JsonApiSerializer.Test.Models.Articles;
using Newtonsoft.Json;
using AutoFixture;
using Xunit;
using System;
using AutoFixture.Kernel;
using JsonApiSerializer.Test.Models.Locations;
using JsonApiSerializer.Test.Models.Timer;
using System.Linq;

namespace JsonApiSerializer.Test.RoundTripTests
{
    public class SerializeAndDeserializeTests
    {
        public JsonApiSerializerSettings settings = new JsonApiSerializerSettings
        {
            Formatting = Formatting.Indented //pretty print makes it easier to debug
        };

        
        [Theory]
        [InlineData(typeof(Article[]))]
        [InlineData(typeof(Article))]
        [InlineData(typeof(ArticleWithRelationship))]
        [InlineData(typeof(ArticleWithDatalessRelationship))]
        [InlineData(typeof(ArticleWithNoType))]
        [InlineData(typeof(ArticleWithResourceIdentifier))]
        [InlineData(typeof(ArticleWithReadonlyType))]
        [InlineData(typeof(ArticleWithIdType<string>))]
        [InlineData(typeof(ArticleWithIdType<int>))]
        [InlineData(typeof(ArticleWithIdType<long>))]
        [InlineData(typeof(ArticleWithIdType<Guid>))]
        [InlineData(typeof(ArticleWithIdType<uint>))]
        [InlineData(typeof(ArticleWithIdType<ulong>))]
        [InlineData(typeof(ArticleWithIdType<int?>))]
        [InlineData(typeof(ArticleWithIdType<long?>))]
        [InlineData(typeof(ArticleWithIdType<Guid?>))]
        [InlineData(typeof(ArticleWithIdType<uint?>))]
        [InlineData(typeof(ArticleWithIdType<ulong?>))]
        [InlineData(typeof(Timer))]
        public void When_serialize_should_deserialize_to_equal_objects(Type type)
        {
            var fixture = new Fixture();

            //handle recursion
            fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
                .ForEach(b => fixture.Behaviors.Remove(b));
            fixture.Behaviors.Add(new OmitOnRecursionBehavior(3));

            var item1 = new SpecimenContext(fixture).Resolve(type);

            var json = JsonConvert.SerializeObject(item1, settings);
            var item2 = JsonConvert.DeserializeObject(json, type, settings);

            var equal = TestUtils.JsonObjectEqualityComparer<object>.Instance.Equals(item1, item2);

            Assert.True(equal);
        }
    }
}
