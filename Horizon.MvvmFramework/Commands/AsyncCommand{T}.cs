using System;
using System.Threading.Tasks;
using Horizon.MvvmFramework.Exceptions;
using JetBrains.Annotations;

namespace Horizon.MvvmFramework.Commands
{
    internal class AsyncCommand<T> : CommandBase
    {
        [CanBeNull]
        private readonly Func<T, bool> _canExecute;

        [NotNull]
        private readonly Func<T, Task> _executeAsync;

        public AsyncCommand([NotNull] Func<T, Task> executeAsync, [CanBeNull] Func<T, bool> canExecute = null)
        {
            _executeAsync = executeAsync;
            _canExecute = canExecute;
        }

        public override bool CanExecute(object parameter)
        {
            var isParameterValid = parameter is T;

            return isParameterValid
                   && (_canExecute?.Invoke((T)parameter) ?? true);
        }

        public override async void Execute(object parameter)
        {
            Throw.IfOperationIsInvalid(isOperationInvalid: !CanExecute(parameter), message: "The command can not executeAsync");

            await _executeAsync
                .Invoke((T)parameter)
                .ConfigureAwait(false);
        }
    }
}