using Trainline.OrderIndexFeeder.ExternalServices.ProductService.JsonApi;

namespace Trainline.OrderIndexFeeder.ExternalServices.ProductService
{
    public class FareType : Data
    {
        public string Name { get; set; }
        public Condition[] Conditions { get; set; }
        public bool? Refundable { get; set; }
        public bool? Exchangeable { get; set; }
        public string ConditionDetails { get; set; }
    }
}