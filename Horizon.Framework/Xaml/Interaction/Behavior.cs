using System;
using System.Windows;

namespace Horizon.Framework.Xaml.Interaction
{
    public abstract class Behavior : IAttachable
    {
        public FrameworkElement AssociatedObject { get; private set; }

        public void Attach(FrameworkElement frameworkElement)
        {
            if (AssociatedObject != null)
            {
                throw new InvalidOperationException("Behavior has already been attached to an object");
            }

            AssociatedObject = frameworkElement;
            OnAttached();
        }

        protected virtual void OnAttached()
        {
        }

        public virtual void Detach()
        {
            OnDetaching();
            AssociatedObject = null;
        }

        protected virtual void OnDetaching()
        { 
        }
    }
}