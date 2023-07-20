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
    public class ProfileMapper:Profile
    {
        public ProfileMapper(){
             CreateMap<Animal, AnimalDto>();
             CreateMap<CreateAnimalDto, Animal>();
             CreateMap<AnimalDto, AnimalCreated>();
             CreateMap<Animal, AnimalUpdated>();
        }
    }
}