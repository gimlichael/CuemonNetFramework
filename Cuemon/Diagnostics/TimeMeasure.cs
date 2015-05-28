using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using System.Threading;
using Cuemon.Collections.Generic;
using Cuemon.ComponentModel;
using Cuemon.Threading;

namespace Cuemon.Diagnostics
{
	/// <summary>
	/// Provides a way of time measuring implementations of either <see cref="IMethodPerformanceTiming"/> and/or <see cref="IPropertyPerformanceTiming"/>.
	/// </summary>
	internal sealed class TimeMeasure
	{
	    private EventHandler<TimeMeasureRunningEventArgs> _timeMeasureRunning = null;
        private EventHandler<TimeMeasureCompletedEventArgs> _timeMeasureCompleted = null;
        private readonly object _padLock = new object();

		#region Constructors
		TimeMeasure()
		{
		    this.Timestamp = DateTime.UtcNow;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="TimeMeasure"/>.
		/// </summary>
		/// <param name="timing">The class to listen and perform time measurement on.</param>
        /// <param name="args">The method entered args as called from <see cref="Instrumentation.OnMethodEntered(System.Reflection.MethodBase)"/>.</param>
        internal TimeMeasure(IMethodPerformanceTiming timing, MethodEnteredEventArgs args) : this()
		{
		    if (timing == null) { throw new ArgumentNullException("timing"); }
		    TimingMethodEnteredHandling(null, args);
            timing.MethodExited += new EventHandler<MethodExitedEventArgs>(TimingMethodExitedHandling);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="TimeMeasure"/> struct.
		/// </summary>
		/// <param name="timing">The class to listen and perform time measurement on.</param>
		/// <param name="args">The property changings args as called from <see cref="Instrumentation.OnPropertyChanging(System.Reflection.MethodBase)"/>.</param>
		internal TimeMeasure(IPropertyPerformanceTiming timing, PropertyChangingEventArgs args) : this()
		{
			if (timing == null) { throw new ArgumentNullException("timing"); }
		    TimingPropertyChangingHandling(null, args);
            timing.PropertyChanged += new PropertyChangedEventHandler(TimingPropertyChangedHandling);
		}
		#endregion

		#region Methods
        private void TimingMethodEnteredHandling(object sender, MethodEnteredEventArgs e)
        {
            if (e.ThreadSafeId.HasValue)
            {
                if (!this.ThreadSafeId.HasValue)
                {
                    lock (_padLock)
                    {
                        if (!this.ThreadSafeId.HasValue)
                        {
                            this.ThreadSafeId = e.ThreadSafeId;
                            this.OnTimeMeasureRunning(e.MethodName);
                        }
                    }
                }
                return;
            }

            this.OnTimeMeasureRunning(e.MethodName);
        }

	    private void TimingMethodExitedHandling(object sender, MethodExitedEventArgs e)
	    {
	        DateTime current = DateTime.UtcNow;
            if (e.ThreadSafeIdReference.HasValue)
            {
                if (this.ThreadSafeId.HasValue && this.ThreadSafeId.Value.Equals(e.ThreadSafeIdReference.Value))
                {
                    this.OnTimeMeasureCompleted(e.MethodName, (current - this.Timestamp), e.Data);
                }
                return;
            }

            lock (_padLock)
            {
	            this.OnTimeMeasureCompleted(e.MethodName, (current - this.Timestamp), e.Data);
	        }
	    }

		private void TimingPropertyChangingHandling(object sender, PropertyChangingEventArgs e)
		{
			this.OnTimeMeasureRunning(e.PropertyName);
		}

		private void TimingPropertyChangedHandling(object sender, PropertyChangedEventArgs e)
		{
		    DateTime current = DateTime.UtcNow;
            this.OnTimeMeasureCompleted(e.PropertyName, (current - this.Timestamp));
		}
		#endregion

		#region Events
		/// <summary>
		/// Occurs when the performance timing is running.
		/// </summary>
		public event EventHandler<TimeMeasureRunningEventArgs> TimeMeasureRunning
        {
            add
            {
                EventUtility.AddEvent(value, ref _timeMeasureRunning);
            }
            remove
            {
                EventUtility.RemoveEvent(value, ref _timeMeasureRunning);
            }
        }

		/// <summary>
		/// Occurs when the performance timing has completed.
		/// </summary>
		public event EventHandler<TimeMeasureCompletedEventArgs> TimeMeasureCompleted
		{
            add
            {
                EventUtility.AddEvent(value, ref _timeMeasureCompleted);
            }
            remove
            {
                EventUtility.RemoveEvent(value, ref _timeMeasureCompleted);
            }
		}
		#endregion

		#region Methods
		private void OnTimeMeasureRunning(string memberName)
		{
            this.IsMeasuring = true;
            EventUtility.Raise(_timeMeasureRunning, this, new TimeMeasureRunningEventArgs(memberName));		        
		}

		private void OnTimeMeasureCompleted(string memberName, TimeSpan elapsed)
		{
			this.OnTimeMeasureCompleted(memberName, elapsed, new Dictionary<string, object>());
		}

		private void OnTimeMeasureCompleted(string memberName, TimeSpan elapsed, IDictionary<string, object> data)
		{
		    this.IsMeasuring = false;
		    EventUtility.Raise(_timeMeasureCompleted, this, new TimeMeasureCompletedEventArgs(memberName, elapsed, data));
		}
		#endregion

		#region Properties
        private DateTime Timestamp { get; set; }

        private Guid? ThreadSafeId { get; set; }

		/// <summary>
		/// Gets a value indicating whether the <see cref="TimeMeasure"/> timer is running. 
		/// </summary>
		/// <value>
		/// 	<c>true</c> if the <see cref="TimeMeasure"/> instance is currently running and measuring elapsed time for an interval; otherwise, <c>false</c>.
		/// </value>
		public bool IsMeasuring { get; private set; }
		#endregion
	}
}