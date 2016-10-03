using System.ComponentModel;

namespace Horizon.Framework.Xaml.Collections
{
    /// <summary>
    /// Event args for the InnerElementChanged event of the <see cref="AttentiveCollection{T}"/>.
    /// </summary>
    public class NotifyInnerElementChangedEventArgs : PropertyChangedEventArgs
    {
        /// <summary>
        /// Creates a new instance of the <see cref="NotifyCollectionItemChangedEventArgs"/> class.
        /// </summary>
        /// <param name="changedItem"> Reference to the changed item. </param>
        /// <param name="propertyName"> Name of the property of the changed item, which changed its value. </param>
        public NotifyInnerElementChangedEventArgs(object changedItem, string propertyName) : base(propertyName)
        {
            ChangedItem = changedItem;
        }

        public object ChangedItem { get; }
    }
}