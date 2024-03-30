using System;
using System.Collections.Generic;

namespace ShoppingCart.Api.Data.Entities
{
    public class Order
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public int UserId { get; set; }
        public DateTime Created { get; set; }
        public DateTime Update { get; set; }
        public DateTime Delivery { get; set; }
        public double TotalAmount { get; set; }
        public double DeliveryCharge { get; set; }
        public string Status { get; set; }
        public int StoreId { get; set; }
        public string Address { get; set; }
        public int? TransactionId { get; set; }

        public User User { get; set; }
        public Store Store { get; set; }
        public Transaction Transaction { get; set; }
        public ICollection<OrderItem> OrderItems { get; set; }
    }
}
