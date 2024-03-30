using System.Threading.Tasks;
using ShoppingCart.Api.Repositories.Contracts;

namespace ShoppingCart.Api.Uow
{
    public interface IUnitOfWork
    {
        IUserRepository UserRepository { get; }
        ISearchRepository SearchRepository { get; }
        IProductRepository ProductRepository { get; }
        IOrderRepository OrdersRepository { get; }
        IStoreRepository StoreRepository { get; }
        Task<bool> SaveChanges();
        bool HasChanges();
    }
}
