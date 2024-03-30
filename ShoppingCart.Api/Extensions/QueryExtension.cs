using ShoppingCart.Api.DTOs;
using System.Collections.Generic;
using System.Linq;

namespace ShoppingCart.Api.Extensions
{
    public static class QueryExtension
    {
        public static IQueryable<T> AddPagination<T>(this IQueryable<T> query, int pageNumber, int pageSize)
        {
            return query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize);
        }

        public static ProductModelDto GetProductModel(this List<ProductDetailDto> products)
        {
            var productModel = new ProductModelDto();

            foreach (var product in products)
            {
                productModel.Products.Add(product.Id, product);
            }
            return productModel;
        }
    }
}
