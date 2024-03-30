using System.Threading.Tasks;
using ShoppingCart.Api.Data.Entities;

namespace ShoppingCart.Api.Services.Contracts
{
    public interface ITokenService
    {
        Task<string> CreateToken(User user, string existingToken = null);
    }
}