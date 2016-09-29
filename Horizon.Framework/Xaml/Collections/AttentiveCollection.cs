using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using JetBrains.Annotations;

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
            AttachCollectionWatcher();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AttentiveCollection{T}"/> class.
        /// </summary>
        /// <param name="list">The list from which the elements are copied.</param>
        public AttentiveCollection([NotNull] List<T> list) : base(list)
        {
            Contract.Requires(list != null);

            FireCollectionChangeWhenInnerElementChanges = false;
            AttachCollectionWatcher();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AttentiveCollection{T}"/> class.
        /// </summary>
        /// <param name="collection">The collection from which the elements are copied.</param>
        public AttentiveCollection([NotNull] IEnumerable<T> collection) : base(collection)
        {
            Contract.Requires(collection != null);

            FireCollectionChangeWhenInnerElementChanges = false;
            AttachCollectionWatcher();
        }

        /// <summary>
        /// Occurs when an inner element changes its content.
        /// </summary>
        public event PropertyChangedEventHandler InnerElementChanged;

        /// <summary>
        /// Gets or sets a value indicating whether a collection changed event should be fired
        /// after a change of an inner element.
        /// </summary>
        public bool FireCollectionChangeWhenInnerElementChanges { get; set; }

        private void AttachCollectionWatcher()
        {
            CollectionChanged += AttachOrDetachCollectionWatcher;
        }

        private void AttachOrDetachCollectionWatcher([NotNull] object sender, [NotNull] NotifyCollectionChangedEventArgs e)
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
            OnInnerElementChanged(e);

            if (FireCollectionChangeWhenInnerElementChanges)
            {
                var changedEventArgs = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, sender);
                OnCollectionChanged(changedEventArgs);
            }
        }

        private void OnInnerElementChanged([NotNull] PropertyChangedEventArgs e)
        {
            InnerElementChanged?.Invoke(this, e);
        }
    }
}