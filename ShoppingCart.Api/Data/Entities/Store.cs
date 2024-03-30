using System.Collections.Generic;

namespace ShoppingCart.Api.Data.Entities
{
    public class Store
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int AccountId { get; set; }
        public string Address { get; set; }

        public Account Account { get; set; }
        public ICollection<StoreItem> Items { get; set; }
        public ICollection<Order> Orders { get; set; }
    }
}
