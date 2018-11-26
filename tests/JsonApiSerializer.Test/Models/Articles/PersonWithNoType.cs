using JsonApiSerializer.JsonApi;
using Newtonsoft.Json;

namespace JsonApiSerializer.Test.Models.Articles
{
    public class PersonWithNoType
    {
        public string Id { get; set; }

        [JsonProperty(propertyName: "first-name")]
        public string FirstName { get; set; }

        [JsonProperty(propertyName: "last-name")]
        public string LastName { get; set; }

        public string Twitter { get; set; }

        public Links Links { get; set; }

        public Meta Meta { get; set; }
    }
}
