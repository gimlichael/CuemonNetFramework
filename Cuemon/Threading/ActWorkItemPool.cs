using System;
using System.Collections.Generic;
using Cuemon.Collections.Generic;

namespace Cuemon.Threading
{
    /// <summary>
    /// Provides a pool of threads to execute work items implementing the <see cref="IActWorkItem"/> interface.
    /// </summary>
    public class ActWorkItemPool : IActWorkItemPool
    {
        private EventHandler<WorkItemExceptionEventArgs> _worktItemException = null;

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="ActWorkItemPool"/> class.
        /// </summary>
        public ActWorkItemPool()
        {
            PreliminaryExceptions = new List<Exception>();
        }
        #endregion

        #region Events
        /// <summary>
        /// Occurs when an exception that was thrown in the pool by a class implementing the <see cref="IActWorkItem"/> interface.
        /// </summary>
        public event EventHandler<WorkItemExceptionEventArgs> WorkItemException
        {
            add
            {
                EventUtility.AddEvent(value, ref _worktItemException);
            }
            remove
            {
                EventUtility.RemoveEvent(value, ref _worktItemException);
            }
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets a read-only collection of an <see cref="Exception" /> that caused one or more of the <see cref="IActWorkItem" /> to end prematurely. If all of the <see cref="IActWorkItem" /> completed successfully, this will be an empty read-only collection.
        /// </summary>
        /// <value>The exceptions.</value>
	    public IReadOnlyCollection<Exception> Exceptions
        {
            get { return new ReadOnlyCollection<Exception>(PreliminaryExceptions); }
        }

        private IList<Exception> PreliminaryExceptions { get; set; }

        internal void AddException(Exception ex)
        {
            lock (PreliminaryExceptions)
            {
                PreliminaryExceptions.Add(ex);
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Raises the <see cref="WorkItemException"/> event.
        /// </summary>
        /// <param name="exception">The exception that was thrown in the pool by a class implementing the <see cref="IActWorkItem"/> interface.</param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="exception"/> is null.
        /// </exception>
        protected virtual void OnWorkItemException(Exception exception)
        {
            if (exception == null) { throw new ArgumentNullException("exception"); }
            EventUtility.Raise(_worktItemException, this, new WorkItemExceptionEventArgs(exception));
        }

        internal void RaiseWorkItemException(Exception exception)
        {
            OnWorkItemException(exception);
        }
        #endregion

        #region Methods
        /// <summary>
        /// The work to be processed one thread at a time.
        /// </summary>
        /// <param name="work">The work item to execute one thread at a time.</param>
        public virtual void ProcessWork(IActWorkItem work)
        {
            if (work == null) { throw new ArgumentNullException("work"); }
            ActWorkItem concreteWork = work as ActWorkItem;
            if (concreteWork != null)
            {
                concreteWork.PoolReference = this;
            }
            else
            {
                work.Data.Add("wippReference", this);
            }
            ThreadPoolUtility.QueueWork(DefaultProcessWorkCallback, work);
        }

        private static void DefaultProcessWorkCallback(IActWorkItem work)
        {
            ActWorkItemPool workWorkItemPoolReference = null;
            try
            {
                ActWorkItem concreteWork = work as ActWorkItem;
                FactoryActWorkItem factoryConcreteWork = work as FactoryActWorkItem;
                if (concreteWork != null)
                {
                    workWorkItemPoolReference = concreteWork.PoolReference;
                }

                if (factoryConcreteWork != null)
                {
                    workWorkItemPoolReference = factoryConcreteWork.PoolReference;
                }

                if (workWorkItemPoolReference == null)
                {
                    workWorkItemPoolReference = work.Data["wippReference"] as ActWorkItemPool;
                }

                if (workWorkItemPoolReference == null) { throw new ArgumentException("Unable to establish a reference to the ActWorkItemPool.", "work"); }
                work.ProcessWork();
            }
            catch (Exception ex)
            {
                if (workWorkItemPoolReference == null) { return; }
                workWorkItemPoolReference.RaiseWorkItemException(ex);
                workWorkItemPoolReference.AddException(ex);
            }
            finally
            {
                work.Synchronization.Signal();
                work = null;
            }
        }
        #endregion
    }
}