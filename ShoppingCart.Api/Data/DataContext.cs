using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ShoppingCart.Api.Data.Entities;

namespace ShoppingCart.Api.Data
{
    public class DataContext : IdentityDbContext<User, Role, int, IdentityUserClaim<int>, UserRole, IdentityUserLogin<int>, IdentityRoleClaim<int>, IdentityUserToken<int>>
    {
        public DataContext(DbContextOptions options) : base(options) { }

        public DbSet<Account> Accounts { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Store> Stores { get; set; }
        public DbSet<StoreItem> StoreItems { get; set; }
        public DbSet<Transaction> Transactions { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<User>()
                .HasOne(u => u.Account)
                .WithOne(a => a.User)
                .HasForeignKey<User>(u => u.AccountId)
                .OnDelete(DeleteBehavior.Restrict);
            builder.Entity<User>()
                .HasMany(u => u.Orders)
                .WithOne(o => o.User)
                .HasForeignKey(o => o.UserId);

            builder.Entity<Role>()
                .HasMany(ar => ar.UserRoles)
                .WithOne(ur => ur.Role)
                .HasForeignKey(ur => ur.RoleId);

            builder.Entity<User>()
                .HasMany(u => u.UserRoles)
                .WithOne(ur => ur.User)
                .HasForeignKey(ur => ur.UserId);

            builder.Entity<Store>()
                .HasOne(s => s.Account)
                .WithOne(a => a.Store)
                .HasForeignKey<Store>(s => s.AccountId)
                .OnDelete(DeleteBehavior.Restrict);
            builder.Entity<Store>()
                .HasMany(s => s.Items)
                .WithOne(si => si.Store)
                .HasForeignKey(si => si.StoreId);
            builder.Entity<Store>()
                .HasMany(s => s.Orders)
                .WithOne(o => o.Store)
                .HasForeignKey(o => o.StoreId);

            builder.Entity<Product>()
                .HasMany(p => p.StoreItems)
                .WithOne(si => si.Product)
                .HasForeignKey(si => si.ProductId);

            builder.Entity<StoreItem>()
                .HasMany(si => si.OrderItems)
                .WithOne(oi => oi.StoreItem)
                .HasForeignKey(oi => oi.StoreItemId);

            builder.Entity<Order>()
                .HasMany(o => o.OrderItems)
                .WithOne(oi => oi.Order)
                .HasForeignKey(oi => oi.OrderId)
                .OnDelete(DeleteBehavior.Restrict);
            builder.Entity<Order>()
                .HasOne(o => o.Transaction)
                .WithOne(t => t.Order)
                .HasForeignKey<Order>(o => o.TransactionId);
            
            builder.Entity<CartItem>()
                .HasOne(c => c.User)
                .WithMany(u => u.CartItems)
                .HasForeignKey(c => c.UserId);
            builder.Entity<CartItem>()
                .HasOne(c => c.StoreItem)
                .WithMany(si => si.CartItems)
                .HasForeignKey(c => c.StoreItemId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
