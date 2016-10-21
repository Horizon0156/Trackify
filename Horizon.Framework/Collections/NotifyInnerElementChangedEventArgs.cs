using Horizon.Framework.Exceptions;
using JetBrains.Annotations;
using System.ComponentModel;

namespace Horizon.Framework.Collections
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
        /// <exception cref="ArgumentNullException"> If any of the provided arguments is null. </exception>
        public NotifyInnerElementChangedEventArgs([NotNull] object changedItem, [NotNull] string propertyName) : base(propertyName)
        {
            Throw.IfArgumentIsNull(changedItem, nameof(changedItem));
            Throw.IfArgumentIsNull(propertyName, nameof(propertyName));

            ChangedItem = changedItem;
        }

        public object ChangedItem { get; }
    }
}