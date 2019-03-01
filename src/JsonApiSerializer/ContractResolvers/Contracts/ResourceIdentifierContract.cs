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
    using System.Reflection;
    using Attributes;

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

            var propertyAttr = jsonObjectContract.CreatedType.GetTypeInfo().GetCustomAttribute<JsonApiProperties>() ?? new JsonApiProperties();
            var propertyNameId = propertyAttr.Id == string.Empty ? PropertyNames.Id : propertyAttr.Id;
            var propertyNameType = propertyAttr.Type ==  string.Empty ? PropertyNames.Type : propertyAttr.Type;

            foreach (var prop in jsonObjectContract.Properties.Where(x => !x.Ignored))
            {
                switch (prop.PropertyName)
                {
                    //In addition, a resource object MAY contain any of these top - level members: links, meta, attributes, relationships
                    case PropertyNames.Meta:
                        MetaProperty = prop;
                        break;
                    default:
                        if (prop.PropertyName == propertyNameId)
                        {
                            IdProperty = prop;
                        }
                        else if (prop.PropertyName == propertyNameType)
                        {
                            TypeProperty = prop;
                        }
                        break;
                }
            }
        }
    }
}
