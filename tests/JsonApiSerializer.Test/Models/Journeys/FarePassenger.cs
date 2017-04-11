using Trainline.OrderIndexFeeder.ExternalServices.ProductService.JsonApi;

namespace Trainline.OrderIndexFeeder.ExternalServices.ProductService
{
    public class FarePassenger : Data
    {
        public Passenger[] Passengers { get; set; }
    }
}