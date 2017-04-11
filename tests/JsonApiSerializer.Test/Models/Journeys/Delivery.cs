using Trainline.OrderIndexFeeder.ExternalServices.ProductService.JsonApi;

namespace Trainline.OrderIndexFeeder.ExternalServices.ProductService
{
    public class Delivery : Data
    {
        public DeliveryCollection Collection { get; set; }
        public DeliveryMethod DeliveryMethod { get; set; }
        public Recipient[] Recipients { get; set; }
    }
}