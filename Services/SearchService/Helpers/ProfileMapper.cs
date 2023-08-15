using AutoMapper;
using Events;
using SearchService.Model;

namespace SearchService;
public class ProfileMapper : Profile
{
    public ProfileMapper()
    {

        CreateMap<AnimalCreated, Animal>().ForMember(d => d.Address, o => o.MapFrom(s => s));
        CreateMap<AnimalCreated, Address>();
        CreateMap<AnimalUpdated, Animal>().ForMember(d => d.Address, o => o.MapFrom(s => s));
    }
}