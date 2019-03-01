using System;

namespace JsonApiSerializer.ContractResolvers.Attributes
{
    public class JsonApiProperties : Attribute
    {
        public string Id { get; set; }

        public string Type { get; set; }

        public JsonApiProperties()
        {
            this.Id = string.Empty;
            this.Type = string.Empty;
        }
    }
}
