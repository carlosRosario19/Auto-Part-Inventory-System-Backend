using AutoMapper;
using AutoPartInventorySystem.DTOs.AutoPart;
using AutoPartInventorySystem.Models;

namespace AutoPartInventorySystem.Profiles
{
    public class AutoPartProfile : Profile
    {
        public AutoPartProfile()
        {
            // AutoPart → AutoPartDto
            CreateMap<AutoPart, AutoPartDto>()
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.Name))
                .ForMember(dest => dest.Brands, opt => opt.MapFrom(src => src.Brands));

            // AddAutoPartDto → AutoPart
            CreateMap<AddAutoPartDto, AutoPart>()
                .ForMember(dest => dest.ImageUrl, opt => opt.Ignore())
                .ForMember(dest => dest.Brands, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow));

            // UpdateAutoPartDto → AutoPart
            CreateMap<UpdateAutoPartDto, AutoPart>()
                .ForMember(dest => dest.ImageUrl, opt => opt.Ignore())
                .ForMember(dest => dest.Brands, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow));
        }
    }
}
