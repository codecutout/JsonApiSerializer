using JsonApiSerializer.JsonApi;
using Newtonsoft.Json;

namespace JsonApiSerializer.Test.Models.Articles
{
    public class PersonWithAttributes
    {
        [JsonProperty(Order = 100)]
        public string Type { get; set; } = "people";

        [JsonProperty(Order = 50)]
        public string Id { get; set; }

        [JsonProperty(propertyName: "first-name", Order = 10)]
        public string FirstName { get; set; }

        [JsonProperty(propertyName: "last-name", Order = 5)]
        public string LastName { get; set; }

        [JsonIgnore]
        public string Twitter { get; set; }

        public Links Links { get; set; }
    }
}
