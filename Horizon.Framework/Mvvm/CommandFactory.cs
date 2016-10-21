using Horizon.Framework.Exceptions;
using System;
using System.Windows.Input;

namespace Horizon.Framework.Mvvm
{
    /// <summary>
    /// Represents a command factory.
    /// </summary>
    public class CommandFactory : ICommandFactory
    {
        /// <inheritdoc/>
        public ICommand CreateCommand(Action execute)
        {
            Throw.IfArgumentIsNull(execute, nameof(execute));

            return new Command(execute);
        }

        /// <inheritdoc/>
        public ICommand CreateCommand(Action execute, Func<bool> canExecute)
        {
            Throw.IfArgumentIsNull(execute, nameof(execute));

            return new Command(execute, canExecute);
        }

        /// <inheritdoc/>
        public ICommand CreateCommand<T>(Action<T> execute)
        {
            Throw.IfArgumentIsNull(execute, nameof(execute));

            return new Command<T>(execute);
        }

        /// <inheritdoc/>
        public ICommand CreateCommand<T>(Action<T> execute, Func<T, bool> canExecute)
        {
            Throw.IfArgumentIsNull(execute, nameof(execute));

            return new Command<T>(execute, canExecute);
        }
    }
}