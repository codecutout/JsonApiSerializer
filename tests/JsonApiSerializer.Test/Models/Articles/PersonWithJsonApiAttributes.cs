using JsonApiSerializer.JsonApi;
using Newtonsoft.Json;

namespace JsonApiSerializer.Test.Models.Articles
{
    using ContractResolvers.Attributes;

    [JsonApiProperties(Id = "email", Type = "resourceType")]
    public class PersonWithJsonApiAttributes
    {
        public int Id { get; set; }

        public string Email { get; set; }

        public string Type { get; set; }

        public string ResourceType { get; set; } = "people";
    }
}
