using JetBrains.Annotations;
using System;
using System.Windows.Input;

namespace Horizon.Framework.Mvvm
{
    internal abstract class CommandBase : ICommand
    {
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

        public virtual bool CanExecute([CanBeNull] object parameter)
        {
            return true;
        }

        public virtual void Execute([CanBeNull] object parameter)
        {
        }
    }
}