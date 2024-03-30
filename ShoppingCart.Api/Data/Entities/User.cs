using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace ShoppingCart.Api.Data.Entities
{
    public class User : IdentityUser<int>
    {
        public string Name { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Gender { get; set; }
        public DateTime LastActive { get; set; }
        public int AccountId { get; set; }
        public string Address { get; set; }

        public Account Account { get; set; }
        public ICollection<UserRole> UserRoles { get; set; }
        public ICollection<Order> Orders { get; set; }
        public ICollection<CartItem> CartItems { get; set; }
    }
}
