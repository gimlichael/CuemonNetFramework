using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
namespace Cuemon.Threading
{
    internal class ActionFactorySortedWorkItem<TKey> : ISortedWorkItem<TKey>
    {
        private readonly ISortedWorkItem<TKey> _workItem;

        #region Constructors
        public ActionFactorySortedWorkItem(TKey sortOrder, CountdownEvent sync)
        {
            _workItem = new FactorySortedWorkItem<TKey>(sortOrder, sync);
        }

        public ActionFactorySortedWorkItem(TKey sortOrder, CountdownEvent sync, Action method) : this(sortOrder, sync)
        {
            this.Method = method;
        }
        #endregion

        #region Methods
        public virtual void ProcessWork()
        {
            this.Method();
        }
        #endregion

        #region Properties

        public Action Method { get; private set; }

        /// <summary>
        /// Gets the result of the work processed in <see cref="ProcessWork" />.
        /// </summary>
        /// <value>The result of the work processed in <see cref="ProcessWork" />.</value>
        /// <remarks>This method will always return null as the implementation is Action based.</remarks>
        public object Result
        {
            get { return null; }
        }

        public IDictionary<string, object> Data
        {
            get { return this.WorkItemReference.Data; }
        }

        /// <summary>
        /// Gets an instance of the <see cref="CountdownEvent" /> object used for threaded synchronization signaling.
        /// </summary>
        public CountdownEvent Synchronization
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

        private ISortedWorkItem<TKey> WorkItemReference { get { return _workItem; } }
        #endregion
    }

    internal class ActionFactorySortedWorkItem<TKey, T1> : ActionFactorySortedWorkItem<TKey>
    {
        #region Constructors
        public ActionFactorySortedWorkItem(TKey sortOrder, CountdownEvent sync, T1 arg1) : base(sortOrder, sync)
        {
            this.Arg1 = arg1;
        }

        public ActionFactorySortedWorkItem(TKey sortOrder, CountdownEvent sync, Action<T1> method, T1 arg1) : base(sortOrder, sync)
        {
            this.Method = method;
            this.Arg1 = arg1;
        }
        #endregion

        #region Methods
        public override void ProcessWork()
        {
            this.Method(this.Arg1);
        }
        #endregion

        #region Properties

        public new Action<T1> Method { get; private set; }
        public T1 Arg1 { get; private set; }
        #endregion
    }

    internal class ActionFactorySortedWorkItem<TKey, T1, T2> : ActionFactorySortedWorkItem<TKey, T1>
    {
        #region Constructors
        public ActionFactorySortedWorkItem(TKey sortOrder, CountdownEvent sync, T1 arg1, T2 arg2) : base(sortOrder, sync, arg1)
        {
            this.Arg2 = arg2;
        }

        public ActionFactorySortedWorkItem(TKey sortOrder, CountdownEvent sync, Action<T1, T2> method, T1 arg1, T2 arg2) : base(sortOrder, sync, arg1)
        {
            this.Method = method;
            this.Arg2 = arg2;
        }
        #endregion

        #region Methods
        public override void ProcessWork()
        {
            this.Method(this.Arg1, this.Arg2);
        }
        #endregion

        #region Properties

        public new Action<T1, T2> Method { get; private set; }
        public T2 Arg2 { get; private set; }
        #endregion
    }

    internal class ActionFactorySortedWorkItem<TKey, T1, T2, T3> : ActionFactorySortedWorkItem<TKey, T1, T2>
    {
        #region Constructors
        public ActionFactorySortedWorkItem(TKey sortOrder, CountdownEvent sync, T1 arg1, T2 arg2, T3 arg3) : base(sortOrder, sync, arg1, arg2)
        {
            this.Arg3 = arg3;
        }

        public ActionFactorySortedWorkItem(TKey sortOrder, CountdownEvent sync, Action<T1, T2, T3> method, T1 arg1, T2 arg2, T3 arg3) : base(sortOrder, sync, arg1, arg2)
        {
            this.Method = method;
            this.Arg3 = arg3;
        }
        #endregion

        #region Methods
        public override void ProcessWork()
        {
            this.Method(this.Arg1, this.Arg2, this.Arg3);
        }
        #endregion

        #region Properties

        public new Action<T1, T2, T3> Method { get; private set; }
        public T3 Arg3 { get; private set; }
        #endregion
    }

    internal class ActionFactorySortedWorkItem<TKey, T1, T2, T3, T4> : ActionFactorySortedWorkItem<TKey, T1, T2, T3>
    {
        #region Constructors
        public ActionFactorySortedWorkItem(TKey sortOrder, CountdownEvent sync, T1 arg1, T2 arg2, T3 arg3, T4 arg4) : base(sortOrder, sync, arg1, arg2, arg3)
        {
            this.Arg4 = arg4;
        }

        public ActionFactorySortedWorkItem(TKey sortOrder, CountdownEvent sync, Action<T1, T2, T3, T4> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4) : base(sortOrder, sync, arg1, arg2, arg3)
        {
            this.Method = method;
            this.Arg4 = arg4;
        }
        #endregion

        #region Methods
        public override void ProcessWork()
        {
            this.Method(this.Arg1, this.Arg2, this.Arg3, this.Arg4);
        }
        #endregion

        #region Properties
        public new Action<T1, T2, T3, T4> Method { get; private set; }
        public T4 Arg4 { get; private set; }
        #endregion
    }

    internal class ActionFactorySortedWorkItem<TKey, T1, T2, T3, T4, T5> : ActionFactorySortedWorkItem<TKey, T1, T2, T3, T4>
    {
        #region Constructors
        public ActionFactorySortedWorkItem(TKey sortOrder, CountdownEvent sync, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5) : base(sortOrder, sync, arg1, arg2, arg3, arg4)
        {
            this.Arg5 = arg5;
        }

        public ActionFactorySortedWorkItem(TKey sortOrder, CountdownEvent sync, Action<T1, T2, T3, T4, T5> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5) : base(sortOrder, sync, arg1, arg2, arg3, arg4)
        {
            this.Method = method;
            this.Arg5 = arg5;
        }
        #endregion

        #region Methods
        public override void ProcessWork()
        {
            this.Method(this.Arg1, this.Arg2, this.Arg3, this.Arg4, this.Arg5);
        }

        public new Action<T1, T2, T3, T4, T5> Method { get; private set; }
        #endregion

        #region Properties
        public T5 Arg5 { get; private set; }
        #endregion
    }
}