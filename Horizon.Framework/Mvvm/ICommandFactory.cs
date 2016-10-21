using JetBrains.Annotations;
using System;
using System.Windows.Input;

namespace Horizon.Framework.Mvvm
{
    /// <summary>
    /// Defines a command factory.
    /// </summary>
    public interface ICommandFactory
    {
        /// <summary>
        /// Creates a new command which executes the given action.
        /// </summary>
        /// <param name="execute"> Action which will be executed by this command. </param>
        /// <returns> The command. </returns>
        /// <exception cref="ArgumentNullException">If the execution action is not set. </exception>
        ICommand CreateCommand([NotNull] Action execute);

        /// <summary>
        /// Creates a new command which executes the given action.
        /// </summary>
        /// <param name="execute"> Action which will be executed by this command. </param>
        /// <param name="canExecute"> Delegate which determines wheather the command can be executed. </param>
        /// <returns> The command. </returns>
        /// <exception cref="ArgumentNullException">If the execution action is not set. </exception>
        ICommand CreateCommand([NotNull] Action execute, [CanBeNull] Func<bool> canExecute);

        /// <summary>
        /// Creates a new command which executes the given action.
        /// </summary>
        /// <param name="execute"> Action which will be executed by this command. </param>
        /// <typeparam name="T"> The type of the parameter passsed to the command. </typeparam>
        /// <returns> The command. </returns>
        /// <exception cref="ArgumentNullException">If the execution action is not set. </exception>
        ICommand CreateCommand<T>([NotNull] Action<T> execute);

        /// <summary>
        /// Creates a new command which executes the given action.
        /// </summary>
        /// <param name="execute"> Action which will be executed by this command. </param>
        /// <param name="canExecute"> Delegate which determines wheather the command can be executed. </param>
        /// <typeparam name="T"> The type of the parameter passsed to the command. </typeparam>
        /// <returns> The command. </returns>
        /// <exception cref="ArgumentNullException">If the execution action is not set. </exception>
        ICommand CreateCommand<T>([NotNull] Action<T> execute, [CanBeNull] Func<T, bool> canExecute);
    }
}