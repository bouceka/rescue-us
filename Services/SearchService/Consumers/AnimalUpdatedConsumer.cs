using AutoMapper;
using Events;
using MassTransit;
using MongoDB.Entities;

namespace SearchService;
public class AnimalUpdatedConsumer : IConsumer<AnimalUpdated>
{
    private readonly IMapper _mapper;

    public AnimalUpdatedConsumer(IMapper mapper)
    {
        _mapper = mapper;
    }

    public async Task Consume(ConsumeContext<AnimalUpdated> animalUpdated)
    {
        Console.WriteLine("Consuming animal update " + animalUpdated.Message.Id);

        var animal = _mapper.Map<Animal>(animalUpdated.Message);

        var result = await DB.Update<Animal>().Match(animal => animal.ID == animalUpdated.Message.Id).ModifyOnly(
            animal => new
            {
                animal.Name,
                animal.Age,
                animal.Description,
                animal.Breed,
                animal.Sex,
                animal.Weight,
                animal.Color,
                animal.Type,
                animal.CoverImageUrl,
                animal.UpdatedAt,

            }, animal).ExecuteAsync();
        if (!result.IsAcknowledged)
            throw new MessageException(typeof(AnimalUpdated), "Problem updating mongodb");
    }
}