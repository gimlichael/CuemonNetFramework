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
        private volatile bool _isDisposed;
        private readonly ManualResetEvent _resetEvent;

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

        private ManualResetEvent ResetEvent
        {
            get
            {
                ThrowIfDisposed();
                return _resetEvent;
            }
        }
        #endregion

        #region Methods
        /// <summary>
        /// The method that is invoked when the state of the <see cref="CountdownEvent"/> is signaled, allowing one or more waiting threads to proceed.
        /// </summary>
        protected override void Set()
        {
            ThrowIfDisposed();
            if (ResetEventHasValidState) { ResetEvent.Set(); }
        }

        /// <summary>
        /// Blocks the current thread until the <see cref="CountdownEvent"/> is set, using a <see cref="TimeSpan"/> to measure the timeout. Default is 30 seconds.
        /// </summary>
        /// <param name="timeout">A <see cref="T:System.TimeSpan"/> that represents the time to wait.</param>
        public override void Wait(TimeSpan timeout)
        {
            ThrowIfDisposed();
            if (IsSet) { return; }
            if (ResetEventHasValidState) { ResetEvent.WaitOne(timeout == TimeSpan.MaxValue ? int.MaxValue : (int)timeout.TotalMilliseconds, false); }
            if (ElapsedTime > timeout) { throw new TimeoutException(); }
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        private void Dispose(bool disposing)
        {
            if (_isDisposed || !disposing) { return; }
            _isDisposed = true;
            _resetEvent.Close();
        }

        private bool ResetEventHasValidState
        {
            get
            {
                return (ResetEvent != null &&
                       !ResetEvent.SafeWaitHandle.IsClosed);
            }
        }

        private void ThrowIfDisposed()
        {
            if (_isDisposed) { throw new ObjectDisposedException("CountdownEvent"); }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }
        #endregion
    }
}