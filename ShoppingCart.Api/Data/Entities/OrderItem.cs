﻿namespace ShoppingCart.Api.Data.Entities
{
    public class OrderItem
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int StoreItemId { get; set; }
        public string Status { get; set; }
        public double Price { get; set; }
        public int Count { get; set; }

        public StoreItem StoreItem { get; set; }
        public Order Order { get; set; }
    }
}
