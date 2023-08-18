using AnimalService.DTOs;
using AnimalService.Entities;
using AutoMapper;
using Events;
using AnimalService.Helpers;
using Events.Models;

namespace AnimalService.Helpers
{
    public class ProfileMapper : Profile
    {
        public ProfileMapper()
        {
            CreateMap<Animal, AnimalDto>().ForMember(animal => animal.CoverImageUrl,
                    opt => opt.MapFrom(src => src.Images.FirstOrDefault(x => x.IsMain).Url)).IncludeMembers(x => x.Address).IncludeMembers(x => x.Images);
            CreateMap<AddressDto, AnimalDto>();

            CreateMap<Address, AddressDto>();
            CreateMap<Address, AnimalDto>();

            // From flat structure into nested structure
            CreateMap<CreateAnimalDto, Animal>().ForMember(d => d.Address, o => o.MapFrom(s => s)).ForMember(x => x.Slug, opt => opt.MapFrom(animal => $"{GlobalHelper.GenerateSlug(animal.Name)}-{animal.PublicId}"));
            CreateMap<CreateAnimalDto, Address>();

            // Publish it in nested structure
            CreateMap<AnimalDto, AnimalCreated>().IncludeMembers(x => x.Address).IncludeMembers(x => x.Images);
            CreateMap<AddressDto, AnimalCreated>();
            CreateMap<AddressDto, EventAddress>();
            CreateMap<List<ImageDto>, AnimalCreated>();

            CreateMap<Animal, AnimalUpdated>().IncludeMembers(x => x.Address).IncludeMembers(x => x.Images);
            CreateMap<Address, EventAddress>();
            CreateMap<Image, EventImage>();
            CreateMap<List<Image>, AnimalUpdated>();


            CreateMap<List<Image>, AnimalDto>();
            CreateMap<Image, ImageDto>();
        }
    }
}