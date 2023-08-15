using AnimalService.Entities;
using Microsoft.EntityFrameworkCore;
using MassTransit;

namespace AnimalService.Data
{
    public class AnimalDbContext : DbContext
    {
        public AnimalDbContext(DbContextOptions options) : base(options) { }

        public DbSet<Animal> Animals { get; set; }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // add in memory outbox https://masstransit.io/documentation/patterns/in-memory-outbox
            builder.AddInboxStateEntity();
            builder.AddOutboxMessageEntity();
            builder.AddOutboxStateEntity();

            // builder.HasSequence<int>("PublicId_seq")
            //       .StartsAt(1000)
            //       .IncrementsBy(1);

            // builder.Entity<Animal>()
            //             .Property(o => o.PublicId)
            //             .HasDefaultValueSql("nextval('\"PublicId_seq\"')");

        }

    }
}