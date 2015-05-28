using System;
using System.Diagnostics;
using System.Threading;

namespace Cuemon.Threading
{
    /// <summary>
    /// Provides support for spin-based waiting.
    /// </summary>
    /// <remarks>This class was inspired by the newly introduced SpintWait in .NET 4.0, and I think .NET 2.0 users should benefit from this lightweight implementation.</remarks>
    public struct Spinner
    {
        private static readonly int Step = ThreadUtility.NumberOfLogicalProcessors;
        private static readonly bool IsSingleCore = ThreadUtility.NumberOfCores == 1;

        #region Properties
        /// <summary>
        /// Gets the number of times <see cref="SpinOnce"/> has been called on this instance.
        /// </summary>
        /// <value>Returns the number of times <see cref="SpinOnce"/> has been called on this instance.</value>
        public int Count { get; private set; }

        /// <summary>
        /// Gets whether the next call to <see cref="SpinOnce"/> will yield the processor, triggering a forced context switch.
        /// </summary>
        /// <value><c>true</c> if the next call to <see cref="SpinOnce"/> will yield the processor, triggering a forced context switch; otherwise <c>false</c>.</value>
        public bool NextSpinWillYield
        {
            get { return this.Count > Step || IsSingleCore; }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Performs a single spin.
        /// </summary>
        public void SpinOnce()
        {
            if (this.Count == int.MaxValue) { this.Count = Step; }
            if (this.NextSpinWillYield)
            {
                int current = this.Count >= Step ? this.Count - Step : this.Count;
                if (current % 20 == 19)
                {
                    Thread.Sleep(1);
                }
                else if (current % 5 == 4)
                {
                    Thread.Sleep(0);
                }
                else
                {
                    ThreadUtility.Yield();
                }
            }
            else
            {
                Thread.SpinWait(ThreadUtility.NumberOfCores << this.Count);
            }
            this.Count++;
        }

        /// <summary>
        /// Resets the spin counter.
        /// </summary>
        public void Reset()
        {
            this.Count = 0;
        }

        /// <summary>
        /// Spins until the specified <paramref name="condition"/> is satisfied.
        /// </summary>
        /// <param name="condition">A delegate to be executed over and over until it returns <c>true</c>.</param>
        /// <returns><c>true</c> if the condition is satisfied within the timeout; otherwise, <c>false</c>.</returns>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="condition"/> is null.
        /// </exception>
        public static bool SpinUntil(Doer<bool> condition)
        {
            if (condition == null) { throw new ArgumentNullException("condition"); }
            return SpinUntil(condition, TimeSpan.MaxValue);
        }

        /// <summary>
        /// Spins until the specified <paramref name="condition"/> is satisfied or until the specified <paramref name="timeout"/> is expired.
        /// </summary>
        /// <param name="condition">A delegate to be executed over and over until it returns <c>true</c>.</param>
        /// <param name="timeout">A <see cref="TimeSpan"/> that represents the amount of time to wait, or a <see cref="TimeSpan.MaxValue"/> to wait indefinitely.</param>
        /// <returns><c>true</c> if the condition is satisfied within the timeout; otherwise, <c>false</c>.</returns>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="condition"/> is null -or- <paramref name="timeout"/> is null.
        /// </exception>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// <paramref name="timeout"/> cannot be negative.
        /// </exception>
        public static bool SpinUntil(Doer<bool> condition, TimeSpan timeout)
        {
            if (condition == null) { throw new ArgumentNullException("condition"); }
            if (timeout == null) { throw new ArgumentNullException("timeout"); }
            if (timeout.Ticks < 0) { throw new ArgumentOutOfRangeException("timeout", "The timeout cannot be negative."); }
            if (timeout == TimeSpan.Zero) { return false; }
            Spinner wait = new Spinner();
            Stopwatch watch = Stopwatch.StartNew();
            while (!condition())
            {
                if (watch.Elapsed > timeout) { return false; }
                wait.SpinOnce();
            }
            return true;
        }
        #endregion
    }
}