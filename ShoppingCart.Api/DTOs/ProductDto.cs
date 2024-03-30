namespace ShoppingCart.Api.DTOs
{
    public class ProductDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Features { get; set; }
        public bool Available { get; set; }
        public double Amount { get; set; }
    }
}
