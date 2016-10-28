using JetBrains.Annotations;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Horizon.Framework.Mvvm
{
    /// <summary>
    /// Represents an observeable object which fires change notifications.
    /// </summary>
    public abstract class ObserveableObject : INotifyPropertyChanged
    {
        /// <inheritdoc/>
        public event PropertyChangedEventHandler PropertyChanged;

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
        protected void SetProperty<T>([CanBeNull] ref T property, [CanBeNull] T value, [CallerMemberName][CanBeNull] string propertyName = null)
        {
            if (!AreEqual(value, property))
            {
                property = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private static bool AreEqual([CanBeNull] object value1, [CanBeNull] object value2)
        {
            return value1?.Equals(value2) ?? value2 == null;
        }
    }
}