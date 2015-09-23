using System;
using System.Collections.Generic;
using Cuemon.Collections.Generic;

namespace Cuemon.Threading
{
    /// <summary>
    /// Provides a pool of threads to execute work items implementing the <see cref="ISortedDoerWorkItem{TKey,TResult}"/> interface.
    /// </summary>
    public sealed class SortedDoerWorkItemPool<TKey, TResult> : DoerWorkItemPool<TResult>, ISortedDoerWorkItemPool<TKey, TResult>
    {
        private readonly IDictionary<TKey, TResult> _preliminaryResult;

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="SortedDoerWorkItemPool{TKey,TResult}"/> class.
        /// </summary>
        public SortedDoerWorkItemPool() : this(Comparer<TKey>.Default)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SortedDoerWorkItemPool{TKey,TResult}"/> class.
        /// </summary>
        /// <param name="comparer">The <see cref="IComparer{T}"/> implementation to use when comparing keys.</param>
        public SortedDoerWorkItemPool(IComparer<TKey> comparer)
        {
            _preliminaryResult = new SortedDictionary<TKey, TResult>(comparer);
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the result of the work processed in <see cref="ProcessWork"/>.
        /// </summary>
        /// <value>
        /// The result of the work processed in <see cref="ProcessWork"/>.
        /// </value>
        public override IReadOnlyCollection<TResult> Result
        {
            get { return new ReadOnlyCollection<TResult>(PreliminaryResult.Values); }
        }

        private IDictionary<TKey, TResult> PreliminaryResult
        {
            get { return _preliminaryResult; }
        }
        #endregion

        #region Methods
        internal void AddResult(TKey key, TResult value)
        {
            lock (PreliminaryResult)
            {
                PreliminaryResult.Add(key, value);
            }
        }

        void IDoerWorkItemPool<TResult>.ProcessWork(IDoerWorkItem<TResult> work)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The work to be processed one thread at a time.
        /// </summary>
        /// <param name="work">The work item to execute one thread at a time.</param>
        public void ProcessWork(ISortedDoerWorkItem<TKey, TResult> work)
        {
            if (work == null) { throw new ArgumentNullException("work"); }
            SortedDoerWorkItem<TKey, TResult> concreteWork = work as SortedDoerWorkItem<TKey, TResult>;
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

        private static void DefaultProcessWorkCallback(ISortedDoerWorkItem<TKey, TResult> work)
        {
            SortedDoerWorkItemPool<TKey, TResult> workItemPoolReference = null;
            try
            {
                SortedDoerWorkItem<TKey, TResult> concreteWork = work as SortedDoerWorkItem<TKey, TResult>;
                FactorySortedWorkItem<TKey, TResult> factoryConcreteWork = work as FactorySortedWorkItem<TKey, TResult>;
                if (concreteWork != null)
                {
                    workItemPoolReference = concreteWork.PoolReference;
                }

                if (factoryConcreteWork != null)
                {
                    workItemPoolReference = factoryConcreteWork.PoolReference;
                }

                if (workItemPoolReference == null)
                {
                    workItemPoolReference = work.Data["wippReference"] as SortedDoerWorkItemPool<TKey, TResult>;
                }

                if (workItemPoolReference == null) { throw new ArgumentException("Unable to establish a reference to the SortedDoerWorkItemPool.", "work"); }
                work.ProcessWork();
                workItemPoolReference.AddResult(work.SortOrder, work.Result);
            }
            catch (Exception ex)
            {
                if (workItemPoolReference == null) { return; }
                workItemPoolReference.RaiseWorkItemException(ex);
                workItemPoolReference.AddException(ex);
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