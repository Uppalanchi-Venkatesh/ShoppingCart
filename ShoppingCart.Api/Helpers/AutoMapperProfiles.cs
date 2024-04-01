using ShoppingCart.Api.DTOs;
using ShoppingCart.Api.Seed;
using AutoMapper;
using ShoppingCart.Api.Data.Entities;

namespace ShoppingCart.Api.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            #region Entity DTO Mapping

            CreateMap<UserRegisterDto, User>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.UserName.ToLower()));

            CreateMap<User, UserProfileDto>().ReverseMap();

            CreateMap<User, UserInfoDto>();

            CreateMap<SearchContext, SearchContextDto>();

            CreateMap<Product, ProductDetailDto>();

            CreateMap<OrderItem, OrderItemDto>()
                .ForMember(dest => dest.Name, act => act.MapFrom(src => src.StoreItem.Product.Name))
                .ForMember(dest => dest.ProductId, act => act.MapFrom(src => src.StoreItem.ProductId));

            CreateMap<Order, BaseOrderDto>();
            CreateMap<Order, UserOrderDto>()
                .IncludeBase<Order, BaseOrderDto>();
            CreateMap<Order, UserOrderDetailDto>()
                .IncludeBase<Order, UserOrderDto>();
            CreateMap<Order, StoreOrderDto>()
                .ForMember(dest => dest.StoreName, act => act.MapFrom(src => src.Store.Name))
                .IncludeBase<Order, BaseOrderDto>();

            CreateMap<CartItem, CartItemDto>()
                .ForMember(dest => dest.Name, act => act.MapFrom(src => src.StoreItem.Product.Name))
                .ForMember(dest => dest.StoreId, act => act.MapFrom(src => src.StoreItem.StoreId))
                .ForMember(dest => dest.StoreName, act => act.MapFrom(src => src.StoreItem.Store.Name))
                .ForMember(dest => dest.ProductId, act => act.MapFrom(src => src.StoreItem.ProductId))
                .ForMember(dest => dest.Amount, act => act.MapFrom(src => src.StoreItem.Product.Amount))
                .ForMember(dest => dest.Available, act => act.MapFrom(src => src.StoreItem.Available));

            #endregion

            #region SeedData Entity Mapping

            CreateMap<ProductSeed, Product>();

            #endregion
        }
    }
}
