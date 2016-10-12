using Horizon.Framework.Exceptions;
using JetBrains.Annotations;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace Horizon.Framework.Mvvm
{
    /// <summary>
    /// This class provides the base functionality for a ViewModel.
    /// </summary>
    public abstract class ViewModel : INotifyPropertyChanged
    {
        /// <inheritdoc/>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Creates a new command which executes the given action.
        /// </summary>
        /// <param name="execute"> Action which will be executed by this command. </param>
        /// <returns> The command. </returns>
        /// <exception cref="ArgumentNullException">If the execution action is not set. </exception>
        protected ICommand CreateCommand([NotNull] Action execute)
        {
            Throw.IfArgumentIsNull(execute, nameof(execute));

            return new Command(execute);
        }

        /// <summary>
        /// Creates a new command which executes the given action.
        /// </summary>
        /// <param name="execute"> Action which will be executed by this command. </param>
        /// <param name="canExecute"> Delegate which determines wheather the command can be executed. </param>
        /// <returns> The command. </returns>
        /// <exception cref="ArgumentNullException">If the execution action is not set. </exception>
        protected ICommand CreateCommand([NotNull] Action execute, [CanBeNull] Func<bool> canExecute)
        {
            Throw.IfArgumentIsNull(execute, nameof(execute));

            return new Command(execute, canExecute);
        }

        /// <summary>
        /// Creates a new command which executes the given action.
        /// </summary>
        /// <param name="execute"> Action which will be executed by this command. </param>
        /// <typeparam name="T"> The type of the parameter passsed to the command. </typeparam>
        /// <returns> The command. </returns>
        /// <exception cref="ArgumentNullException">If the execution action is not set. </exception>
        protected ICommand CreateCommand<T>([NotNull] Action<T> execute)
        {
            Throw.IfArgumentIsNull(execute, nameof(execute));

            return new Command<T>(execute);
        }

        /// <summary>
        /// Creates a new command which executes the given action.
        /// </summary>
        /// <param name="execute"> Action which will be executed by this command. </param>
        /// <param name="canExecute"> Delegate which determines wheather the command can be executed. </param>
        /// <typeparam name="T"> The type of the parameter passsed to the command. </typeparam>
        /// <returns> The command. </returns>
        /// <exception cref="ArgumentNullException">If the execution action is not set. </exception>
        protected ICommand CreateCommand<T>([NotNull] Action<T> execute, [CanBeNull] Func<T, bool> canExecute)
        {
            Throw.IfArgumentIsNull(execute, nameof(execute));

            return new Command<T>(execute, canExecute);
        }

        /// <summary>
        /// Notifies a property change.
        /// </summary>
        /// <param name="propertyName"> The name of the changed property (CallerMemberName) </param>
        protected void OnPropertyChanged([CallerMemberName][CanBeNull] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Sets the given property to the provided value.
        /// If the property has changed, the change will be notified.
        /// </summary>
        /// <typeparam name="T"> The type of the property. </typeparam>
        /// <param name="property"> A reference to the property. </param>
        /// <param name="value"> The value which will be assigned to the property. </param>
        /// <param name="propertyName"> The name of the property (CallerMemberName) </param>
        protected void SetProperty<T>([NotNull] ref T property, [CanBeNull] T value, [CallerMemberName][CanBeNull] string propertyName = null)
        {
            if (!AreEqual(value, property))
            {
                property = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private bool AreEqual([CanBeNull] object value1, [CanBeNull] object value2)
        {
            return value1.Equals(value2);
        }
    }
}