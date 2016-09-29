using JetBrains.Annotations;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace Horizon.Framework.Mvvm
{
    public class ViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected ICommand CreateCommand([NotNull] Action execute)
        {
            return new Command(execute);
        }

        protected ICommand CreateCommand([NotNull] Action execute, [CanBeNull] Func<bool> canExecute)
        {
            return new Command(execute, canExecute);
        }

        protected ICommand CreateCommand<T>([NotNull] Action<T> execute)
        {
            return new Command<T>(execute);
        }

        protected ICommand CreateCommand<T>([NotNull] Action<T> execute, [CanBeNull] Func<T, bool> canExecute)
        {
            return new Command<T>(execute, canExecute);
        }

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected void SetProperty<T>(ref T property, T value, [CallerMemberName] string propertyName = null)
        {
            if (!AreEqual(value, property))
            {
                property = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private bool AreEqual(object value1, object value2)
        {
            return value1.Equals(value2);
        }
    }
}