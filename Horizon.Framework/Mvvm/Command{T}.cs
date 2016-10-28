using Horizon.Framework.Exceptions;
using JetBrains.Annotations;
using System;

namespace Horizon.Framework.Mvvm
{
    internal class Command<T> : CommandBase
    {
        [CanBeNull]
        private readonly Func<T, bool> _canExecute;

        [NotNull]
        private readonly Action<T> _execute;

        public Command([NotNull] Action<T> execute, [CanBeNull] Func<T, bool> canExecute = null)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        public override bool CanExecute(object parameter)
        {
            var isParameterValid = parameter is T;

            return isParameterValid
                   && (_canExecute?.Invoke((T)parameter) ?? true);
        }

        public override void Execute(object parameter)
        {
            Throw.IfOperationIsInvalid(isOperationInvalid: !CanExecute(parameter), message: "The command can not execute");

            _execute.Invoke((T)parameter);
        }
    }
}