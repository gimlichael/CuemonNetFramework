using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Text;
using System.Threading;
using Cuemon.Collections.Generic;
using Cuemon.ComponentModel;
using Cuemon.Reflection;
using Cuemon.Threading;
namespace Cuemon.Diagnostics
{
	/// <summary>
	/// An abstract class for diagnostics, monitoring and measuring performance.
	/// </summary>
	public abstract class Instrumentation : IInstrumentation
	{
		private static TimeSpan _timeMeasureThreshold = TimeSpan.Zero;
		private bool _enableMethodPerformanceTiming;
		private bool _enablePropertyPerformanceTiming;
	    private EventHandler<MethodEnteredEventArgs> _methodEntered = null;
	    private EventHandler<MethodExitedEventArgs> _methodExited = null;

		#region Constructors
		/// <summary>
		/// Initializes a new instance of the <see cref="Instrumentation"/> class where both <see cref="EnableMethodPerformanceTiming"/> and <see cref="EnablePropertyPerformanceTiming"/> is enabled.
		/// </summary>
		protected Instrumentation() : this(true, true)
		{
		}

        /// <summary>
        /// Initializes a new instance of the <see cref="Instrumentation" /> class.
        /// </summary>
        /// <param name="enableMethodPerformanceTiming"><c>true</c> to enable method performance timing; otherwise <c>false</c>.</param>
        /// <param name="enablePropertyPerformanceTiming"><c>true</c> to enable property performance timing; otherwise <c>false</c>.</param>
        protected Instrumentation(bool enableMethodPerformanceTiming, bool enablePropertyPerformanceTiming)
        {
            this.EnableMethodPerformanceTiming = enableMethodPerformanceTiming;
            this.EnablePropertyPerformanceTiming = enablePropertyPerformanceTiming;
        }
		#endregion

        #region Events
        /// <summary>
		/// Occurs when a property value changes and marks the end of a timing.
		/// </summary>
		public event PropertyChangedEventHandler PropertyChanged;

	    /// <summary>
	    /// Occurs when a property value is changing and marks the beginning om timing.
	    /// </summary>
	    public event PropertyChangingEventHandler PropertyChanging;


		/// <summary>
		/// Occurs when a method is being invoked and marks the beginning of timing.
		/// </summary>
		public event EventHandler<MethodEnteredEventArgs> MethodEntered
		{
		    add
		    {
                EventUtility.AddEvent(value, ref _methodEntered);
		    }
            remove
            {
                EventUtility.RemoveEvent(value, ref _methodEntered);
            }
		}

		/// <summary>
		/// Occurs when a method has been invoked and marks the end of a timing.
		/// </summary>
		public event EventHandler<MethodExitedEventArgs> MethodExited
        {
            add
            {
                EventUtility.AddEvent(value, ref _methodExited);
            }
            remove
            {
                EventUtility.RemoveEvent(value, ref _methodExited);
            }
        }
		#endregion

		#region Properties
		/// <summary>
		/// Gets or sets a time measuring threshold before <see cref="TimeMeasureCompletedHandling(object,Cuemon.Diagnostics.TimeMeasureCompletedEventArgs)"/> is invoked. Default is <see cref="TimeSpan.Zero"/>, which is equivalent to no threshold.
		/// </summary>
		/// <value>
		/// The time measuring threshold before <see cref="TimeMeasureCompletedHandling(object,Cuemon.Diagnostics.TimeMeasureCompletedEventArgs)"/> is invoked.
		/// </value>
		public TimeSpan TimeMeasureThreshold
		{
			get { return _timeMeasureThreshold; }
			set { _timeMeasureThreshold = value; }
		}

		/// <summary>
		/// Gets or sets a value indicating whether performance timing of method calls is enabled.
		/// </summary>
		/// <value>
		///   <c>true</c> if performance timing of method calls is enabled; otherwise, <c>false</c>.
		/// </value>
		public bool EnableMethodPerformanceTiming
		{
			get { return _enableMethodPerformanceTiming; }
			set { _enableMethodPerformanceTiming = value; }
		}

		/// <summary>
		/// Gets or sets a value indicating whether performance timing of property calls is enabled.
		/// </summary>
		/// <value>
		///   <c>true</c> if performance timing of property calls is enabled; otherwise, <c>false</c>.
		/// </value>
		public bool EnablePropertyPerformanceTiming
		{
			get { return _enablePropertyPerformanceTiming; }
			set { _enablePropertyPerformanceTiming = value; }
		}
		#endregion

		#region Methods
		/// <summary>
		/// Creates and returns a snapshot of the most important information for the current process where this instance resides.
		/// </summary>
		public ProcessSnapshot GetCurrentProcessSnapshot()
		{
			return ProcessSnapshot.GetCurrentProcess();
		}

        /// <summary>
        /// Raises the <see cref="MethodEntered"/> event.
        /// </summary>
        /// <param name="method">The method being invoked.</param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="method"/> is null.
        /// </exception>
        protected void OnMethodEntered(MethodBase method)
        {
            if (!this.EnableMethodPerformanceTiming) { return; }
            if (method == null) { throw new ArgumentNullException("method"); }
            this.OnMethodEntered(new MethodSignature(method));
        }

		/// <summary>
		/// Raises the <see cref="MethodEntered"/> event.
		/// </summary>
        /// <param name="method">The method being invoked.</param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="method"/> is null.
        /// </exception>
        protected void OnMethodEntered(MethodSignature method)
		{
            if (!this.EnableMethodPerformanceTiming) { return; }
            if (method == null) { throw new ArgumentNullException("method"); }
            this.OnMethodEntered(method, null);
		}

        /// <summary>
        /// Raises the <see cref="MethodEntered" /> event.
        /// </summary>
        /// <param name="method">The method being invoked.</param>
        /// <param name="threadSafeId">Either a reference to a <see cref="Guid"/> used for thread safe time measuring or null for normal usage.</param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="method"/> is null.
        /// </exception>
        protected void OnMethodEntered(MethodBase method, Guid? threadSafeId)
        {
            if (!this.EnableMethodPerformanceTiming) { return; }
            if (method == null) { throw new ArgumentNullException("method"); }
            this.OnMethodEntered(new MethodSignature(method), threadSafeId);
        }

        /// <summary>
        /// Raises the <see cref="MethodEntered" /> event.
        /// </summary>
        /// <param name="method">The method being invoked.</param>
        /// <param name="threadSafeId">Either a reference to a <see cref="Guid"/> used for thread safe time measuring or null for normal usage.</param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="method"/> is null.
        /// </exception>
        protected virtual void OnMethodEntered(MethodSignature method, Guid? threadSafeId)
        {
            if (!this.EnableMethodPerformanceTiming) { return; }
            if (method == null) { throw new ArgumentNullException("method"); }
            MethodEnteredEventArgs args = new MethodEnteredEventArgs(method.ToString(), threadSafeId);
            TimeMeasure measure = new TimeMeasure(this as IMethodPerformanceTiming, args);
            measure.TimeMeasureCompleted += new EventHandler<TimeMeasureCompletedEventArgs>(TimeMeasureCompletedHandling);
            EventUtility.Raise(_methodEntered, this, args);
        }

        /// <summary>
        /// Raises the <see cref="MethodExited"/> event.
        /// </summary>
        /// <param name="method">The method being invoked.</param>
        /// <param name="threadSafeIdReference">A reference to the earlier generated unique id by <see cref="OnMethodEntered(Cuemon.Reflection.MethodSignature)"/> used for thread safe time measuring.</param>
        /// <param name="parameters">A variable number of objects that provide parameter values of the method being invoked.</param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="method"/> is null.
        /// </exception>
        protected void OnMethodExited(MethodBase method, Guid threadSafeIdReference, params object[] parameters)
        {
            if (!this.EnableMethodPerformanceTiming) { return; }
            if (method == null) { throw new ArgumentNullException("method"); }
            this.OnMethodExited(new MethodSignature(method), threadSafeIdReference, parameters);
        }

        /// <summary>
        /// Raises the <see cref="MethodExited"/> event.
        /// </summary>
        /// <param name="method">The method being invoked.</param>
        /// <param name="threadSafeIdReference">A reference to the earlier generated unique id by <see cref="OnMethodEntered(Cuemon.Reflection.MethodSignature)"/> used for thread safe time measuring.</param>
        /// <param name="parameters">A variable number of objects that provide parameter values of the method being invoked.</param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="method"/> is null.
        /// </exception>
        protected void OnMethodExited(MethodSignature method, Guid threadSafeIdReference, params object[] parameters)
        {
            if (!this.EnableMethodPerformanceTiming) { return; }
            if (method == null) { throw new ArgumentNullException("method"); }
            this.OnMethodExited(method, threadSafeIdReference as Guid?, parameters);
        }

        /// <summary>
        /// Raises the <see cref="MethodExited"/> event.
        /// </summary>
        /// <param name="method">The method being invoked.</param>
        /// <param name="threadSafeIdReference">A reference to the earlier generated unique id by <see cref="OnMethodEntered(Cuemon.Reflection.MethodSignature)"/> used for thread safe time measuring.</param>
        /// <param name="parameters">A sequence of key/value pairs that provide parameter names and values of the method being invoked.</param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="method"/> is null.
        /// </exception>
        protected void OnMethodExited(MethodBase method, Guid threadSafeIdReference, IEnumerable<KeyValuePair<string, object>> parameters)
        {
            if (!this.EnableMethodPerformanceTiming) { return; }
            if (method == null) { throw new ArgumentNullException("method"); }
            this.OnMethodExited(new MethodSignature(method), threadSafeIdReference, parameters);
        }

        /// <summary>
        /// Raises the <see cref="MethodExited"/> event.
        /// </summary>
        /// <param name="method">The method being invoked.</param>
        /// <param name="threadSafeIdReference">A reference to the earlier generated unique id by <see cref="OnMethodEntered(Cuemon.Reflection.MethodSignature)"/> used for thread safe time measuring.</param>
        /// <param name="parameters">A sequence of key/value pairs that provide parameter names and values of the method being invoked.</param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="method"/> is null.
        /// </exception>
        protected void OnMethodExited(MethodSignature method, Guid threadSafeIdReference, IEnumerable<KeyValuePair<string, object>> parameters)
        {
            if (!this.EnableMethodPerformanceTiming) { return; }
            if (method == null) { throw new ArgumentNullException("method"); }
            this.OnMethodExited(method, threadSafeIdReference as Guid?, parameters);
        }

        /// <summary>
        /// Raises the <see cref="MethodExited"/> event.
        /// </summary>
        /// <param name="method">The method being invoked.</param>
        /// <param name="parameters">A sequence of key/value pairs that provide parameter names and values of the method being invoked.</param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="method"/> is null.
        /// </exception>
        protected void OnMethodExited(MethodBase method, IEnumerable<KeyValuePair<string, object>> parameters)
        {
            if (!this.EnableMethodPerformanceTiming) { return; }
            if (method == null) { throw new ArgumentNullException("method"); }
            this.OnMethodExited(new MethodSignature(method), parameters);
        }

        /// <summary>
        /// Raises the <see cref="MethodExited"/> event.
        /// </summary>
        /// <param name="method">The method being invoked.</param>
        /// <param name="parameters">A sequence of key/value pairs that provide parameter names and values of the method being invoked.</param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="method"/> is null.
        /// </exception>
        protected void OnMethodExited(MethodSignature method, IEnumerable<KeyValuePair<string, object>> parameters)
        {
            if (!this.EnableMethodPerformanceTiming) { return; }
            if (method == null) { throw new ArgumentNullException("method"); }
            this.OnMethodExited(method, null, parameters);
        }

        /// <summary>
        /// Raises the <see cref="MethodExited"/> event.
        /// </summary>
        /// <param name="method">The method being invoked.</param>
        /// <param name="parameters">A variable number of objects that provide parameter values of the method being invoked.</param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="method"/> is null.
        /// </exception>
        protected void OnMethodExited(MethodBase method, params object[] parameters)
        {
            if (!this.EnableMethodPerformanceTiming) { return; }
            if (method == null) { throw new ArgumentNullException("method"); }
            this.OnMethodExited(new MethodSignature(method), parameters);
        }

        /// <summary>
        /// Raises the <see cref="MethodExited"/> event.
        /// </summary>
        /// <param name="method">The method being invoked.</param>
        /// <param name="parameters">A variable number of objects that provide parameter values of the method being invoked.</param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="method"/> is null.
        /// </exception>
        protected void OnMethodExited(MethodSignature method, params object[] parameters)
        {
            if (!this.EnableMethodPerformanceTiming) { return; }
            if (method == null) { throw new ArgumentNullException("method"); }
            this.OnMethodExited(method, null, parameters);
        }

        /// <summary>
        /// Raises the <see cref="MethodExited" /> event.
        /// </summary>
        /// <param name="method">The method being invoked.</param>
        /// <param name="threadSafeIdReference">Either a reference to the earlier generated unique id by <see cref="OnMethodEntered(Cuemon.Reflection.MethodSignature)" /> used for thread safe time measuring or null for normal usage.</param>
        /// <param name="parameters">A variable number of objects that provide parameters information of the method being invoked.</param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="method"/> is null.
        /// </exception>
        protected void OnMethodExited(MethodBase method, Guid? threadSafeIdReference, params object[] parameters)
        {
            if (!this.EnableMethodPerformanceTiming) { return; }
            if (method == null) { throw new ArgumentNullException("method"); }
            this.OnMethodExited(new MethodSignature(method), threadSafeIdReference, parameters);
        }

        /// <summary>
        /// Raises the <see cref="MethodExited" /> event.
        /// </summary>
        /// <param name="method">The method being invoked.</param>
        /// <param name="threadSafeIdReference">Either a reference to the earlier generated unique id by <see cref="OnMethodEntered(Cuemon.Reflection.MethodSignature)" /> used for thread safe time measuring or null for normal usage.</param>
        /// <param name="parameters">A variable number of objects that provide parameters information of the method being invoked.</param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="method"/> is null.
        /// </exception>
        protected void OnMethodExited(MethodSignature method, Guid? threadSafeIdReference, params object[] parameters)
        {
            if (!this.EnableMethodPerformanceTiming) { return; }
            if (method == null) { throw new ArgumentNullException("method"); }
            this.OnMethodExited(method, threadSafeIdReference, MethodSignature.MergeParameters(method.Parameters, parameters));
        }

        /// <summary>
        /// Raises the <see cref="MethodExited" /> event.
        /// </summary>
        /// <param name="method">The method being invoked.</param>
        /// <param name="threadSafeIdReference">Either a reference to the earlier generated unique id by <see cref="OnMethodEntered(Cuemon.Reflection.MethodSignature)" /> used for thread safe time measuring or null for normal usage.</param>
        /// <param name="parameters">A sequence of key/value pairs that provide additional user-defined information of the method being invoked.</param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="method"/> is null.
        /// </exception>
        protected void OnMethodExited(MethodBase method, Guid? threadSafeIdReference, IEnumerable<KeyValuePair<string, object>> parameters)
        {
            if (!this.EnableMethodPerformanceTiming) { return; }
            if (method == null) { throw new ArgumentNullException("method"); }
            this.OnMethodExited(new MethodSignature(method), threadSafeIdReference, parameters);
        }

        /// <summary>
        /// Raises the <see cref="MethodExited" /> event.
        /// </summary>
        /// <param name="method">The method being invoked.</param>
        /// <param name="threadSafeIdReference">Either a reference to the earlier generated unique id by <see cref="OnMethodEntered(Cuemon.Reflection.MethodSignature)" /> used for thread safe time measuring or null for normal usage.</param>
        /// <param name="parameters">A sequence of key/value pairs that provide additional user-defined information of the method being invoked.</param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="method"/> is null.
        /// </exception>
        protected virtual void OnMethodExited(MethodSignature method, Guid? threadSafeIdReference, IEnumerable<KeyValuePair<string, object>> parameters)
        {
            if (!this.EnableMethodPerformanceTiming) { return; }
            if (method == null) { throw new ArgumentNullException("method"); }
            MethodExitedEventArgs args = new MethodExitedEventArgs(method.ToString(), threadSafeIdReference);
            if (parameters != null) { foreach (KeyValuePair<string, object> parameter in parameters) { args.Data.Add(parameter.Key, parameter.Value); } }
            EventUtility.Raise(_methodExited, this, args);
        }

		/// <summary>
		/// Raises the <see cref="PropertyChanging"/> event.
		/// </summary>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="method"/> is null.
        /// </exception>
		protected void OnPropertyChanging(MethodBase method)
		{
            if (!this.EnablePropertyPerformanceTiming) { return; }
            if (method == null) { throw new ArgumentNullException("method"); }
            this.OnPropertyChanging(new MethodSignature(method));
		}

		/// <summary>
		/// Raises the <see cref="PropertyChanging"/> event.
		/// </summary>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="method"/> is null.
        /// </exception>
		protected virtual void OnPropertyChanging(MethodSignature method)
		{
			if (!this.EnablePropertyPerformanceTiming) { return; }
            if (method == null) { throw new ArgumentNullException("method"); }
            PropertyChangingEventArgs args = new PropertyChangingEventArgs(method.ToString());
            TimeMeasure measure = new TimeMeasure(this as IPropertyPerformanceTiming, args);
            measure.TimeMeasureCompleted += new EventHandler<TimeMeasureCompletedEventArgs>(TimeMeasureCompletedHandling);
			PropertyChangingEventHandler handler = PropertyChanging;
			if (handler != null) { handler(this, args); }
		}

        /// <summary>
        /// Raises the <see cref="PropertyChanged"/> event.
        /// </summary>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="method"/> is null.
        /// </exception>
        protected void OnPropertyChanged(MethodBase method)
        {
            if (!this.EnablePropertyPerformanceTiming) { return; }
            if (method == null) { throw new ArgumentNullException("method"); }
            this.OnPropertyChanged(new MethodSignature(method));
        }

		/// <summary>
		/// Raises the <see cref="PropertyChanged"/> event.
		/// </summary>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="method"/> is null.
        /// </exception>
        protected virtual void OnPropertyChanged(MethodSignature method)
		{
            if (!this.EnablePropertyPerformanceTiming) { return; }
            if (method == null) { throw new ArgumentNullException("method"); }
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) { handler(this, new PropertyChangedEventArgs(method.ToString())); }
        }

		private void TimeMeasureCompletedHandling(object sender, TimeMeasureCompletedEventArgs e)
		{
            TimeMeasure measure = sender as TimeMeasure;
            if (measure != null) { measure.TimeMeasureCompleted -= new EventHandler<TimeMeasureCompletedEventArgs>(TimeMeasureCompletedHandling); }

            if (this.EnableMethodPerformanceTiming || this.EnablePropertyPerformanceTiming)
            {
                if (this.TimeMeasureThreshold == TimeSpan.Zero | e.Elapsed > this.TimeMeasureThreshold)
                {
                    this.TimeMeasureCompletedHandling(e.MemberName, e.Elapsed, e.Data);
                }
            }
		}

		/// <summary>
		/// This method handles the subscription on <see cref="TimeMeasure.TimeMeasureCompleted"/> issued by <see cref="OnMethodEntered(Cuemon.Reflection.MethodSignature)"/>.
		/// </summary>
		/// <param name="memberName">The name of the member that was invoked.</param>
		/// <param name="elapsed">The total elapsed time measured on the member that was invoked.</param>
		/// <param name="data">A collection of key/value pairs that provide additional user-defined information about the member invoked.</param>
		/// <remarks>
		/// Override this method to change the default behavior (which currently is <see cref="Trace.WriteLine(string)"/>) of time measured data.<br/>
		/// Method is only invoked if the following is true:<br/>
		/// - OnMethodEntered paired with OnMethodExited or OnPropertyChanging paired with OnPropertyChanged methods was invoked<br/>
		/// - <see cref="EnableMethodPerformanceTiming"/> and/or <see cref="EnablePropertyPerformanceTiming"/> is set to <c>true</c><br/>
		/// - <see cref="TimeMeasureThreshold"/> must be set to either <see cref="TimeSpan.Zero"/> or the elapsed time measuring is larger than <see cref="TimeMeasureThreshold"/>.
		/// </remarks>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="memberName"/> is null or <br/>
        /// <paramref name="data"/> is null.
        /// </exception>
        /// <exception cref="Cuemon.ArgumentEmptyException">
        /// <paramref name="memberName"/> is empty.
        /// </exception>
		protected virtual void TimeMeasureCompletedHandling(string memberName, TimeSpan elapsed, IDictionary<string, object> data)
		{
            if (memberName == null) { throw new ArgumentNullException("memberName"); }
            if (memberName.Length == 0) { throw new ArgumentEmptyException("memberName"); }
			if (data == null) { throw new ArgumentNullException("data"); }
			StringBuilder result = new StringBuilder(String.Format(CultureInfo.InvariantCulture, "{0} took {1}ms to execute.", memberName, Math.Round(elapsed.TotalMilliseconds)));
			if (data.Count > 0)
			{
				result.Append(" Parameters: { ");
				foreach (KeyValuePair<string, object> keyValuePair in data)
				{
					result.AppendFormat(CultureInfo.InvariantCulture, "{0}={1}, ", keyValuePair.Key, keyValuePair.Value ?? "null");
				}
				result.Remove(result.Length - 2, 2);
				result.Append(" }");
            }
#if DEBUG
            Debug.WriteLine(result.ToString());
#else
            Trace.WriteLine(result.ToString());
#endif
		}
	    #endregion
	}
}
