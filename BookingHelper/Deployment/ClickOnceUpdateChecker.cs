using System;
using System.Deployment.Application;
using System.Threading.Tasks;

namespace BookingHelper.Deployment
{
    internal class ClickOnceUpdateChecker : IUpdateChecker
    {
        private TaskCompletionSource<bool> _updateOperationTask;

        public Uri ApplicationProductPage => ApplicationDeployment.IsNetworkDeployed ?
            ApplicationDeployment.CurrentDeployment.UpdateLocation
            : null;

        public Task<bool> IsUpdateAvailable()
        {
            if (!ApplicationDeployment.IsNetworkDeployed)
            {
                return Task.FromResult(false);
            }

            if (_updateOperationTask != null)
            {
                _updateOperationTask.SetCanceled();
                ApplicationDeployment.CurrentDeployment.CheckForUpdateCompleted -= UpdateOperationSource;
                ApplicationDeployment.CurrentDeployment.CheckForUpdateAsyncCancel();
            }

            _updateOperationTask = new TaskCompletionSource<bool>();
            ApplicationDeployment.CurrentDeployment.CheckForUpdateCompleted += UpdateOperationSource;
            ApplicationDeployment.CurrentDeployment.CheckForUpdateAsync();

            return _updateOperationTask.Task;
        }

        private void UpdateOperationSource(object sender, CheckForUpdateCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                _updateOperationTask.SetCanceled();
            }
            else if (e.Error != null)
            {
                _updateOperationTask.SetException(e.Error);
            }
            else
            {
                _updateOperationTask.SetResult(e.UpdateAvailable);
            }
        }
    }
}