using Events;
using MassTransit;
using MongoDB.Entities;

namespace SearchService;
public class AnimalDeletedConsumer : IConsumer<AnimalDeleted>
{
    public async Task Consume(ConsumeContext<AnimalDeleted> animalDeleted)
    {
        Console.WriteLine("Consuming animal delete " + animalDeleted.Message.Id);

        var result = await DB.DeleteAsync<Animal>(animalDeleted.Message.Id);

        if (!result.IsAcknowledged)
            throw new MessageException(typeof(AnimalDeleted), "Problem deleting Course");
    }

}