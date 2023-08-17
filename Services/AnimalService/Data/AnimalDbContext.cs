using AnimalService.Entities;
using Microsoft.EntityFrameworkCore;
using MassTransit;

namespace AnimalService.Data
{
    public class AnimalDbContext : DbContext
    {
        public AnimalDbContext()
        {
        }

        public AnimalDbContext(DbContextOptions options) : base(options) { }

        public DbSet<Animal> Animals { get; set; }
        public DbSet<Image> Images { get; set; }
        public DbSet<Address> Address { get; set; }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // add in memory outbox https://masstransit.io/documentation/patterns/in-memory-outbox
            builder.AddInboxStateEntity();
            builder.AddOutboxMessageEntity();
            builder.AddOutboxStateEntity();

        }

    }
}