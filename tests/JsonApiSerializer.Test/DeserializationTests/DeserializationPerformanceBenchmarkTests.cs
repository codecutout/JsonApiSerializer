using JsonApiSerializer.Test.Models.Articles;
using JsonApiSerializer.Test.Models.Articles.VanillaJson;
using JsonApiSerializer.Test.TestUtils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
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
        [Trait("Category", "performance")]
        public void When_compared_with_standard_json_deserialization_should_be_comparable()
        {
            TimeSpan testTime = TimeSpan.FromSeconds(30);

            var json = EmbeddedResource.Read("Data.Articles.sample-with-full-link.json");

            var jsonIterations = CountIterations<DocumentRootVanillaJson<List<ArticleVanillaJson>>>(testTime / 2, json, new JsonSerializerSettings());
            var jsonApiIterations = CountIterations<List<Article>>(testTime / 2, json, new JsonApiSerializerSettings());

            output.WriteLine($"Json performed {jsonIterations} deserializations in {testTime.TotalSeconds}s");
            output.WriteLine($"JsonApi performed {jsonApiIterations} deserializations in {testTime.TotalSeconds}s");
            output.WriteLine($"JsonApi speed compared with Json ratio is {1.0 * jsonApiIterations / jsonIterations}");
        }

        public static int CountIterations<T>(TimeSpan timeout, string json, JsonSerializerSettings settings)
        {
            var sw = new Stopwatch();
            int iterations = 0;
            GC.Collect();
            sw.Start();
            while (sw.Elapsed < timeout)
            {
                JsonConvert.DeserializeObject<T>(json, settings);
                iterations++;
            }
            sw.Stop();
            return iterations;
        }
    }
}
