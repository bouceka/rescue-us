using AnimalService.Entities;
using AnimalService.Helpers;
using Microsoft.EntityFrameworkCore;
using shortid;
using shortid.Configuration;

namespace AnimalService.Data;

public class DbInitializer
{
    public static void InitDb(WebApplication app)
    {
        using var scope = app.Services.CreateScope();

        SeedData(scope.ServiceProvider.GetService<AnimalDbContext>());
    }

    private static void SeedData(AnimalDbContext context)
    {
        context.Database.Migrate();

        if (context.Animals.Any())
        {
            Console.WriteLine("Already have data - no need to seed");
            return;
        }

        var animals = new List<Animal>()
        {
           new Animal
            {
                Id = Guid.Parse("afbee524-5972-4075-8800-7d1f9d7b0a0c"),
                PublicId = GlobalHelper.GenerateShortId(),
                Name = "Dee Dee",
                Type = "Dog",
                Description = "lorem ipsum",
                Breed = "Double doodle",
                Sex = "Female",
                Color = "White",
                Slug= GlobalHelper.GenerateSlug("Dee Dee"),
                Weight = 10,
                Age = 2,
                Status = Status.Available,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                Address = new Address
                {
                    Id = Guid.NewGuid(),
                    AnimalId = Guid.Parse("afbee524-5972-4075-8800-7d1f9d7b0a0c"),
                    Address1 = "1234 45th Ave",
                    Address2 = "",
                    City = "Vancouver",
                    Country = "Canada",
                    State = "BC",
                    PostalCode = "V4R 3D1",
                },
                Images= new List<Image>{
                    new Image
                    {
                    Id = Guid.NewGuid(),
                 Url = "https://placedog.net/500",
                IsMain = true,
                 PublicId = Guid.NewGuid().ToString()
                 }
                 }


            },
            new Animal
            {
                Id = Guid.Parse("6d67f915-3988-4390-d353-08db842b6b67"),
                PublicId = GlobalHelper.GenerateShortId(),
                Name = "Buttercup",
                Type = "Cat",
                Description = "lorem ipsum",
                Breed = "Bengal cat",
                Sex = "Male",
                Color = "Beige",
                Slug = GlobalHelper.GenerateSlug("Buttercup"),
                Weight = 5,
                Age = 5,
                Status = Status.Available,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                Address = new Address
                {
                    Id = Guid.NewGuid(),
                    AnimalId = Guid.Parse("6d67f915-3988-4390-d353-08db842b6b67"),
                    Address1 = "1234 45th Ave",
                    Address2 = "",
                    City = "Vancouver",
                    Country = "Canada",
                    State = "BC",
                    PostalCode = "V4R 3D1",
                },
                 Images= new List<Image>{
                    new Image
                    {
                    Id = Guid.NewGuid(),
                     Url = "https://placekitten.com/200/300",
                    IsMain = true,
                     PublicId = Guid.NewGuid().ToString()
                 }
                 }
            }
        };

        context.AddRange(animals);

        context.SaveChanges();
    }
}
