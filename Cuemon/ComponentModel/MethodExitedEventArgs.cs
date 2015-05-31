using System;
using System.Collections.Generic;

namespace Cuemon.ComponentModel
{
	/// <summary>
	/// Provides data for the <see cref="INotifyMethodExited.MethodExited"/> event.
	/// </summary>
	public class MethodExitedEventArgs : EventArgs, IData
	{
		private readonly string _methodName;
	    private readonly Guid? _threadSafeIdReference;
		private readonly IDictionary<string, object> _data = new Dictionary<string, object>();

		#region Constructors
		
		MethodExitedEventArgs()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="MethodExitedEventArgs"/> class.
		/// </summary>
		/// <param name="methodName">The name of the method that was invoked.</param>
		public MethodExitedEventArgs(string methodName)
		{
			_methodName = methodName;
		}

        internal MethodExitedEventArgs(string methodName, Guid? threadSafeIdReference)
        {
            _methodName = methodName;
            _threadSafeIdReference = threadSafeIdReference;
        }
		#endregion

		#region Properties
		/// <summary>
		/// Gets the name of the method that was invoked.
		/// </summary>
		/// <value>The name of the method that was invoked.</value>
		public string MethodName { get { return _methodName; } }

        internal Guid? ThreadSafeIdReference { get { return _threadSafeIdReference; } }

		/// <summary>
		/// Gets a collection of key/value pairs that provide additional user-defined information about the member.
		/// </summary>
		public IDictionary<string, object> Data { get { return _data; } }
		#endregion

		#region Methods
		/// <summary>
		/// Represents an <see cref="MethodExitedEventArgs"/> event with no event data.
		/// </summary>
		public new static readonly MethodExitedEventArgs Empty = new MethodExitedEventArgs();
		#endregion
	}
}