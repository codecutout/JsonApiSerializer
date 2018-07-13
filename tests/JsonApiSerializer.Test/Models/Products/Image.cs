using Newtonsoft.Json;

namespace JsonApiSerializer.Test.Models.Products
{
    public class Image : IMarketedContent
    {
        public static readonly string ResourceTypeName = "images";

        [JsonProperty("imageType")] public string ImageType { get; set; }

        [JsonProperty("title")] public string Title { get; set; }

        public string Type { get; } = ResourceTypeName;
        public string Id { get; set; }
    }
}