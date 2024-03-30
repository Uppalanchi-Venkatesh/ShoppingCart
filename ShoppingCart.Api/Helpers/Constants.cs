namespace ShoppingCart.Api.Helpers
{
    public class TransactionType
    {
        public const string Order = "Order";
    }

    public class Constants
    {
        public const string TestUser = "test_user";
        public const string Admin = "admin";
        public const string OrderBy = "OrderBy";
        public const string Price = "Price";
        public const string Brand = "Brand";
        public const string Category = "Category";
        public const string ShoppingCartWallet = "ShoppingCart Wallet";
    }

    public class OrderBy
    {
        public const string HighToLow = "HighToLow";
        public const string LowToHigh = "LowToHigh";
        public const string Latest = "Latest";
        public const string Oldest = "Oldest";
        public const string Default = "Default";
    }

    public class Status
    {
        public const string Created = "Created";
        public const string Failed = "Failed";
        public const string Confirmed = "Confirmed";
        public const string Ordered = "Ordered";
    }
}
