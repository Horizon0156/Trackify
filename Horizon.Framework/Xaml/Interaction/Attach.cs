using JetBrains.Annotations;
using System;
using System.Windows;

namespace Horizon.Framework.Xaml.Interaction
{
    public sealed class Attach
    {
        public static DependencyProperty BehaviorsProperty = DependencyProperty.RegisterAttached(
            "BehaviorsInternal",
            typeof(BehaviorCollection),
            typeof(Attach),
            new PropertyMetadata(null, AttachOrDetachAssociatedObject));

        [NotNull]
        public static BehaviorCollection GetBehaviors([NotNull] DependencyObject frameworkElement)
        {
            var behaviorCollection = frameworkElement.GetValue(BehaviorsProperty) as BehaviorCollection;

            if (behaviorCollection == null)
            {
                behaviorCollection = new BehaviorCollection();
                frameworkElement.SetValue(BehaviorsProperty, behaviorCollection);
            }

            return behaviorCollection;
        }

        public static void SetBehaviors(DependencyObject obj, BehaviorCollection value)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj));
            }
            obj.SetValue(BehaviorsProperty, value);
        }

        private static void AttachOrDetachAssociatedObject(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var oldCollection = e.OldValue as BehaviorCollection;
            var newCollection = e.NewValue as BehaviorCollection;

            var frameworkElement = d as FrameworkElement;

            if (frameworkElement == null)
            {
                throw new InvalidOperationException("Attachment of an interaction is for framework elements only.");
            }

            oldCollection?.Detach();
            newCollection?.Attach(frameworkElement);
        }
    }
}