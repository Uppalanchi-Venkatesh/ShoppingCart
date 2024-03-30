using ShoppingCart.Api.Data;
using ShoppingCart.Api.DTOs;
using ShoppingCart.Api.Extensions;
using ShoppingCart.Api.Repositories.Contracts;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShoppingCart.Api.Repositories.Implementations
{
    public class ProductRepository : BaseRepository, IProductRepository
    {
        public ProductRepository(DataContext dataContext, IMapper mapper) : base(dataContext, mapper)
        {
        }

        public async Task<ProductModelDto> GetProduct(int productId, int userId)
        {
            var userLocation = await GetUserLocation(userId);

            var model = await DataContext.Products
                .Where(p => p.Id == productId)
                .Select(p => p.Model)
                .FirstOrDefaultAsync();

            if (string.IsNullOrEmpty(model)) return null;

            var products = await GetProducts(model);

            return products.GetProductModel();
        }

        private async Task<List<ProductDetailDto>> GetProducts(string model)
        {
            var products = await DataContext.Products
                .Where(p => p.Model == model)
                .ProjectTo<ProductDetailDto>(Mapper.ConfigurationProvider)
                .AsNoTracking()
                .ToListAsync();

            foreach (var product in products)
            {
                product.DeliveryCharge = product.Amount >= 500 ? 0 : 60;

                if (product.Available && string.IsNullOrEmpty(product.StoreName))
                {
                    var storeItem = await DataContext.StoreItems
                        .Where(si => si.ProductId == product.Id && si.Available > 0)
                        .Select(si => new { si.Store.Name, si.Id })
                        .AsNoTracking()
                        .FirstOrDefaultAsync();

                    if (storeItem != null)
                    {
                        product.StoreItemId = storeItem.Id;
                        product.StoreName = storeItem.Name;
                    }
                }
            }
            return products;
        }
    }
}
