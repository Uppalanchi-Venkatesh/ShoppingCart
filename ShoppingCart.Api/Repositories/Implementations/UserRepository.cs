using ShoppingCart.Api.Data;
using ShoppingCart.Api.DTOs;
using ShoppingCart.Api.Repositories.Contracts;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace ShoppingCart.Api.Repositories.Implementations
{
    public class UserRepository : BaseRepository, IUserRepository
    {
        public UserRepository(DataContext dataContext, IMapper mapper) : base(dataContext, mapper)
        {

        }

        public async Task<UserProfileDto> GetProfile(int id)
        {
            var user = await DataContext.Users
                .Where(u => u.Id == id)
                .ProjectTo<UserProfileDto>(Mapper.ConfigurationProvider)
                .AsNoTracking()
                .FirstOrDefaultAsync();
            return user;
        }

        /*public async Task<AddressDto> GetAddress(int userId)
        {
            var address = await DataContext.Users
                .Where(u => u.Id == userId)
                .Select(u => u.Address)
                .ProjectTo<AddressDto>(Mapper.ConfigurationProvider)
                .AsNoTracking()
                .FirstOrDefaultAsync();
            
            return address;
        }*/

        /*public async Task UpdateAddress(int userId, AddressDto addressDto)
        {
            if (!await DataContext.Locations.AnyAsync(l => l.Id == addressDto.AreaId &&
                                                           l.Type.Equals(LocationType.Area.ToString())))
                throw new HttpException("Invalid location Id of Area.");

            var user = await DataContext.Users.Include(u => u.Address).SingleAsync(u => u.Id == userId);
            user.Address = "";
        }*/

        /*public async Task RemoveAddress(int userId)
        {
            var address = await DataContext.Addresses
                .Where(a => a.User.Id == userId)
                .SingleOrDefaultAsync();
            if (address != null)
                DataContext.Addresses.Remove(address);
        }*/
    }
}
