using Microsoft.EntityFrameworkCore;
using System;
using System.IO;
using System.Reflection;

namespace Trackify.DataModels
{
    internal class DatabaseContext : DbContext, IDatabaseContext
    {
        public DbSet<TimeAcquisition> TimeAcquisitions { get; set; }

        public string StorageLocation { get; private set; }

        public void EnsureDatabaseIsCreated()
        {
            Database.EnsureCreated();
        }

        void IDatabaseContext.SaveChanges()
        {
            SaveChanges();
        }

        public void ClearTimeAcquisitions()
        {
            TimeAcquisitions.RemoveRange(TimeAcquisitions);
            SaveChanges();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            StorageLocation = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), 
                Assembly.GetExecutingAssembly().GetName().Name, 
                "Data");

            Directory.CreateDirectory(StorageLocation);

            var databasePath = Path.Combine(StorageLocation, "TimeAcquisitions.db");
            optionsBuilder.UseSqlite($"Filename={databasePath}");
        }
    }
}