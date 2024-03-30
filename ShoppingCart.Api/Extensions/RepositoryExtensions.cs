using Microsoft.Extensions.DependencyInjection;
using ShoppingCart.Api.Repositories.Contracts;
using ShoppingCart.Api.Repositories.Implementations;

namespace ShoppingCart.Api.Extensions
{
    public static class RepositoryExtensions
    {
        public static IServiceCollection AddRepositoryExtensions(this IServiceCollection services)
        {
            services.AddScoped<BaseRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<ISearchRepository, SearchRepository>();
            services.AddScoped<IStoreRepository, StoreRepository>();
            services.AddScoped<IOrderRepository, OrderRepository>();

            return services;
        }
    }
}
