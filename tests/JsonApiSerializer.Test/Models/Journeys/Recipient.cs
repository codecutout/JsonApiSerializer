using Trainline.OrderIndexFeeder.ExternalServices.ProductService.JsonApi;

namespace Trainline.OrderIndexFeeder.ExternalServices.ProductService
{
    public class Recipient : Data
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Country { get; set; }
        public string Postcode { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }
        public string Address4 { get; set; }

        public FarePassenger[] FarePassengers { get; set; }
    }
}