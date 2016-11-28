using System;
using System.Windows.Input;
using JetBrains.Annotations;

namespace Horizon.MvvmFramework.Commands
{
    internal abstract class CommandBase : ICommand
    {
        public event EventHandler CanExecuteChanged;

        public virtual bool CanExecute([CanBeNull] object parameter)
        {
            return true;
        }

        public virtual void Execute([CanBeNull] object parameter)
        {
        }

        public void NotifyChange()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}