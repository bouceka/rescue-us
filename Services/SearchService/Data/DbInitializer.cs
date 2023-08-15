using MongoDB.Driver;
using MongoDB.Entities;
using SearchService.Services;

namespace SearchService.Data
{
    public class DbInitializer
    {


        public static async Task InitDb(WebApplication app)
        {
            await DB.InitAsync("SearchDb", MongoClientSettings
                       .FromConnectionString(app.Configuration.GetConnectionString("MongoDbConnection")));

            await DB.Index<Animal>()
                .Key(x => x.Type, KeyType.Text)
                .Key(x => x.Sex, KeyType.Text)
                .CreateAsync();

            var count = await DB.CountAsync<Animal>();

            using var scope = app.Services.CreateScope();

            var httpClient = scope.ServiceProvider.GetRequiredService<AnimalServiceHttpClient>();

            var animals = await httpClient.GetAnimalsForSearchDb();
            Console.WriteLine(animals.Count + " returned from the animal service");

            if (animals.Count > 0) await DB.SaveAsync(animals);
        }
    }
}