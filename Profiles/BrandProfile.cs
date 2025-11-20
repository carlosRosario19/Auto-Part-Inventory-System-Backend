using AutoMapper;
using AutoPartInventorySystem.DTOs.Brand;
using AutoPartInventorySystem.Models;

namespace AutoPartInventorySystem.Profiles
{
    public class BrandProfile : Profile
    {
        public BrandProfile()
        {
            CreateMap<AddBrandDto, Brand>()
                .ForMember(dest => dest.ImageUrl, opt => opt.Ignore());

            CreateMap<UpdateBrandDto, Brand>()
                .ForMember(dest => dest.ImageUrl, opt => opt.Ignore());

            CreateMap<Brand, BrandDto>();
        }
    }
}
