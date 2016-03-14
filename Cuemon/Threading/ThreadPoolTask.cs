using System;
using System.Reflection;

namespace Cuemon.Threading
{
    /// <summary>
    /// Represents an asynchronous operation.
    /// </summary>
    /// <seealso cref="AsyncCall{TState, TResult}" />
    public sealed class ThreadPoolTask<T> : AsyncCall<ThreadPoolTask<T>, T>
    {
        internal ThreadPoolTask(Doer<AsyncCallback, object, IAsyncResult> beginMethod, Doer<IAsyncResult, T> endMethod, MethodInfo delegateInfo, Template genericArguments)
            : base(beginMethod, endMethod, null)
        {
            AsyncState = this;
            DelegateInfo = delegateInfo;
            GenericArguments = genericArguments;
            Invoke();
        }

        private Template GenericArguments { get; set; }

        private MethodInfo DelegateInfo { get; set; }

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
                    if (AggregatedExceptions.Count > 0) { Exception = ExceptionUtility.Refine(new ThreadException(AggregatedExceptions), DelegateInfo, GenericArguments); }
                    SetOperationCompleted();
                }
            }
        }
    }

    /// <summary>
    /// Represents an asynchronous operation.
    /// </summary>
    /// <seealso cref="AsyncCall{TState}" />
    public class ThreadPoolTask : AsyncCall<ThreadPoolTask>
    {
        internal ThreadPoolTask(Doer<AsyncCallback, object, IAsyncResult> beginMethod, Act<IAsyncResult> endMethod, MethodInfo delegateInfo, Template genericArguments)
            : base(beginMethod, endMethod, null)
        {
            AsyncState = this;
            DelegateInfo = delegateInfo;
            GenericArguments = genericArguments;
            Invoke();
        }

        /// <summary>
        /// Invokes the asynchronous operation.
        /// </summary>
        public sealed override void Invoke()
        {
            base.Invoke();
        }

        private Template GenericArguments { get; set; }

        private MethodInfo DelegateInfo { get; set; }

        /// <summary>
        /// The method to be called when a corresponding asynchronous operation completes.
        /// </summary>
        /// <param name="result">The result of the asynchronous operation.</param>
        protected sealed override void AsyncCallback(IAsyncResult result)
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
                    if (AggregatedExceptions.Count > 0) { Exception = ExceptionUtility.Refine(new ThreadException(AggregatedExceptions), DelegateInfo, GenericArguments); }
                    SetOperationCompleted();
                }
            }
        }
    }
}