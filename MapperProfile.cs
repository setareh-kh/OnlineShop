using AutoMapper;
using OnlineShop.Dtos.Requests;
using OnlineShop.Dtos.Responses;
using OnlineShop.Models;

namespace OnlineShop
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            //mapping requestDtos to models
            CreateMap<AddCategoryRequest, Category>().ForMember(dest => dest.Id, opt => opt.Ignore());
            CreateMap<UpdateCategoryRequest, Category>()
                    .ForMember(dest => dest.Id, opt => opt.Ignore())
                    .ForAllMembers(options => options.Condition((src, dest, srcMember) =>
                        srcMember != null));
            CreateMap<AddProductRequest, Product>();
            CreateMap<UpdateProductRequest, Product>()
                        .ForAllMembers(options => options.Condition((src, dest, srcMember) =>
                        srcMember != null));

            //
            CreateMap<Category, CategoryResponse>();
            CreateMap<Product,ProductResponse>();

        }
    }
}