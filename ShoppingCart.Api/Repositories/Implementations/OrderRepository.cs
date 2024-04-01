using ShoppingCart.Api.Data;
using ShoppingCart.Api.DTOs;
using ShoppingCart.Api.Helpers;
using ShoppingCart.Api.Repositories.Contracts;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ShoppingCart.Api.Data.Entities;

namespace ShoppingCart.Api.Repositories.Implementations
{
    public class OrderRepository : BaseRepository, IOrderRepository
    {
        public OrderRepository(DataContext dataContext, IMapper mapper) : base(dataContext, mapper)
        {
        }

        #region Checkout
        public async Task<CheckoutDto> CheckOut(int userId, List<CheckoutItem> items)
        {
            if (items == null || !items.Any())
                throw new HttpException("No item available!");

            if (items.GroupBy(i => i.StoreItemId).Any(g => g.Count() > 1))
                throw new HttpException("Duplicate StoreItem is not allowed");

            if (items.Any(i => i.ItemQuantity <= 0))
                throw new HttpException("Invalid item quantity.");

            var store = await DataContext.StoreItems
                .Where(si => si.Id == items.First().StoreItemId)
                .Select(si => si.Store)
                .FirstOrDefaultAsync() ?? throw new HttpException("Unable to find store.");
            bool isValidStore = true;

            foreach (var item in items)
            {
                var storeItem = await DataContext.StoreItems
                    .Where(si => si.Id == item.StoreItemId)
                    .Select(si => new
                    {
                        si.Id,
                        si.StoreId,
                        si.Available,
                        si.ProductId,
                        si.Product.Name,
                        si.Product.Amount
                    })
                    .FirstOrDefaultAsync() ?? throw new HttpException($"Item not available with id:{item.StoreItemId}");

                item.AmountPerUnit = storeItem.Amount;
                item.Total = storeItem.Amount * item.ItemQuantity;
                item.Name = storeItem.Name;
                item.ProductId = storeItem.ProductId;
                item.ErrorMessage = null;

                if (storeItem.StoreId != store.Id)
                    isValidStore = false;

                if (storeItem.Available <= 0)
                    item.ErrorMessage = $"The item {storeItem.Name} is sold out!";
                else if (storeItem.Available < item.ItemQuantity)
                    item.ErrorMessage = $"Only {storeItem.Available} unit(s) of {storeItem.Name} are available now!";
            }

            var checkout = new CheckoutDto
            {
                Items = items,
                Price = items.Sum(i => i.Total),
                IsValid = isValidStore && items.All(i => string.IsNullOrEmpty(i.ErrorMessage))
            };

            if (!isValidStore)
            {
                checkout.ErrorMessage = "Items belong to more than one store!";
                return checkout;
            }

            checkout.StoreId = store.Id;
            checkout.StoreName = store.Name;
            checkout.Total = checkout.Price;

            return checkout;
        }

        #endregion

        #region Order

        public async Task<int> OrderItems(int userId, OrderRequestDto orderRequest)
        {
            var order = await CreateOrder(userId, orderRequest) ?? throw new HttpException("Order validation failed");

            await using var transaction = await DataContext.Database.BeginTransactionAsync();
            try
            {
                var itemIds = order.OrderItems.Select(i => i.StoreItemId).ToArray();

                var storeItems = await DataContext.StoreItems.Where(i => itemIds.Contains(i.Id)).ToListAsync();

                var productUpdate = new List<int>();

                foreach (var item in order.OrderItems)
                {
                    var storeItem = storeItems.First(i => i.Id == item.StoreItemId);
                    if (storeItem.Available < item.Count)
                        throw new HttpException("Item(s) not available.");
                    storeItem.Available -= item.Count;
                    if (storeItem.Available == 0) productUpdate.Add(storeItem.ProductId);
                    item.Status = Status.Ordered;
                }

                var from = await DataContext.Accounts
                    .Where(a => a.User.Id == userId)
                    .Select(a => a.User.AccountId)
                    .SingleAsync();
                var to = await DataContext.Accounts
                    .Where(a => a.Store.Id == order.StoreId)
                    .Select(a => a.Store.AccountId)
                    .SingleAsync();

                var accTransaction = await ProcessTransaction(from, to, order.TotalAmount, $"Transaction on order #{order.Id}");

                order.Status = Status.Confirmed;
                accTransaction.Type = TransactionType.Order;
                order.Transaction = accTransaction;

                if (!await SaveChanges()) throw new HttpException("Failed to order.");
                await transaction.CommitAsync();
                await UpdateProductAvailability(productUpdate);
            }
            catch
            {
                await transaction.RollbackAsync();
                order.Status = Status.Failed;
                foreach (var item in order.OrderItems) item.Status = Status.Failed;
                await SaveChanges();
                throw;
            }

            return order.Id;
        }

        private async Task<Order> CreateOrder(int userId, OrderRequestDto orderRequest)
        {
            if (orderRequest.Items == null || !orderRequest.Items.Any() ||
                orderRequest.Items.GroupBy(i => i.StoreItemId).Any(g => g.Count() > 1)
                || orderRequest.Items.Any(i => i.ItemQuantity <= 0))
                return null;

            string userAddress = await DataContext.Users
                .Where(a => a.Id == userId)
                .Select(a => a.Address)
                .FirstOrDefaultAsync();

            if (userAddress == null) return null;

            var storeInfo = await DataContext.StoreItems
                .Where(si => si.Id == orderRequest.Items.First().StoreItemId)
                .Select(si => new { si.StoreId, si.Store.Address })
                .FirstOrDefaultAsync();
            if (storeInfo == null) return null;

            foreach (var item in orderRequest.Items)
            {
                var result = await DataContext.StoreItems
                    .Where(si => si.Id == item.StoreItemId && si.StoreId == storeInfo.StoreId && item.ItemQuantity <= si.Available)
                    .AnyAsync();
                if (!result) return null;
            }

            orderRequest.StoreId = storeInfo.StoreId;

            var totalPrice = await GetTotalPrice(orderRequest);
            var deliveryCharge = totalPrice < 500 ? 0 : 40.0;

            if (Math.Abs(totalPrice + deliveryCharge - orderRequest.TotalAmount) > 0.001) return null;

            var order = new Order
            {
                UserId = userId,
                Type = "Product",
                Created = DateTime.UtcNow,
                Update = DateTime.UtcNow,
                Status = Status.Created,
                StoreId = orderRequest.StoreId,
                TotalAmount = orderRequest.TotalAmount,
                Address = userAddress,
                DeliveryCharge = deliveryCharge,
                OrderItems = new List<OrderItem>()
            };

            foreach (var item in orderRequest.Items)
            {
                order.OrderItems.Add(new OrderItem
                {
                    Status = Status.Created,
                    Count = item.ItemQuantity,
                    Price = item.Price,
                    StoreItemId = item.StoreItemId
                });
            }

            await DataContext.Orders.AddAsync(order);

            return await SaveChanges() ? order : null;
        }

        private async Task<double> GetTotalPrice(OrderRequestDto orderRequest)
        {
            double total = 0;

            var ids = orderRequest.Items.Select(i => i.StoreItemId).ToArray();

            var storeItems = await DataContext.StoreItems
                .Where(si => ids.Contains(si.Id)).Select(si => new
                {
                    si.Id,
                    si.Product.Amount
                }).ToListAsync();

            foreach (var item in orderRequest.Items)
            {
                var amount = storeItems.First(i => i.Id == item.StoreItemId).Amount;
                item.Price = amount;
                total += amount * item.ItemQuantity;
            }

            return total;
        }

        public async Task<Response<UserOrderDto, BaseParams>> GetUserOrders(int userId, BaseParams @params)
        {
            var orders = DataContext.Orders
                .Where(o => o.UserId == userId)
                .OrderByDescending(o => o.Created)
                .ProjectTo<UserOrderDto>(Mapper.ConfigurationProvider)
                .AsNoTracking();

            return await Response<UserOrderDto, BaseParams>.CreateAsync(orders, @params);
        }

        public async Task<UserOrderDetailDto> GetUserOrder(int userId, int orderId)
        {
            var order = await DataContext.Orders
                .Where(o => o.UserId == userId && o.Id == orderId)
                .ProjectTo<UserOrderDetailDto>(Mapper.ConfigurationProvider)
                .AsNoTracking()
                .FirstOrDefaultAsync();

            return order;
        }

        #endregion

        #region Cart

        public async Task<List<CartStoreDto>> GetCart(int userId)
        {
            var items = await DataContext.CartItems
                .Where(i => i.UserId == userId)
                .ProjectTo<CartItemDto>(Mapper.ConfigurationProvider)
                .AsNoTracking()
                .ToListAsync();

            var group = items.GroupBy(i => i.StoreId);
            var result = new List<CartStoreDto>();
            foreach (var item in group)
            {
                result.Add(new CartStoreDto
                {
                    StoreId = item.First().StoreId,
                    StoreName = item.First().StoreName,
                    CartItems = item.ToList()
                });
            }
            return result;
        }

        public async Task<CartItemDto> GetCart(int userId, int storeItemId)
        {
            return await DataContext.CartItems
                .Where(i => i.UserId == userId && i.StoreItemId == storeItemId)
                .ProjectTo<CartItemDto>(Mapper.ConfigurationProvider)
                .AsNoTracking()
                .FirstOrDefaultAsync();
        }

        public async Task<CartItem> AddToCart(int userId, int storeItemId, int productId)
        {
            if (storeItemId != 0)
                return await AddToCart(userId, storeItemId);

            var userLocation = await GetUserLocation(userId);
            storeItemId = await DataContext.StoreItems
                .Where(si => si.ProductId == productId && si.Available > 0)
                .Select(si => si.Id)
                .FirstOrDefaultAsync();

            if (storeItemId != 0)
                return await AddToCart(userId, storeItemId);

            storeItemId = await DataContext.StoreItems
                .Where(si => si.ProductId == productId)
                .Select(si => si.Id)
                .FirstOrDefaultAsync();

            if (storeItemId == 0)
                throw new HttpException("Invalid Product");

            return await AddToCart(userId, storeItemId);
        }

        private async Task<CartItem> AddToCart(int userId, int storeItemId)
        {
            var exist = await DataContext.CartItems.AnyAsync(i => i.UserId == userId && i.StoreItemId == storeItemId);
            if (exist) throw new HttpException("Item already in cart.");
            exist = await DataContext.StoreItems.AnyAsync(i => i.Id == storeItemId);
            if (!exist) throw new HttpException("Invalid Store Item");
            var item = new CartItem { StoreItemId = storeItemId, UserId = userId };
            DataContext.CartItems.Add(item);
            return item;
        }

        public async Task<bool> RemoveFromCart(int userId, int[] storeItemIds)
        {
            var cartItem = await DataContext.CartItems.Where(i => i.UserId == userId && storeItemIds.Contains(i.StoreItemId)).ToListAsync();
            if (cartItem.Count == 0) return false;
            DataContext.CartItems.RemoveRange(cartItem);
            return true;
        }

        #endregion
    }
}
