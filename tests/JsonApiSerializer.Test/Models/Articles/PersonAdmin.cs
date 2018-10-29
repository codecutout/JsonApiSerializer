using Newtonsoft.Json;

namespace JsonApiSerializer.Test.Models.Articles
{
    public class PersonAdmin : Person
    {
        [JsonProperty("administrator-rights")]
        public string[] AdministratorRights { get; set; }
    }
}