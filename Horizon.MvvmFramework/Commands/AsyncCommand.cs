using System;
using System.Threading.Tasks;
using Horizon.MvvmFramework.Exceptions;
using JetBrains.Annotations;

namespace Horizon.MvvmFramework.Commands
{
    internal class AsyncCommand : CommandBase
    {
        [CanBeNull]
        private readonly Func<bool> _canExecute;

        [NotNull]
        private readonly Func<Task> _executeAsync;

        public AsyncCommand([NotNull] Func<Task> executeAsync, [CanBeNull] Func<bool> canExecute = null)
        {
            _executeAsync = executeAsync;
            _canExecute = canExecute;
        }

        public override bool CanExecute(object parameter)
        {
            return _canExecute?.Invoke() ?? true;
        }

        public override async void Execute(object parameter)
        {
            Throw.IfOperationIsInvalid(isOperationInvalid: !CanExecute(parameter), message: "The command can not executeAsync");

            await _executeAsync
                .Invoke()
                .ConfigureAwait(false);
        }
    }
}