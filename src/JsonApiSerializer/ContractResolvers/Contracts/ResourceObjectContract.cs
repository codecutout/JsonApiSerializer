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
    internal class ResourceObjectContract : JsonObjectContractWrap
    {
        public readonly string DefaultType;
        public readonly JsonProperty IdProperty;
        public readonly JsonProperty TypeProperty;
        public readonly JsonProperty LinksProperty;
        public readonly JsonProperty MetaProperty;
        public readonly JsonProperty[] Attributes;
        public readonly JsonProperty[] Relationships;

        public ResourceObjectContract(JsonObjectContract jsonObjectContract, Func<Type, bool> isRelationship)
            : base(jsonObjectContract)
        {
            //populate ResourceObjectContract fields
            if (jsonObjectContract.Converter is ResourceObjectConverter resourceObjectConverter)
                DefaultType = resourceObjectConverter.GenerateDefaultTypeNameInternal(jsonObjectContract.UnderlyingType);

            var attributes = new List<JsonProperty>();
            var relationships = new List<JsonProperty>();
            foreach (var prop in jsonObjectContract.Properties.Where(x => !x.Ignored))
            {
                switch (prop.PropertyName)
                {
                    //In addition, a resource object MAY contain any of these top - level members: links, meta, attributes, relationships
                    case PropertyNames.Id: //Id is optional on base objects
                        IdProperty = prop;
                        break;
                    case PropertyNames.Links:
                        LinksProperty = prop;
                        break;
                    case PropertyNames.Meta:
                        MetaProperty = prop;
                        break;
                    case PropertyNames.Type:
                        TypeProperty = prop;
                        break;
                    case var _ when isRelationship(prop.PropertyType):
                        relationships.Add(prop);
                        break;
                    default:
                        attributes.Add(prop);
                        break;
                }
            }

            this.Attributes = attributes.ToArray();
            this.Relationships = relationships.ToArray();
        }

    }
}
