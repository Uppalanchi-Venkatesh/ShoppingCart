﻿using System;
using System.Collections.Generic;

namespace ShoppingCart.Api.Helpers
{
    public class BaseParams
    {
        private const int MaxSize = 25;

        private int _pageNumber = 1;
        public int PageNumber
        {
            get => _pageNumber;
            set => _pageNumber = value <= 0 ? 1 : value;
        }

        private int _pageSize = 12;

        public int PageSize
        {
            get => _pageSize;
            set
            {
                _pageSize = value switch
                {
                    > MaxSize => MaxSize,
                    <= 0 => 12,
                    _ => value
                };
                SetTotalPages();
            }
        }

        public int TotalPages { get; set; }

        private int _totalCount;
        public int TotalCount
        {
            get => _totalCount;
            set
            {
                _totalCount = value;
                SetTotalPages();
            }
        }

        public string OrderBy { get; set; }

        private void SetTotalPages()
        {
            if (_totalCount <= 0) return;
            TotalPages = (int)Math.Ceiling(_totalCount / (decimal)_pageSize);
        }

        public BaseParams() { }

        public BaseParams(Dictionary<string, string> queryParams)
        {
            PageNumber = GetValue(queryParams, nameof(PageNumber));
            PageSize = GetValue(queryParams, nameof(PageSize));
            queryParams.TryGetValue(Constants.OrderBy, out var orderBy);
            OrderBy = orderBy;
        }

        private static int GetValue(Dictionary<string, string> queryParams, string key)
        {
            queryParams.TryGetValue(key, out string value);
            _ = int.TryParse(value, out int intValue);
            return intValue;
        }
    }

    public class OrderParams : BaseParams
    {
        public string Status { get; set; }
        public string StoreName { get; set; }
    }
}
