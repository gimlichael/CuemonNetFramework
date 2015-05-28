using System;
using System.Collections.Generic;

namespace Cuemon.Threading
{
	/// <summary>
	/// An abstract class providing a way for outsourcing thread intensive work to a <see cref="IActWorkItemPool"/> implemented class.
	/// </summary>
	public abstract class ActWorkItem : IActWorkItem
	{
		private readonly Dictionary<string, object> _data;
        private readonly ISynchronization _synchronization;
	    private Exception _exception = null;

		#region Constructors
		/// <summary>
		/// Initializes a new instance of the <see cref="ActWorkItem"/> class.
		/// </summary>
        /// <param name="synchronization">An instance of an object implementing the <see cref="ISynchronization"/> interface used for threaded synchronization signaling.</param>
        protected ActWorkItem(ISynchronization synchronization)
		{
            _synchronization = synchronization;
			_data = new Dictionary<string, object>();
		}
		#endregion

		#region Properties
        /// <summary>
        /// Gets the <see cref="Exception" /> that caused the <see cref="IActWorkItem" /> to end prematurely. If the <see cref="IActWorkItem" /> completed successfully, this will return null.
        /// </summary>
        /// <value>The <see cref="Exception" /> that caused the <see cref="IActWorkItem" /> to end prematurely.</value>
	    public Exception Exception
	    {
	        get { return _exception; }
            set { _exception = value; }
	    }

		/// <summary>
		/// Gets a collection of key/value pairs that provide additional user-defined information about this <see cref="ActWorkItem"/>.
		/// </summary>
		public IDictionary<string, object> Data
		{
			get { return _data; }
		}

        /// <summary>
        /// Gets or sets the reference to the <see cref="IActWorkItemPool"/> owner.
        /// </summary>
        /// <value>The reference to the <see cref="IActWorkItemPool"/> owner..</value>
        internal ActWorkItemPool PoolReference { get; set; }

		/// <summary>
        /// Gets an instance of the object implementing the <see cref="ISynchronization"/> interface used for threaded synchronization signaling.
		/// </summary>
        public ISynchronization Synchronization
		{
			get { return _synchronization; }
		}
		#endregion

		#region Methods
		/// <summary>
		/// The work to be processed by a pool of threads.
		/// </summary>
		public abstract void ProcessWork();

        /// <summary>
        /// Creates and returns an <see cref="IActWorkItem"/> implemented object that will invoke the specified <paramref name="method"/>.
        /// </summary>
        /// <param name="synchronization">An instance of an object implementing the <see cref="ISynchronization"/> interface used for threaded synchronization signaling.</param>
        /// <param name="method">The method to call from the associated <see cref="ActWorkItemPool"/>.</param>
        /// <returns>An <see cref="IActWorkItem"/> implemented object that will invoke the specified <paramref name="method"/> from the associated <see cref="ActWorkItemPool"/>.</returns>
        public static IActWorkItem Create(ISynchronization synchronization, Act method)
        {
            return new ActFactoryWorkItem(synchronization, method);
        }

        /// <summary>
        /// Creates and returns an <see cref="IActWorkItem"/> implemented object that will invoke the specified <paramref name="method"/>.
        /// </summary>
        /// <typeparam name="T">The type of the parameter of the method.</typeparam>
        /// <param name="synchronization">An instance of an object implementing the <see cref="ISynchronization"/> interface used for threaded synchronization signaling.</param>
        /// <param name="method">The method to call from the associated <see cref="ActWorkItemPool"/>.</param>
        /// <param name="arg">The parameter of the method that this delegate encapsulates.</param>
        /// <returns>An <see cref="IActWorkItem"/> implemented object that will invoke the specified <paramref name="method"/> from the associated <see cref="ActWorkItemPool"/>.</returns>
        public static IActWorkItem Create<T>(ISynchronization synchronization, Act<T> method, T arg)
        {
            return new ActFactoryWorkItem<T>(synchronization, method, arg);
        }

        /// <summary>
        /// Creates and returns an <see cref="IActWorkItem"/> implemented object that will invoke the specified <paramref name="method"/>.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter of the method.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the method.</typeparam>
        /// <param name="synchronization">An instance of an object implementing the <see cref="ISynchronization"/> interface used for threaded synchronization signaling.</param>
        /// <param name="method">The method to call from the associated <see cref="ActWorkItemPool"/>.</param>
        /// <param name="arg1">The first parameter of the method that this delegate encapsulates.</param>
        /// <param name="arg2">The second parameter of the method that this delegate encapsulates.</param>
        /// <returns>An <see cref="IActWorkItem"/> implemented object that will invoke the specified <paramref name="method"/> from the associated <see cref="ActWorkItemPool"/>.</returns>
        public static IActWorkItem Create<T1, T2>(ISynchronization synchronization, Act<T1, T2> method, T1 arg1, T2 arg2)
        {
            return new ActFactoryWorkItem<T1, T2>(synchronization, method, arg1, arg2);
        }

        /// <summary>
        /// Creates and returns an <see cref="IActWorkItem"/> implemented object that will invoke the specified <paramref name="method"/>.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter of the method.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the method.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the method.</typeparam>
        /// <param name="synchronization">An instance of an object implementing the <see cref="ISynchronization"/> interface used for threaded synchronization signaling.</param>
        /// <param name="method">The method to call from the associated <see cref="ActWorkItemPool"/>.</param>
        /// <param name="arg1">The first parameter of the method that this delegate encapsulates.</param>
        /// <param name="arg2">The second parameter of the method that this delegate encapsulates.</param>
        /// <param name="arg3">The third parameter of the method that this delegate encapsulates.</param>
        /// <returns>An <see cref="IActWorkItem"/> implemented object that will invoke the specified <paramref name="method"/> from the associated <see cref="ActWorkItemPool"/>.</returns>
        public static IActWorkItem Create<T1, T2, T3>(ISynchronization synchronization, Act<T1, T2, T3> method, T1 arg1, T2 arg2, T3 arg3)
        {
            return new ActFactoryWorkItem<T1, T2, T3>(synchronization, method, arg1, arg2, arg3);
        }

        /// <summary>
        /// Creates and returns an <see cref="IActWorkItem"/> implemented object that will invoke the specified <paramref name="method"/>.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter of the method.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the method.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the method.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of the method.</typeparam>
        /// <param name="synchronization">An instance of an object implementing the <see cref="ISynchronization"/> interface used for threaded synchronization signaling.</param>
        /// <param name="method">The method to call from the associated <see cref="ActWorkItemPool"/>.</param>
        /// <param name="arg1">The first parameter of the method that this delegate encapsulates.</param>
        /// <param name="arg2">The second parameter of the method that this delegate encapsulates.</param>
        /// <param name="arg3">The third parameter of the method that this delegate encapsulates.</param>
        /// <param name="arg4">The fourth parameter of the method that this delegate encapsulates.</param>
        /// <returns>An <see cref="IActWorkItem"/> implemented object that will invoke the specified <paramref name="method"/> from the associated <see cref="ActWorkItemPool"/>.</returns>
        public static IActWorkItem Create<T1, T2, T3, T4>(ISynchronization synchronization, Act<T1, T2, T3, T4> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            return new ActFactoryWorkItem<T1, T2, T3, T4>(synchronization, method, arg1, arg2, arg3, arg4);
        }

        /// <summary>
        /// Creates and returns an <see cref="IActWorkItem"/> implemented object that will invoke the specified <paramref name="method"/>.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter of the method.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the method.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the method.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of the method.</typeparam>
        /// <typeparam name="T5">The type of the fifth parameter of the method.</typeparam>
        /// <param name="synchronization">An instance of an object implementing the <see cref="ISynchronization"/> interface used for threaded synchronization signaling.</param>
        /// <param name="method">The method to call from the associated <see cref="ActWorkItemPool"/>.</param>
        /// <param name="arg1">The first parameter of the method that this delegate encapsulates.</param>
        /// <param name="arg2">The second parameter of the method that this delegate encapsulates.</param>
        /// <param name="arg3">The third parameter of the method that this delegate encapsulates.</param>
        /// <param name="arg4">The fourth parameter of the method that this delegate encapsulates.</param>
        /// <param name="arg5">The fifth parameter of the method that this delegate encapsulates.</param>
        /// <returns>An <see cref="IActWorkItem"/> implemented object that will invoke the specified <paramref name="method"/> from the associated <see cref="ActWorkItemPool"/>.</returns>
        public static IActWorkItem Create<T1, T2, T3, T4, T5>(ISynchronization synchronization, Act<T1, T2, T3, T4, T5> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        {
            return new ActFactoryWorkItem<T1, T2, T3, T4, T5>(synchronization, method, arg1, arg2, arg3, arg4, arg5);
        }
		#endregion
	}
}