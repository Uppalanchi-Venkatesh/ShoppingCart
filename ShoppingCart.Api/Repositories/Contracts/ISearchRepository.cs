using System.Collections.Generic;
using System.Threading.Tasks;
using ShoppingCart.Api.DTOs;

namespace ShoppingCart.Api.Repositories.Contracts
{
    public interface ISearchRepository
    {
        Task<Response<ProductDto, SearchContextDto>> Search(Dictionary<string, string> queryParams);
    }
}
