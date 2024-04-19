using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace JsonApiSerializer.JsonApi
{
    /// <summary>
    /// Represents generic MetaData.
    /// </summary>
    public class Meta : Dictionary<string, JToken>
    {
    }
}
