using ShoppingCart.Api.Extensions;
using System;
using System.Collections.Generic;

namespace ShoppingCart.Api.Helpers
{
    public class SearchContext : BaseParams
    {
        public Dictionary<string, string> QueryParams { get; }
        public string SearchText { get; }
        public string Category { get; set; }
        public string Brand { get; set; }
        public int? PriceFrom { get; }
        public int? PriceTo { get; }
        public Dictionary<string, string> Filters { get; }

        public SearchContext(Dictionary<string, string> queryParams) : base(queryParams)
        {
            QueryParams = queryParams ?? new Dictionary<string, string>();
            QueryParams.TryGetValue("q", out var searchText);
            SearchText = searchText?.Trim() ?? string.Empty;
            QueryParams.TryGetValue(Constants.Category, out var category);
            Category = category;
            QueryParams.TryGetValue(Constants.Brand, out var brand);
            Brand = brand;
            if (QueryParams.TryGetValue(Constants.Price, out var range) && range.IsValidIntegerRange())
            {
                range.GetRange(out int? from, out var to);
                PriceFrom = from;
                PriceTo = to;
            }
            Filters = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        }
    }
}
