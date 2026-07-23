using Microsoft.EntityFrameworkCore;

namespace EventEase.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Venue> Venue { get; set; }
        public DbSet<Event> Event { get; set; }
        public DbSet<Booking> Booking { get; set; }
        public DbSet<EventType> EventType { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<EventType>().HasData(
                new EventType { EventTypeId = 1, Name = "Conference" },
                new EventType { EventTypeId = 2, Name = "Wedding" },
                new EventType { EventTypeId = 3, Name = "Naming" },
                new EventType { EventTypeId = 4, Name = "Birthday" },
                new EventType { EventTypeId = 5, Name = "Concert" }
            );
        }
    }
}