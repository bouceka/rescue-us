using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AnimalService.DTOs;
using AnimalService.Entities;
using AutoMapper;
using Events;

namespace AnimalService.Helpers
{
    public class ProfileMapper : Profile
    {
        public ProfileMapper()
        {
            CreateMap<Animal, AnimalDto>().IncludeMembers(x => x.Address);
            CreateMap<AddressDto, AnimalDto>();

            CreateMap<Address, AddressDto>();
            CreateMap<Address, AnimalDto>();
            
            // From flat structure into nested
            CreateMap<CreateAnimalDto, Animal>().ForMember(d => d.Address, o => o.MapFrom(s => s));
            CreateMap<CreateAnimalDto, Address>();

            // Publish it in flat structure
            CreateMap<AnimalDto, AnimalCreated>().IncludeMembers(x => x.Address);;
            CreateMap<AddressDto, AnimalCreated>();

            CreateMap<Animal, AnimalUpdated>();
        }
    }
}