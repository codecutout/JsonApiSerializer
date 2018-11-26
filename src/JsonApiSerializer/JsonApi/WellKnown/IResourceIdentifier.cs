using System;
using System.Collections.Generic;
using System.Text;

namespace JsonApiSerializer.JsonApi.WellKnown
{
    interface IResourceIdentifier<TResourceObject>
    {
        TResourceObject Value { get; set; }
    }
}
