using System;
using System.Collections.Generic;
using System.Text;
using Cuemon.ComponentModel;

namespace Cuemon.Diagnostics
{
	/// <summary>
	/// Provides data for the <see cref="TimeMeasure.TimeMeasureCompleted"/> event.
	/// </summary>
	public sealed class TimeMeasureCompletedEventArgs : EventArgs
	{
		private readonly string _memberName;
		private readonly TimeSpan _elapsed;
		private readonly IDictionary<string, object> _data = new Dictionary<string, object>();

		#region Constructors
		TimeMeasureCompletedEventArgs()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="TimeMeasureCompletedEventArgs"/> class.
		/// </summary>
		/// <param name="memberName">The name of the member that was invoked.</param>
		/// <param name="elapsed">The total elapsed time measured on the member that was invoked.</param>
		public TimeMeasureCompletedEventArgs(string memberName, TimeSpan elapsed) : this(memberName, elapsed, new Dictionary<string, object>())
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="TimeMeasureCompletedEventArgs"/> class.
		/// </summary>
		/// <param name="memberName">The name of the member that was invoked.</param>
		/// <param name="elapsed">The total elapsed time measured on the member that was invoked.</param>
		/// <param name="data">A collection of key/value pairs that provide additional user-defined information about the member invoked.</param>
		public TimeMeasureCompletedEventArgs(string memberName, TimeSpan elapsed, IDictionary<string, object> data)
		{
			if (data == null) { throw new ArgumentNullException("data"); }
			_memberName = memberName;
			_elapsed = elapsed;
			_data = data;
		}
		#endregion

		#region Properties
		/// <summary>
		/// Gets the total elapsed time measured on the member that was invoked.
		/// </summary>
		public TimeSpan Elapsed { get { return _elapsed; } }

		/// <summary>
		/// Gets the name of the member that was invoked.
		/// </summary>
		/// <value>The name of the member that was invoked.</value>
		public string MemberName { get { return _memberName; } }

		/// <summary>
		/// Gets a collection of key/value pairs that provide additional user-defined information about the member that was invoked.
		/// </summary>
		public IDictionary<string, object> Data { get { return _data; } }
		#endregion

		#region Methods
		/// <summary>
		/// Represents an <see cref="TimeMeasureCompletedEventArgs"/> event with no event data.
		/// </summary>
		public new static readonly TimeMeasureCompletedEventArgs Empty = new TimeMeasureCompletedEventArgs();
		#endregion
	}
}
