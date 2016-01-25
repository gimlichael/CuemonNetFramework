using System;
using System.Threading;

namespace Cuemon.Threading
{
    /// <summary>
    /// Represents a synchronization primitive that is signaled when its count reaches zero.
    /// </summary>
    public abstract class CountdownEventBase : ISynchronization
    {
        private int _currentCount;
        private readonly DateTime _created;

        /// <summary>
        /// Initializes a new instance of the <see cref="CountdownEventBase"/> class.
        /// </summary>
        /// <param name="initialCount">The number of signals initially required to set the <see cref="CountdownEventBase"/>.</param>
        protected CountdownEventBase(int initialCount)
        {
            _currentCount = initialCount;
            _created = DateTime.UtcNow;
        }

        #region Properties
        /// <summary>
        /// Determines whether the event is set.
        /// </summary>
        /// <returns>
        ///     <c>true</c> if the event is set; otherwise, <c>false</c>.
        /// </returns>
        public bool IsSet
        {
            get { return CurrentCount <= 0; }
        }

        /// <summary>
        /// Gets the elapsed waiting time of the current thread.
        /// </summary>
        public TimeSpan ElapsedTime
        {
            get { return (DateTime.UtcNow - _created); }
        }

        /// <summary>
        /// Gets the number of remaining signals required to set the event.
        /// </summary>
        public int CurrentCount
        {
            get { return _currentCount; }
        }

        /// <summary>
        /// Increments the <see cref="CurrentCount"/> by a specified value.
        /// </summary>
        /// <param name="signalCount">The value by which to increase <see cref="CurrentCount"/>.</param>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// <paramref name="signalCount"/> is less than or equal to zero.
        /// </exception>
        /// <exception cref="System.InvalidOperationException">
        /// <see cref="CurrentCount"/> is equal to or greater than <see cref="int.MaxValue"/> after count is incremented by <paramref name="signalCount"/>.
        /// </exception>
        public void AddCount(int signalCount)
        {
            if (signalCount < 1) { throw new ArgumentOutOfRangeException("signalCount"); }

            Spinner wait = new Spinner();
            while (true)
            {
                var comparand = _currentCount;
                if (comparand < 1) { return; }
                if (comparand > (int.MaxValue - signalCount)) { throw new InvalidOperationException("The increment operation would cause the CurrentCount to overflow."); }
                if (Interlocked.CompareExchange(ref _currentCount, comparand + signalCount, comparand) == comparand) { break; }
                wait.SpinOnce();
            }
        }
        #endregion

        #region Methods

        /// <summary>
        /// Registers a signal with this instance, decrementing the value of <see cref="CurrentCount"/>.
        /// </summary>
        public void Signal()
        {
            if (Interlocked.Decrement(ref _currentCount) == 0)
            {
                Set();
            }
        }

        /// <summary>
        /// Registers multiple signals with this instance, decrementing the value of <see cref="CurrentCount"/> by the specified <paramref name="count"/> amount.
        /// </summary>
        /// <param name="count">The number of signals to register.</param>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// <paramref name="count"/> is less than or equal to zero.
        /// </exception>
        /// <exception cref="System.InvalidOperationException">
        /// The current instance is already set -or- <paramref name="count"/> is greater than <see cref="CurrentCount"/>.
        /// </exception>
        public void Signal(int count)
        {
            if (count < 1) { throw new ArgumentOutOfRangeException("count"); }
            int comparand;
            Spinner wait = new Spinner();
            while (true)
            {
                comparand = _currentCount;
                if (comparand < count) { throw new InvalidOperationException("Invalid attempt made to decrement the event's count below zero."); }
                if (Interlocked.CompareExchange(ref _currentCount, comparand - count, comparand) == comparand) { break; }
                wait.SpinOnce();
            }

            if (comparand == count) { Set(); }
        }

        /// <summary>
        /// The method that is invoked when the state of the event is signaled, allowing one or more waiting threads to proceed.
        /// </summary>
        protected abstract void Set();

        /// <summary>
        /// Blocks the current thread until this instance is <see cref="Set"/>.
        /// </summary>
        public void Wait()
        {
            Wait(TimeSpan.FromSeconds(30));
        }

        /// <summary>
        /// Blocks the current thread until this instance is <see cref="Set"/>, using a <see cref="TimeSpan"/> to measure the timeout. Default is 30 seconds.
        /// </summary>
        /// <param name="timeout">A <see cref="T:System.TimeSpan"/> that represents the time to wait.</param>
        public abstract void Wait(TimeSpan timeout);
        #endregion
    }
}