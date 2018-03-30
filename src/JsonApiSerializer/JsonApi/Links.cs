using System.Collections.Generic;

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
