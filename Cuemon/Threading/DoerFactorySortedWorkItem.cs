using System.Collections.Generic;

namespace Cuemon.Threading
{
    internal class DoerFactorySortedWorkItem<TKey, TResult> : ISortedDoerWorkItem<TKey, TResult>
    {
        private readonly ISortedDoerWorkItem<TKey, TResult> _workItem;
        private TResult _result;

        #region Constructors
        public DoerFactorySortedWorkItem(TKey sortOrder, ISynchronization sync)
        {
            _workItem = new FactorySortedWorkItem<TKey, TResult>(sortOrder, sync);
        }

        public DoerFactorySortedWorkItem(TKey sortOrder, ISynchronization sync, Doer<TResult> method) : this(sortOrder, sync)
        {
            this.Method = method;
        }
        #endregion

        #region Methods
        public virtual void ProcessWork()
        {
            this.Result = this.Method();
        }
        #endregion

        #region Properties

        public Doer<TResult> Method { get; private set; }

        /// <summary>
        /// Gets the result of the work processed in <see cref="ProcessWork" />.
        /// </summary>
        /// <value>The result of the work processed in <see cref="ProcessWork" />.</value>
        public TResult Result
        {
            get { return _result; }
            protected set { _result = value; }
        }

        /// <summary>
        /// Gets a collection of key/value pairs that provide additional user-defined information about this class.
        /// </summary>
        public IDictionary<string, object> Data
        {
            get { return this.WorkItemReference.Data; }
        }

        /// <summary>
        /// Gets an instance of the object implementing the <see cref="ISynchronization" /> interface used for threaded synchronization signaling.
        /// </summary>
        public ISynchronization Synchronization
        {
            get { return this.WorkItemReference.Synchronization; }
        }

		/// <summary>
		/// Gets the <typeparamref name="TKey"/> that represents the sort order value.
		/// </summary>
		public TKey SortOrder
		{
			get { return this.WorkItemReference.SortOrder; }
		}

        private ISortedDoerWorkItem<TKey, TResult> WorkItemReference { get { return _workItem; } }
        #endregion
    }

    internal class DoerFactorySortedWorkItem<TKey, T1, TResult> : DoerFactorySortedWorkItem<TKey, TResult>
    {
        #region Constructors
        public DoerFactorySortedWorkItem(TKey sortOrder, ISynchronization sync, T1 arg1) : base(sortOrder, sync)
        {
            this.Arg1 = arg1;
        }

        public DoerFactorySortedWorkItem(TKey sortOrder, ISynchronization sync, Doer<T1, TResult> method, T1 arg1) : base(sortOrder, sync)
        {
            this.Method = method;
            this.Arg1 = arg1;
        }
        #endregion

        #region Methods
        public override void ProcessWork()
        {
            this.Result = this.Method(this.Arg1);
        }
        #endregion

        #region Properties

        public new Doer<T1, TResult> Method { get; private set; }
        public T1 Arg1 { get; private set; }
        #endregion
    }

    internal class DoerFactorySortedWorkItem<TKey, T1, T2, TResult> : DoerFactorySortedWorkItem<TKey, T1, TResult>
    {
        #region Constructors
        public DoerFactorySortedWorkItem(TKey sortOrder, ISynchronization sync, T1 arg1, T2 arg2) : base(sortOrder, sync, arg1)
        {
            this.Arg2 = arg2;
        }

        public DoerFactorySortedWorkItem(TKey sortOrder, ISynchronization sync, Doer<T1, T2, TResult> method, T1 arg1, T2 arg2) : base(sortOrder, sync, arg1)
        {
            this.Method = method;
            this.Arg2 = arg2;
        }
        #endregion

        #region Methods
        public override void ProcessWork()
        {
            this.Result = this.Method(this.Arg1, this.Arg2);
        }
        #endregion

        #region Properties

        public new Doer<T1, T2, TResult> Method { get; private set; }
        public T2 Arg2 { get; private set; }
        #endregion
    }

    internal class DoerFactorySortedWorkItem<TKey, T1, T2, T3, TResult> : DoerFactorySortedWorkItem<TKey, T1, T2, TResult>
    {
        #region Constructors
        public DoerFactorySortedWorkItem(TKey sortOrder, ISynchronization sync, T1 arg1, T2 arg2, T3 arg3) : base(sortOrder, sync, arg1, arg2)
        {
            this.Arg3 = arg3;
        }

        public DoerFactorySortedWorkItem(TKey sortOrder, ISynchronization sync, Doer<T1, T2, T3, TResult> method, T1 arg1, T2 arg2, T3 arg3) : base(sortOrder, sync, arg1, arg2)
        {
            this.Method = method;
            this.Arg3 = arg3;
        }
        #endregion

        #region Methods
        public override void ProcessWork()
        {
            this.Result = this.Method(this.Arg1, this.Arg2, this.Arg3);
        }
        #endregion

        #region Properties

        public new Doer<T1, T2, T3, TResult> Method { get; private set; }
        public T3 Arg3 { get; private set; }
        #endregion
    }

    internal class DoerFactorySortedWorkItem<TKey, T1, T2, T3, T4, TResult> : DoerFactorySortedWorkItem<TKey, T1, T2, T3, TResult>
    {
        #region Constructors
        public DoerFactorySortedWorkItem(TKey sortOrder, ISynchronization sync, T1 arg1, T2 arg2, T3 arg3, T4 arg4) : base(sortOrder, sync, arg1, arg2, arg3)
        {
            this.Arg4 = arg4;
        }

        public DoerFactorySortedWorkItem(TKey sortOrder, ISynchronization sync, Doer<T1, T2, T3, T4, TResult> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4) : base(sortOrder, sync, arg1, arg2, arg3)
        {
            this.Method = method;
            this.Arg4 = arg4;
        }
        #endregion

        #region Methods
        public override void ProcessWork()
        {
            this.Result = this.Method(this.Arg1, this.Arg2, this.Arg3, this.Arg4);
        }
        #endregion

        #region Properties
        public new Doer<T1, T2, T3, T4, TResult> Method { get; private set; }
        public T4 Arg4 { get; private set; }
        #endregion
    }

    internal class DoerFactorySortedWorkItem<TKey, T1, T2, T3, T4, T5, TResult> : DoerFactorySortedWorkItem<TKey, T1, T2, T3, T4, TResult>
    {
        #region Constructors
        public DoerFactorySortedWorkItem(TKey sortOrder, ISynchronization sync, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5) : base(sortOrder, sync, arg1, arg2, arg3, arg4)
        {
            this.Arg5 = arg5;
        }

        public DoerFactorySortedWorkItem(TKey sortOrder, ISynchronization sync, Doer<T1, T2, T3, T4, T5, TResult> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5) : base(sortOrder, sync, arg1, arg2, arg3, arg4)
        {
            this.Method = method;
            this.Arg5 = arg5;
        }
        #endregion

        #region Methods
        public override void ProcessWork()
        {
            this.Result = this.Method(this.Arg1, this.Arg2, this.Arg3, this.Arg4, this.Arg5);
        }

        public new Doer<T1, T2, T3, T4, T5, TResult> Method { get; private set; }
        #endregion

        #region Properties
        public T5 Arg5 { get; private set; }
        #endregion
    }
}