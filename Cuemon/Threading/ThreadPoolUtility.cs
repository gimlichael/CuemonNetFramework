using System;
using System.Collections.Generic;
using System.Threading;

namespace Cuemon.Threading
{
    /// <summary>
    /// Provide ways to work more efficient with <see cref="ThreadPool"/> related tasks.
    /// </summary>
    public static class ThreadPoolUtility
    {
        /// <summary>
        /// Provides information about available threads in the <see cref="ThreadPool"/>.
        /// </summary>
        /// <returns>A new instance of the <see cref="ThreadPoolInfo"/> object.</returns>
        public static ThreadPoolInfo Probe()
        {
            return new ThreadPoolInfo();
        }

        /// <summary>
        /// Gets the settings currently applied to the <see cref="ThreadPool"/>.
        /// </summary>
        /// <returns>A new instance of the <see cref="ThreadPoolSettings"/> object.</returns>
        public static ThreadPoolSettings GetSettings()
        {
            return new ThreadPoolSettings();
        }

        /// <summary>
        /// Queues the specified <paramref name="method"/> for execution. The <paramref name="method"/> executes when a thread pool thread becomes available.
        /// </summary>
        /// <param name="method">The delegate that is being invoked when a thread pool thread becomes available.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="method"/> is null.
        /// </exception>
        public static ThreadPoolTask RunAction(Act method)
        {
            Validator.ThrowIfNull(method, nameof(method));
            var factory = ActFactory.Create(method);
            return QueueUserWorkItemCore(factory);
        }

        /// <summary>
        /// Queues the specified <paramref name="method"/> for execution. The <paramref name="method"/> executes when a thread pool thread becomes available.
        /// </summary>
        /// <typeparam name="T">The type of the parameter of the delegate <paramref name="method" />.</typeparam>
        /// <param name="method">The delegate that is being invoked when a thread pool thread becomes available.</param>
        /// <param name="arg">The parameter of the delegate <paramref name="method" />.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="method"/> is null.
        /// </exception>
        public static ThreadPoolTask RunAction<T>(Act<T> method, T arg)
        {
            Validator.ThrowIfNull(method, nameof(method));
            var factory = ActFactory.Create(method, arg);
            return QueueUserWorkItemCore(factory);
        }

        /// <summary>
        /// Queues the specified <paramref name="method"/> for execution. The <paramref name="method"/> executes when a thread pool thread becomes available.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter of the delegate <paramref name="method" />.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the delegate <paramref name="method" />.</typeparam>
        /// <param name="method">The delegate that is being invoked when a thread pool thread becomes available.</param>
        /// <param name="arg1">The first parameter of the delegate <paramref name="method" />.</param>
        /// <param name="arg2">The second parameter of the delegate <paramref name="method" />.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="method"/> is null.
        /// </exception>
        public static ThreadPoolTask RunAction<T1, T2>(Act<T1, T2> method, T1 arg1, T2 arg2)
        {
            Validator.ThrowIfNull(method, nameof(method));
            var factory = ActFactory.Create(method, arg1, arg2);
            return QueueUserWorkItemCore(factory);
        }

        /// <summary>
        /// Queues the specified <paramref name="method"/> for execution. The <paramref name="method"/> executes when a thread pool thread becomes available.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter of the delegate <paramref name="method" />.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the delegate <paramref name="method" />.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the delegate <paramref name="method" />.</typeparam>
        /// <param name="method">The delegate that is being invoked when a thread pool thread becomes available.</param>
        /// <param name="arg1">The first parameter of the delegate <paramref name="method" />.</param>
        /// <param name="arg2">The second parameter of the delegate <paramref name="method" />.</param>
        /// <param name="arg3">The third parameter of the delegate <paramref name="method" />.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="method"/> is null.
        /// </exception>
        public static ThreadPoolTask RunAction<T1, T2, T3>(Act<T1, T2, T3> method, T1 arg1, T2 arg2, T3 arg3)
        {
            Validator.ThrowIfNull(method, nameof(method));
            var factory = ActFactory.Create(method, arg1, arg2, arg3);
            return QueueUserWorkItemCore(factory);
        }

        /// <summary>
        /// Queues the specified <paramref name="method"/> for execution. The <paramref name="method"/> executes when a thread pool thread becomes available.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter of the delegate <paramref name="method" />.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the delegate <paramref name="method" />.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the delegate <paramref name="method" />.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of the delegate <paramref name="method" />.</typeparam>
        /// <param name="method">The delegate that is being invoked when a thread pool thread becomes available.</param>
        /// <param name="arg1">The first parameter of the delegate <paramref name="method" />.</param>
        /// <param name="arg2">The second parameter of the delegate <paramref name="method" />.</param>
        /// <param name="arg3">The third parameter of the delegate <paramref name="method" />.</param>
        /// <param name="arg4">The fourth parameter of the delegate <paramref name="method" />.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="method"/> is null.
        /// </exception>
        public static ThreadPoolTask RunAction<T1, T2, T3, T4>(Act<T1, T2, T3, T4> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            Validator.ThrowIfNull(method, nameof(method));
            var factory = ActFactory.Create(method, arg1, arg2, arg3, arg4);
            return QueueUserWorkItemCore(factory);
        }

        /// <summary>
        /// Queues the specified <paramref name="method"/> for execution. The <paramref name="method"/> executes when a thread pool thread becomes available.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter of the delegate <paramref name="method" />.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the delegate <paramref name="method" />.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the delegate <paramref name="method" />.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of the delegate <paramref name="method" />.</typeparam>
        /// <typeparam name="T5">The type of the fifth parameter of the delegate <paramref name="method" />.</typeparam>
        /// <param name="method">The delegate that is being invoked when a thread pool thread becomes available.</param>
        /// <param name="arg1">The first parameter of the delegate <paramref name="method" />.</param>
        /// <param name="arg2">The second parameter of the delegate <paramref name="method" />.</param>
        /// <param name="arg3">The third parameter of the delegate <paramref name="method" />.</param>
        /// <param name="arg4">The fourth parameter of the delegate <paramref name="method" />.</param>
        /// <param name="arg5">The fifth parameter of the delegate <paramref name="method" />.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="method"/> is null.
        /// </exception>
        public static ThreadPoolTask RunAction<T1, T2, T3, T4, T5>(Act<T1, T2, T3, T4, T5> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        {
            Validator.ThrowIfNull(method, nameof(method));
            var factory = ActFactory.Create(method, arg1, arg2, arg3, arg4, arg5);
            return QueueUserWorkItemCore(factory);
        }

        /// <summary>
        /// Queues the specified <paramref name="method"/> for execution. The <paramref name="method"/> executes when a thread pool thread becomes available.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter of the delegate <paramref name="method" />.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the delegate <paramref name="method" />.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the delegate <paramref name="method" />.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of the delegate <paramref name="method" />.</typeparam>
        /// <typeparam name="T5">The type of the fifth parameter of the delegate <paramref name="method" />.</typeparam>
        /// <typeparam name="T6">The type of the sixth parameter of the delegate <paramref name="method" />.</typeparam>
        /// <param name="method">The delegate that is being invoked when a thread pool thread becomes available.</param>
        /// <param name="arg1">The first parameter of the delegate <paramref name="method" />.</param>
        /// <param name="arg2">The second parameter of the delegate <paramref name="method" />.</param>
        /// <param name="arg3">The third parameter of the delegate <paramref name="method" />.</param>
        /// <param name="arg4">The fourth parameter of the delegate <paramref name="method" />.</param>
        /// <param name="arg5">The fifth parameter of the delegate <paramref name="method" />.</param>
        /// <param name="arg6">The sixth parameter of the delegate <paramref name="method" />.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="method"/> is null.
        /// </exception>
        public static ThreadPoolTask RunAction<T1, T2, T3, T4, T5, T6>(Act<T1, T2, T3, T4, T5, T6> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6)
        {
            Validator.ThrowIfNull(method, nameof(method));
            var factory = ActFactory.Create(method, arg1, arg2, arg3, arg4, arg5, arg6);
            return QueueUserWorkItemCore(factory);
        }

        /// <summary>
        /// Queues the specified <paramref name="method"/> for execution. The <paramref name="method"/> executes when a thread pool thread becomes available.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter of the delegate <paramref name="method" />.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the delegate <paramref name="method" />.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the delegate <paramref name="method" />.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of the delegate <paramref name="method" />.</typeparam>
        /// <typeparam name="T5">The type of the fifth parameter of the delegate <paramref name="method" />.</typeparam>
        /// <typeparam name="T6">The type of the sixth parameter of the delegate <paramref name="method" />.</typeparam>
        /// <typeparam name="T7">The type of the seventh parameter of the delegate <paramref name="method" />.</typeparam>
        /// <param name="method">The delegate that is being invoked when a thread pool thread becomes available.</param>
        /// <param name="arg1">The first parameter of the delegate <paramref name="method" />.</param>
        /// <param name="arg2">The second parameter of the delegate <paramref name="method" />.</param>
        /// <param name="arg3">The third parameter of the delegate <paramref name="method" />.</param>
        /// <param name="arg4">The fourth parameter of the delegate <paramref name="method" />.</param>
        /// <param name="arg5">The fifth parameter of the delegate <paramref name="method" />.</param>
        /// <param name="arg6">The sixth parameter of the delegate <paramref name="method" />.</param>
        /// <param name="arg7">The seventh parameter of the delegate <paramref name="method" />.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="method"/> is null.
        /// </exception>
        public static ThreadPoolTask RunAction<T1, T2, T3, T4, T5, T6, T7>(Act<T1, T2, T3, T4, T5, T6, T7> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7)
        {
            Validator.ThrowIfNull(method, nameof(method));
            var factory = ActFactory.Create(method, arg1, arg2, arg3, arg4, arg5, arg6, arg7);
            return QueueUserWorkItemCore(factory);
        }

        /// <summary>
        /// Queues the specified <paramref name="method"/> for execution. The <paramref name="method"/> executes when a thread pool thread becomes available.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter of the delegate <paramref name="method" />.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the delegate <paramref name="method" />.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the delegate <paramref name="method" />.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of the delegate <paramref name="method" />.</typeparam>
        /// <typeparam name="T5">The type of the fifth parameter of the delegate <paramref name="method" />.</typeparam>
        /// <typeparam name="T6">The type of the sixth parameter of the delegate <paramref name="method" />.</typeparam>
        /// <typeparam name="T7">The type of the seventh parameter of the delegate <paramref name="method" />.</typeparam>
        /// <typeparam name="T8">The type of the eighth parameter of the delegate <paramref name="method" />.</typeparam>
        /// <param name="method">The delegate that is being invoked when a thread pool thread becomes available.</param>
        /// <param name="arg1">The first parameter of the delegate <paramref name="method" />.</param>
        /// <param name="arg2">The second parameter of the delegate <paramref name="method" />.</param>
        /// <param name="arg3">The third parameter of the delegate <paramref name="method" />.</param>
        /// <param name="arg4">The fourth parameter of the delegate <paramref name="method" />.</param>
        /// <param name="arg5">The fifth parameter of the delegate <paramref name="method" />.</param>
        /// <param name="arg6">The sixth parameter of the delegate <paramref name="method" />.</param>
        /// <param name="arg7">The seventh parameter of the delegate <paramref name="method" />.</param>
        /// <param name="arg8">The eighth parameter of the delegate <paramref name="method" />.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="method"/> is null.
        /// </exception>
        public static ThreadPoolTask RunAction<T1, T2, T3, T4, T5, T6, T7, T8>(Act<T1, T2, T3, T4, T5, T6, T7, T8> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8)
        {
            Validator.ThrowIfNull(method, nameof(method));
            var factory = ActFactory.Create(method, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8);
            return QueueUserWorkItemCore(factory);
        }

        /// <summary>
        /// Queues the specified <paramref name="method"/> for execution. The <paramref name="method"/> executes when a thread pool thread becomes available.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter of the delegate <paramref name="method" />.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the delegate <paramref name="method" />.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the delegate <paramref name="method" />.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of the delegate <paramref name="method" />.</typeparam>
        /// <typeparam name="T5">The type of the fifth parameter of the delegate <paramref name="method" />.</typeparam>
        /// <typeparam name="T6">The type of the sixth parameter of the delegate <paramref name="method" />.</typeparam>
        /// <typeparam name="T7">The type of the seventh parameter of the delegate <paramref name="method" />.</typeparam>
        /// <typeparam name="T8">The type of the eighth parameter of the delegate <paramref name="method" />.</typeparam>
        /// <typeparam name="T9">The type of the ninth parameter of the delegate <paramref name="method" />.</typeparam>
        /// <param name="method">The delegate that is being invoked when a thread pool thread becomes available.</param>
        /// <param name="arg1">The first parameter of the delegate <paramref name="method" />.</param>
        /// <param name="arg2">The second parameter of the delegate <paramref name="method" />.</param>
        /// <param name="arg3">The third parameter of the delegate <paramref name="method" />.</param>
        /// <param name="arg4">The fourth parameter of the delegate <paramref name="method" />.</param>
        /// <param name="arg5">The fifth parameter of the delegate <paramref name="method" />.</param>
        /// <param name="arg6">The sixth parameter of the delegate <paramref name="method" />.</param>
        /// <param name="arg7">The seventh parameter of the delegate <paramref name="method" />.</param>
        /// <param name="arg8">The eighth parameter of the delegate <paramref name="method" />.</param>
        /// <param name="arg9">The ninth parameter of the delegate <paramref name="method" />.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="method"/> is null.
        /// </exception>
        public static ThreadPoolTask RunAction<T1, T2, T3, T4, T5, T6, T7, T8, T9>(Act<T1, T2, T3, T4, T5, T6, T7, T8, T9> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9)
        {
            Validator.ThrowIfNull(method, nameof(method));
            var factory = ActFactory.Create(method, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9);
            return QueueUserWorkItemCore(factory);
        }

        /// <summary>
        /// Queues the specified <paramref name="method"/> for execution. The <paramref name="method"/> executes when a thread pool thread becomes available.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter of the delegate <paramref name="method" />.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the delegate <paramref name="method" />.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the delegate <paramref name="method" />.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of the delegate <paramref name="method" />.</typeparam>
        /// <typeparam name="T5">The type of the fifth parameter of the delegate <paramref name="method" />.</typeparam>
        /// <typeparam name="T6">The type of the sixth parameter of the delegate <paramref name="method" />.</typeparam>
        /// <typeparam name="T7">The type of the seventh parameter of the delegate <paramref name="method" />.</typeparam>
        /// <typeparam name="T8">The type of the eighth parameter of the delegate <paramref name="method" />.</typeparam>
        /// <typeparam name="T9">The type of the ninth parameter of the delegate <paramref name="method" />.</typeparam>
        /// <typeparam name="T10">The type of the tenth parameter of the delegate <paramref name="method" />.</typeparam>
        /// <param name="method">The delegate that is being invoked when a thread pool thread becomes available.</param>
        /// <param name="arg1">The first parameter of the delegate <paramref name="method" />.</param>
        /// <param name="arg2">The second parameter of the delegate <paramref name="method" />.</param>
        /// <param name="arg3">The third parameter of the delegate <paramref name="method" />.</param>
        /// <param name="arg4">The fourth parameter of the delegate <paramref name="method" />.</param>
        /// <param name="arg5">The fifth parameter of the delegate <paramref name="method" />.</param>
        /// <param name="arg6">The sixth parameter of the delegate <paramref name="method" />.</param>
        /// <param name="arg7">The seventh parameter of the delegate <paramref name="method" />.</param>
        /// <param name="arg8">The eighth parameter of the delegate <paramref name="method" />.</param>
        /// <param name="arg9">The ninth parameter of the delegate <paramref name="method" />.</param>
        /// <param name="arg10">The tenth parameter of the delegate <paramref name="method" />.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="method"/> is null.
        /// </exception>
        public static ThreadPoolTask RunAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(Act<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10)
        {
            Validator.ThrowIfNull(method, nameof(method));
            var factory = ActFactory.Create(method, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10);
            return QueueUserWorkItemCore(factory);
        }

        /// <summary>
        /// Queues the specified <paramref name="method"/> for execution. The <paramref name="method"/> executes when a thread pool thread becomes available.
        /// </summary>
        /// <typeparam name="TResult">The type of the result returned by the proxy <see cref="ThreadPoolTask"/>.</typeparam>
        /// <param name="method">the function delegate that is being invoked when a thread pool thread becomes available.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="method"/> is null.
        /// </exception>
        public static ThreadPoolTask<TResult> RunFunction<TResult>(Doer<TResult> method)
        {
            Validator.ThrowIfNull(method, nameof(method));
            var factory = DoerFactory.Create(method);
            return QueueUserWorkItemCore(factory);
        }

        /// <summary>
        /// Queues the specified <paramref name="method"/> for execution. The <paramref name="method"/> executes when a thread pool thread becomes available.
        /// </summary>
        /// <typeparam name="T">The type of the parameter of the function delegate <paramref name="method" />.</typeparam>
        /// <typeparam name="TResult">The type of the result returned by the proxy <see cref="ThreadPoolTask"/>.</typeparam>
        /// <param name="method">the function delegate that is being invoked when a thread pool thread becomes available.</param>
        /// <param name="arg">The parameter of the function delegate <paramref name="method" />.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="method"/> is null.
        /// </exception>
        public static ThreadPoolTask<TResult> RunFunction<T, TResult>(Doer<T, TResult> method, T arg)
        {
            Validator.ThrowIfNull(method, nameof(method));
            var factory = DoerFactory.Create(method, arg);
            return QueueUserWorkItemCore(factory);
        }

        /// <summary>
        /// Queues the specified <paramref name="method"/> for execution. The <paramref name="method"/> executes when a thread pool thread becomes available.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter of the function delegate <paramref name="method" />.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the function delegate <paramref name="method" />.</typeparam>
        /// <typeparam name="TResult">The type of the result returned by the proxy <see cref="ThreadPoolTask"/>.</typeparam>
        /// <param name="method">the function delegate that is being invoked when a thread pool thread becomes available.</param>
        /// <param name="arg1">The first parameter of the function delegate <paramref name="method" />.</param>
        /// <param name="arg2">The second parameter of the function delegate <paramref name="method" />.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="method"/> is null.
        /// </exception>
        public static ThreadPoolTask<TResult> RunFunction<T1, T2, TResult>(Doer<T1, T2, TResult> method, T1 arg1, T2 arg2)
        {
            Validator.ThrowIfNull(method, nameof(method));
            var factory = DoerFactory.Create(method, arg1, arg2);
            return QueueUserWorkItemCore(factory);
        }

        /// <summary>
        /// Queues the specified <paramref name="method"/> for execution. The <paramref name="method"/> executes when a thread pool thread becomes available.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter of the function delegate <paramref name="method" />.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the function delegate <paramref name="method" />.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the function delegate <paramref name="method" />.</typeparam>
        /// <typeparam name="TResult">The type of the result returned by the proxy <see cref="ThreadPoolTask"/>.</typeparam>
        /// <param name="method">the function delegate that is being invoked when a thread pool thread becomes available.</param>
        /// <param name="arg1">The first parameter of the function delegate <paramref name="method" />.</param>
        /// <param name="arg2">The second parameter of the function delegate <paramref name="method" />.</param>
        /// <param name="arg3">The third parameter of the function delegate <paramref name="method" />.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="method"/> is null.
        /// </exception>
        public static ThreadPoolTask<TResult> RunFunction<T1, T2, T3, TResult>(Doer<T1, T2, T3, TResult> method, T1 arg1, T2 arg2, T3 arg3)
        {
            Validator.ThrowIfNull(method, nameof(method));
            var factory = DoerFactory.Create(method, arg1, arg2, arg3);
            return QueueUserWorkItemCore(factory);
        }

        /// <summary>
        /// Queues the specified <paramref name="method"/> for execution. The <paramref name="method"/> executes when a thread pool thread becomes available.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter of the function delegate <paramref name="method" />.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the function delegate <paramref name="method" />.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the function delegate <paramref name="method" />.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of the function delegate <paramref name="method" />.</typeparam>
        /// <typeparam name="TResult">The type of the result returned by the proxy <see cref="ThreadPoolTask"/>.</typeparam>
        /// <param name="method">the function delegate that is being invoked when a thread pool thread becomes available.</param>
        /// <param name="arg1">The first parameter of the function delegate <paramref name="method" />.</param>
        /// <param name="arg2">The second parameter of the function delegate <paramref name="method" />.</param>
        /// <param name="arg3">The third parameter of the function delegate <paramref name="method" />.</param>
        /// <param name="arg4">The fourth parameter of the function delegate <paramref name="method" />.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="method"/> is null.
        /// </exception>
        public static ThreadPoolTask<TResult> RunFunction<T1, T2, T3, T4, TResult>(Doer<T1, T2, T3, T4, TResult> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            Validator.ThrowIfNull(method, nameof(method));
            var factory = DoerFactory.Create(method, arg1, arg2, arg3, arg4);
            return QueueUserWorkItemCore(factory);
        }

        /// <summary>
        /// Queues the specified <paramref name="method"/> for execution. The <paramref name="method"/> executes when a thread pool thread becomes available.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter of the function delegate <paramref name="method" />.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the function delegate <paramref name="method" />.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the function delegate <paramref name="method" />.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of the function delegate <paramref name="method" />.</typeparam>
        /// <typeparam name="T5">The type of the fifth parameter of the function delegate <paramref name="method" />.</typeparam>
        /// <typeparam name="TResult">The type of the result returned by the proxy <see cref="ThreadPoolTask"/>.</typeparam>
        /// <param name="method">the function delegate that is being invoked when a thread pool thread becomes available.</param>
        /// <param name="arg1">The first parameter of the function delegate <paramref name="method" />.</param>
        /// <param name="arg2">The second parameter of the function delegate <paramref name="method" />.</param>
        /// <param name="arg3">The third parameter of the function delegate <paramref name="method" />.</param>
        /// <param name="arg4">The fourth parameter of the function delegate <paramref name="method" />.</param>
        /// <param name="arg5">The fifth parameter of the function delegate <paramref name="method" />.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="method"/> is null.
        /// </exception>
        public static ThreadPoolTask<TResult> RunFunction<T1, T2, T3, T4, T5, TResult>(Doer<T1, T2, T3, T4, T5, TResult> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        {
            Validator.ThrowIfNull(method, nameof(method));
            var factory = DoerFactory.Create(method, arg1, arg2, arg3, arg4, arg5);
            return QueueUserWorkItemCore(factory);
        }

        /// <summary>
        /// Queues the specified <paramref name="method"/> for execution. The <paramref name="method"/> executes when a thread pool thread becomes available.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter of the function delegate <paramref name="method" />.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the function delegate <paramref name="method" />.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the function delegate <paramref name="method" />.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of the function delegate <paramref name="method" />.</typeparam>
        /// <typeparam name="T5">The type of the fifth parameter of the function delegate <paramref name="method" />.</typeparam>
        /// <typeparam name="T6">The type of the sixth parameter of the function delegate <paramref name="method" />.</typeparam>
        /// <typeparam name="TResult">The type of the result returned by the proxy <see cref="ThreadPoolTask"/>.</typeparam>
        /// <param name="method">the function delegate that is being invoked when a thread pool thread becomes available.</param>
        /// <param name="arg1">The first parameter of the function delegate <paramref name="method" />.</param>
        /// <param name="arg2">The second parameter of the function delegate <paramref name="method" />.</param>
        /// <param name="arg3">The third parameter of the function delegate <paramref name="method" />.</param>
        /// <param name="arg4">The fourth parameter of the function delegate <paramref name="method" />.</param>
        /// <param name="arg5">The fifth parameter of the function delegate <paramref name="method" />.</param>
        /// <param name="arg6">The sixth parameter of the function delegate <paramref name="method" />.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="method"/> is null.
        /// </exception>
        public static ThreadPoolTask<TResult> RunFunction<T1, T2, T3, T4, T5, T6, TResult>(Doer<T1, T2, T3, T4, T5, T6, TResult> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6)
        {
            Validator.ThrowIfNull(method, nameof(method));
            var factory = DoerFactory.Create(method, arg1, arg2, arg3, arg4, arg5, arg6);
            return QueueUserWorkItemCore(factory);
        }

        /// <summary>
        /// Queues the specified <paramref name="method"/> for execution. The <paramref name="method"/> executes when a thread pool thread becomes available.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter of the function delegate <paramref name="method" />.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the function delegate <paramref name="method" />.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the function delegate <paramref name="method" />.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of the function delegate <paramref name="method" />.</typeparam>
        /// <typeparam name="T5">The type of the fifth parameter of the function delegate <paramref name="method" />.</typeparam>
        /// <typeparam name="T6">The type of the sixth parameter of the function delegate <paramref name="method" />.</typeparam>
        /// <typeparam name="T7">The type of the seventh parameter of the function delegate <paramref name="method" />.</typeparam>
        /// <typeparam name="TResult">The type of the result returned by the proxy <see cref="ThreadPoolTask"/>.</typeparam>
        /// <param name="method">the function delegate that is being invoked when a thread pool thread becomes available.</param>
        /// <param name="arg1">The first parameter of the function delegate <paramref name="method" />.</param>
        /// <param name="arg2">The second parameter of the function delegate <paramref name="method" />.</param>
        /// <param name="arg3">The third parameter of the function delegate <paramref name="method" />.</param>
        /// <param name="arg4">The fourth parameter of the function delegate <paramref name="method" />.</param>
        /// <param name="arg5">The fifth parameter of the function delegate <paramref name="method" />.</param>
        /// <param name="arg6">The sixth parameter of the function delegate <paramref name="method" />.</param>
        /// <param name="arg7">The seventh parameter of the function delegate <paramref name="method" />.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="method"/> is null.
        /// </exception>
        public static ThreadPoolTask<TResult> RunFunction<T1, T2, T3, T4, T5, T6, T7, TResult>(Doer<T1, T2, T3, T4, T5, T6, T7, TResult> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7)
        {
            Validator.ThrowIfNull(method, nameof(method));
            var factory = DoerFactory.Create(method, arg1, arg2, arg3, arg4, arg5, arg6, arg7);
            return QueueUserWorkItemCore(factory);
        }

        /// <summary>
        /// Queues the specified <paramref name="method"/> for execution. The <paramref name="method"/> executes when a thread pool thread becomes available.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter of the function delegate <paramref name="method" />.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the function delegate <paramref name="method" />.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the function delegate <paramref name="method" />.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of the function delegate <paramref name="method" />.</typeparam>
        /// <typeparam name="T5">The type of the fifth parameter of the function delegate <paramref name="method" />.</typeparam>
        /// <typeparam name="T6">The type of the sixth parameter of the function delegate <paramref name="method" />.</typeparam>
        /// <typeparam name="T7">The type of the seventh parameter of the function delegate <paramref name="method" />.</typeparam>
        /// <typeparam name="T8">The type of the eighth parameter of the function delegate <paramref name="method" />.</typeparam>
        /// <typeparam name="TResult">The type of the result returned by the proxy <see cref="ThreadPoolTask"/>.</typeparam>
        /// <param name="method">the function delegate that is being invoked when a thread pool thread becomes available.</param>
        /// <param name="arg1">The first parameter of the function delegate <paramref name="method" />.</param>
        /// <param name="arg2">The second parameter of the function delegate <paramref name="method" />.</param>
        /// <param name="arg3">The third parameter of the function delegate <paramref name="method" />.</param>
        /// <param name="arg4">The fourth parameter of the function delegate <paramref name="method" />.</param>
        /// <param name="arg5">The fifth parameter of the function delegate <paramref name="method" />.</param>
        /// <param name="arg6">The sixth parameter of the function delegate <paramref name="method" />.</param>
        /// <param name="arg7">The seventh parameter of the function delegate <paramref name="method" />.</param>
        /// <param name="arg8">The eighth parameter of the function delegate <paramref name="method" />.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="method"/> is null.
        /// </exception>
        public static ThreadPoolTask<TResult> RunFunction<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(Doer<T1, T2, T3, T4, T5, T6, T7, T8, TResult> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8)
        {
            Validator.ThrowIfNull(method, nameof(method));
            var factory = DoerFactory.Create(method, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8);
            return QueueUserWorkItemCore(factory);
        }

        /// <summary>
        /// Queues the specified <paramref name="method"/> for execution. The <paramref name="method"/> executes when a thread pool thread becomes available.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter of the function delegate <paramref name="method" />.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the function delegate <paramref name="method" />.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the function delegate <paramref name="method" />.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of the function delegate <paramref name="method" />.</typeparam>
        /// <typeparam name="T5">The type of the fifth parameter of the function delegate <paramref name="method" />.</typeparam>
        /// <typeparam name="T6">The type of the sixth parameter of the function delegate <paramref name="method" />.</typeparam>
        /// <typeparam name="T7">The type of the seventh parameter of the function delegate <paramref name="method" />.</typeparam>
        /// <typeparam name="T8">The type of the eighth parameter of the function delegate <paramref name="method" />.</typeparam>
        /// <typeparam name="T9">The type of the ninth parameter of the function delegate <paramref name="method" />.</typeparam>
        /// <typeparam name="TResult">The type of the result returned by the proxy <see cref="ThreadPoolTask"/>.</typeparam>
        /// <param name="method">the function delegate that is being invoked when a thread pool thread becomes available.</param>
        /// <param name="arg1">The first parameter of the function delegate <paramref name="method" />.</param>
        /// <param name="arg2">The second parameter of the function delegate <paramref name="method" />.</param>
        /// <param name="arg3">The third parameter of the function delegate <paramref name="method" />.</param>
        /// <param name="arg4">The fourth parameter of the function delegate <paramref name="method" />.</param>
        /// <param name="arg5">The fifth parameter of the function delegate <paramref name="method" />.</param>
        /// <param name="arg6">The sixth parameter of the function delegate <paramref name="method" />.</param>
        /// <param name="arg7">The seventh parameter of the function delegate <paramref name="method" />.</param>
        /// <param name="arg8">The eighth parameter of the function delegate <paramref name="method" />.</param>
        /// <param name="arg9">The ninth parameter of the function delegate <paramref name="method" />.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="method"/> is null.
        /// </exception>
        public static ThreadPoolTask<TResult> RunFunction<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>(Doer<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9)
        {
            Validator.ThrowIfNull(method, nameof(method));
            var factory = DoerFactory.Create(method, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9);
            return QueueUserWorkItemCore(factory);
        }

        /// <summary>
        /// Queues the specified <paramref name="method"/> for execution. The <paramref name="method"/> executes when a thread pool thread becomes available.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter of the function delegate <paramref name="method" />.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the function delegate <paramref name="method" />.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the function delegate <paramref name="method" />.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of the function delegate <paramref name="method" />.</typeparam>
        /// <typeparam name="T5">The type of the fifth parameter of the function delegate <paramref name="method" />.</typeparam>
        /// <typeparam name="T6">The type of the sixth parameter of the function delegate <paramref name="method" />.</typeparam>
        /// <typeparam name="T7">The type of the seventh parameter of the function delegate <paramref name="method" />.</typeparam>
        /// <typeparam name="T8">The type of the eighth parameter of the function delegate <paramref name="method" />.</typeparam>
        /// <typeparam name="T9">The type of the ninth parameter of the function delegate <paramref name="method" />.</typeparam>
        /// <typeparam name="T10">The type of the tenth parameter of the function delegate <paramref name="method" />.</typeparam>
        /// <typeparam name="TResult">The type of the result returned by the proxy <see cref="ThreadPoolTask"/>.</typeparam>
        /// <param name="method">the function delegate that is being invoked when a thread pool thread becomes available.</param>
        /// <param name="arg1">The first parameter of the function delegate <paramref name="method" />.</param>
        /// <param name="arg2">The second parameter of the function delegate <paramref name="method" />.</param>
        /// <param name="arg3">The third parameter of the function delegate <paramref name="method" />.</param>
        /// <param name="arg4">The fourth parameter of the function delegate <paramref name="method" />.</param>
        /// <param name="arg5">The fifth parameter of the function delegate <paramref name="method" />.</param>
        /// <param name="arg6">The sixth parameter of the function delegate <paramref name="method" />.</param>
        /// <param name="arg7">The seventh parameter of the function delegate <paramref name="method" />.</param>
        /// <param name="arg8">The eighth parameter of the function delegate <paramref name="method" />.</param>
        /// <param name="arg9">The ninth parameter of the function delegate <paramref name="method" />.</param>
        /// <param name="arg10">The tenth parameter of the function delegate <paramref name="method" />.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="method"/> is null.
        /// </exception>
        public static ThreadPoolTask<TResult> RunFunction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>(Doer<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10)
        {
            Validator.ThrowIfNull(method, nameof(method));
            var factory = DoerFactory.Create(method, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10);
            return QueueUserWorkItemCore(factory);
        }

        /// <summary>
        /// Waits for all of the provided <paramref name="tasks"/> objects to complete execution.
        /// </summary>
        /// <param name="tasks">A sequence of <see cref="ThreadPoolTask"/> instances on which to wait.</param>
        public static void WaitAll(IEnumerable<ThreadPoolTask> tasks)
        {
            Validator.ThrowIfNull(tasks, nameof(tasks));
            foreach (var task in tasks) { task.Wait(); }
        }

        /// <summary>
        /// Waits for all of the provided <paramref name="tasks"/> objects to complete execution.
        /// </summary>
        /// <param name="tasks">A sequence of <see cref="ThreadPoolTask"/> instances on which to wait.</param>
        public static void WaitAll<T>(IEnumerable<ThreadPoolTask<T>> tasks)
        {
            Validator.ThrowIfNull(tasks, nameof(tasks));
            foreach (var task in tasks) { task.Wait(); }
        }

        private static ThreadPoolTask<T> QueueUserWorkItemCore<TTuple, T>(DoerFactory<TTuple, T> factory) where TTuple : Template
        {
            return new ThreadPoolTask<T>(factory.BeginExecuteMethod, factory.EndExecuteMethod, factory.DelegateInfo, factory.GenericArguments);
        }

        private static ThreadPoolTask QueueUserWorkItemCore<TTuple>(ActFactory<TTuple> factory) where TTuple : Template
        {
            return new ThreadPoolTask(factory.BeginExecuteMethod, factory.EndExecuteMethod, factory.DelegateInfo, factory.GenericArguments);
        }
    }
}