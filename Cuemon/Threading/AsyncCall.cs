using System;
using System.Threading;

namespace Cuemon.Threading
{
	/// <summary>
	/// Provides a way of encapsulating asynchronous operations using a synchronous programming model.
	/// </summary>
	/// <typeparam name="TState">The type of the return value of <see cref="AsyncCall{TState}.AsyncState"/>.</typeparam>
	/// <typeparam name="TResult">The type of the return value of <see cref="Result"/>.</typeparam>
	public sealed class AsyncCall<TState, TResult> : AsyncCall<TState>
	{
		private readonly Doer<IAsyncResult, TResult> _endMethod;
		private TResult _result;

		#region Constructors
		/// <summary>
		/// Initializes a new instance of the <see cref="AsyncCall&lt;TState, TResult&gt;"/> class.
		/// </summary>
		/// <param name="beginMethod">The delegate that begins the asynchronous operation.</param>
		/// <param name="endMethod">The delegate that ends the asynchronous operation.</param>
		/// <param name="state">An object containing data to be used by the <paramref name="beginMethod"/> delegate.</param>
		public AsyncCall(Doer<AsyncCallback, object, IAsyncResult> beginMethod, Doer<IAsyncResult, TResult> endMethod, TState state) : base(beginMethod, state)
		{
			if (beginMethod == null) { throw new ArgumentNullException("beginMethod"); }
			if (endMethod == null) { throw new ArgumentNullException("endMethod"); }
			_endMethod = endMethod;
		}
		#endregion

		#region Properties
		/// <summary>
		/// Gets the result.
		/// </summary>
		public TResult Result
		{
			get { return _result; }
			private set { _result = value; }
		}

		/// <summary>
		/// A reference to the delegate that ends the asynchronous operation.
		/// </summary>
		private new Doer<IAsyncResult, TResult> EndMethod
		{
			get { return _endMethod; }
		}
		#endregion

		#region Methods
		/// <summary>
		/// Creates and returns a <see cref="AsyncCallResult{TState,TResult}"/> representation of the current <see cref="AsyncCall{TState,TResult}"/>.
		/// </summary>
		/// <returns>A <see cref="AsyncCallResult{TState,TResult}"/> representation of the current <see cref="AsyncCall{TState,TResult}"/>.</returns>
		public AsyncCallResult<TState, TResult> ToAsyncCallResult()
		{
			AsyncCallResult<TState, TResult> result = new AsyncCallResult<TState, TResult>(this.AsyncState, this.Result);
			result.Exception = this.Exception;
			return result;
		}

		/// <summary>
		/// The method to be called when a corresponding asynchronous operation completes.
		/// </summary>
		/// <param name="result">The result of the asynchronous operation.</param>
		protected override void AsyncCallback(IAsyncResult result)
		{
			try
			{
				this.AsyncResult = result;
				this.Result = this.EndMethod(result);
			}
			catch (Exception ex)
			{
				this.Exception = ex;
			}
			finally
			{
                this.OperationComplete = true;
			}
		}
		#endregion
	}

	/// <summary>
	/// Provides a way of encapsulating asynchronous operations using a synchronous programming model.
	/// </summary>
	/// <typeparam name="TState">The type of the return value of <see cref="AsyncState"/>.</typeparam>
	public class AsyncCall<TState> : AsyncCall
	{
		private readonly TState _asyncState;

		#region Constructors
		/// <summary>
		/// Initializes a new instance of the <see cref="AsyncCall&lt;TState&gt;"/> class.
		/// </summary>
		/// <param name="beginMethod">The delegate that begins the asynchronous operation.</param>
		/// <param name="state">An object containing data to be used by the <paramref name="beginMethod"/> delegate.</param>
		protected AsyncCall(Doer<AsyncCallback, object, IAsyncResult> beginMethod, TState state)
			: base(beginMethod)
		{
			if (beginMethod == null) { throw new ArgumentNullException("beginMethod"); }
			_asyncState = state;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="AsyncCall&lt;TState&gt;"/> class.
		/// </summary>
		/// <param name="beginMethod">The delegate that begins the asynchronous operation.</param>
		/// <param name="endMethod">The delegate that ends the asynchronous operation.</param>
		/// <param name="state">An object containing data to be used by the <paramref name="beginMethod"/> delegate.</param>
		public AsyncCall(Doer<AsyncCallback, object, IAsyncResult> beginMethod, Act<IAsyncResult> endMethod, TState state)
			: base(beginMethod, endMethod)
		{
			if (beginMethod == null) { throw new ArgumentNullException("beginMethod"); }
			if (endMethod == null) { throw new ArgumentNullException("endMethod"); }
			_asyncState = state;
		}
		#endregion

		#region Methods
        /// <summary>
        /// Invokes the asynchronous operation.
        /// </summary>
        /// <param name="timeout">A <see cref="T:System.TimeSpan" /> that represents the time to wait for the asynchronous operation to complete.</param>
		public override void Invoke(TimeSpan timeout)
		{
			this.BeginMethod(AsyncCallback, this.AsyncState);
            Spinner.SpinUntil(OperationCompleteWrapper, timeout);
		}
		#endregion

		#region Properties
		/// <summary>
		/// Gets a user-defined <typeparamref name="TState"/> that qualifies or contains information about an asynchronous operation.
		/// </summary>
		/// <returns>A user-defined <typeparamref name="TState"/> that qualifies or contains information about an asynchronous operation.</returns>
		public new TState AsyncState
		{
			get { return _asyncState; }
		}
		#endregion
	}

	/// <summary>
	/// Provides a way of encapsulating asynchronous operations using a synchronous programming model.
	/// </summary>
	public class AsyncCall : IAsyncResult
	{
		private readonly Doer<AsyncCallback, object, IAsyncResult> _beginMethod;
		private readonly Act<IAsyncResult> _endMethod;
		private IAsyncResult _asyncResult;
		private Exception _exception;

		#region Constructors
		/// <summary>
		/// Initializes a new instance of the <see cref="AsyncCall"/> class.
		/// </summary>
		/// <param name="beginMethod">The delegate that begins the asynchronous operation.</param>
		protected AsyncCall(Doer<AsyncCallback, object, IAsyncResult> beginMethod)
		{
			if (beginMethod == null) { throw new ArgumentNullException("beginMethod"); }
			_beginMethod = beginMethod;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="AsyncCall"/> class.
		/// </summary>
		/// <param name="beginMethod">The delegate that begins the asynchronous operation.</param>
		/// <param name="endMethod">The delegate that ends the asynchronous operation.</param>
		public AsyncCall(Doer<AsyncCallback, object, IAsyncResult> beginMethod, Act<IAsyncResult> endMethod)
		{
			if (beginMethod == null) { throw new ArgumentNullException("beginMethod"); }
			if (endMethod == null) { throw new ArgumentNullException("endMethod"); }
			_beginMethod = beginMethod;
			_endMethod = endMethod;
		}
		#endregion

		#region Properties
		/// <summary>
		/// Gets the <see cref="Exception"/> that caused the <see cref="AsyncCall"/> to end prematurely. If the <see cref="AsyncCall"/> completed successfully or has not yet thrown any exceptions, this will return null.
		/// </summary>
		public Exception Exception
		{
			get { return _exception; }
			set { _exception = value; }
		}

		/// <summary>
		/// Gets or sets the asynchronous result from <see cref="AsyncCallback"/>.
		/// </summary>
		/// <value>
		/// The asynchronous result produced from <see cref="AsyncCallback"/>.
		/// </value>
		protected IAsyncResult AsyncResult
		{
			get { return _asyncResult; }
			set { _asyncResult = value; }
		}

		/// <summary>
		/// Gets a user-defined object that qualifies or contains information about an asynchronous operation.
		/// </summary>
		/// <returns>A user-defined object that qualifies or contains information about an asynchronous operation.</returns>
		public object AsyncState
		{
			get { return this.AsyncResult.AsyncState; }
		}

		/// <summary>
		/// Gets a <see cref="T:System.Threading.WaitHandle"/> that is used to wait for an asynchronous operation to complete.
		/// </summary>
		/// <returns>A <see cref="T:System.Threading.WaitHandle"/> that is used to wait for an asynchronous operation to complete.</returns>
		public WaitHandle AsyncWaitHandle
		{
			get { return this.AsyncResult.AsyncWaitHandle; }
		}

		/// <summary>
		/// Gets a value that indicates whether the asynchronous operation completed synchronously.
		/// </summary>
		/// <returns>true if the asynchronous operation completed synchronously; otherwise, false.</returns>
		public bool CompletedSynchronously
		{
			get { return this.AsyncResult.CompletedSynchronously; }
		}

		/// <summary>
		/// Gets a value that indicates whether the asynchronous operation has completed.
		/// </summary>
		/// <returns>true if the operation is complete; otherwise, false.
		///   </returns>
		public bool IsCompleted
		{
			get { return this.AsyncResult.IsCompleted; }
		}
		#endregion

		#region Methods
		/// <summary>
		/// A reference to the delegate that begins the asynchronous operation.
		/// </summary>
		protected Doer<AsyncCallback, object, IAsyncResult> BeginMethod
		{
			get { return _beginMethod; }
		}

		/// <summary>
		/// A reference to the delegate that ends the asynchronous operation.
		/// </summary>
		protected Act<IAsyncResult> EndMethod
		{
			get { return _endMethod; }
		}

		/// <summary>
		/// Invokes the asynchronous operation.
		/// </summary>
        public void Invoke()
		{
			this.Invoke(TimeSpan.FromSeconds(30));
		}

        /// <summary>
        /// Invokes the asynchronous operation.
        /// </summary>
        /// <param name="timeout">A <see cref="T:System.TimeSpan"/> that represents the time to wait for the asynchronous operation to complete.</param>
        public virtual void Invoke(TimeSpan timeout)
        {
            this.BeginMethod(this.AsyncCallback, null);
            Spinner.SpinUntil(OperationCompleteWrapper, timeout);
        }

	    /// <summary>
        /// Infrastructure. Returns the state of <see cref="OperationComplete"/>.
        /// </summary>
        /// <returns><c>true</c> if the asynchronous operation is completed; otherwise, <c>false</c>.</returns>
        protected bool OperationCompleteWrapper()
	    {
	        return this.OperationComplete;
	    }

		/// <summary>
		/// The method to be called when a corresponding asynchronous operation completes.
		/// </summary>
		/// <param name="result">The result of the asynchronous operation.</param>
        /// <remarks>This method signals the <see cref="OperationComplete"/> property and thereby resumes the current thread.</remarks>
		protected virtual void AsyncCallback(IAsyncResult result)
		{
			try
			{
				this.EndMethod(result);
				this.AsyncResult = result;
			}
			catch (Exception ex)
			{
				this.Exception = ex;
			}
			finally
			{
			    this.OperationComplete = true;
			}
		}

        /// <summary>
        /// Gets or sets a value indicating whether the asynchronous operation is completed.
        /// </summary>
        /// <value><c>true</c> if the asynchronous operation is completed; otherwise, <c>false</c>.</value>
        internal bool OperationComplete { get; set; }
		#endregion
	}
}