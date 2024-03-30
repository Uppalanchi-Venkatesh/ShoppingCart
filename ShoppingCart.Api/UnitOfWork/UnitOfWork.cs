using ShoppingCart.Api.Data;
using ShoppingCart.Api.Repositories.Contracts;
using ShoppingCart.Api.Repositories.Implementations;
using AutoMapper;
using System.Threading.Tasks;

namespace ShoppingCart.Api.Uow
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;

        public IUserRepository UserRepository => new UserRepository(_context, _mapper);
        public ISearchRepository SearchRepository => new SearchRepository(_context, _mapper);
        public IProductRepository ProductRepository => new ProductRepository(_context, _mapper);
        public IOrderRepository OrdersRepository => new OrderRepository(_context, _mapper);
        public IStoreRepository StoreRepository => new StoreRepository(_context, _mapper);

        public UnitOfWork(DataContext dataContext, IMapper mapper)
        {
            _context = dataContext;
            _mapper = mapper;
        }

        public async Task<bool> SaveChanges()
        {
            return await _context.SaveChangesAsync() > 0;
        }

        public bool HasChanges()
        {
            return _context.ChangeTracker.HasChanges();
        }
    }
}
