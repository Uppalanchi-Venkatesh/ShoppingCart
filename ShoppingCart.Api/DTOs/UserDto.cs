namespace ShoppingCart.Api.DTOs
{
    public class UserDto
    {
        public string UserName { get; set; }
        public string Token { get; set; }
        public string Name { get; set; }
    }

    public class UserInfoDto
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Name { get; set; }
        public bool Exist { get; set; }
    }
}
