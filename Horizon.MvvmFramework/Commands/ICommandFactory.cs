using System;
using System.Threading.Tasks;
using System.Windows.Input;
using JetBrains.Annotations;

namespace Horizon.MvvmFramework.Commands
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
        /// <param name="canExecute"> Delegate which determines wheather the command can be executed. </param>
        /// <returns> The command. </returns>
        /// <exception cref="ArgumentNullException">If the execution action is not set. </exception>
        ICommand CreateCommand([NotNull] Action execute, [CanBeNull] Func<bool> canExecute = null);

        /// <summary>
        /// Creates a new command which executes the given action.
        /// </summary>
        /// <param name="execute"> Action which will be executed by this command. </param>
        /// <param name="canExecute"> Delegate which determines wheather the command can be executed. </param>
        /// <typeparam name="T"> The type of the parameter passsed to the command. </typeparam>
        /// <returns> The command. </returns>
        /// <exception cref="ArgumentNullException">If the execution action is not set. </exception>
        ICommand CreateCommand<T>([NotNull] Action<T> execute, [CanBeNull] Func<T, bool> canExecute = null);

        /// <summary>
        /// Creates an asynchronous command which executes the given action.
        /// </summary>
        /// <param name="executeAsync"> Asynchronous action which will be executed by this command. </param>
        /// <param name="canExecute"> Delegate which determines wheather the command can be executed. </param>
        /// <returns> The command. </returns>
        /// <exception cref="ArgumentNullException">If the execution action is not set. </exception>
        ICommand CreateAyncCommand([NotNull] Func<Task> executeAsync, [CanBeNull] Func<bool> canExecute = null);

        /// <summary>
        /// Creates an asynchronous command which executes the given action.
        /// </summary>
        /// <param name="executeAsync"> Asynchronous action which will be executed by this command. </param>
        /// <param name="canExecute"> Delegate which determines wheather the command can be executed. </param>
        /// <typeparam name="T"> The type of the parameter passsed to the command. </typeparam>
        /// <returns> The command. </returns>
        /// <exception cref="ArgumentNullException">If the execution action is not set. </exception>
        ICommand CreateAyncCommand<T>([NotNull] Func<T, Task> executeAsync, [CanBeNull] Func<T, bool> canExecute = null);
    }
}