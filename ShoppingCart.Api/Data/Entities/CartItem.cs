namespace ShoppingCart.Api.Data.Entities
{
    public class CartItem
    {
        public int Id { get; set; }
        public int StoreItemId { get; set; }
        public int UserId { get; set; }

        public StoreItem StoreItem { get; set; }
        public User User { get; set; }
    }
}
