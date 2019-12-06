using System;
using System.Collections.Generic;
using System.Text;

namespace JsonApiSerializer.Test.Models.Products
{
    public class Seller
    {
        public string Type { get; set; } = "seller";
        public string Id { get; set; }
        public string Name { get; set; }
        public Product[] Products { get; set; }
    }
}
