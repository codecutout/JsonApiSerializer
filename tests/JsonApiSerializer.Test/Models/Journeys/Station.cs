using Trainline.OrderIndexFeeder.ExternalServices.ProductService.JsonApi;

namespace Trainline.OrderIndexFeeder.ExternalServices.ProductService
{
    public class Station : Data
    {
        public string Name { get; set; }
        public string Country { get; set; }
        public string Timezone { get; set; }
    }
}