using Trainline.OrderIndexFeeder.ExternalServices.ProductService.JsonApi;

namespace Trainline.OrderIndexFeeder.ExternalServices.ProductService
{
    public class JourneyPlan : Data
    {
        public Station Origin { get; set; }
        public Station Destination { get; set; }
        public Leg[] Legs { get; set; }
    }
}