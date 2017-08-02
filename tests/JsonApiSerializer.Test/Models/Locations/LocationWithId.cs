using System.Collections.Generic;

namespace JsonApiSerializer.Test.Models.Locations
{
    public class LocationWithId : ILocationWithId
    {
        public string Id { get; set; }
        public string Description { get; set; }
        public IEnumerable<ILocationWithId> Parents { get; set; }
    }
}
