using AutoMapper;
using PhotoGalleryApi.DTOs;
using PhotoGalleryApi.Entities;

namespace PhotoGalleryApi.MappingProfiles
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            // Photo Mapping
            CreateMap<Photo, PhotoResponseDto>();
            CreateMap<PhotoCreateDto, Photo>();
            CreateMap<PhotoResponseDto, Photo>();
            CreateMap<PhotoUpdateDto, Photo>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            // User Mapping
            CreateMap<User, UserResponseDto>();
        }
    }
}
