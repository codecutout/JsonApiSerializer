using JsonApiSerializer.Test.Models.Articles;
using JsonApiSerializer.Test.TestUtils;
using Newtonsoft.Json;
using System.Diagnostics;
using Xunit;
using Xunit.Abstractions;

namespace JsonApiSerializer.Test.DeserializationTests
{
    public class DeserializationPerformanceBenchmarkTests
    {
        private readonly ITestOutputHelper output;

        public DeserializationPerformanceBenchmarkTests(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Fact(Skip = "Used for benchmarking only")]
        public void When_object_root_with_array_should_deserialize()
        {
            var json = EmbeddedResource.Read("Data.Articles.sample.json");
            var iterations = 50000;
            var settings = new JsonApiSerializerSettings();

            //warmup
            var articles = JsonConvert.DeserializeObject<Article[]>(
                    json,
                    settings);

            var stopwatch = Stopwatch.StartNew();
            for(var i=0;i< iterations; i++)
            {
                articles = JsonConvert.DeserializeObject<Article[]>(
                    json,
                    settings);
            }
            stopwatch.Stop();

            var elapsedMilliseconds = 1.0 * stopwatch.ElapsedMilliseconds / iterations;

            output.WriteLine($"{elapsedMilliseconds:0.00}ms per deserialization");
        }
    }
}
