using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonApiSerializer.JsonApi
{
    /// <summary>
    /// Represents a set of links.
    /// </summary>
    /// <seealso cref="System.Collections.Generic.Dictionary{System.String, JsonApiSerializer.JsonApi.Link}" />
    public class Links : Dictionary<string, Link>
    {
    }
}
