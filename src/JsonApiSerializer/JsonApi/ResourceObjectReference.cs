using Newtonsoft.Json;

namespace JsonApiSerializer
{
    public struct ResourceObjectReference
    {
        public string Id;
        public string Type;

        public ResourceObjectReference(string id, string type)
        {
            Id = id;
            Type = type;
        }

        public override string ToString()
        {
            return Id + ":" + Type;
        }
    }
}
