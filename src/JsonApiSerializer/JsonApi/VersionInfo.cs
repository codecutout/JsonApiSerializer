using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonApiSerializer.JsonApi
{
    /// <summary>
    /// Represents the JsonApi version information.
    /// </summary>
    public class VersionInfo
    {
        public string Version { get; set; }

        public Meta Meta { get; set; }
    }
}
