using System;
using Trainline.OrderIndexFeeder.ExternalServices.ProductService.JsonApi;

namespace Trainline.OrderIndexFeeder.ExternalServices.ProductService
{
    public class Leg : Data
    {
        public string TimetableId { get; set; }
        public DateTimeOffset LocalArriveAt { get; set; }
        public DateTimeOffset LocalDepartAt { get; set; }
        public string TransportMode { get; set; }
        public string TransportDesignation { get; set; }

        public Station Origin { get; set; }
        public Station Destination { get; set; }
        public Carrier Carrier { get; set; }
    }
}