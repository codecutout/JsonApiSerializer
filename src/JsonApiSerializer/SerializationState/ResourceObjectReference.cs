using System;
using System.Collections.Generic;

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

        public override string ToString()
        {
            return Id + ":" + Type;
        }

        public bool Equals(ResourceObjectReference other)
        {
            return other.Id == Id && other.Type == Type;
        }

        public override bool Equals(object obj)
        {
            return obj is ResourceObjectReference reference && Equals(reference);
        }

        public override int GetHashCode()
        {
            var hashCode = 1325953389;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Id);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Type);
            return hashCode;
        }
    }
}
