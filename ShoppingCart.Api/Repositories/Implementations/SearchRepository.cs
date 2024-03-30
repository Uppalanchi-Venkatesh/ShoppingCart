using ShoppingCart.Api.Data;
using ShoppingCart.Api.DTOs;
using ShoppingCart.Api.Extensions;
using ShoppingCart.Api.Helpers;
using ShoppingCart.Api.Repositories.Contracts;
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
    public class SearchRepository : BaseRepository, ISearchRepository
    {
        public SearchRepository(DataContext dataContext, IMapper mapper) : base(dataContext, mapper)
        {
        }

        #region Search

        public async Task<Response<ProductDto, SearchContextDto>> Search(Dictionary<string, string> queryParams)
        {
            var context = new SearchContext(new Dictionary<string, string>(queryParams, StringComparer.OrdinalIgnoreCase));
            if (string.IsNullOrEmpty(context.SearchText))
            {
                throw new HttpException("Search value shouldn't be empty.");
            }
            return await GetProducts(context);
        }

        private async Task<Response<ProductDto, SearchContextDto>> GetProducts(SearchContext context)
        {
            var productsQuery = ApplyFilters(context);

            var products = context.OrderBy switch
            {
                OrderBy.HighToLow => from product in DataContext.Products
                                     orderby product.Amount descending
                                     select product,
                OrderBy.LowToHigh => from product in DataContext.Products
                                     orderby product.Amount
                                     select product,
                OrderBy.Latest => from product in DataContext.Products
                                  orderby product.Created descending
                                  select product,
                _ => from product in DataContext.Products
                     select product
            };

            var resultQuery = products
                .ProjectTo<ProductDto>(Mapper.ConfigurationProvider)
                .AsNoTracking();

            resultQuery = resultQuery.AddPagination(context.PageNumber, context.PageSize);

            return Response<ProductDto, SearchContextDto>.Create(await resultQuery.ToListAsync(), Mapper.Map<SearchContextDto>(context));
        }

        private IQueryable<Product> ApplyFilters(SearchContext context)
        {
            var query = DataContext.Products.AsQueryable();
            if (!string.IsNullOrEmpty(context.Category))
            {
                query.Where(x => x.Category.ToLower() == context.Category.ToLower());

                foreach (var filter in context.Filters)
                {
                    var values = filter.Value.Split(',');
                }
            }

            if (context.PriceFrom != null)
                query = query.Where(p => p.Amount >= context.PriceFrom);
            if (context.PriceTo != null)
                query = query.Where(p => p.Amount <= context.PriceTo);

            if (!string.IsNullOrEmpty(context.Brand))
            {
                var values = context.Brand.Split(',');
                query = query.Where(p => values.Contains(p.Brand));
            }

            return query;
        }

        #endregion
    }
}
