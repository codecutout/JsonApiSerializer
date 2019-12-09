using System;
using System.Collections.Generic;
using System.Text;

namespace JsonApiSerializer.Test.Models.Products
{
    public class Category
    {
        public string Type { get; set; } = "category";
        public string Id { get; set; }
        public string Name { get; set; }
        public Product[] Products { get; set; }
    }
}
