namespace Cuemon.Threading
{
	/// <summary>
    /// Defines a way to control work operations that returns a value on a per thread basis.
	/// </summary>
	public interface IDoerWorkItem<out TResult> : IActWorkItem
	{
		/// <summary>
        /// Gets the result of the work processed by <see cref="IActWorkItem.ProcessWork"/>.
		/// </summary>
		/// <value>
        /// The result of the work processed by <see cref="IActWorkItem.ProcessWork"/>.
		/// </value>
		TResult Result { get; }
	}
}