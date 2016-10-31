using System;
using System.Threading.Tasks;
using BookingHelper.Deployment;

namespace BookingHelper
{
    internal class DummyUpdateChecker : IUpdateChecker
    {
        public Task<bool> IsUpdateAvailable()
        {
            return Task.FromResult(false);
        }

        public Uri ApplicationProductPage => new Uri("https://horizon777.bitbucket.io/Software/BookingHelper/BookingHelper.html");
    }
}