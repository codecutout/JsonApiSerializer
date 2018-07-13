using JsonApiSerializer.JsonApi;
using Newtonsoft.Json;

namespace JsonApiSerializer.Test.Models.Products
{
    public class Product
    {
        public static readonly string ResourceTypeName = "products";
        public string Type { get; } = ResourceTypeName;

        public string Id { get; set; }

        [JsonProperty("marketingName")]
        public string MarketingName { get; set; }

        [JsonProperty("content")]
        public Relationship<IMarketedContent> Content { get; set; }
    }
}