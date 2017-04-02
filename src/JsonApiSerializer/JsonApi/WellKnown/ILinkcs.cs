using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonApiSerializer.JsonApi.WellKnown
{
    /// <summary>
    /// Represents a Link. Links are deserialized specially to account for them being represented as either a string or an object
    /// </summary>
    public interface ILink
    {
        string Href { get; set; }
    }
}
