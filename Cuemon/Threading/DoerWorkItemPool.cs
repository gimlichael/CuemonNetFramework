using System;
using System.Collections.Generic;
using Cuemon.Collections.Generic;

namespace Cuemon.Threading
{
	/// <summary>
	/// Provides a pool of threads to execute work items implementing the <see cref="IDoerWorkItem{TResult}"/> interface.
	/// </summary>
	public class DoerWorkItemPool<TResult> : ActWorkItemPool, IDoerWorkItemPool<TResult>
	{
		private readonly IList<TResult> _preliminaryResult;

		#region Constructors
		/// <summary>
        /// Initializes a new instance of the <see cref="DoerWorkItemPool{TResult}"/> class.
		/// </summary>
        public DoerWorkItemPool()
		{
			_preliminaryResult = new List<TResult>();
		}
		#endregion

		#region Properties
		/// <summary>
		/// Gets the result of the work processed in <see cref="ProcessWork"/>.
		/// </summary>
		/// <value>
		/// The result of the work processed in <see cref="ProcessWork"/>.
		/// </value>
		public virtual IReadOnlyCollection<TResult> Result
		{
			get { return new ReadOnlyCollection<TResult>(this.PreliminaryResult); }
		}

		private IList<TResult> PreliminaryResult
		{
			get { return _preliminaryResult; }
		}
		#endregion

		#region Methods
		internal void AddResult(TResult value)
		{
			lock (this.PreliminaryResult)
			{
				this.PreliminaryResult.Add(value);
			}
		}

		void IActWorkItemPool.ProcessWork(IActWorkItem work)
		{
            throw new NotImplementedException();
		}

        /// <summary>
        /// The work to be processed one thread at a time.
        /// </summary>
        /// <param name="work">The work item to execute one thread at a time.</param>
        public virtual void ProcessWork(IDoerWorkItem<TResult> work)
        {
            if (work == null) { throw new ArgumentNullException("work"); }
            DoerWorkItem<TResult> concreteWork = work as DoerWorkItem<TResult>;
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

        private static void DefaultProcessWorkCallback(IDoerWorkItem<TResult> work)
		{
            DoerWorkItemPool<TResult> workItemPoolReference = null;
			try
			{
                DoerWorkItem<TResult> concreteWork = work as DoerWorkItem<TResult>;
			    FactoryDoerWorkItem<TResult> factoryConcreteWork = work as FactoryDoerWorkItem<TResult>;
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
                    workItemPoolReference = work.Data["wippReference"] as DoerWorkItemPool<TResult>;    
                }
				
				if (workItemPoolReference == null) { throw new ArgumentException("Unable to establish a reference to the DoerWorkItemPool.", "work"); }
				work.ProcessWork();
				workItemPoolReference.AddResult(work.Result);
			}
            catch (Exception ex)
            {
                if (workItemPoolReference == null) { return; }
                workItemPoolReference.RaiseWorkItemException(ex);
                workItemPoolReference.ExceptionsCore.Add(ex);
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