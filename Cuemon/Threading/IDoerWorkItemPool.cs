using Cuemon.Collections.Generic;

namespace Cuemon.Threading
{
	/// <summary>
	/// Defines a way to guide a thread pool to execute work items.
	/// </summary>
	public interface IDoerWorkItemPool<TResult> : IActWorkItemPool
	{
		/// <summary>
		/// Gets the result of the work items processed by <see cref="ProcessWork"/>.
		/// </summary>
		/// <value>
        /// The result of the work items processed by <see cref="ProcessWork"/>.
		/// </value>
		IReadOnlyCollection<TResult> Result { get; }


		/// <summary>
		/// The work to be processed one thread at a time.
		/// </summary>
		/// <param name="work">The work item to execute one thread at a time.</param>
		void ProcessWork(IDoerWorkItem<TResult> work);
	}
}
