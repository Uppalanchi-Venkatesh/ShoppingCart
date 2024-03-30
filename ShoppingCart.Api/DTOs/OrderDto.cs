using System;
using System.Collections.Generic;

namespace ShoppingCart.Api.DTOs
{
    public class BaseOrderDto
    {
        public int Id { get; set; }
        public DateTime Update { get; set; }
        public string Status { get; set; }
    }

    public class UserOrderDto : BaseOrderDto
    {
        public DateTime Delivery { get; set; }
        public DateTime Created { get; set; }
        public double TotalAmount { get; set; }
        public double DeliveryCharge { get; set; }
        public ICollection<OrderItemDto> OrderItems { get; set; }
    }

    public class UserOrderDetailDto : UserOrderDto
    {
        public string Name { get; set; }
        public string Mobile { get; set; }
        public string Address { get; set; }
        public int? TransactionId { get; set; }
    }

    public class StoreOrderDto : BaseOrderDto
    {
        public DateTime Created { get; set; }
        public double TotalAmount { get; set; }
        public double DeliveryCharge { get; set; }
        public int? TransactionId { get; set; }
        public string StoreName { get; set; }
        public string StoreId { get; set; }
        public ICollection<OrderItemDto> OrderItems { get; set; }
    }

    public class OrderItemDto
    {
        public int Id { get; set; }
        public int StoreItemId { get; set; }
        public int ProductId { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }
        public int Count { get; set; }
    }

    public class OrderRequestDto
    {
        public string PayOption { get; set; }
        public double TotalAmount { get; set; }
        public int StoreId { get; set; }
        public List<OrderRequestItem> Items { get; set; }
    }

    public class OrderRequestItem
    {
        public int StoreItemId { get; set; }
        public int ItemQuantity { get; set; }
        public double Price { get; set; }
    }
}
