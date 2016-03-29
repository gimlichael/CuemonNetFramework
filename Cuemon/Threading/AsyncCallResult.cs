using System;

namespace Cuemon.Threading
{
    /// <summary>
    /// A class designed to encapsulate the result of an asynchronous operation.
    /// </summary>
    /// <typeparam name="TState">The type of the return value of <see cref="AsyncState"/>.</typeparam>
    /// <typeparam name="TResult">The type of the return value of <see cref="Result"/>.</typeparam>
    public sealed class AsyncCallResult<TState, TResult>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AsyncCallResult&lt;TState, TResult&gt;"/> class.
        /// </summary>
        /// <param name="state">The state that qualifies or contains information about an asynchronous operation.</param>
        /// <param name="result">The result of the asynchronous operation.</param>
        public AsyncCallResult(TState state, TResult result)
        {
            AsyncState = state;
            Result = result;
        }

        /// <summary>
        /// Gets a user-defined object that qualifies or contains information about an asynchronous operation.
        /// </summary>
        /// <returns>
        /// A user-defined object that qualifies or contains information about an asynchronous operation.
        ///   </returns>
        public TState AsyncState { get; }

        /// <summary>
        /// Gets the result of the asynchronous operation.
        /// </summary>
        public TResult Result { get; internal set; }

        /// <summary>
        /// Gets the result of the asynchronous operation and cast it as <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The type to return the <see cref="Result"/> as.</typeparam>
        /// <returns>The value of the <see cref="Result"/> cast as the <typeparamref name="T"/>.</returns>
        public T ResultAs<T>() where T : TResult
        {
            return ResultAs<T>(Result);
        }

        /// <summary>
        /// Gets the result of the asynchronous operation and cast it as <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The type to return the <see cref="Result"/> as.</typeparam>
        /// <param name="result">The result of the asynchronous operation.</param>
        /// <returns>The value of the <see cref="Result"/> cast as the <typeparamref name="T"/>.</returns>
        public T ResultAs<T>(TResult result) where T : TResult
        {
            return (T)result;
        }

        /// <summary>
        /// Gets the <see cref="Exception"/> that caused the <see cref="AsyncCall{TState,TResult}"/> to end prematurely. If the <see cref="AsyncCall{TState,TResult}"/> completed successfully or has not yet thrown any exceptions, this will return null.
        /// </summary>
        public Exception Exception { get; internal set; }
    }
}