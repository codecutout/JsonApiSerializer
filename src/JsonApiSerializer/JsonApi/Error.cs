using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JsonApiSerializer.JsonApi.WellKnown;

namespace JsonApiSerializer.JsonApi
{
    /// <summary>
    /// Represents an Error.
    /// </summary>
    public class Error : IError
    {
        public string Id { get; set; }

        public string Status { get; set; }

        public string Code { get; set; }

        public string Title { get; set; }

        public string Detail { get; set; }

        public ErrorSource Source { get; set; }

        public Links Links { get; set; }

        public Meta Meta { get; set; }
    }

    /// <summary>
    /// Represents the source of an error
    /// </summary>
    public class ErrorSource
    {
        public string Pointer { get; set; }
        public string Parameter { get; set; }
    }
}
