using System;
using System.ComponentModel;
using Cuemon.ComponentModel;

namespace Cuemon.Diagnostics
{
	/// <summary>
	/// Notifies and provides timing data for clients when a property has been invoked.
	/// </summary>
	public interface IPropertyPerformanceTiming : INotifyPropertyChanged, INotifyPropertyChanging
	{
	}
}