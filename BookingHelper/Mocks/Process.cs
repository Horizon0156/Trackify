namespace BookingHelper.Mocks
{
    internal sealed class Process : IProcess
    {
        public void Start(string processPath)
        {
            System.Diagnostics.Process.Start(processPath);
        }
    }
}