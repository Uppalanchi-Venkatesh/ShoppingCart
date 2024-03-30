using System.Collections.Generic;

namespace ShoppingCart.Api.DTOs
{
    public class ProductModelDto
    {
        public Dictionary<int, ProductDetailDto> Products { get; set; } = new();
    }

    public class ProductDetailDto
    {
        public int Id { get; set; }
        public double Amount { get; set; }
        public string Category { get; set; }
        public bool Available { get; set; }
        public double DeliveryCharge { get; set; }
        public string StoreName { get; set; }
        public int StoreItemId { get; set; }
    }
}
