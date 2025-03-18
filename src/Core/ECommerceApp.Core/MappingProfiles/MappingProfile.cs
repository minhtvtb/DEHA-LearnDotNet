using AutoMapper;
using ECommerceApp.Core.DTOs;
using ECommerceApp.Core.Entities;

namespace ECommerceApp.Core.MappingProfiles
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Product, ProductDto>()
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category != null ? src.Category.Name : string.Empty));
            
            CreateMap<ProductDto, Product>();

            CreateMap<Category, CategoryDto>();
            CreateMap<CategoryDto, Category>();

            CreateMap<Order, OrderDto>();
            CreateMap<OrderDto, Order>();

            CreateMap<OrderItem, OrderItemDto>()
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product != null ? src.Product.Name : string.Empty))
                .ForMember(dest => dest.ProductImageUrl, opt => opt.MapFrom(src => src.Product != null ? src.Product.ImageUrl : string.Empty));
            
            CreateMap<OrderItemDto, OrderItem>();

            CreateMap<Review, ReviewDto>()
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product != null ? src.Product.Name : string.Empty));
            
            CreateMap<ReviewDto, Review>();
        }
    }
} 