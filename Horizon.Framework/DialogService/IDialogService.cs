using Horizon.Framework.Mvvm;
using System.Threading.Tasks;

namespace Horizon.Framework.DialogService
{
    /// <summary>
    /// Interface for a Dialog service to show dialogs from a ViewModel.
    /// </summary>
    public interface IDialogService
    {
        /// <summary>
        /// Shows the dialog.
        /// </summary>
        /// <param name="viewModel"> The view model of the dialog which has to be shown. </param>
        void ShowDialog(ViewModel viewModel);

        /// <summary>
        /// Shows the dialog and returns a task waiting for its completion.
        /// </summary>
        /// <param name="viewModel"> The view model of the dialog which has to be shown. </param>
        /// <returns> Task indicating the dialogs lifetime. </returns>
        Task ShowModalDialog(ViewModel viewModel);
    }
}
