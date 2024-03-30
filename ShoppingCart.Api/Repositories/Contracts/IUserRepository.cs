using System.Collections.Generic;
using ShoppingCart.Api.DTOs;
using System.Threading.Tasks;

namespace ShoppingCart.Api.Repositories.Contracts
{
    public interface IUserRepository
    {
        Task<int> GetUserIdByUserName(string userName);
        Task<UserInfoDto> GetUserInfo(string userName);
        Task<bool> UserExist(string userName);
        Task<UserProfileDto> GetProfile(int id);
        /*Task<AddressDto> GetAddress(int userId);
        Task UpdateAddress(int userId, AddressDto address);
        Task RemoveAddress(int userId);*/
    }
}
