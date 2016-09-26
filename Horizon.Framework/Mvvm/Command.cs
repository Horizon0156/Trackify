using JetBrains.Annotations;
using System;
using System.Windows.Input;

namespace Horizon.Framework.Mvvm
{
    internal class Command : ICommand
    {
        [CanBeNull]
        private readonly Func<bool> _canExecute;

        [NotNull]
        private readonly Action _execute;

        public Command([NotNull] Action execute, [CanBeNull] Func<bool> canExecute = null)
        {
            if (execute == null)
            {
                throw new ArgumentNullException("execute");
            }

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

        public bool CanExecute()
        {
            return _canExecute != null
                ? _canExecute.Invoke()
                : true;
        }

        public void Execute()
        {
            _execute.Invoke();
        }

        bool ICommand.CanExecute(object parameter)
        {
            return CanExecute();
        }

        void ICommand.Execute(object parameter)
        {
            Execute();
        }
    }
}