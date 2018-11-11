using JsonApiSerializer.JsonApi;
using JsonApiSerializer.JsonApi.WellKnown;
using JsonApiSerializer.JsonConverters;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JsonApiSerializer.ContractResolvers.Contracts
{
    internal class ResourceRelationshipContract : JsonObjectContractWrap
    {
        public readonly JsonProperty LinksProperty;
        public readonly JsonProperty MetaProperty;
        public readonly JsonProperty DataProperty;

        public ResourceRelationshipContract(JsonObjectContract jsonObjectContract)
            : base(jsonObjectContract)
        {
            LinksProperty = Properties.GetClosestMatchProperty(PropertyNames.Links);
            MetaProperty = Properties.GetClosestMatchProperty(PropertyNames.Meta);
            DataProperty = Properties.GetClosestMatchProperty(PropertyNames.Data);
        }
    }
}
