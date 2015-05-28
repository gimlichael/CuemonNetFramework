using System;
using System.Threading;

namespace Cuemon.Threading
{
    /// <summary>
    /// Represents a synchronization primitive that is signaled when its count reaches zero.
    /// Unlike <see cref="CountdownEvent"/> this class does not rely on <see cref="ManualResetEvent"/>.
    /// </summary>
    public sealed class CountdownEventSlim : CountdownEventBase
    {
        
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="CountdownEventSlim"/> class.
        /// </summary>
        /// <param name="initialCount">The number of signals initially required to set the <see cref="CountdownEventSlim"/>.</param>
        public CountdownEventSlim(int initialCount) : base(initialCount)
        {
        }
        #endregion

        #region Properties
        private bool OperationComplete { get; set; }
        #endregion

        #region Methods
        /// <summary>
        /// The method that is invoked when the state of the <see cref="CountdownEventSlim"/> is signaled, allowing one or more waiting threads to proceed.
        /// </summary>
        protected override void Set()
        {
            this.OperationComplete = true;
        }

        /// <summary>
        /// Blocks the current thread until the <see cref="CountdownEvent"/> is set, using a <see cref="TimeSpan"/> to measure the timeout. Default is 30 seconds.
        /// </summary>
        /// <param name="timeout">A <see cref="T:System.TimeSpan"/> that represents the time to wait.</param>
        public override void Wait(TimeSpan timeout)
        {
            if (this.IsSet) { return; }
            Spinner.SpinUntil(DelegateUtility.DynamicWrap(OperationComplete), timeout);
            if (this.ElapsedTime > timeout) { throw new TimeoutException(); }
        }
        #endregion
    }
}