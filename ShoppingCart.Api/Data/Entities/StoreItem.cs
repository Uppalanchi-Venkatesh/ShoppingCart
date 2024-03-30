using System.Collections.Generic;

namespace ShoppingCart.Api.Data.Entities
{
    public class StoreItem
    {
        public int Id { get; set; }
        public int StoreId { get; set; }
        public int ProductId { get; set; }
        public int SoldQuantity { get; set; }
        public int Available { get; set; }

        public Store Store { get; set; }
        public Product Product { get; set; }
        public ICollection<OrderItem> OrderItems { get; set; }
        public ICollection<CartItem> CartItems { get; set; }
    }
}
