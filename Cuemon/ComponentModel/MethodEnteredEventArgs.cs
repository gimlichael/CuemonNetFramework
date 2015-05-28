using System;
using System.Collections.Generic;
using System.Text;
using Cuemon.Diagnostics;
namespace Cuemon.ComponentModel
{
	/// <summary>
	/// Provides data for the <see cref="INotifyMethodEntered.MethodEntered"/> event.
	/// </summary>
	public class MethodEnteredEventArgs : EventArgs, IData
	{
		private readonly string _methodName;
	    private readonly Guid? _threadSafeId;
		private readonly IDictionary<string, object> _data = new Dictionary<string, object>();

		#region Constructors
		MethodEnteredEventArgs()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="MethodEnteredEventArgs"/> class.
		/// </summary>
		/// <param name="methodName">The name of the method being invoked.</param>
		public MethodEnteredEventArgs(string methodName)
		{
			_methodName = methodName;
		}

        internal MethodEnteredEventArgs(string methodName, Guid? threadSafeId)
        {
            _methodName = methodName;
            _threadSafeId = threadSafeId;
        }
		#endregion

		#region Properties
		/// <summary>
		/// Gets the name of the method being invoked.
		/// </summary>
		/// <value>The name of the method being invoked.</value>
		public string MethodName { get { return _methodName; } }

        internal Guid? ThreadSafeId { get { return _threadSafeId; } }

		/// <summary>
		/// Gets a collection of key/value pairs that provide additional user-defined information about the member.
		/// </summary>
		public IDictionary<string, object> Data { get { return _data; } }
		#endregion

		#region Methods
		/// <summary>
		/// Represents an <see cref="MethodEnteredEventArgs"/> event with no event data.
		/// </summary>
		public new static readonly MethodEnteredEventArgs Empty = new MethodEnteredEventArgs();
		#endregion
	}
}