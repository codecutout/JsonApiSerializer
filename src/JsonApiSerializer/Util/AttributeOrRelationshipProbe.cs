using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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


        public AttributeOrRelationshipProbe() :base(){
        }
    }
}
