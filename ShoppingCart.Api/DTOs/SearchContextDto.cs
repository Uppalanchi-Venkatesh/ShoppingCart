using System.Collections.Generic;
using ShoppingCart.Api.Helpers;

namespace ShoppingCart.Api.DTOs
{
    public class SearchContextDto : BaseParams
    {
        public string SearchText { get; set; }
        public string Price { get; set; }
        public string Category { get; set; }
        public string Brand { get; set; }
        public Dictionary<string, string> Filters { get; set; }
    }
}
