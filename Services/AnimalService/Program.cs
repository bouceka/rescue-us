using AnimalService.Consumers;
using AnimalService.Data;
using MassTransit;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddDbContext<AnimalDbContext>(option =>
{
    option.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

// Configure RabbitMq
builder.Services.AddMassTransit(x =>
{
    // add in memory outbox
    x.AddEntityFrameworkOutbox<AnimalDbContext>(o =>
    {
        o.QueryDelay = TimeSpan.FromSeconds(10);

        o.UseSqlServer();
        o.UseBusOutbox();
    });

    x.SetEndpointNameFormatter(new KebabCaseEndpointNameFormatter("animal", false));

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


app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

