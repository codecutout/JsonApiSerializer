using Trainline.OrderIndexFeeder.ExternalServices.ProductService.JsonApi;

namespace Trainline.OrderIndexFeeder.ExternalServices.ProductService
{
    public class PassengerType : Data
    {
        public string Name { get; set; }
        public string Group { get; set; }
        public AgeRestriction AgeRestriction { get; set; }
    }
}