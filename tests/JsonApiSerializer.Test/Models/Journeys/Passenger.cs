using Trainline.OrderIndexFeeder.ExternalServices.ProductService.JsonApi;

namespace Trainline.OrderIndexFeeder.ExternalServices.ProductService
{
    public class Passenger : Data
    {
        public string Gender { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }

        public PassengerType PassengerType { get; set; }
    }
}