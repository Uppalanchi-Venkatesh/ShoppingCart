using ShoppingCart.Api.Data;
using ShoppingCart.Api.DTOs;
using ShoppingCart.Api.Helpers;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ShoppingCart.Api.Data.Entities;

namespace ShoppingCart.Api.Repositories.Implementations
{
    public class BaseRepository
    {
        public DataContext DataContext { get; }
        public IMapper Mapper { get; }

        public BaseRepository(DataContext dataContext, IMapper mapper)
        {
            DataContext = dataContext;
            Mapper = mapper;
        }

        #region Save

        public async Task<bool> SaveChanges()
        {
            return await DataContext.SaveChangesAsync() > 0;
        }

        public bool HasChanges()
        {
            return DataContext.ChangeTracker.HasChanges();
        }

        #endregion

        #region User

        public async Task<int> GetUserIdByUserName(string userName)
        {
            return await DataContext.Users.Where(u => u.UserName == userName.ToLower())
                .Select(u => u.Id)
                .SingleOrDefaultAsync();
        }

        public async Task<bool> UserExist(string userName)
        {
            return await DataContext.Users.AnyAsync(u => u.UserName == userName.ToLower());
        }

        public async Task<UserInfoDto> GetUserInfo(string userName)
        {
            var user = await DataContext.Users
                .Where(u => u.UserName == userName.ToLower())
                .ProjectTo<UserInfoDto>(Mapper.ConfigurationProvider).FirstOrDefaultAsync();

            if (user == null)
                user = new UserInfoDto();
            else
                user.Exist = true;

            return user;
        }

        public async Task<string> GetUserLocation(int userId)
        {
            if (userId == 0)
                return null;

            return await DataContext.Users.Where(a => a.Id == userId)
                .Select(a => a.Address)
                .FirstOrDefaultAsync();
        }
        #endregion

        #region Product

        public async Task UpdateProductAvailability(List<int> productIds)
        {
            foreach (var productId in productIds)
            {
                var product = await DataContext.Products.Where(p => p.Id == productId).FirstAsync();
                if (await DataContext.StoreItems.Where(si => si.ProductId == productId && si.Available > 0).AnyAsync())
                {
                    continue;
                }
                await SaveChanges();
            }
        }

        #endregion

        #region Account

        public async Task<Transaction> ProcessTransaction(int from, int to, double amount, string description)
        {
            var transaction = new Transaction
            {
                Amount = amount,
                Date = DateTime.UtcNow,
                Description = description,
                FromId = from,
                ToId = to
            };

            var fromAcc = await DataContext.Accounts.SingleAsync(a => a.Id == from);
            var toAcc = await DataContext.Accounts.SingleAsync(a => a.Id == to);

            if (fromAcc.Balance < amount)
                throw new HttpException("Insufficient balance to cover this transactions.");

            fromAcc.Balance -= amount;
            toAcc.Balance += amount;

            return transaction;
        }

        #endregion
    }
}
