using JsonApiSerializer.JsonApi;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace JsonApiSerializer.Test.Models.Articles
{
    public class PersonWithSerializationConditions
    {
        public string Type { get; set; } = "people";

        public string Id { get; set; }

        [JsonProperty(propertyName: "first-name")]
        public string FirstName { get; set; }

        [JsonProperty(propertyName: "last-name")]
        public string LastName { get; set; }

        public string Twitter { get; set; }

        public Links Links { get; set; }

        public Meta Meta { get; set; }

        [JsonIgnore]
        public List<string> SerializeProperties { get; set; }

        public bool ShouldSerializeType()
        {
            return SerializeProperties == null || SerializeProperties.Contains(nameof(Type));
        }

        public bool ShouldSerializeId()
        {
            return SerializeProperties == null || SerializeProperties.Contains(nameof(Id));
        }

        public bool ShouldSerializeFirstName()
        {
            return SerializeProperties == null || SerializeProperties.Contains(nameof(FirstName));
        }

        public bool ShouldSerializeLastName()
        {
            return SerializeProperties == null || SerializeProperties.Contains(nameof(LastName));
        }

        public bool ShouldSerializeTwitter()
        {
            return SerializeProperties == null || SerializeProperties.Contains(nameof(Twitter));
        }

        public bool ShouldSerializeLinks()
        {
            return SerializeProperties == null || SerializeProperties.Contains(nameof(Links));
        }

    }
}
