using Trainline.OrderIndexFeeder.ExternalServices.ProductService.JsonApi;

namespace Trainline.OrderIndexFeeder.ExternalServices.ProductService
{
    public class FareLeg : Data
    {
        public Leg Leg { get; set; }
        public Comfort Comfort { get; set; }
        public TravelClass TravelClass { get; set; }
    }
}