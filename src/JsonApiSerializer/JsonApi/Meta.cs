using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace JsonApiSerializer.JsonApi
{
    /// <summary>
    /// Represents generic MetaData.
    /// </summary>
    /// <seealso cref="System.Collections.Generic.Dictionary{System.String, Newtonsoft.Json.Linq.JToken}" />
    public class Meta : Dictionary<string, JToken>
    {
    }
}
