using JsonApiSerializer.JsonApi.WellKnown;
using Newtonsoft.Json.Linq;
using System;

namespace JsonApiSerializer.SerializationState
{
    internal struct ResourceObjectReference : IEquatable<ResourceObjectReference>
    {
        public readonly string Id;
        public readonly string Type;

        public ResourceObjectReference(string id, string type)
        {
            Id = id;
            Type = type;
        }

        public ResourceObjectReference(JObject jobj) 
            : this(jobj[PropertyNames.Id]?.ToString(), jobj[PropertyNames.Type]?.ToString())
        {
        }

        public override string ToString()
        {
            return Id + ":" + Type;
        }

        public bool Equals(ResourceObjectReference other)
        {
            return other.Id == this.Id && other.Type == this.Type;
        }

        public override bool Equals(object obj)
        {
            return obj is ResourceObjectReference && Equals((ResourceObjectReference)obj);
        }

        public override int GetHashCode()
        {
            unchecked // Overflow is fine, just wrap
            {
                int hash = 17;
                hash = hash * 23 + Id?.GetHashCode() ?? 0;
                hash = hash * 23 + Type?.GetHashCode() ?? 0;
                return hash;
            }
        }
    }
}
