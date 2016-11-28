using Microsoft.EntityFrameworkCore;
using System;
using System.IO;

namespace BookingHelper.DataModels
{
    internal class BookingsContext : DbContext, IBookingsContext
    {
        public DbSet<Booking> Bookings { get; set; }

        public string StorageLocation { get; private set; }

        public void EnsureDatabaseIsCreated()
        {
            Database.EnsureCreated();
        }

        void IBookingsContext.SaveChanges()
        {
            SaveChanges();
        }

        public void ResetBookings()
        {
            Bookings.RemoveRange(Bookings);
            SaveChanges();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            StorageLocation = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "BookingHelper", "Data");
            Directory.CreateDirectory(StorageLocation);

            var databasePath = Path.Combine(StorageLocation, "Bookings.db");
            optionsBuilder.UseSqlite($"Filename={databasePath}");
        }
    }
}