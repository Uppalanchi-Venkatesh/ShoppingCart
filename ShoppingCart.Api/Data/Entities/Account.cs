namespace ShoppingCart.Api.Data.Entities
{
    public class Account
    {
        public int Id { get; set; }
        public double Balance { get; set; }
        public User User { get; set; }
        public Store Store { get; set; }
    }
}
