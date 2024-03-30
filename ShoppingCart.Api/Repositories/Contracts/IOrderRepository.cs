using ShoppingCart.Api.DTOs;
using ShoppingCart.Api.Helpers;
using System.Collections.Generic;
using System.Threading.Tasks;
using ShoppingCart.Api.Data.Entities;

namespace ShoppingCart.Api.Repositories.Contracts;

public interface IOrderRepository
{
    Task<CheckoutDto> CheckOut(int userId, List<CheckoutItem> items);
    Task<int> OrderItems(int userId, OrderRequestDto orderRequest);
    Task<Response<UserOrderDto, BaseParams>> GetUserOrders(int userId, BaseParams @params);
    Task<UserOrderDetailDto> GetUserOrder(int userId, int orderId);
    Task<List<CartStoreDto>> GetCart(int userId);
    Task<CartItemDto> GetCart(int userId, int storeItemId);
    Task<CartItem> AddToCart(int userId, int storeItemId, int productId);
    Task<bool> RemoveFromCart(int userId, int[] storeItemIds);
}