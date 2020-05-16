using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Academy.Cs.Patterns.ActiveRecord
{
    /// <summary>
    /// A thread-safe data model to be used by an application.
    /// </summary>
    /// <remarks>
    /// Property access is garaunteed atomic using an object-scoped <see
    /// cref="ReaderWriterLockSlim"/>. Note, events may be raised from within read locks,
    /// but never a write lock.
    /// </remarks>
    public abstract class ActiveModel : IDisposable, INotifyPropertyChanging, INotifyPropertyChanged
    {
        private readonly ReaderWriterLockSlim _dataLock = new ReaderWriterLockSlim();

        private readonly HashSet<ICollection> _childCollections = new HashSet<ICollection>();

        private readonly HashSet<ActiveModel> _children = new HashSet<ActiveModel>();

        private bool _disposed;

        /// <summary>
        /// Initializes a new <see cref="ActiveModel"/> instance.
        /// </summary>
        protected ActiveModel()
        {
            this.ShouldNotify = true;
        }

        /// <summary>
        /// Frees resources and performs other cleanup operations before this instance is reclaimed
        /// by garbage collection.
        /// </summary>
        ~ActiveModel()
        {
            this.Dispose(false);
        }

        /// <summary>
        /// Occurs when a property value is changing.
        /// </summary>
        /// <remarks>
        /// An example of why this event is useful:
        /// 
        ///   1. Some Listener caches a Result that is dependent on some Model.Property.
        ///   2. The Model.Property changes.
        ///   3. Some Object accesses Listener.Result and is unaware the value is invalid.
        ///   4. The Model raises the <see cref="PropertyChanged"/> event.
        ///   5. The Listener handles the event and caches the new Result.
        ///   
        /// To inform the Object that Listener.Result is invalid, the Listener can handle the
        /// <see cref="PropertyChanging"/> event to flag the result as invalid until the
        /// <see cref="PropertyChanged"/> event is handled and caches the new Result.
        /// </remarks>
        public event PropertyChangingEventHandler PropertyChanging;

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        
        /// <summary>
        /// Gets or sets whether data manipulation notifications should be sent.
        /// </summary>
        protected bool ShouldNotify
        {
            get;
            set;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting
        /// unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting
        /// unmanaged resources.
        /// </summary>
        /// <param name="disposing">
        /// If <c>true</c>, the method has been called directly or indirectly by a user's code.
        /// Managed and unmanaged resources should be disposed.
        /// 
        /// If <c>false</c>, the method has been called by the runtime from inside the finalizer
        /// and other objects should not be referenced. Only unmanaged resources can be disposed.
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            try
            {
                if (disposing)
                {
                    _dataLock.Dispose();
                }
            }
            finally
            {
                _disposed = true;
            }
        }

        /// <summary>
        /// Gets or sets whether the current state is valid.
        /// </summary>
        public bool IsValid
        {
            get
            {
                throw new NotImplementedException();
            }

            private set
            {
                throw new NotImplementedException();
            }
        }
        
        /// <summary>
        /// Validates the current state of this instance.
        /// </summary>
        /// <returns><c>true</c> if this instance is valid; otherwise, <c>false</c>.</returns>
        protected virtual bool Validate()
        {
            throw new NotImplementedException();
        }

        protected bool RegisterChildCollection<T>(ObservableCollection<T> children)//?
            where T : ActiveModel
        {
            if (_childCollections.Add(children))//lock?  https://docs.microsoft.com/en-us/dotnet/api/system.collections.concurrent?view=netframework-4.8
            {
                children.CollectionChanged += this.ChildCollectionChanged;
                foreach (ActiveModel child in children)
                {
                    this.RegisterChild(child);
                }

                return true;
            }

            return false;
        }

        protected bool UnregisterChildCollection<T>(ObservableCollection<T> children)//?//what if 2 collections hold the same child instance? prevent this by contract (implied)
            where T : ActiveModel
        {
            if (_childCollections.Remove(children))//lock?
            {
                children.CollectionChanged -= this.ChildCollectionChanged;
                foreach (ActiveModel child in children)
                {
                    this.UnregisterChild(child);
                }

                return true;
            }

            return false;
        }

        protected bool RegisterChild(ActiveModel child)//?
        {
            if (_children.Add(child))//lock? gaurd against null
            {
                child.PropertyChanging += this.ChildPropertyChanging;
                child.PropertyChanged += this.ChildPropertyChanged;
                return true;
            }

            return false;
        }

        protected bool UnregisterChild(ActiveModel child)//?
        {
            if (_children.Remove(child))//lock?
            {
                child.PropertyChanged -= this.ChildPropertyChanged;
                child.PropertyChanging -= this.ChildPropertyChanging;
                return true;
            }

            return false;
        }

        protected virtual void OnChildPropertyChanging(ActiveModel child, string propertyName)//?
        {
        }

        protected virtual void OnChildPropertyChanged(ActiveModel child, string propertyName)//?
        {
        }

        /// <summary>
        /// Invoked when a property value is changing.
        /// </summary>
        /// <param name="propertyName">The name of the property whose value is changing.</param>
        /// <returns>
        /// <c>true</c> if changing the property value should continue; <c>false</c> if the
        /// operation should be canceled.
        /// </returns>
        protected virtual bool OnPropertyChanging(string propertyName)
        {
            if (this.ShouldNotify)
            {
                this.PropertyChanging?.Invoke(this, new PropertyChangingEventArgs(propertyName));
            }

            return true;
        }

        /// <summary>
        /// Invoked after a property value has changed.
        /// </summary>
        /// <param name="propertyName">The name of the property that changed.</param>
        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (this.ShouldNotify)
            {
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        /// <summary>
        /// Safely sets the value of a property and invokes the relevant events.
        /// </summary>
        /// <remarks>
        /// It would be ideal, for simplicity, if the <see cref="Interlocked"/> class could be used
        /// to atomically update <paramref name="field"/> if <paramref name="value"/> differs from
        /// its existing value. However, the events surrounding the change complicate the operation.
        /// </remarks>
        /// <typeparam name="T">The type of the property value.</typeparam>
        /// <param name="field">The field storing the property value.</param>
        /// <param name="value">The value to assign the property.</param>
        /// <param name="propertyName">The name of the property whose value is changing.</param>
        /// <returns>
        /// <c>true</c> if the property value has changed; <c>false</c> if the operation was
        /// canceled or <paramref name="field"/> was already equal to <paramref name="value"/>.
        /// </returns>
        protected bool Set<T>(ref T field, T value, [CallerMemberName] string propertyName = "")
        {
            _dataLock.EnterUpgradeableReadLock();
            try
            {
                if (!EqualityComparer<T>.Default.Equals(field, value))
                {
                    this.OnPropertyChanging(propertyName);

                    _dataLock.EnterWriteLock();
                    try
                    {
                        field = value;
                    }
                    finally
                    {
                        _dataLock.ExitWriteLock();
                    }

                    this.OnPropertyChanged(propertyName);
                    return true;
                }
            }
            finally
            {
                _dataLock.ExitUpgradeableReadLock();
            }

            return false;
        }

        /// <summary>
        /// Safely gets the value of a property.
        /// </summary>
        /// <typeparam name="T">The type of the property value.</typeparam>
        /// <param name="field">The field storing the property value.</param>
        /// <returns>The value of <paramref name="field"/>.</returns>
        protected T Get<T>(ref T field)
        {
            _dataLock.EnterReadLock();
            try
            {
                return field;
            }
            finally
            {
                _dataLock.ExitReadLock();
            }
        }

        private void ChildCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)//?//lock to prevent adding items from sender that is being unregistered
        {
            Debug.Assert(sender != null, $"Sender should not be null.");
            Debug.Assert(sender is ICollection, $"Unexpected sender type, {sender.GetType().Name}.");//fix
            Debug.Assert(_childCollections.Contains((ICollection)sender), "Sender is not registered."); //issues if we includ eloccking

            foreach (ActiveModel child in e.NewItems.Cast<ActiveModel>())
            {
                this.RegisterChild(child);
            }

            foreach (ActiveModel child in e.OldItems.Cast<ActiveModel>())
            {
                this.UnregisterChild(child);
            }
        }

        private void ChildPropertyChanging(object sender, PropertyChangingEventArgs e)//?
        {
            this.OnChildPropertyChanging((ActiveModel)sender, e.PropertyName);
        }

        private void ChildPropertyChanged(object sender, PropertyChangedEventArgs e)//?
        {
            this.OnChildPropertyChanged((ActiveModel)sender, e.PropertyName);
        }
    }
}