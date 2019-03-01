using JsonApiSerializer.JsonApi;
using Newtonsoft.Json;

namespace JsonApiSerializer.Test.Models.Articles
{
    using ContractResolvers.Attributes;

    [JsonApiProperties(Type = null)]
    public class PersonWithJsonApiAttributesNoType
    {
        public string Id { get; set; }

        public string Email { get; set; }

        public string Type { get; set; }
    }
}
