namespace Cuemon.Threading
{
    /// <summary>
    /// An abstract class providing a way for outsourcing thread intensive work to a <see cref="ISortedDoerWorkItemPool{TKey,TResult}" /> implemented class.
    /// </summary>
    /// <typeparam name="TKey">The type of the key used to determine the sort order.</typeparam>
    /// <typeparam name="TResult">The type of the result.</typeparam>
	public abstract class SortedDoerWorkItem<TKey, TResult> : DoerWorkItem<TResult>, ISortedDoerWorkItem<TKey, TResult>
	{
		private readonly TKey _sortOrder;

		#region Constructors
		/// <summary>
        /// Initializes a new instance of the <see cref="SortedDoerWorkItem{TKey,TResult}"/> class.
		/// </summary>
		/// <param name="sortOrder">The object that represents the sort order value.</param>
        /// <param name="synchronization">An instance of an object implementing the <see cref="ISynchronization"/> interface used for threaded synchronization signaling.</param>
        protected SortedDoerWorkItem(TKey sortOrder, ISynchronization synchronization) : base(synchronization)
		{
			_sortOrder = sortOrder;
		}
		#endregion

		#region Properties
        /// <summary>
        /// Gets the object that represents the sort order value.
        /// </summary>
        /// <value>A <typeparamref name="TKey"/> that is the sort order value.</value>
		public TKey SortOrder
		{
			get { return _sortOrder; }
		}

        internal new SortedDoerWorkItemPool<TKey, TResult> PoolReference { get; set; }
		#endregion

		#region Methods
		#endregion
	}

    /// <summary>
    /// Provides static helper methods for function delegates based on <see cref="ISortedDoerWorkItem{TKey,TResult}"/>.
    /// </summary>
    public static class SortedDoerWorkItem
    {
        /// <summary>
        /// Creates and returns an <see cref="ISortedDoerWorkItem{TKey,TResult}"/> implemented object that will invoke the specified <paramref name="method"/>.
        /// </summary>
        /// <typeparam name="TKey">The type of the key used to determine the sort order.</typeparam>
        /// <typeparam name="TResult">The type of the return value of the method that this delegate encapsulates.</typeparam>
        /// <param name="sortOrder">The object that represents the sort order value.</param>
        /// <param name="synchronization">An instance of an object implementing the <see cref="ISynchronization"/> interface used for threaded synchronization signaling.</param>
        /// <param name="method">The method to call from the associated <see cref="SortedDoerWorkItemPool{TKey, TResult}"/>.</param>
        /// <returns>An <see cref="ISortedDoerWorkItem{TKey,TResult}"/> implemented object that will invoke the specified <paramref name="method"/> from the associated <see cref="SortedDoerWorkItemPool{TKey, TResult}"/>.</returns>
        public static ISortedDoerWorkItem<TKey, TResult> Create<TKey, TResult>(TKey sortOrder, ISynchronization synchronization, Doer<TResult> method)
        {
            return new DoerFactorySortedWorkItem<TKey, TResult>(sortOrder, synchronization, method);
        }

        /// <summary>
        /// Creates and returns an <see cref="ISortedDoerWorkItem{TKey,TResult}"/> implemented object that will invoke the specified <paramref name="method"/>.
        /// </summary>
        /// <typeparam name="T">The type of the parameter of the method that this delegate encapsulates.</typeparam>
        /// <typeparam name="TResult">The type of the return value of the method that this delegate encapsulates.</typeparam>
        /// <typeparam name="TKey">The type of the key used to determine the sort order.</typeparam>
        /// <param name="sortOrder">The object that represents the sort order value.</param>
        /// <param name="synchronization">An instance of an object implementing the <see cref="ISynchronization"/> interface used for threaded synchronization signaling.</param>
        /// <param name="method">The method to call from the associated <see cref="SortedDoerWorkItemPool{TKey, TResult}"/>.</param>
        /// <param name="arg">The parameter of the method that this delegate encapsulates.</param>
        /// <returns>An <see cref="ISortedDoerWorkItem{TKey,TResult}"/> implemented object that will invoke the specified <paramref name="method"/> from the associated <see cref="SortedDoerWorkItemPool{TKey, TResult}"/>.</returns>
        public static ISortedDoerWorkItem<TKey, TResult> Create<TKey, T, TResult>(TKey sortOrder, ISynchronization synchronization, Doer<T, TResult> method, T arg)
        {
            return new DoerFactorySortedWorkItem<TKey, T, TResult>(sortOrder, synchronization, method, arg);
        }

        /// <summary>
        /// Creates and returns an <see cref="ISortedDoerWorkItem{TKey,TResult}"/> implemented object that will invoke the specified <paramref name="method"/>.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter of the method that this delegate encapsulates.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the method that this delegate encapsulates.</typeparam>
        /// <typeparam name="TResult">The type of the return value of the method that this delegate encapsulates.</typeparam>
        /// <typeparam name="TKey">The type of the key used to determine the sort order.</typeparam>
        /// <param name="sortOrder">The object that represents the sort order value.</param>
        /// <param name="synchronization">An instance of an object implementing the <see cref="ISynchronization"/> interface used for threaded synchronization signaling.</param>
        /// <param name="method">The method to call from the associated <see cref="SortedDoerWorkItemPool{TKey, TResult}"/>.</param>
        /// <param name="arg1">The first parameter of the method that this delegate encapsulates.</param>
        /// <param name="arg2">The second parameter of the method that this delegate encapsulates.</param>
        /// <returns>An <see cref="ISortedDoerWorkItem{TKey,TResult}"/> implemented object that will invoke the specified <paramref name="method"/> from the associated <see cref="SortedDoerWorkItemPool{TKey, TResult}"/>.</returns>
        public static ISortedDoerWorkItem<TKey, TResult> Create<TKey, T1, T2, TResult>(TKey sortOrder, ISynchronization synchronization, Doer<T1, T2, TResult> method, T1 arg1, T2 arg2)
        {
            return new DoerFactorySortedWorkItem<TKey, T1, T2, TResult>(sortOrder, synchronization, method, arg1, arg2);
        }

        /// <summary>
        /// Creates and returns an <see cref="ISortedDoerWorkItem{TKey,TResult}"/> implemented object that will invoke the specified <paramref name="method"/>.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter of the method that this delegate encapsulates.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the method that this delegate encapsulates.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the method that this delegate encapsulates.</typeparam>
        /// <typeparam name="TResult">The type of the return value of the method that this delegate encapsulates.</typeparam>
        /// <typeparam name="TKey">The type of the key used to determine the sort order.</typeparam>
        /// <param name="sortOrder">The object that represents the sort order value.</param>
        /// <param name="synchronization">An instance of an object implementing the <see cref="ISynchronization"/> interface used for threaded synchronization signaling.</param>
        /// <param name="method">The method to call from the associated <see cref="SortedDoerWorkItemPool{TKey, TResult}"/>.</param>
        /// <param name="arg1">The first parameter of the method that this delegate encapsulates.</param>
        /// <param name="arg2">The second parameter of the method that this delegate encapsulates.</param>
        /// <param name="arg3">The third parameter of the method that this delegate encapsulates.</param>
        /// <returns>An <see cref="ISortedDoerWorkItem{TKey,TResult}"/> implemented object that will invoke the specified <paramref name="method"/> from the associated <see cref="SortedDoerWorkItemPool{TKey, TResult}"/>.</returns>
        public static ISortedDoerWorkItem<TKey, TResult> Create<TKey, T1, T2, T3, TResult>(TKey sortOrder, ISynchronization synchronization, Doer<T1, T2, T3, TResult> method, T1 arg1, T2 arg2, T3 arg3)
        {
            return new DoerFactorySortedWorkItem<TKey, T1, T2, T3, TResult>(sortOrder, synchronization, method, arg1, arg2, arg3);
        }

        /// <summary>
        /// Creates and returns an <see cref="ISortedDoerWorkItem{TKey,TResult}"/> implemented object that will invoke the specified <paramref name="method"/>.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter of the method that this delegate encapsulates.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the method that this delegate encapsulates.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the method that this delegate encapsulates.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of the method that this delegate encapsulates.</typeparam>
        /// <typeparam name="TResult">The type of the return value of the method that this delegate encapsulates.</typeparam>
        /// <typeparam name="TKey">The type of the key used to determine the sort order.</typeparam>
        /// <param name="sortOrder">The object that represents the sort order value.</param>
        /// <param name="synchronization">An instance of an object implementing the <see cref="ISynchronization"/> interface used for threaded synchronization signaling.</param>
        /// <param name="method">The method to call from the associated <see cref="SortedDoerWorkItemPool{TKey, TResult}"/>.</param>
        /// <param name="arg1">The first parameter of the method that this delegate encapsulates.</param>
        /// <param name="arg2">The second parameter of the method that this delegate encapsulates.</param>
        /// <param name="arg3">The third parameter of the method that this delegate encapsulates.</param>
        /// <param name="arg4">The fourth parameter of the method that this delegate encapsulates.</param>
        /// <returns>An <see cref="ISortedDoerWorkItem{TKey,TResult}"/> implemented object that will invoke the specified <paramref name="method"/> from the associated <see cref="SortedDoerWorkItemPool{TKey, TResult}"/>.</returns>
        public static ISortedDoerWorkItem<TKey, TResult> Create<TKey, T1, T2, T3, T4, TResult>(TKey sortOrder, ISynchronization synchronization, Doer<T1, T2, T3, T4, TResult> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            return new DoerFactorySortedWorkItem<TKey, T1, T2, T3, T4, TResult>(sortOrder, synchronization, method, arg1, arg2, arg3, arg4);
        }

        /// <summary>
        /// Creates and returns an <see cref="ISortedDoerWorkItem{TKey,TResult}"/> implemented object that will invoke the specified <paramref name="method"/>.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter of the method that this delegate encapsulates.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the method that this delegate encapsulates.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the method that this delegate encapsulates.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of the method that this delegate encapsulates.</typeparam>
        /// <typeparam name="T5">The type of the fifth parameter of the method that this delegate encapsulates.</typeparam>
        /// <typeparam name="TResult">The type of the return value of the method that this delegate encapsulates.</typeparam>
        /// <typeparam name="TKey">The type of the key used to determine the sort order.</typeparam>
        /// <param name="sortOrder">The object that represents the sort order value.</param>
        /// <param name="synchronization">An instance of an object implementing the <see cref="ISynchronization"/> interface used for threaded synchronization signaling.</param>
        /// <param name="method">The method to call from the associated <see cref="SortedDoerWorkItemPool{TKey, TResult}"/>.</param>
        /// <param name="arg1">The first parameter of the method that this delegate encapsulates.</param>
        /// <param name="arg2">The second parameter of the method that this delegate encapsulates.</param>
        /// <param name="arg3">The third parameter of the method that this delegate encapsulates.</param>
        /// <param name="arg4">The fourth parameter of the method that this delegate encapsulates.</param>
        /// <param name="arg5">The fifth parameter of the method that this delegate encapsulates.</param>
        /// <returns>An <see cref="ISortedDoerWorkItem{TKey,TResult}"/> implemented object that will invoke the specified <paramref name="method"/> from the associated <see cref="SortedDoerWorkItemPool{TKey, TResult}"/>.</returns>
        public static ISortedDoerWorkItem<TKey, TResult> Create<TKey, T1, T2, T3, T4, T5, TResult>(TKey sortOrder, ISynchronization synchronization, Doer<T1, T2, T3, T4, T5, TResult> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        {
            return new DoerFactorySortedWorkItem<TKey, T1, T2, T3, T4, T5, TResult>(sortOrder, synchronization, method, arg1, arg2, arg3, arg4, arg5);
        }
    }
}