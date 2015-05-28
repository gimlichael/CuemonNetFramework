namespace Cuemon.Threading
{
	/// <summary>
    /// Defines a way to control work operations that returns a value on a per thread basis where the sorted result applies to <typeparamref name="TKey"/>.
	/// </summary>
	public interface ISortedDoerWorkItem<out TKey, out TResult> : IDoerWorkItem<TResult>
	{
		/// <summary>
		/// Gets the <typeparamref name="TKey"/> that represents the sort order value.
		/// </summary>
		TKey SortOrder { get; }
	}
}