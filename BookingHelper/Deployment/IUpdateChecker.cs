using System;
using System.Threading.Tasks;

namespace BookingHelper.Deployment
{
    internal interface IUpdateChecker
    {
        Task<bool> IsUpdateAvailable();

        Uri ApplicationProductPage { get; }
    }
}