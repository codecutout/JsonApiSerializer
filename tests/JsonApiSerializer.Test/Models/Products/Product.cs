using System;
using System.Collections.Generic;
using System.Text;

namespace JsonApiSerializer.Test.Models.Products
{
    public class Product
    {
        public string Type { get; set; } = "product";
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public Category Category { get; set; }
        public Seller[] Sellers { get; set; }
    }
}
