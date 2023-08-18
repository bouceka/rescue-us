using AutoMapper;
using Events;
using Events.Models;
using SearchService.Model;

namespace SearchService;
public class ProfileMapper : Profile
{
    public ProfileMapper()
    {

        CreateMap<AnimalCreated, Animal>().IncludeMembers(x => x.Address).IncludeMembers(x => x.Images);
        
        CreateMap<EventAddress, Address>();
        CreateMap<EventImage, Image>();
        CreateMap<List<EventImage>, Animal>();
        
        CreateMap<AnimalUpdated, Animal>().IncludeMembers(x => x.Address).IncludeMembers(x => x.Images);
    }
}