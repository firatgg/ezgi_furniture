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
                
            CreateMap<ContactMessage, ContactMessageDto>();
            CreateMap<ContactMessageDto, ContactMessage>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedDate, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedDate, opt => opt.Ignore())
                .ForMember(dest => dest.IsRead, opt => opt.Ignore());
            CreateMap<SocialPost, SocialPostDto>().ReverseMap();
        }
    }
}
