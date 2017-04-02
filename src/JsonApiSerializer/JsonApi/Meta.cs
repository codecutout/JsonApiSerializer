using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
