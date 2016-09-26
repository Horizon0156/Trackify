using JetBrains.Annotations;
using System.Windows;

namespace Horizon.Framework.Xaml.Interaction
{
    public interface IAttachable
    {
        [CanBeNull]
        FrameworkElement AssociatedObject { get; }

        void Attach([NotNull] FrameworkElement frameWorkElement);

        void Detach();
    }
}