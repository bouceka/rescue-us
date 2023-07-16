using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AnimalService.DTOs;
using AnimalService.Entities;
using AutoMapper;

namespace AnimalService.Helpers
{
    public class ProfileMapper:Profile
    {
        public ProfileMapper(){
             CreateMap<CreateAnimalDto, Animal>();
             CreateMap<Animal, AnimalDto>();
        }
    }
}