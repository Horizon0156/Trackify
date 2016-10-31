using System;
using System.IO;
using System.Net.Mime;
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
            var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "BookingHelper", "Data");
            Directory.CreateDirectory(path);

            var databasePath = Path.Combine(path, "Bookings.db");
            optionsBuilder.UseSqlite($"Filename={databasePath}");
        }
    }
}