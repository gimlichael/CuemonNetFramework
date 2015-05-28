namespace Cuemon.Threading
{
    /// <summary>
    /// An abstract class providing a way for outsourcing thread intensive work to a <see cref="IDoerWorkItem{TResult}" /> implemented class.
    /// </summary>
    /// <typeparam name="TResult">The type of the result.</typeparam>
	public abstract class DoerWorkItem<TResult> : ActWorkItem, IDoerWorkItem<TResult>
	{
        private TResult _result;

		#region Constructors
		/// <summary>
        /// Initializes a new instance of the <see cref="DoerWorkItem{TResult}"/> class.
		/// </summary>
        /// <param name="synchronization">An instance of the <see cref="CountdownEvent"/> object used for threaded synchronization signaling.</param>
        protected DoerWorkItem(ISynchronization synchronization) : base(synchronization)
		{
		}
		#endregion

		#region Properties
		/// <summary>
        /// Gets the result of the work processed by <see cref="ActWorkItem.ProcessWork"/>.
		/// </summary>
		/// <value>
        /// The result of the work processed by <see cref="ActWorkItem.ProcessWork"/>.
		/// </value>
		public TResult Result
		{
			get { return _result; }
			protected set { _result = value; }
		}

        internal new DoerWorkItemPool<TResult> PoolReference  { get; set; }
		#endregion

		#region Methods
        #endregion
	}

    /// <summary>
    /// Provides static helper methods for function delegates based on <see cref="IDoerWorkItem{TResult}"/>.
    /// </summary>
    public static class DoerWorkItem
    {
        /// <summary>
        /// Creates and returns an <see cref="IDoerWorkItem{TResult}"/> implemented object that will invoke the specified <paramref name="method"/>.
        /// </summary>
        /// <param name="synchronization">An instance of an object implementing the <see cref="ISynchronization"/> interface used for threaded synchronization signaling.</param>
        /// <param name="method">The method to call from the associated <see cref="DoerWorkItemPool{TResult}"/>.</param>
        /// <returns>An <see cref="IDoerWorkItem{TResult}"/> implemented object that will invoke the specified <paramref name="method"/> from the associated <see cref="DoerWorkItemPool{TResult}"/>.</returns>
        public static IDoerWorkItem<TResult> Create<TResult>(ISynchronization synchronization, Doer<TResult> method)
        {
            return new DoerFactoryWorkItem<TResult>(synchronization, method);
        }

        /// <summary>
        /// Creates and returns an <see cref="IDoerWorkItem{TResult}"/> implemented object that will invoke the specified <paramref name="method"/>.
        /// </summary>
        /// <typeparam name="T">The type of the parameter of the method that this delegate encapsulates.</typeparam>
        /// <typeparam name="TResult">The type of the return value of the method that this delegate encapsulates.</typeparam>
        /// <param name="synchronization">An instance of an object implementing the <see cref="ISynchronization"/> interface used for threaded synchronization signaling.</param>
        /// <param name="method">The method to call from the associated <see cref="DoerWorkItemPool{TResult}"/>.</param>
        /// <param name="arg">The parameter of the method that this delegate encapsulates.</param>
        /// <returns>An <see cref="IDoerWorkItem{TResult}"/> implemented object that will invoke the specified <paramref name="method"/> from the associated <see cref="DoerWorkItemPool{TResult}"/>.</returns>
        public static IDoerWorkItem<TResult> Create<T, TResult>(ISynchronization synchronization, Doer<T, TResult> method, T arg)
        {
            return new DoerFactoryWorkItem<T, TResult>(synchronization, method, arg);
        }

        /// <summary>
        /// Creates and returns an <see cref="IDoerWorkItem{TResult}"/> implemented object that will invoke the specified <paramref name="method"/>.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter of the method that this delegate encapsulates.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the method that this delegate encapsulates.</typeparam>
        /// <typeparam name="TResult">The type of the return value of the method that this delegate encapsulates.</typeparam>
        /// <param name="synchronization">An instance of an object implementing the <see cref="ISynchronization"/> interface used for threaded synchronization signaling.</param>
        /// <param name="method">The method to call from the associated <see cref="DoerWorkItemPool{TResult}"/>.</param>
        /// <param name="arg1">The first parameter of the method that this delegate encapsulates.</param>
        /// <param name="arg2">The second parameter of the method that this delegate encapsulates.</param>
        /// <returns>An <see cref="IDoerWorkItem{TResult}"/> implemented object that will invoke the specified <paramref name="method"/> from the associated <see cref="DoerWorkItemPool{TResult}"/>.</returns>
        public static IDoerWorkItem<TResult> Create<T1, T2, TResult>(ISynchronization synchronization, Doer<T1, T2, TResult> method, T1 arg1, T2 arg2)
        {
            return new DoerFactoryWorkItem<T1, T2, TResult>(synchronization, method, arg1, arg2);
        }

        /// <summary>
        /// Creates and returns an <see cref="IDoerWorkItem{TResult}"/> implemented object that will invoke the specified <paramref name="method"/>.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter of the method that this delegate encapsulates.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the method that this delegate encapsulates.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the method that this delegate encapsulates.</typeparam>
        /// <typeparam name="TResult">The type of the return value of the method that this delegate encapsulates.</typeparam>
        /// <param name="synchronization">An instance of an object implementing the <see cref="ISynchronization"/> interface used for threaded synchronization signaling.</param>
        /// <param name="method">The method to call from the associated <see cref="DoerWorkItemPool{TResult}"/>.</param>
        /// <param name="arg1">The first parameter of the method that this delegate encapsulates.</param>
        /// <param name="arg2">The second parameter of the method that this delegate encapsulates.</param>
        /// <param name="arg3">The third parameter of the method that this delegate encapsulates.</param>
        /// <returns>An <see cref="IDoerWorkItem{TResult}"/> implemented object that will invoke the specified <paramref name="method"/> from the associated <see cref="DoerWorkItemPool{TResult}"/>.</returns>
        public static IDoerWorkItem<TResult> Create<T1, T2, T3, TResult>(ISynchronization synchronization, Doer<T1, T2, T3, TResult> method, T1 arg1, T2 arg2, T3 arg3)
        {
            return new DoerFactoryWorkItem<T1, T2, T3, TResult>(synchronization, method, arg1, arg2, arg3);
        }

        /// <summary>
        /// Creates and returns an <see cref="IDoerWorkItem{TResult}"/> implemented object that will invoke the specified <paramref name="method"/>.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter of the method that this delegate encapsulates.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the method that this delegate encapsulates.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the method that this delegate encapsulates.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of the method that this delegate encapsulates.</typeparam>
        /// <typeparam name="TResult">The type of the return value of the method that this delegate encapsulates.</typeparam>
        /// <param name="synchronization">An instance of an object implementing the <see cref="ISynchronization"/> interface used for threaded synchronization signaling.</param>
        /// <param name="method">The method to call from the associated <see cref="DoerWorkItemPool{TResult}"/>.</param>
        /// <param name="arg1">The first parameter of the method that this delegate encapsulates.</param>
        /// <param name="arg2">The second parameter of the method that this delegate encapsulates.</param>
        /// <param name="arg3">The third parameter of the method that this delegate encapsulates.</param>
        /// <param name="arg4">The fourth parameter of the method that this delegate encapsulates.</param>
        /// <returns>An <see cref="IDoerWorkItem{TResult}"/> implemented object that will invoke the specified <paramref name="method"/> from the associated <see cref="DoerWorkItemPool{TResult}"/>.</returns>
        public static IDoerWorkItem<TResult> Create<T1, T2, T3, T4, TResult>(ISynchronization synchronization, Doer<T1, T2, T3, T4, TResult> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            return new DoerFactoryWorkItem<T1, T2, T3, T4, TResult>(synchronization, method, arg1, arg2, arg3, arg4);
        }

        /// <summary>
        /// Creates and returns an <see cref="IDoerWorkItem{TResult}"/> implemented object that will invoke the specified <paramref name="method"/>.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter of the method that this delegate encapsulates.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the method that this delegate encapsulates.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the method that this delegate encapsulates.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of the method that this delegate encapsulates.</typeparam>
        /// <typeparam name="T5">The type of the fifth parameter of the method that this delegate encapsulates.</typeparam>
        /// <typeparam name="TResult">The type of the return value of the method that this delegate encapsulates.</typeparam>
        /// <param name="synchronization">An instance of an object implementing the <see cref="ISynchronization"/> interface used for threaded synchronization signaling.</param>
        /// <param name="method">The method to call from the associated <see cref="DoerWorkItemPool{TResult}"/>.</param>
        /// <param name="arg1">The first parameter of the method that this delegate encapsulates.</param>
        /// <param name="arg2">The second parameter of the method that this delegate encapsulates.</param>
        /// <param name="arg3">The third parameter of the method that this delegate encapsulates.</param>
        /// <param name="arg4">The fourth parameter of the method that this delegate encapsulates.</param>
        /// <param name="arg5">The fifth parameter of the method that this delegate encapsulates.</param>
        /// <returns>An <see cref="IDoerWorkItem{TResult}"/> implemented object that will invoke the specified <paramref name="method"/> from the associated <see cref="DoerWorkItemPool{TResult}"/>.</returns>
        public static IDoerWorkItem<TResult> Create<T1, T2, T3, T4, T5, TResult>(ISynchronization synchronization, Doer<T1, T2, T3, T4, T5, TResult> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        {
            return new DoerFactoryWorkItem<T1, T2, T3, T4, T5, TResult>(synchronization, method, arg1, arg2, arg3, arg4, arg5);
        }
    }
}