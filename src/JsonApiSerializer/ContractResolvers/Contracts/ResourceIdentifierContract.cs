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
    internal class ResourceIdentifierContract : JsonObjectContractWrap
    {
        public readonly JsonProperty IdProperty;
        public readonly JsonProperty TypeProperty;
        public readonly JsonProperty MetaProperty;
        public readonly JsonProperty ResourceObjectProperty;

        public ResourceIdentifierContract(JsonObjectContract jsonObjectContract)
            : base(jsonObjectContract)
        {
            ResourceObjectProperty = jsonObjectContract.Properties.First(x => x.UnderlyingName == nameof(IResourceIdentifier<object>.Value));

            foreach (var prop in jsonObjectContract.Properties.Where(x => !x.Ignored))
            {
                switch (prop.PropertyName)
                {
                    //In addition, a resource object MAY contain any of these top - level members: links, meta, attributes, relationships
                    case PropertyNames.Id: //Id is optional on base objects
                        IdProperty = prop;
                        break;
                    case PropertyNames.Meta:
                        MetaProperty = prop;
                        break;
                    case PropertyNames.Type:
                        TypeProperty = prop;
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
