﻿using System;
using System.Threading;
namespace Cuemon.Threading
{
    /// <summary>
    /// Provide ways to work more efficient with <see cref="ThreadPool"/> related tasks.
    /// </summary>
    public static partial class ThreadPoolUtility
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
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="method"/> is null.
        /// </exception>
        public static void QueueWork(Act method)
        {
            if (method == null) { throw new ArgumentNullException("method"); }
            ActFactory factory = new ActFactory(method);
            QueueUserWorkItemCore(factory);
        }

        /// <summary>
        /// Queues the specified <paramref name="method"/> for execution. The <paramref name="method"/> executes when a thread pool thread becomes available.
        /// </summary>
        /// <typeparam name="T">The type of the parameter of the delegate <paramref name="method" />.</typeparam>
        /// <param name="method">The delegate that is being invoked when a thread pool thread becomes available.</param>
        /// <param name="arg">The parameter of the delegate <paramref name="method" />.</param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="method"/> is null.
        /// </exception>
        public static void QueueWork<T>(Act<T> method, T arg)
        {
            if (method == null) { throw new ArgumentNullException("method"); }
            ActFactory<T> factory = new ActFactory<T>(method, arg);
            QueueUserWorkItemCore(factory);
        }

        /// <summary>
        /// Queues the specified <paramref name="method"/> for execution. The <paramref name="method"/> executes when a thread pool thread becomes available.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter of the delegate <paramref name="method" />.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the delegate <paramref name="method" />.</typeparam>
        /// <param name="method">The delegate that is being invoked when a thread pool thread becomes available.</param>
        /// <param name="arg1">The first parameter of the delegate <paramref name="method" />.</param>
        /// <param name="arg2">The second parameter of the delegate <paramref name="method" />.</param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="method"/> is null.
        /// </exception>
        public static void QueueWork<T1, T2>(Act<T1, T2> method, T1 arg1, T2 arg2)
        {
            if (method == null) { throw new ArgumentNullException("method"); }
            ActFactory<T1, T2> factory = new ActFactory<T1, T2>(method, arg1, arg2);
            QueueUserWorkItemCore(factory);
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
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="method"/> is null.
        /// </exception>
        public static void QueueWork<T1, T2, T3>(Act<T1, T2, T3> method, T1 arg1, T2 arg2, T3 arg3)
        {
            if (method == null) { throw new ArgumentNullException("method"); }
            ActFactory<T1, T2, T3> factory = new ActFactory<T1, T2, T3>(method, arg1, arg2, arg3);
            QueueUserWorkItemCore(factory);
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
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="method"/> is null.
        /// </exception>
        public static void QueueWork<T1, T2, T3, T4>(Act<T1, T2, T3, T4> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            if (method == null) { throw new ArgumentNullException("method"); }
            ActFactory<T1, T2, T3, T4> factory = new ActFactory<T1, T2, T3, T4>(method, arg1, arg2, arg3, arg4);
            QueueUserWorkItemCore(factory);
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
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="method"/> is null.
        /// </exception>
        public static void QueueWork<T1, T2, T3, T4, T5>(Act<T1, T2, T3, T4, T5> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        {
            if (method == null) { throw new ArgumentNullException("method"); }
            ActFactory<T1, T2, T3, T4, T5> factory = new ActFactory<T1, T2, T3, T4, T5>(method, arg1, arg2, arg3, arg4, arg5);
            QueueUserWorkItemCore(factory);
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
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="method"/> is null.
        /// </exception>
        public static void QueueWork<T1, T2, T3, T4, T5, T6>(Act<T1, T2, T3, T4, T5, T6> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6)
        {
            if (method == null) { throw new ArgumentNullException("method"); }
            ActFactory<T1, T2, T3, T4, T5, T6> factory = new ActFactory<T1, T2, T3, T4, T5, T6>(method, arg1, arg2, arg3, arg4, arg5, arg6);
            QueueUserWorkItemCore(factory);
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
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="method"/> is null.
        /// </exception>
        public static void QueueWork<T1, T2, T3, T4, T5, T6, T7>(Act<T1, T2, T3, T4, T5, T6, T7> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7)
        {
            if (method == null) { throw new ArgumentNullException("method"); }
            ActFactory<T1, T2, T3, T4, T5, T6, T7> factory = new ActFactory<T1, T2, T3, T4, T5, T6, T7>(method, arg1, arg2, arg3, arg4, arg5, arg6, arg7);
            QueueUserWorkItemCore(factory);
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
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="method"/> is null.
        /// </exception>
        public static void QueueWork<T1, T2, T3, T4, T5, T6, T7, T8>(Act<T1, T2, T3, T4, T5, T6, T7, T8> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8)
        {
            if (method == null) { throw new ArgumentNullException("method"); }
            ActFactory<T1, T2, T3, T4, T5, T6, T7, T8> factory = new ActFactory<T1, T2, T3, T4, T5, T6, T7, T8>(method, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8);
            QueueUserWorkItemCore(factory);
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
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="method"/> is null.
        /// </exception>
        public static void QueueWork<T1, T2, T3, T4, T5, T6, T7, T8, T9>(Act<T1, T2, T3, T4, T5, T6, T7, T8, T9> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9)
        {
            if (method == null) { throw new ArgumentNullException("method"); }
            ActFactory<T1, T2, T3, T4, T5, T6, T7, T8, T9> factory = new ActFactory<T1, T2, T3, T4, T5, T6, T7, T8, T9>(method, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9);
            QueueUserWorkItemCore(factory);
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
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="method"/> is null.
        /// </exception>
        public static void QueueWork<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(Act<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10)
        {
            if (method == null) { throw new ArgumentNullException("method"); }
            ActFactory<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> factory = new ActFactory<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(method, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10);
            QueueUserWorkItemCore(factory);
        }

        private static void QueueUserWorkItemCore(ActFactory factory)
        {
            ThreadPool.QueueUserWorkItem(QueueUserWorkItem, factory);
        }

        private static void QueueUserWorkItem(object o)
        {
            ActFactory factory = o as ActFactory;
            if (factory == null) { return; }
            factory.ExecuteMethod();
        }
    }
}