using JsonApiSerializer.JsonApi.WellKnown;
using Newtonsoft.Json.Serialization;

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
