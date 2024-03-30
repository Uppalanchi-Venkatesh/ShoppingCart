using System.Threading.Tasks;
using ShoppingCart.Api.Data.Entities;
using ShoppingCart.Api.DTOs;
using ShoppingCart.Api.Helpers;

namespace ShoppingCart.Api.Repositories.Contracts
{
    public interface IStoreRepository
    {
        Task<StoreOrderDto> GetOrder(int userId, int orderId);
        Task<Response<StoreOrderDto, OrderParams>> GetOrders(OrderParams orderParams, int userId);
    }
}
