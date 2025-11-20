using AutoMapper;
using AutoPartInventorySystem.DTOs.Category;
using AutoPartInventorySystem.Models;

namespace AutoPartInventorySystem.Profiles
{
    public class CategoryProfile : Profile
    {
        public CategoryProfile()
        {
            CreateMap<Category, CategoryDto>();

            CreateMap<AddCategoryDto, Category>()
                .ForMember(dest => dest.ImageUrl, opt => opt.Ignore()); // handled manually

            CreateMap<UpdateCategoryDto, Category>()
                .ForMember(dest => dest.ImageUrl, opt => opt.Ignore()); // handled manually
        }
    }
}
