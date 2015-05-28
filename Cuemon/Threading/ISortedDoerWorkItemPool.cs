namespace Cuemon.Threading
{
	/// <summary>
	/// Defines a way to guide a thread pool to execute work items.
	/// </summary>
	public interface ISortedDoerWorkItemPool<in TKey, TResult> : IDoerWorkItemPool<TResult>
	{
		/// <summary>
		/// The work to be processed one thread at a time.
		/// </summary>
		/// <param name="work">The work item to execute one thread at a time.</param>
		void ProcessWork(ISortedDoerWorkItem<TKey, TResult> work);
	}
}
