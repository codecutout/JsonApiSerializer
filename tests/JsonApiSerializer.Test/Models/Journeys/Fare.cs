using Trainline.OrderIndexFeeder.ExternalServices.ProductService.JsonApi;

namespace Trainline.OrderIndexFeeder.ExternalServices.ProductService
{
    public class Fare : Data
    {
        public Price Price { get; set; }
        public ValidUntil ValidUntil { get; set; }

        public VendorInformation Commercials { get; set; }

        public FareType FareType { get; set; }

        public FareLeg[] FareLegs { get; set; }

        public FarePassenger[] FarePassengers { get; set; }

    }
}