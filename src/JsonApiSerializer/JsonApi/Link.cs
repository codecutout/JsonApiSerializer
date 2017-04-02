using JsonApiSerializer.JsonApi.WellKnown;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonApiSerializer.JsonApi
{
    /// <summary>
    /// Represents a Link.
    /// </summary>
    /// <seealso cref="JsonApiSerializer.JsonApi.WellKnown.ILink" />
    public class Link : ILink
    {
        public string Href { get; set; }

        public Meta Meta { get; set; }
    }

}
