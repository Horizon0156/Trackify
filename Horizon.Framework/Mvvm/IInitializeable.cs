using System.Threading.Tasks;

namespace Horizon.Framework.Mvvm
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