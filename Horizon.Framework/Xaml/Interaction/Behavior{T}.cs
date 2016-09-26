using System;
using System.Windows;

namespace Horizon.Framework.Xaml.Interaction
{
    public abstract class Behavior<T> : Behavior where T : FrameworkElement
    {
        private T _associatedObject;

        public new T AssociatedObject
        {
            get
            {
                return (T) base.AssociatedObject;
            }
        }

        protected override void OnAttached()
        {
            base.OnAttached();

            if (!(base.AssociatedObject is T))
            {
                throw new InvalidOperationException(string.Format("This behaviour can only be attached to an object of type {0}", typeof(T).Name));
            }
        }
    }
}