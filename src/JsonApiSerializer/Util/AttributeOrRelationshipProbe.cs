using Newtonsoft.Json;

namespace JsonApiSerializer.Util
{
    internal class AttributeOrRelationshipProbe : JsonWriterCapture
    {
        public enum Type
        {
            Attribute,
            Relationship
        }

        public Type PropertyType { get; set; } = Type.Attribute;


        public AttributeOrRelationshipProbe(JsonWriter writer) :base(writer){
        }
    }
}
