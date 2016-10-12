using Horizon.Framework.Exceptions;
using JetBrains.Annotations;
using System;
using System.Windows.Input;

namespace Horizon.Framework.Mvvm
{
    internal class Command<T> : ICommand
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

        public event EventHandler CanExecuteChanged
        {
            add
            {
                CommandManager.RequerySuggested += value;
            }
            remove
            {
                CommandManager.RequerySuggested -= value;
            }
        }

        bool ICommand.CanExecute(object parameter)
        {
            return parameter is T
                && CanExecute((T)parameter);
        }

        void ICommand.Execute(object parameter)
        {
            Execute((T)parameter);
        }

        private bool CanExecute(T parameter)
        {
            return _canExecute != null
                ? _canExecute.Invoke(parameter)
                : true;
        }

        private void Execute(T parameter)
        {
            Throw.IfOperationIsInvalid(isOperationInvalid: !CanExecute(parameter), message: "The command can not execute");

            _execute.Invoke(parameter);
        }
    }
}