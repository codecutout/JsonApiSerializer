using Newtonsoft.Json;

namespace JsonApiSerializer.Test.Models.Products
{
    public class Video : IMarketedContent
    {
        public static readonly string ResourceTypeName = "videos";

        [JsonProperty("videoType")] public string VideoType { get; set; }

        [JsonProperty("title")] public string Title { get; set; }
        public string Type { get; } = ResourceTypeName;
        public string Id { get; set; }
    }
}