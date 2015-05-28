using System;
using System.ComponentModel;
using Cuemon.ComponentModel;

namespace Cuemon.Diagnostics
{
	/// <summary>
	/// Notifies and provides timing data for clients when a method has been invoked.
	/// </summary>
	public interface IMethodPerformanceTiming : INotifyMethodExited, INotifyMethodEntered
	{
	}
}