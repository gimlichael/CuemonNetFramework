using System;
using System.Diagnostics;
using System.Threading;

namespace Cuemon.Management
{
    /// <summary>
    /// Represents a lightweight, read-only implementation of a Windows NT performance counter similar to Windows Performance Monitor.
    /// </summary>
    public sealed class PerformanceMonitorCounter
    {
        private readonly string _categoryName;
        private readonly string _computerName;
        private readonly string _instanceName;
        private readonly string _counterName;
        private readonly float _counterValue;
        private readonly DateTime _utcCreated;

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="PerformanceMonitorCounter" /> class.
        /// </summary>
        /// <param name="counter">The <see cref="PerformanceCounter"/> to retrieve values from.</param>
        internal PerformanceMonitorCounter(PerformanceCounter counter) : this(counter, TimeSpan.FromMilliseconds(128))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PerformanceMonitorCounter" /> class.
        /// </summary>
        /// <param name="counter">The <see cref="PerformanceCounter"/> to retrieve values from.</param>
        /// <param name="sampleDelay">The delay before measuring a new calculated counter sample. Default is 128 ms.</param>
        internal PerformanceMonitorCounter(PerformanceCounter counter, TimeSpan sampleDelay)
        {
            _categoryName = counter.CategoryName;
            _computerName = counter.MachineName;
            _instanceName = counter.InstanceName;
            _counterName = counter.CounterName;
            if ((_counterValue = counter.NextValue()) == 0.0)
            {
                Thread.Sleep(sampleDelay);
                _counterValue = counter.NextValue();
            }
            _utcCreated = DateTime.UtcNow;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the name of the performance monitor object.
        /// </summary>
        /// <value>The name of the performance monitor object with which this counter is associated.</value>
        public string Object
        {
            get { return _categoryName; }
        }

        /// <summary>
        /// Gets the computer name for this counter.
        /// </summary>
        /// <value>The computer on which the counter and its associated object reside.</value>
        public string Computer
        {
            get { return _computerName; }
        }

        /// <summary>
        /// Gets the instance name for this counter.
        /// </summary>
        /// <value>The name of the performance monitor instance, or an empty string, if the counter is a single-instance counter.</value>
        public string Instance
        {
            get { return _instanceName; }
        }

        /// <summary>
        /// Gets the name of the counter that is associated with the performance monitor object.
        /// </summary>
        /// <value>The name of the counter, which generally describes the quantity being counted.</value>
        public string Counter
        {
            get { return _counterName; }
        }

        /// <summary>
        /// Gets the counter sample value that was present @ <see cref="UtcSampleTime"/>.
        /// </summary>
        /// <value>A snapshot of the counter sample value that was present @ <see cref="UtcSampleTime"/>.</value>
        public float Value
        {
            get { return _counterValue; }
        }

        /// <summary>
        /// Gets time when the counter sample was obtained.
        /// </summary>
        /// <value>The time when the counter sample was obtained.</value>
        /// <remarks>This property is measured in Coordinated Universal Time (UTC) (also known as Greenwich Mean Time).</remarks>
        public DateTime UtcSampleTime
        {
            get { return _utcCreated; }
        }
        #endregion
    }
}