namespace BookingHelper.Mocks
{
    internal sealed class Process : IProcess
    {
        public void Start(string processPath, string arguments)
        {
            System.Diagnostics.Process.Start(processPath, arguments);
        }
    }
}