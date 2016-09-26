using System.Collections.ObjectModel;
using System.Windows;

namespace Horizon.Framework.Xaml.Interaction
{
    public class BehaviorCollection : Collection<Behavior>, IAttachable
    {
        public FrameworkElement AssociatedObject { get; private set; }

        public void Attach(FrameworkElement frameworkElement)
        {
            AssociatedObject = frameworkElement;

            foreach (var element in this)
            {
                element.Attach(frameworkElement);
            }
        }

        public void Detach()
        {
            foreach (var element in this)
            {
                element.Detach();
            }
            AssociatedObject = null;
        }

        protected override void ClearItems()
        {
            foreach (var element in this)
            {
                element.Detach();
            }

            base.ClearItems();
        }

        protected override void InsertItem(int index, Behavior item)
        {
            base.InsertItem(index, item);

            if (AssociatedObject != null)
            {
                item.Attach(AssociatedObject);
            }
        }

        protected override void RemoveItem(int index)
        {
            if (index >= 0 && index < Count)
            {
                this[index].Detach();
            }
            base.RemoveItem(index);
        }

        protected override void SetItem(int index, Behavior item)
        {
            if (index >= 0 && index < Count)
            {
                this[index].Detach();
            }

            base.SetItem(index, item);

            if (AssociatedObject != null)
            {
                item.Attach(AssociatedObject);
            }
        }
    }
}