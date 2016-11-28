using System.Threading.Tasks;

namespace Horizon.MvvmFramework.Components
{
    /// <summary>
    /// Interface for an initializeable component.
    /// </summary>
    public interface IInitializeable
    {
        /// <summary>
        /// Initializes the component asynchronously.
        /// </summary>
        /// <returns> The operational Task. </returns>
        Task InitializeAsync();
    }
}