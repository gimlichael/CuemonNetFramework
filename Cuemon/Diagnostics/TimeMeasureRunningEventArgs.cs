using System;

namespace Cuemon.Diagnostics
{
	/// <summary>
	/// Provides data for the <see cref="TimeMeasure.TimeMeasureRunning"/> event.
	/// </summary>
	public sealed class TimeMeasureRunningEventArgs : EventArgs
	{
		private readonly string _memberName;

		#region Constructors
		TimeMeasureRunningEventArgs()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="TimeMeasureRunningEventArgs"/> class.
		/// </summary>
		/// <param name="memberName">The name of the member that was invoked.</param>
		public TimeMeasureRunningEventArgs(string memberName)
		{
			_memberName = memberName;
		}
		#endregion

		#region Properties
		/// <summary>
		/// Gets the name of the member that was invoked.
		/// </summary>
		/// <value>The name of the member that was invoked.</value>
		public string MemberName { get { return _memberName; } }
		#endregion

		#region Methods
		/// <summary>
		/// Represents an <see cref="TimeMeasureRunningEventArgs"/> event with no event data.
		/// </summary>
		public new static readonly TimeMeasureRunningEventArgs Empty = new TimeMeasureRunningEventArgs();
		#endregion
	}
}
