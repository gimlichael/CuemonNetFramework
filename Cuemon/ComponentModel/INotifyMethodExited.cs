using System;
namespace Cuemon.ComponentModel
{
	/// <summary>
	/// Notifies clients that a method has been invoked.
	/// </summary>
	public interface INotifyMethodExited
	{
		/// <summary>
		/// Occurs when a method has been invoked.
		/// </summary>
		event EventHandler<MethodExitedEventArgs> MethodExited;
	}
}
