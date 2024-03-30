using ShoppingCart.Api.Data;
using ShoppingCart.Api.Helpers;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using ShoppingCart.Api.Data.Entities;

namespace ShoppingCart.Api.Seed
{
    public class SeedData
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;
        private readonly IConfiguration _config;
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        private static readonly Random Random = new();

        public SeedData(
            UserManager<User> userManager,
            RoleManager<Role> roleManager,
            IConfiguration config,
            DataContext context,
            IMapper mapper
        )
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _config = config;
            _context = context;
            _mapper = mapper;
        }

        public async Task SeedDatabase()
        {
            await _context.Database.MigrateAsync();
            await SeedRoles();
            await SeedUsers();
            await SeedStore();
            await SeedProduct();
        }

        async Task SeedRoles()
        {
            if (await _roleManager.Roles.AnyAsync()) return;

            foreach (var role in Enum.GetNames<RoleType>())
            {
                await _roleManager.CreateAsync(new Role { Name = role });
            }
        }

        async Task SeedUsers()
        {
            if (await _userManager.Users.AnyAsync()) return;

            var data = await File.ReadAllTextAsync("Seed/UserSeed.json");
            var users = JsonSerializer.Deserialize<List<User>>(data);

            if (users == null) return;

            foreach (var user in users)
            {
                user.UserName = user.UserName.ToLower();
                user.Account = new Account
                {
                    Balance = 100000
                };
                user.Address = GetRandomAddress();

                switch (user.UserName)
                {
                    case Constants.Admin:
                        user.Account.Balance = 2000000;
                        await _userManager.CreateAsync(user, _config["AdminPassword"]);
                        await _userManager.AddToRolesAsync(user, Enum.GetNames<RoleType>());
                        break;
                    case Constants.TestUser:
                        await _userManager.CreateAsync(user, _config["AdminPassword"]);
                        await _userManager.AddToRolesAsync(user, new[]
                        {
                            RoleType.User.ToString()
                        });
                        break;
                    default:
                        await _userManager.CreateAsync(user, _config["AdminPassword"]);
                        await _userManager.AddToRoleAsync(user, RoleType.User.ToString());
                        break;
                }
            }
        }

        async Task SeedStore()
        {
            if (await _context.Stores.AnyAsync()) return;

            var stores = new[]
            { 
                "SuperComNet", "StoreEcom", "CORSECA", 
                "PETILANTEOnline", "RetailNet", "AkshnavOnline",
                "OmniTechRetail", "IWQNBecommerce", "RetailHomes", "HomeKart"
            };

            var admin = await _context.Users.Where(u => u.UserName == "luffy").FirstAsync();

            foreach (var name in stores)
            {
                var store = new Store
                {
                    Name = name,
                    Account = new Account(),
                    Address = GetRandomAddress()
                };
                store.Address = GetRandomAddress();
                await _context.Stores.AddAsync(store);
                await _context.SaveChangesAsync();
            }

            await _userManager.AddToRoleAsync(admin, RoleType.StoreAdmin.ToString());
        }

        async Task SeedProduct()
        {
            if (await _context.Products.AnyAsync()) return;

            var data = await File.ReadAllTextAsync("Seed/ProductSeed.json");
            var productSeeds = JsonSerializer.Deserialize<List<ProductSeed>>(data);

            if (productSeeds != null)
            {
                foreach (var productSeed in productSeeds)
                {
                    var product = _mapper.Map<Product>(productSeed);
                    product.Created = new DateTime(2021, 1, 1).AddDays(Random.Next(365));

                    var stores = await _context.Stores.ToListAsync();
                    var storeItems = new List<StoreItem>();
                    foreach (var store in stores.OrderBy(_ => Guid.NewGuid()).Take(2))
                    {
                        storeItems.Add(new StoreItem
                        {
                            Store = store,
                            SoldQuantity = Random.Next(0, 1000),
                            Available = Random.Next(0, 1000)
                        });
                    }

                    product.StoreItems = storeItems;

                    await _context.Products.AddAsync(product);
                    await _context.SaveChangesAsync();
                }
            }
        }

        static string GetRandomAddress()
        {
            var addressess = new[]
            {
                "Bull Temple","Hotel Dwarka", "Vidyarthi Bhavan", "Maharaja Agrasen Hospital", "Church Parking",
                "Kanti Sweets", "Cafe Coffee Day", "Eden Park Restaurant", "Chaipoint", "Space Matrix",
                "Brundavan Cafe", "BMS College of Engineering", "Ashok Nagar Post Office"
            };

            return addressess[Random.Next(0, addressess.Length)];
        }
    }

    public class ProductSeed
    {
        public string Name { get; set; }
        public string Category { get; set; }
        public string Brand { get; set; }
        public string Model { get; set; }
        public string Description { get; set; }
        public string Features { get; set; }
        public double Amount { get; set; }
    }
}
