using System;
using Horizon.MvvmFramework.Exceptions;
using JetBrains.Annotations;

namespace Horizon.MvvmFramework.Commands
{
    internal class Command : CommandBase
    {
        [CanBeNull]
        private readonly Func<bool> _canExecute;

        [NotNull]
        private readonly Action _execute;

        public Command([NotNull] Action execute, [CanBeNull] Func<bool> canExecute = null)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        public override bool CanExecute(object parameter)
        {
            return _canExecute?.Invoke() ?? true;
        }

        public override void Execute(object parameter)
        {
            Throw.IfOperationIsInvalid(isOperationInvalid: !CanExecute(parameter), message: "The command can not execute");

            _execute.Invoke();
        }
    }
}