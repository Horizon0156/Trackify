using Microsoft.EntityFrameworkCore;

namespace BookingHelper.DataModels
{
    internal class BookingsContext : DbContext, IBookingsContext
    {
        public DbSet<Booking> Bookings { get; set; }

        public void EnsureDatabaseIsCreated()
        {
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Filename=Bookings.db");
        }
    }
}