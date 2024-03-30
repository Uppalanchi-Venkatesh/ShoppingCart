using System.Collections.Generic;
using System.Threading.Tasks;
using ShoppingCart.Api.DTOs;

namespace ShoppingCart.Api.Repositories.Contracts;

public interface IProductRepository
{
    Task<ProductModelDto> GetProduct(int productId, int userId);
}