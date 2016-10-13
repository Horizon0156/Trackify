using Horizon.Framework.Exceptions;
using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;

namespace Horizon.Framework.Xaml.Collections
{
    /// <summary>
    /// An observeable collection which also fires an update if an inner element changes.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <seealso cref="System.Collections.ObjectModel.ObservableCollection{T}" />
    public sealed class AttentiveCollection<T> : ObservableCollection<T> where T : INotifyPropertyChanged
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AttentiveCollection{T}"/> class.
        /// </summary>
        public AttentiveCollection()
        {
            FireCollectionChangeWhenInnerElementChanges = false;
            AttachElementWatcher();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AttentiveCollection{T}"/> class.
        /// </summary>
        /// <param name="list">The list from which the elements are copied.</param>
        /// <exception cref="ArgumentNullException"> If any of the provided arguments is null. </exception>
        public AttentiveCollection([NotNull] List<T> list) : base(list)
        {
            Throw.IfArgumentIsNull(list, nameof(list));

            FireCollectionChangeWhenInnerElementChanges = false;
            AttachElementWatcher();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AttentiveCollection{T}"/> class.
        /// </summary>
        /// <param name="collection">The collection from which the elements are copied.</param>
        /// <exception cref="ArgumentNullException"> If any of the provided arguments is null. </exception>
        public AttentiveCollection([NotNull] IEnumerable<T> collection) : base(collection)
        {
            Throw.IfArgumentIsNull(collection, nameof(collection));

            FireCollectionChangeWhenInnerElementChanges = false;
            AttachElementWatcher();
        }

        /// <summary>
        /// Occurs when an inner element changes its content.
        /// </summary>
        public event EventHandler<NotifyInnerElementChangedEventArgs> InnerElementChanged;

        /// <summary>
        /// Gets or sets a value indicating whether a collection changed event should be fired
        /// after a change of an inner element.
        /// </summary>
        public bool FireCollectionChangeWhenInnerElementChanges { get; set; }

        private void AttachElementWatcher()
        {
            foreach (var item in this)
            {
                ((INotifyPropertyChanged)item).PropertyChanged += InformAboutChangedItem;
            }

            CollectionChanged += AttachOrDetachElementWatcher;
        }

        private void AttachOrDetachElementWatcher([NotNull] object sender, [NotNull] NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
            {
                foreach (var oldItem in e.OldItems)
                {
                    ((INotifyPropertyChanged)oldItem).PropertyChanged -= InformAboutChangedItem;
                }
            }

            if (e.NewItems != null)
            {
                foreach (var newItem in e.NewItems)
                {
                    ((INotifyPropertyChanged)newItem).PropertyChanged += InformAboutChangedItem;
                }
            }
        }

        private void InformAboutChangedItem([NotNull] object sender, [NotNull] PropertyChangedEventArgs e)
        {
            OnInnerElementChanged(new NotifyInnerElementChangedEventArgs(sender, e.PropertyName));

            if (FireCollectionChangeWhenInnerElementChanges)
            {
                var changedEventArgs = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, sender, sender);
                OnCollectionChanged(changedEventArgs);
            }
        }

        private void OnInnerElementChanged([NotNull] NotifyInnerElementChangedEventArgs e)
        {
            InnerElementChanged?.Invoke(this, e);
        }
    }
}