# Build Microservices Project with .NET Core and RabbitMQ

![Project image](https://res.cloudinary.com/boucekdev/image/upload/f_auto,q_auto/v1/github/hjl7mdpbnrgtnomzajpk)

The concept of microservices architecture has become increasingly popular in recent times as a contemporary method for constructing large and intricate applications. Instead of creating a single, extensive codebase for an application (known as a monolithic approach), microservices architecture involves breaking down the application into smaller, autonomous services that interact with each other through APIs.

Each individual microservice is responsible for a specific business function and can be developed and deployed independently from other services. This approach offers several advantages, such as enhanced flexibility, scalability, and resilience, along with simplified maintenance and testing procedures.

When it comes to implementing microservices, .Net Core is a widely preferred option. In this blog post, we will delve into the fundamentals of microservices architecture using .Net Core and provide some illustrative code examples.

## Useful links

[Postman Collection](https://github.com/bouceka/rescue-us/blob/main/Rescue%20Us.postman_collection.json)

[GitHub Repository](https://github.com/bouceka/rescue-us)

## App Overview

For this blog post, I prepared a project that will focus on adoption animals. This project is going to be primarily a walkthrough tutorial on how to build a scalable app implementing .Net, NextJs, React Native, Docker, and more.

However, for this post, we will be focusing on two simple services and how to handle [asynchronous communication](https://learn.microsoft.com/en-us/dotnet/architecture/microservices/architect-microservice-container-applications/asynchronous-message-based-communication#asynchronous-event-driven-communication) between them with RabbitMQ. We will follow [Database per Service](https://microservices.io/patterns/data/database-per-service.html) pattern to make our services more independent. Also, it allows us to create services with various databases (MSSQL and MongoDB). 

![Illustrative Image](https://res.cloudinary.com/ahoy-house/image/upload/f_auto,q_auto/v1/github/vkcdz78xmmly9s7eldhu)

## Setup microservice architecture in .NET Core

In our new project folder, we will create a solution with services and a docker-compose file.

```
dotnet new sln

touch docker-compose.yml

mkdir Services

cd Services

dotnet new webapi -o AnimalService

dotnet new webapi -o SearchService
```

For our development, we will use Docker Compose where we will spin our database and RabbitMQ. We can spin docker-compose with the following command `docker-compose -f docker-compose.yml up`.

### docker-compose.yml

```yml
version: '3.4'
services:
  sqlserver:
    image: 'mcr.microsoft.com/mssql/server:2022-latest'
    environment:
      ACCEPT_EULA: 'Y'
      MSSQL_SA_PASSWORD: 'Password123'
      MSSQL_PID: 'Express'
    ports:
      - '1433:1433'
    restart: always
    volumes:
      - './drive:/var/opt/mssql'
  mongodb_container:
    image: mongo:latest
    ports:
      - 27017:27017
    volumes:
      - './mongodb_data_container:/data/db'
    restart: always
  rabbitmq:
    image: rabbitmq:3-management-alpine
    ports:
      - 5672:5672
      - 15672:15672
```

## Animal Service

Before we start coding anything we need to install packages that are necessary for our service. With NuGet Package Gallery we can install these packages:

`AutoMapper.Extensions.Microsoft.DependencyInjection`

`MassTransit.EntityFrameworkCore`

`MassTransit.RabbitMQ`

`Microsoft.EntityFrameworkCore`

`Microsoft.EntityFrameworkCore.Design`

`Microsoft.EntityFrameworkCore.SqlServer`

Once we have all packages ready we can start shaping our service. Because I decided to do this tutorial about animal adoption, we need to start with Entity. It is an object that we are going to store in our database.

### Entities/Animal.cs

```cs
namespace AnimalService.Entities
{
    public class Animal
    {
        public Guid Id { get; set; }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PublicId { get; set; }
        public int Age { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string Breed { get; set; }
        public string Sex { get; set; }
        public int Weight { get; set; }
        public string Color { get; set; }
        public string Description { get; set; }
        public string CoverImageUrl { get; set; }
        public Status Status { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
```

### Entities/Status.cs

```cs
namespace AnimalService.Entities
{
    public enum Status
    {
        Available,
        Pending,
        Adopted,
        Found,
        Missing
    }
}
```

Now we have to up Entity Framework. Thank to Nuget Packages we will use Microsoft.EntityFrameworkCore.Design and Microsoft.EntityFrameworkCore.SqlServer packages. Be careful, use the same versions of the packages that match your project's version. framework NuGet package Microsoft.EntityFrameworkCore.Design.

We are going to use [Code First Migration](https://learn.microsoft.com/en-us/ef/ef6/modeling/code-first/migrations/) which means Our database schema is going to be generated based on the we write.

The following step is to create a DB context class that is going to use DbContext from Entity Framework that creates an abstraction of our database. Notice, we also add some seed initial data and [outbox](https://masstransit.io/documentation/patterns/transactional-outbox).

### Data/AnimalDbContext.cs

```cs
namespace AnimalService.Data
{
    public class AnimalDbContext : DbContext
    {
        public AnimalDbContext(DbContextOptions options) : base(options) { }

        public DbSet<Animal> Animals { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Add in memory outbox
            builder.AddInboxStateEntity();
            builder.AddOutboxMessageEntity();
            builder.AddOutboxStateEntity();


            builder.Entity<Animal>().HasData(new Animal
            {
                Id = Guid.NewGuid(),
                PublicId = 1,
                Name = "Dee Dee",
                Type = "Dog",
                Description = "lorem ipsum",
                Breed = "Double doodle",
                Sex = "Female",
                Color = "White",
                Weight = 10,
                Age = 2,
                Status = Status.Available,
                CoverImageUrl = "https://placedog.net/500",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
            });
            builder.Entity<Animal>().HasData(new Animal
            {
                Id = Guid.NewGuid(),
                PublicId = 2,
                Name = "Buttercup",
                Type = "Cat",
                Description = "lorem ipsum",
                Breed = "Bengal cat",
                Sex = "Male",
                Color = "Beige",
                Weight = 5,
                Age = 5,
                Status = Status.Available,
                CoverImageUrl = "https://placekitten.com/200/200",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
            });
        }
    }
}
```

Once we have our DB context class ready we have to specify a connection string for our service. For this example, let's put the connect string into `appsettings.Development.json` right behind `Logging` brackets.

### appsettings.Development.json

```json
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;User Id=sa;Password=Password123;Database=RescueUs_Animals;Trusted_Connection=false;MultipleActiveResultSets=True;TrustServerCertificate=Yes"
  },
  "RabbitMq": {
    "Host": "localhost"
  }
```

Logically I don't want to have this tutorial too long, we will include our DB Context and our RabbitMQ to the `program.cs` at once.

### program.cs

```cs
// Connect to MSSQL with DB Context
builder.Services.AddDbContext<AnimalDbContext>(option =>
{
  option.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

// Configure RabbitMQ
builder.Services.AddMassTransit(x =>
{
    // Add outbox
    x.AddEntityFrameworkOutbox<AnimalDbContext>(o =>
    {
        o.QueryDelay = TimeSpan.FromSeconds(10);

        o.UseSqlServer();
        o.UseBusOutbox();
    });

    x.SetEndpointNameFormatter(new KebabCaseEndpointNameFormatter("animal", false));

    // Setup RabbitMQ Endpoint
    x.UsingRabbitMq((context, cfg) =>
    {

        cfg.Host(builder.Configuration["RabbitMq:Host"], "/", host =>
        {
            host.Username(builder.Configuration.GetValue("RabbitMq:Username", "guest"));
            host.Password(builder.Configuration.GetValue("RabbitMq:Password", "guest"));
        });
        cfg.ConfigureEndpoints(context);
    });
});
```

Now we have to generate an entity migration. Remember we have to install `dotnet-ef` globally to run the following commands.

```
dotnet ef migrations add InitialMigration -o Data/Migrations

dotnet ef database update
```

## Mapper

Now we are going to create mapper profiles that will help us to map our RabbitMQ events and DTOs. The AutoMapper will help us with mapping the classes once we move further in our app.

### Helpers/[ProfileMapper.cs](https://github.com/bouceka/rescue-us/blob/main/Services/AnimalService/Helpers/ProfileMapper.cs)

```cs
public ProfileMapper(){
  CreateMap<Animal, AnimalDto>();
  CreateMap<CreateAnimalDto, Animal>();
  CreateMap<AnimalDto, AnimalCreated>();
  CreateMap<Animal, AnimalUpdated>();
}
```

Finally, we can provide service to our program file.

### Program.cs

```cs
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
```

## Add DTOs

We have to specify what shapes our app requires and what shape of data it is going to return from our AnimalController which we will create shortly.

### DTOs/AnimalDTO, CreateDTO, and UpdateDto

```cs
public class AnimalDto
    {
        public Guid Id { get; set; }
        public int PublicId { get; set; }
        public int Age { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string Breed { get; set; }
        public string Sex { get; set; }
        public int Weight { get; set; }
        public string Color { get; set; }
        public string Description { get; set; }
        public string CoverImageUrl { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
```

```cs
  public class CreateAnimalDto
    {
        [Required]
        public int Age { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Type { get; set; }
        [Required]
        public string Breed { get; set; }
        [Required]
        public string Sex { get; set; }
        [Required]
        public int Weight { get; set; }
        [Required]
        public string Color { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public string CoverImageUrl { get; set; }
        [Required]
        public Status Status { get; set; }
    }
```

```cs
    public class UpdateAnimalDto
    {

        public int Age { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string Breed { get; set; }
        public string Sex { get; set; }
        public int Weight { get; set; }
        public string Color { get; set; }
        public string Description { get; set; }
        public string CoverImageUrl { get; set; }
        public string Status { get; set; }

    }
```

In the last part of our Animal service, we have to create a controller where we will query and edit our data. Also, every time we change the data we publish an event that goes to the event bus (RabbitMQ). Notice publishing the event classes with `_publishEndpoint`.

### Controllers/AnimalsController.cs

```cs
namespace AnimalService.Controllers
{
    [Route("api/[controller]")]
    public class AnimalsController : Controller
    {
        private readonly AnimalDbContext _context;
        private readonly IMapper _mapper;
        private readonly IPublishEndpoint _publishEndpoint;

        public AnimalsController(AnimalDbContext context, IMapper mapper, IPublishEndpoint publishEndpoint)
        {
            _publishEndpoint = publishEndpoint;
            _mapper = mapper;
            _context = context;
        }


        [HttpGet]
        public async Task<ActionResult<List<AnimalDto>>> GetAllAnimals()
        {
            var animals = await _context.Animals
            .OrderBy(x => x.UpdatedAt)
            .ToListAsync();

            return _mapper.Map<List<AnimalDto>>(animals);
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<AnimalDto>> GetAnimalById(Guid id)
        {
            var foundAnimal = await _context.Animals.FirstOrDefaultAsync(x => x.Id == id);

            if (foundAnimal == null) return NotFound();

            return _mapper.Map<AnimalDto>(foundAnimal);
        }
        [HttpPost]
        public async Task<ActionResult<AnimalDto>> CreateAnimal(CreateAnimalDto createAnimalDto)
        {
            var animal = _mapper.Map<Animal>(createAnimalDto);

            _context.Animals.Add(animal);

            var newAnimal = _mapper.Map<AnimalDto>(animal);

            await _publishEndpoint.Publish(_mapper.Map<AnimalCreated>(newAnimal));

            var result = await _context.SaveChangesAsync() > 0;

            if (!result) return BadRequest("Could not save changes to the DB");

            return CreatedAtAction(nameof(GetAnimalById),
                new { animal.Id }, newAnimal);
        }
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateAnimal(Guid id, UpdateAnimalDto updateAnimalDto)
        {

            var animal = await _context.Animals
    .FirstOrDefaultAsync(x => x.Id == id);
            if (animal == null) return NotFound();

            animal.Description = updateAnimalDto.Description ?? animal.Description;
            animal.Name = updateAnimalDto.Name ?? animal.Name;
            animal.Status = updateAnimalDto.Status != null ? EnumHelper.EnumParse(updateAnimalDto.Status, animal.Status) : animal.Status;
            animal.Breed = updateAnimalDto.Breed ?? animal.Breed;
            animal.CoverImageUrl = updateAnimalDto.CoverImageUrl ?? animal.CoverImageUrl;
            animal.Color = updateAnimalDto.Color ?? animal.Color;
            animal.Type = updateAnimalDto.Type ?? animal.Type;
            animal.CoverImageUrl = updateAnimalDto.CoverImageUrl ?? animal.CoverImageUrl;
            animal.Weight = updateAnimalDto.Weight == 0 ? animal.Weight : updateAnimalDto.Weight;
            animal.Age = updateAnimalDto.Age == 0 ? animal.Age : updateAnimalDto.Age;
            animal.UpdatedAt = DateTime.UtcNow;

            await _publishEndpoint.Publish(_mapper.Map<AnimalUpdated>(animal));

            var result = await _context.SaveChangesAsync() > 0;

            if (result) return Ok();

            return BadRequest("Problem saving changes");
        }
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteAnimal(Guid id)
        {
            var animal = await _context.Animals.FindAsync(id);

            if (animal == null) return NotFound();

            _context.Animals.Remove(animal);

            await _publishEndpoint.Publish<AnimalDeleted>(new { Id = animal.Id.ToString() });

            var result = await _context.SaveChangesAsync() > 0;

            if (!result) return BadRequest("Could not update DB");

            return Ok();
        }
    }
}
```

## Events

We will create an empty solution that will contain our events that will consume our RabbitMq broker.

Also, we need to add references to our services like so.

```
dotnet add Services/SearchService/SearchService.csproj reference Services/Events/Events.csproj

dotnet add Services/AnimalService/AnimalService.csproj reference Services/Events/Events. csproj
```

### AnimalCreated.cs

```cs
public class AnimalCreated
{
    public Guid Id { get; set; }
    public int Age { get; set; }
    public string Name { get; set; }
    public string Type { get; set; }
    public string Breed { get; set; }
    public string Sex { get; set; }
    public int Weight { get; set; }
    public string Color { get; set; }
    public string Description { get; set; }
    public string CoverImageUrl { get; set; }
    public string Status { get; set; }
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
}
```

```cs
public class AnimalUpdated
{
    public string Id { get; set; }
    public int Age { get; set; }
    public string Name { get; set; }
    public string Type { get; set; }
    public string Breed { get; set; }
    public string Sex { get; set; }
    public int Weight { get; set; }
    public string Color { get; set; }
    public string Description { get; set; }
    public string CoverImageUrl { get; set; }
    public string Status { get; set; }
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
}
```

```cs
public class AnimalDeleted
{
    public string Id { get; set; }
}
```

## Search Service

This service is going to be fairly simple. We will receive events from RabbitMQ and it will mimic our database from Animal Service and we will query the data in MongoDB.

### Packages

`AutoMapper.Extensions.Microsoft.DependencyInjection`

`Microsoft.Extensions.Http.Polly`

`MassTransit.RabbitMQ`

`MongoDB.Entities`


### Helpers/SearchParams.cs

We will create an object that will help us with searching through our database. Notice, that we also implement a simple pagination.

```cs
public class SearchParams
{
    public string SearchTerm { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 4;
    public string Sex { get; set; }
    public string Type { get; set; }
    public string OrderBy { get; set; }
    public string FilterBy { get; set; }
}
```

The next step is to create a MongoDB entity for our Search Service. Notice, we do not need an Id property. We drive this Animal class with MongoDB Entity and that will provide ids for our animal.

### Data/Animal.cs

```cs

public class Animal : Entity
{
    public int PublicId { get; set; }
    public int Age { get; set; }
    public string Name { get; set; }
    public string Type { get; set; }
    public string Breed { get; set; }
    public string Sex { get; set; }
    public int Weight { get; set; }
    public string Color { get; set; }
    public string Description { get; set; }
    public string CoverImageUrl { get; set; }
    public string Status { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
```

### Services/AnimalServiceHttpClient.cs

This service we will have http client, so our service can call our Animal Service.

```cs
public class AnimalServiceHttpClient
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _config;

    public AnimalServiceHttpClient(HttpClient httpClient, IConfiguration config)
    {
        _httpClient = httpClient;
        _config = config;
    }

    public async Task<List<Animal>> GetAnimalsForSearchDb()
    {
        return await _httpClient.GetFromJsonAsync<List<Animal>>(_config["AnimalServiceUrl"]
            + "/api/animals");
    }
}
```

### Data/DbInitializer.cs

Now we have to create DB initializer that will create a collection of Animal class named `SearchDb` and will synchronously receive data from Animal Service and store it in the the database.

```cs
public class DbInitializer
{


    public static async Task InitDb(WebApplication app)
    {
        await DB.InitAsync("SearchDb", MongoClientSettings
                    .FromConnectionString(app.Configuration.GetConnectionString("MongoDbConnection")));

        await DB.Index<Animal>()
            .Key(x => x.Type, KeyType.Text)
            .Key(x => x.Breed, KeyType.Text)
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
```

### Program.cs

Also, we have to include our RabbitMQ, Http service, Mapper for this service as well so our Search Service can work as we planned.

```cs
// Mapper
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
// Http service
builder.Services.AddHttpClient<AnimalServiceHttpClient>().AddPolicyHandler(GetPolicy());
// RabbitMQ
builder.Services.AddMassTransit(x =>
{
    x.AddConsumersFromNamespaceContaining<AnimalCreatedConsumer>();

    x.SetEndpointNameFormatter(new KebabCaseEndpointNameFormatter("search", false));

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(builder.Configuration["RabbitMq:Host"], "/", host =>
        {
            host.Username(builder.Configuration.GetValue("RabbitMq:Username", "guest"));
            host.Password(builder.Configuration.GetValue("RabbitMq:Password", "guest"));
        });

        cfg.ConfigureEndpoints(context);
    });
});
var app = builder.Build();
// Configure DB connection
app.Lifetime.ApplicationStarted.Register(async () =>
{
    try
    {
        await DbInitializer.InitDb(app);
    }
    catch (Exception e)
    {
        Console.WriteLine(e);
    }
});

...
// Add Async Policy
static IAsyncPolicy<HttpResponseMessage> GetPolicy()
    => HttpPolicyExtensions
        .HandleTransientHttpError()
        .OrResult(msg => msg.StatusCode == HttpStatusCode.NotFound)
        .WaitAndRetryForeverAsync(_ => TimeSpan.FromSeconds(3));
```

## Consumers

Consumers will play a crucial role in our service. Our consumers will consume data from RabbitMQ, read the payload and mutate the data in our MongoDB.

### Consumers/AnimalCreatedConsumer.cs

```cs
public class AnimalCreatedConsumer : IConsumer<AnimalCreated>
{
    private readonly IMapper _mapper;

    public AnimalCreatedConsumer(IMapper mapper)
    {
        _mapper = mapper;
    }
    public async Task Consume(ConsumeContext<AnimalCreated> animalCreated)
    {
        Console.WriteLine("Consuming animal created " + animalCreated.Message.Id);

        var animal = _mapper.Map<Animal>(animalCreated.Message);

        await animal.SaveAsync();
    }
}
```

### Consumers/AnimalCreatedConsumer.cs

```cs
public class AnimalCreatedConsumer : IConsumer<AnimalCreated>
{
    private readonly IMapper _mapper;

    public AnimalCreatedConsumer(IMapper mapper)
    {
        _mapper = mapper;
    }
    public async Task Consume(ConsumeContext<AnimalCreated> animalCreated)
    {
        Console.WriteLine("Consuming animal created " + animalCreated.Message.Id);

        var animal = _mapper.Map<Animal>(animalCreated.Message);

        await animal.SaveAsync();
    }
}
```

### Consumers/AnimalDeletedConsumer.cs

```cs
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
```

### Consumers/AnimalUpdatedConsumer.cs

```cs
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
```

## Mapper

Similarly to our Animal Service, we will create a mapper class and will include event classes we will receive from RabbitMQ.

### Helpers/ProfileMapper.cs

```cs
public class ProfileMapper : Profile
{
    public ProfileMapper()
    {

        CreateMap<AnimalCreated, Animal>();
        CreateMap<AnimalUpdated, Animal>();
    }
}
```

## Controller

Finally, we will create a controller that will provide and endpoint `http://localhost:7002/api/search` that will allows to query our data. Our endpoint will allows to sort, filter, and paginate through the data.

### Controllers/SearchController.cs

```cs
[ApiController]
[Route("api/search")]
public class SearchController : ControllerBase
{

    [HttpGet]
    public async Task<ActionResult<List<Animal>>> SearchAnimals([FromQuery] SearchParams searchParams)
    {
        var query = DB.PagedSearch<Animal, Animal>();

        if (!string.IsNullOrEmpty(searchParams.SearchTerm))
        {
            query.Match(Search.Full, searchParams.SearchTerm).SortByTextScore();
        }

        // Sort by parameters
        query = searchParams.OrderBy switch
        {
            "age" => query.Sort(x => x.Ascending(y => y.Age)),
            "weight" => query.Sort(x => x.Ascending(y => y.Weight)),
            _ => query.Sort(x => x.Ascending(y => y.CreatedAt)),
        };

        // Filter by parameters
        query = searchParams.FilterBy switch
        {
            "found" => query.Match(x => x.Status == "Found"),
            "pending" => query.Match(x => x.Status == "Pending"),
            "available" => query.Match(x => x.Status == "Available"),
            "missing" => query.Match(x => x.Status == "Missing"),
            _ => query.Sort(x => x.Ascending(y => y.CreatedAt)),
        };

        if (!string.IsNullOrEmpty(searchParams.Type))
        {
            query.Match(x => x.Type == searchParams.Type);
        }

        if (!string.IsNullOrEmpty(searchParams.Sex))
        {
            query.Match(x => x.Sex == searchParams.Sex);
        }


        query.PageNumber(searchParams.PageNumber);
        query.PageSize(searchParams.PageSize);

        var result = await query.ExecuteAsync();

        return Ok(new
        {
            results = result.Results,
            pageCount = result.PageCount,
            totalCount = result.TotalCount
        });
    }
}
```

## Final thoughts

In this blog post, we've introduced the concept of microservices architecture and illustrated the process of constructing a basic application using .Net and RabbitMQ. We developed two microservices: one to mutate animal data and a second one for searching through the animal data.

Subsequently, we demonstrated how these microservices can be interconnected to create a unified application capable of providing information about animals that are missing or are available to adopt.

Although this example is straightforward, the underlying principles can be applied to far more intricate systems, offering a simplified approach to developing, deploying, and scaling large applications.

In the next blog post we will look into how to build other services on top of this project.
