using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Academy.Cs.Patterns.ActiveRecord
{
    public interface IDataStore // TODO
    {
        bool Save(IDataStorable model);
    }

    public interface IDataStorable // TODO
    {
        // TODO: Schema { get; }
    }

    // TODO: https://en.wikipedia.org/wiki/Active_record_pattern
    public abstract class ActiveRecord : IDisposable, INotifyPropertyChanging, INotifyPropertyChanged, IDataStorable
    {
        private readonly ReaderWriterLockSlim _dataLock = new ReaderWriterLockSlim();

        private readonly HashSet<string> _propertiesChanged = new HashSet<string>();

        private bool _disposed;

        /// <summary>
        /// Initializes a new <see cref="ActiveRecord"/> instance.
        /// </summary>
        protected ActiveRecord()
        {
        }

        /// <summary>
        /// Frees resources and performs other cleanup operations before this instance is reclaimed
        /// by garbage collection.
        /// </summary>
        ~ActiveRecord()
        {
            this.Dispose(false);
        }

        /// <summary>
        /// Occurs when a property value is changing.
        /// </summary>
        public event PropertyChangingEventHandler PropertyChanging;

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        
        // TODO: Associations, RegisterAssociation(Assocation.HasMany, ActiveRecord)

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

        public void Reload()
        {
            throw new NotImplementedException();
        }

        public bool Save()
        {
            throw new NotImplementedException();
        }

        protected virtual bool Validate()
        {
            throw new NotImplementedException();
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
            this.PropertyChanging?.Invoke(this, new PropertyChangingEventArgs(propertyName));
            return true;
        }

        /// <summary>
        /// Invoked after a property value has changed.
        /// </summary>
        /// <param name="propertyName">The name of the property that changed.</param>
        protected virtual void OnPropertyChanged(string propertyName)
        {
            _propertiesChanged.Add(propertyName);
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
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
            bool propertyChanged = false;
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
                        propertyChanged = true;
                    }
                    finally
                    {
                        _dataLock.ExitWriteLock();
                    }
                }
            }
            finally
            {
                _dataLock.ExitUpgradeableReadLock();
            }

            if (propertyChanged)
            {
                this.OnPropertyChanged(propertyName);
            }

            return propertyChanged;
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
    }
}