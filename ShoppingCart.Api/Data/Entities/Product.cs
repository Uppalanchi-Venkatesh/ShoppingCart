using System;
using System.Collections.Generic;

namespace ShoppingCart.Api.Data.Entities
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
        public string Brand { get; set; }
        public string Model { get; set; }
        public string Description { get; set; }
        public string Features { get; set; }
        public double Amount { get; set; }
        public DateTime Created { get; set; }

        public ICollection<StoreItem> StoreItems { get; set; }
    }
}
