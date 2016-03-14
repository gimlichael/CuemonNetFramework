using System;
using System.Collections.Generic;
using System.Threading;

namespace Cuemon.Threading
{
    /// <summary>
    /// Provides a way of encapsulating asynchronous operations using a synchronous programming model.
    /// </summary>
    /// <typeparam name="TState">The type of the return value of <see cref="AsyncCall{TState}.AsyncState"/>.</typeparam>
    /// <typeparam name="TResult">The type of the return value of <see cref="Result"/>.</typeparam>
    public class AsyncCall<TState, TResult> : AsyncCall<TState>
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="AsyncCall&lt;TState, TResult&gt;"/> class.
        /// </summary>
        /// <param name="beginMethod">The delegate that begins the asynchronous operation.</param>
        /// <param name="endMethod">The delegate that ends the asynchronous operation.</param>
        /// <param name="state">An object containing data to be used by the <paramref name="beginMethod"/> delegate.</param>
        public AsyncCall(Doer<AsyncCallback, object, IAsyncResult> beginMethod, Doer<IAsyncResult, TResult> endMethod, TState state) : base(beginMethod, state)
        {
            if (beginMethod == null) { throw new ArgumentNullException(nameof(beginMethod)); }
            if (endMethod == null) { throw new ArgumentNullException(nameof(endMethod)); }
            EndMethod = endMethod;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the result.
        /// </summary>
        public TResult Result { get; protected set; }

        /// <summary>
        /// A reference to the delegate that ends the asynchronous operation.
        /// </summary>
        protected new Doer<IAsyncResult, TResult> EndMethod { get; }

        #endregion

        #region Methods

        /// <summary>
        /// Waits for the <see cref="ThreadPoolTask" /> to complete execution within a specified time interval.
        /// </summary>
        /// <param name="timeout">A <see cref="T:System.TimeSpan" /> that represents the time to wait.</param>
        public override void Wait(TimeSpan timeout)
        {
            Validator.ThrowIfNull(timeout, nameof(timeout));
            Validator.ThrowIfLowerThanOrEqual(timeout.TotalMilliseconds, -1, nameof(timeout));
            Validator.ThrowIfGreaterThan(timeout.TotalMilliseconds, int.MaxValue, nameof(timeout));
            if (AsyncResult == null) { throw new InvalidOperationException("AsyncResult was not set."); }
            if (AsyncResult.AsyncWaitHandle.WaitOne(timeout, false))
            {
                lock (PadLock)
                {
                    if (OperationComplete) { return; }
                    try
                    {
                        Result = EndMethod(AsyncResult);
                    }
                    catch (Exception ex)
                    {
                        lock (AggregatedExceptions) { AggregatedExceptions.Add(ex); }
                    }
                    finally
                    {
                        if (AggregatedExceptions.Count > 0) { Exception = ExceptionUtility.Refine(new ThreadException(AggregatedExceptions), BeginMethod.Method, AsyncResult); }
                        SetOperationCompleted();
                    }
                }
            }
            else
            {
                throw new TimeoutException("The time to complete the asynchronous operation took longer than allowed.");
            }
        }

        /// <summary>
        /// Creates and returns a <see cref="AsyncCallResult{TState,TResult}"/> representation of the current <see cref="AsyncCall{TState,TResult}"/>.
        /// </summary>
        /// <returns>A <see cref="AsyncCallResult{TState,TResult}"/> representation of the current <see cref="AsyncCall{TState,TResult}"/>.</returns>
        public AsyncCallResult<TState, TResult> ToAsyncCallResult()
        {
            AsyncCallResult<TState, TResult> result = new AsyncCallResult<TState, TResult>(AsyncState, Result);
            result.Exception = Exception;
            return result;
        }

        /// <summary>
        /// The method to be called when a corresponding asynchronous operation completes.
        /// </summary>
        /// <param name="result">The result of the asynchronous operation.</param>
        protected override void AsyncCallback(IAsyncResult result)
        {
            lock (PadLock)
            {
                if (OperationComplete) { return; }
                try
                {
                    Result = EndMethod(result);
                }
                catch (Exception ex)
                {
                    AggregatedExceptions.Add(ex);
                }
                finally
                {
                    if (AggregatedExceptions.Count > 0) { Exception = ExceptionUtility.Refine(new ThreadException(AggregatedExceptions), BeginMethod.Method, result); }
                    SetOperationCompleted();
                }
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
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="AsyncCall&lt;TState&gt;"/> class.
        /// </summary>
        /// <param name="beginMethod">The delegate that begins the asynchronous operation.</param>
        /// <param name="state">An object containing data to be used by the <paramref name="beginMethod"/> delegate.</param>
        protected AsyncCall(Doer<AsyncCallback, object, IAsyncResult> beginMethod, TState state)
            : base(beginMethod)
        {
            if (beginMethod == null) { throw new ArgumentNullException(nameof(beginMethod)); }
            AsyncState = state;
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
            if (beginMethod == null) { throw new ArgumentNullException(nameof(beginMethod)); }
            if (endMethod == null) { throw new ArgumentNullException(nameof(endMethod)); }
            AsyncState = state;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Invokes the asynchronous operation.
        /// </summary>
        public override void Invoke()
        {
            AsyncResult = BeginMethod(AsyncCallback, AsyncState);
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets a user-defined <typeparamref name="TState"/> that qualifies or contains information about an asynchronous operation.
        /// </summary>
        /// <returns>A user-defined <typeparamref name="TState"/> that qualifies or contains information about an asynchronous operation.</returns>
        public new TState AsyncState { get; internal set; }
        #endregion
    }

    /// <summary>
    /// Provides a way of encapsulating asynchronous operations using a synchronous programming model.
    /// </summary>
    public class AsyncCall : IAsyncResult
    {
        /// <summary>
        /// A syncronization lock.
        /// </summary>
        protected readonly object PadLock = new object();
        private readonly DateTime _utcCreated = DateTime.UtcNow;

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="AsyncCall"/> class.
        /// </summary>
        /// <param name="beginMethod">The delegate that begins the asynchronous operation.</param>
        protected AsyncCall(Doer<AsyncCallback, object, IAsyncResult> beginMethod)
        {
            if (beginMethod == null) { throw new ArgumentNullException(nameof(beginMethod)); }
            BeginMethod = beginMethod;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AsyncCall"/> class.
        /// </summary>
        /// <param name="beginMethod">The delegate that begins the asynchronous operation.</param>
        /// <param name="endMethod">The delegate that ends the asynchronous operation.</param>
        public AsyncCall(Doer<AsyncCallback, object, IAsyncResult> beginMethod, Act<IAsyncResult> endMethod)
        {
            if (beginMethod == null) { throw new ArgumentNullException(nameof(beginMethod)); }
            if (endMethod == null) { throw new ArgumentNullException(nameof(endMethod)); }
            BeginMethod = beginMethod;
            EndMethod = endMethod;
        }
        #endregion

        #region Properties

        /// <summary>
        /// The aggregated exceptions thrown by the asynchronous operation.
        /// </summary>
        /// <value>The aggregated exceptions thrown by the asynchronous operation.</value>
        protected IList<Exception> AggregatedExceptions { get; } = new List<Exception>();

        /// <summary>
        /// Gets the <see cref="Exception"/> that caused the <see cref="AsyncCall"/> to end prematurely. If the <see cref="AsyncCall"/> completed successfully or has not yet thrown any exceptions, this will return null.
        /// </summary>
        /// <value>The <see cref="Exception"/> that caused the <see cref="AsyncCall"/> to end prematurely.</value>
        public ThreadException Exception { get; set; }

        /// <summary>
        /// Gets or sets the asynchronous result from <see cref="AsyncCallback"/>.
        /// </summary>
        /// <value>
        /// The asynchronous result produced from <see cref="AsyncCallback"/>.
        /// </value>
        protected IAsyncResult AsyncResult { get; set; }

        /// <summary>
        /// Gets a user-defined object that qualifies or contains information about an asynchronous operation.
        /// </summary>
        /// <returns>A user-defined object that qualifies or contains information about an asynchronous operation.</returns>
        public object AsyncState => AsyncResult.AsyncState;

        /// <summary>
        /// Gets a <see cref="T:System.Threading.WaitHandle"/> that is used to wait for an asynchronous operation to complete.
        /// </summary>
        /// <returns>A <see cref="T:System.Threading.WaitHandle"/> that is used to wait for an asynchronous operation to complete.</returns>
        public WaitHandle AsyncWaitHandle => AsyncResult.AsyncWaitHandle;

        /// <summary>
        /// Gets a value that indicates whether the asynchronous operation completed synchronously.
        /// </summary>
        /// <returns>true if the asynchronous operation completed synchronously; otherwise, false.</returns>
        public bool CompletedSynchronously => AsyncResult.CompletedSynchronously;

        /// <summary>
        /// Gets a value that indicates whether the asynchronous operation has completed.
        /// </summary>
        /// <returns><c>true</c> if the operation is complete; otherwise, <c>false</c>.</returns>
        public bool IsCompleted => AsyncResult.IsCompleted;

        /// <summary>
        /// Gets whether the asynchronous operation completed due to an unhandled exception.
        /// </summary>
        /// <value><c>true</c> if the asynchronous operation has thrown an unhandled exception; otherwise, <c>false</c>.</value>
        public bool IsFaulted => (Exception != null);

        /// <summary>
        /// Gets or sets a value indicating whether the asynchronous operation is completed.
        /// </summary>
        /// <value><c>true</c> if the asynchronous operation is completed; otherwise, <c>false</c>.</value>
        internal bool OperationComplete { get; private set; }

        /// <summary>
        /// Marks this asynchronous operation as completed.
        /// </summary>
        protected void SetOperationCompleted()
        {
            OperationComplete = true;
            Elapsed = DateTime.UtcNow - _utcCreated;
        }

        /// <summary>
        /// Gets the elapsed execution time of the asynchronous operation.
        /// </summary>
        /// <value>The elapsed execution time of the asynchronous operation.</value>
        public TimeSpan Elapsed { get; private set; }
        #endregion

        #region Methods
        /// <summary>
        /// Waits for the <see cref="ThreadPoolTask"/> to complete execution.
        /// </summary>
        public void Wait()
        {
            Wait(TimeSpan.FromMilliseconds(int.MaxValue));
        }

        /// <summary>
        /// Waits for the <see cref="ThreadPoolTask"/> to complete execution within a specified time interval.
        /// </summary>
        /// <param name="timeout">A <see cref="T:System.TimeSpan"/> that represents the time to wait.</param>
        public virtual void Wait(TimeSpan timeout)
        {
            Validator.ThrowIfNull(timeout, nameof(timeout));
            Validator.ThrowIfLowerThanOrEqual(timeout.TotalMilliseconds, -1, nameof(timeout));
            Validator.ThrowIfGreaterThan(timeout.TotalMilliseconds, int.MaxValue, nameof(timeout));
            if (AsyncResult == null) { throw new InvalidOperationException("AsyncResult was not set."); }
            if (AsyncResult.AsyncWaitHandle.WaitOne(timeout, false))
            {
                lock (PadLock)
                {
                    if (OperationComplete) { return; }
                    try
                    {
                        EndMethod(AsyncResult);
                    }
                    catch (Exception ex)
                    {
                        lock (AggregatedExceptions) { AggregatedExceptions.Add(ex); }
                    }
                    finally
                    {
                        if (AggregatedExceptions.Count > 0) { Exception = ExceptionUtility.Refine(new ThreadException(AggregatedExceptions), BeginMethod.Method, AsyncResult); }
                        SetOperationCompleted();
                    }
                }
            }
            else
            {
                throw new TimeoutException("The time to complete the asynchronous operation took longer than allowed.");
            }
        }

        /// <summary>
        /// A reference to the delegate that begins the asynchronous operation.
        /// </summary>
        protected Doer<AsyncCallback, object, IAsyncResult> BeginMethod { get; }

        /// <summary>
        /// A reference to the delegate that ends the asynchronous operation.
        /// </summary>
        protected Act<IAsyncResult> EndMethod { get; }

        /// <summary>
        /// Invokes the asynchronous operation.
        /// </summary>
        public virtual void Invoke()
        {
            AsyncResult = BeginMethod(AsyncCallback, null);
        }

        /// <summary>
        /// Ends the asynchronous operation.
        /// </summary>
        public virtual void EndInvoke(IAsyncResult result)
        {
            AsyncCallback(result);
        }

        /// <summary>
        /// The method to be called when a corresponding asynchronous operation completes.
        /// </summary>
        /// <param name="result">The result of the asynchronous operation.</param>
        /// <remarks>This method signals the <see cref="OperationComplete"/> property and thereby resumes the current thread.</remarks>
        protected virtual void AsyncCallback(IAsyncResult result)
        {
            lock (PadLock)
            {
                if (OperationComplete) { return; }
                try
                {
                    EndMethod(result);
                }
                catch (Exception ex)
                {
                    AggregatedExceptions.Add(ex);
                }
                finally
                {
                    if (AggregatedExceptions.Count > 0) { Exception = ExceptionUtility.Refine(new ThreadException(AggregatedExceptions), BeginMethod.Method, result); }
                    SetOperationCompleted();
                }
            }
        }
        #endregion
    }
}