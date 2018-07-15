using JsonApiSerializer.Test.Models.Articles;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace JsonApiSerializer.Test.Models.Articles
{
    public class PersonAdmin : Person
    {
        [JsonProperty("administrator-rights")]
        public string[] AdministratorRights { get; set; }
    }
}