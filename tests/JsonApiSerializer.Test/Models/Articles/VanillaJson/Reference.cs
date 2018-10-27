using Newtonsoft.Json;

namespace JsonApiSerializer.Test.Models.Articles.VanillaJson
{
    public class Reference
    {
        [JsonProperty(PropertyName = "type")]
        public string Type { get; set; }

        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }
    }
}
