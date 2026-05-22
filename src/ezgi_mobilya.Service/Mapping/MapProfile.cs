using AutoMapper;
using ezgi_mobilya.Core.Entities;
using ezgi_mobilya.Service.DTOs;

namespace ezgi_mobilya.Service.Mapping
{
    public class MapProfile : Profile
    {
        public MapProfile()
        {
            CreateMap<Category, CategoryDto>().ReverseMap();
            
            CreateMap<Product, ProductDto>()
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category != null ? src.Category.Name : null))
                .ReverseMap();
                
            CreateMap<ContactMessage, ContactMessageDto>().ReverseMap();
            CreateMap<SocialPost, SocialPostDto>().ReverseMap();
        }
    }
}
