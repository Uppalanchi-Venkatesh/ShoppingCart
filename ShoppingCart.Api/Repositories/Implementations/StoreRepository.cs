using ShoppingCart.Api.Data;
using ShoppingCart.Api.DTOs;
using ShoppingCart.Api.Repositories.Contracts;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using ShoppingCart.Api.Data.Entities;
using ShoppingCart.Api.Helpers;

namespace ShoppingCart.Api.Repositories.Implementations
{
    public class StoreRepository : BaseRepository, IStoreRepository
    {
        public StoreRepository(DataContext dataContext, IMapper mapper) : base(dataContext, mapper)
        {
        }

        public async Task<StoreOrderDto> GetOrder(int userId, int orderId)
        {
            return await DataContext.Orders.Where(o => o.Id == orderId)
                .ProjectTo<StoreOrderDto>(Mapper.ConfigurationProvider)
                .FirstOrDefaultAsync();
        }

        public async Task<Response<StoreOrderDto, OrderParams>> GetOrders(OrderParams orderParams, int userId)
        {
            var orders = from order in DataContext.Orders
                         where order.UserId == userId
                         select order;

            if (!string.IsNullOrWhiteSpace(orderParams.StoreName))
            {
                var inner = PredicateBuilder.False<Order>();
                foreach (var store in orderParams.StoreName.Split(","))
                {
                    inner = inner.Or(o => o.Store.Name.Contains(store));
                }
                orders = orders.Where(inner);
            }

            if (string.IsNullOrEmpty(orderParams.Status))
                orders = orders.Where(o => o.Status == Status.Confirmed);
            else if (orderParams.Status != "All")
                orders = orders.Where(o => o.Status == orderParams.Status);

            orders = orderParams.OrderBy switch
            {
                OrderBy.Oldest => orders.OrderBy(o => o.Created),
                _ => orders.OrderByDescending(o => o.Created)
            };

            var ordersDto = orders
                .ProjectTo<StoreOrderDto>(Mapper.ConfigurationProvider)
                .AsNoTracking();

            return await Response<StoreOrderDto, OrderParams>.CreateAsync(ordersDto, orderParams);
        }
    }
}
