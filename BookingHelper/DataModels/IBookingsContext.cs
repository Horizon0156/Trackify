using BookingHelper.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace BookingHelper.DataModels
{
    internal interface IBookingsContext
    {
        DbSet<Booking> Bookings { get; set; }

        void EnsureDatabaseIsCreated();

        int SaveChanges();
    }
}