using System;
using Newtonsoft.Json;
using Trainline.OrderIndexFeeder.ExternalServices.ProductService.JsonApi;

namespace Trainline.OrderIndexFeeder.ExternalServices.ProductService
{
    public class Product : Data
    {
        public string State { get; set; }

        public DateTimeOffset CreatedAt { get; set; }

        public Price ListPrice { get; set; }

        public Price RequestedPrice { get; set; }


        public VendorInformation Commercials { get; set; }

        public JourneyPlan OutwardJourneyPlan { get; set; }

        public JourneyPlan InwardJourneyPlan { get; set; }

        public Fare[] Fares { get; set; }

        public Delivery[] Deliveries { get; set; }
    }
}
