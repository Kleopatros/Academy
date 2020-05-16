using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;

namespace Academy.Cs.Patterns.ActiveRecord
{
    public interface IRecordStore // TODO
    {
        bool Save(IRecordStorable model);
    }

    public interface IRecordStorable // TODO
    {
        // TODO: Schema { get; }
    }



    //https://guides.rubyonrails.org/active_model_basics.html
    //https://guides.rubyonrails.org/active_record_basics.html
    //https://api.rubyonrails.org/v3.1/classes/ActiveResource/Base.html
    //ActiveModel: This component was created in Rails 3. They took all the model related parts that did not have a database requirement of Rails 2 ActiveRecord and moved it into ActiveModel.So ActiveModel includes things like validations.More information: http://www.rubyinside.com/rails-3-0s-activemodel-how-to-give-ruby-classes-some-activerecord-magic-2937.html
    // ActiveRecord: This is the component that associates a class to the database.This will give the class functionality such as methods that make it easy to pull records from the database(An example is the find method).

    //ActiveResource: Similar to ActiveRecord.However, instead of being backed by a database, an ActiveResource object is backed by another application through a web service API.More information: http://ofps.oreilly.com/titles/9780596521424/activeresource_id59243.html

    //    ActiveListener - used to setup dependency nodes across one or more ActiveModels(many to many sources and consumers?), inherit to provide specific functionality.Like some metaobject could add settings objects as they are made and set a boolean indicating a certain computaiton is now invalid.
    //ActiveMessenger: a better decoupled approach to the above:
    //FinancialComputationMessenger
    // const string SomethingChanged;
    //    bool HasComputationInputChanged;
    //    Who owns this object; if its another model, how is it better than a Listener?

    //    Don't make models trees; use graphs and allow Register to accept an argument stating if the registerer is the responsible owner of the registeree


    //DataModel   - instead of this, what if the model just has no backing source? how to make a lightweight version, is it possible?
    //ActiveModel
    //DataModel.Validator(static?)
    //DataModel.Schema
    //DataModel.Serializer

    //ObservableCollection<T> is OK, but should the super be watching this or the sub?

    //moved changed notivation to inside read lock? lookup advice for raising events inside locks - also consider advice for what to do and what not to do inside event handlers.just like claling aproperty shouldn't be heavy weight

    //use graph and find way to decouple associations. note you may have associations but you may also have subdata (e.g. and array of points). the former must be decoupled, but both must be optionally loaded into memory.

    ////    active record to be reused by front end -- NO
    ////use composition on the validator.what about the model? could do that to, but props would be duplicated and complicates your get/set
    ////https://docs.microsoft.com/en-us/dotnet/architecture/microservices/microservice-ddd-cqrs-patterns/domain-model-layer-validations
    ////you can still use your active record, but it should not have a IDataStore connection, instead it should raise an event when it wants to save, or maybe a savable extension (Model is base, ActiveModel is a savable subclass).


    // TODO: https://en.wikipedia.org/wiki/Active_record_pattern








    /// <summary>
    /// A thread-safe data model backed by some store (a database, file, cache, etc.).
    /// </summary>
    /// <remarks>
    /// Property access is garaunteed atomic using an object-scoped <see
    /// cref="ReaderWriterLockSlim"/>. Beware that events may be raised from within read locks,
    /// but never a write lock.
    /// </remarks>
    public abstract class ActiveRecord : ActiveModel, INotifyPropertyChanging, INotifyPropertyChanged, IRecordStorable
    {
        private readonly HashSet<string> _propertiesChanged = new HashSet<string>();

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
        /// Reads the record from storage, overwriting any changes that have been made to this instance.
        /// </summary>
        public void Reload()
        {
            this.ShouldNotify = false;

            throw new NotImplementedException();

            this.ShouldNotify = true;
            this.OnPropertyChanged(null);
        }

        /// <summary>
        /// Writes the state of this instance to storage.
        /// </summary>
        /// <returns>
        /// <c>true</c> if the instance was saved successfully; <c>false</c> if the write failed
        /// or could not be performed.
        /// </returns>
        public bool Save()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Invoked after a property value has changed.
        /// </summary>
        /// <param name="propertyName">The name of the property that changed.</param>
        protected override void OnPropertyChanged(string propertyName)
        {
            _propertiesChanged.Add(propertyName);
            base.OnPropertyChanged(propertyName);
        }
    }
}