using AutoMapper;
using Events;
using SearchService.Model;

namespace SearchService;
public class ProfileMapper : Profile
{
    public ProfileMapper()
    {

        CreateMap<AnimalCreated, Animal>().IncludeMembers(x => x.Address).IncludeMembers(x => x.Images);
        CreateMap<AnimalCreated, Address>();
        CreateMap<AnimalCreated, Image>();
        CreateMap<AnimalUpdated, Animal>().IncludeMembers(x => x.Address).IncludeMembers(x=>x.Images);
    }
}