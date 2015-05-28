using System;
using System.Threading;

namespace Cuemon.Threading
{
    /// <summary>
    /// Represents a synchronization primitive that is signaled when its count reaches zero.
    /// </summary>
    /// <remarks>This class was inspired by the newly introduced CountdownEvent in .NET 4.0, and I think .NET 2.0 users should benefit from this lightweight implementation.</remarks>
    public sealed class CountdownEvent : CountdownEventBase, IDisposable
    {
        private ManualResetEvent _resetEvent;
        private bool _isDisposed;

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="CountdownEvent"/> class.
        /// </summary>
        /// <param name="initialCount">The number of signals initially required to set the <see cref="CountdownEvent"/>.</param>
        public CountdownEvent(int initialCount) : base(initialCount)
        {
            _resetEvent = new ManualResetEvent(false);
        }
        #endregion

        #region Properties
        private bool IsDisposed
        {
            get { return _isDisposed; }
            set { _isDisposed = value; }
        }

        private ManualResetEvent ResetEvent
        {
            get { return _resetEvent; }
            set { _resetEvent = value; }
        }
        #endregion

        #region Methods
        /// <summary>
        /// The method that is invoked when the state of the <see cref="CountdownEvent"/> is signaled, allowing one or more waiting threads to proceed.
        /// </summary>
        protected override void Set()
        {
            if (this.ResetEvent == null) { return; }
            this.ResetEvent.Set();
        }

        /// <summary>
        /// Blocks the current thread until the <see cref="CountdownEvent"/> is set, using a <see cref="TimeSpan"/> to measure the timeout. Default is 30 seconds.
        /// </summary>
        /// <param name="timeout">A <see cref="T:System.TimeSpan"/> that represents the time to wait.</param>
        public override void Wait(TimeSpan timeout)
        {
            if (this.IsSet) { return; }
            this.ResetEvent.WaitOne(timeout == TimeSpan.MaxValue ? int.MaxValue : (int)timeout.TotalMilliseconds, false);
            if (this.ElapsedTime > timeout) { throw new TimeoutException(); }
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        private void Dispose(bool disposing)
        {
            if (this.IsDisposed) { return; }
            this.IsDisposed = true;
            if (disposing)
            {
                if (this.ResetEvent != null)
                {
                    this.ResetEvent.Close();
                }
                this.ResetEvent = null;
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}