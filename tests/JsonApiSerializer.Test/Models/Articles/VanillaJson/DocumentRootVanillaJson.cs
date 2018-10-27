using JsonApiSerializer.JsonApi;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JsonApiSerializer.Test.Models.Articles.VanillaJson
{
    public class DocumentRootVanillaJson<TData>
    {
        [JsonProperty("jsonapi", NullValueHandling = NullValueHandling.Ignore)]
        public VersionInfo JsonApi { get; set; }

        [JsonProperty(PropertyName = "data")]
        public TData Data { get; set; }

        [JsonProperty(PropertyName = "included")]
        public List<object> Included { get; set; }

        [JsonProperty(PropertyName = "links", NullValueHandling = NullValueHandling.Ignore)]
        public Links Links { get; set; }

        [JsonProperty(PropertyName = "meta", NullValueHandling = NullValueHandling.Ignore)]
        public Meta Meta { get; set; }

        
    }
}
