using System;

namespace Cuemon.ComponentModel
{
	/// <summary>
	/// Notifies clients that a method is being invoked.
	/// </summary>
	public interface INotifyMethodEntered
	{
		/// <summary>
		/// Occurs when a method is being invoked.
		/// </summary>
		event EventHandler<MethodEnteredEventArgs> MethodEntered;
	}
}
