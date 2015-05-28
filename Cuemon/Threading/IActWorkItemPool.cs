using System;
using Cuemon.Collections.Generic;

namespace Cuemon.Threading
{
	/// <summary>
	/// Defines a way to guide a thread pool to execute work items.
	/// </summary>
	public interface IActWorkItemPool
	{
		/// <summary>
		/// The work to be processed one thread at a time.
		/// </summary>
		/// <param name="work">The work item to execute one thread at a time.</param>
		void ProcessWork(IActWorkItem work);

        /// <summary>
        /// Gets a read-only collection of an <see cref="Exception"/> that caused one or more of the <see cref="IActWorkItem"/> to end prematurely. If all of the <see cref="IActWorkItem"/> completed successfully, this will be an empty read-only collection.
        /// </summary>
        IReadOnlyCollection<Exception> Exceptions { get; }
	}
}
