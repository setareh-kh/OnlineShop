using AutoMapper;
using OnlineShop.Dtos.Requests;
using OnlineShop.Models;

namespace OnlineShop
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            //mapping requestDtos to models
            CreateMap<AddCategoryRequest, Category>();
            CreateMap<UpdateCategoryRequest,Category>().ForAllMembers(options =>options.Condition((src, dest, srcMember) =>
                    srcMember != null));

        }
    }
}