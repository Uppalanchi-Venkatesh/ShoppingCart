﻿using System;

namespace ShoppingCart.Api.Data.Entities
{
    public class Transaction
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public int FromId { get; set; }
        public int ToId { get; set; }
        public double Amount { get; set; }
        public string Type { get; set; }
        public string Description { get; set; }

        public Order Order { get; set; }
        public Account FromAccount { get; set; }
        public Account ToAccount { get; set; }
    }
}
