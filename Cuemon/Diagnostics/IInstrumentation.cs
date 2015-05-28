namespace Cuemon.Diagnostics
{
	/// <summary>
	/// Provides a way for diagnostics, monitoring and measuring performance.
	/// </summary>
	public interface IInstrumentation : IMethodPerformanceTiming, IPropertyPerformanceTiming
	{
	}
}