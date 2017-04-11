using JsonApiSerializer.JsonApi;
using Newtonsoft.Json.Linq;

namespace Trainline.OrderIndexFeeder.ExternalServices.ProductService.JsonApi
{
    public abstract class Data
    {
        public string Id { get; set; }
        public string Type { get; set; }
        public Meta Meta { get; set; }
        public Links Links { get; set; }
    }
}