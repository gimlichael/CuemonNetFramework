using System.Collections.Generic;

namespace Cuemon.Threading
{
    internal class ActFactoryWorkItem : IActWorkItem
    {
        private readonly IActWorkItem _workItem;

        #region Constructors
        public ActFactoryWorkItem(ISynchronization sync)
        {
            _workItem = new FactoryActWorkItem(sync);
        }

        public ActFactoryWorkItem(ISynchronization sync, Act method) : this(sync)
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

        public Act Method { get; private set; }

        public IDictionary<string, object> Data
        {
            get { return this.WorkItemReference.Data; }
        }

        /// <summary>
        /// Gets an instance of the <see cref="ISynchronization" /> object used for threaded synchronization signaling.
        /// </summary>
        public ISynchronization Synchronization
        {
            get { return this.WorkItemReference.Synchronization; }
        }

        private IActWorkItem WorkItemReference { get { return _workItem; } }
        #endregion
    }

    internal class ActFactoryWorkItem<T1> : ActFactoryWorkItem
    {
        #region Constructors
        public ActFactoryWorkItem(ISynchronization sync, T1 arg1) : base(sync)
        {
            this.Arg1 = arg1;
        }

        public ActFactoryWorkItem(ISynchronization sync, Act<T1> method, T1 arg1) : base(sync)
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

        public new Act<T1> Method { get; private set; }
        public T1 Arg1 { get; private set; }
        #endregion
    }

    internal class ActFactoryWorkItem<T1, T2> : ActFactoryWorkItem<T1>
    {
        #region Constructors
        public ActFactoryWorkItem(ISynchronization sync, T1 arg1, T2 arg2) : base(sync, arg1)
        {
            this.Arg2 = arg2;
        }

        public ActFactoryWorkItem(ISynchronization sync, Act<T1, T2> method, T1 arg1, T2 arg2) : base(sync, arg1)
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

        public new Act<T1, T2> Method { get; private set; }
        public T2 Arg2 { get; private set; }
        #endregion
    }

    internal class ActFactoryWorkItem<T1, T2, T3> : ActFactoryWorkItem<T1, T2>
    {
        #region Constructors
        public ActFactoryWorkItem(ISynchronization sync, T1 arg1, T2 arg2, T3 arg3) : base(sync, arg1, arg2)
        {
            this.Arg3 = arg3;
        }

        public ActFactoryWorkItem(ISynchronization sync, Act<T1, T2, T3> method, T1 arg1, T2 arg2, T3 arg3) : base(sync, arg1, arg2)
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

        public new Act<T1, T2, T3> Method { get; private set; }
        public T3 Arg3 { get; private set; }
        #endregion
    }

    internal class ActFactoryWorkItem<T1, T2, T3, T4> : ActFactoryWorkItem<T1, T2, T3>
    {
        #region Constructors
        public ActFactoryWorkItem(ISynchronization sync, T1 arg1, T2 arg2, T3 arg3, T4 arg4) : base(sync, arg1, arg2, arg3)
        {
            this.Arg4 = arg4;
        }

        public ActFactoryWorkItem(ISynchronization sync, Act<T1, T2, T3, T4> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4) : base(sync, arg1, arg2, arg3)
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
        public new Act<T1, T2, T3, T4> Method { get; private set; }
        public T4 Arg4 { get; private set; }
        #endregion
    }

    internal class ActFactoryWorkItem<T1, T2, T3, T4, T5> : ActFactoryWorkItem<T1, T2, T3, T4>
    {
        #region Constructors
        public ActFactoryWorkItem(ISynchronization sync, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5) : base(sync, arg1, arg2, arg3, arg4)
        {
            this.Arg5 = arg5;
        }

        public ActFactoryWorkItem(ISynchronization sync, Act<T1, T2, T3, T4, T5> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5) : base(sync, arg1, arg2, arg3, arg4)
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

        public new Act<T1, T2, T3, T4, T5> Method { get; private set; }
        #endregion

        #region Properties
        public T5 Arg5 { get; private set; }
        #endregion
    }
}