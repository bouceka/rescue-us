using AutoMapper;
using Events;


namespace SearchService;
public class ProfileMapper : Profile
{
    public ProfileMapper()
    {

        CreateMap<AnimalCreated, Animal>();
        CreateMap<AnimalUpdated, Animal>();
    }
}