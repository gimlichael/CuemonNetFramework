using System;
using System.Collections.Generic;
using System.Threading;
using Cuemon.Collections.Generic;

namespace Cuemon.Threading
{
    /// <summary>
    /// Provides ways to encapsulate and re-use existing code while adding support for parallel loops and regions using <see cref="Thread"/> backing strategy.
    /// </summary>
    public static class ParallelThread
    {
        /// <summary>
        /// Provides a generic way of executing a parallel foreach-loop while providing ways to encapsulate and re-use existing code.
        /// </summary>
        /// <typeparam name="TSource">The type of the data in the <paramref name="source"/>.</typeparam>
        /// <param name="source">The sequence to iterate over parallel.</param>
        /// <param name="body">The delegate that is invoked once per iteration.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="source"/> is null -or- <paramref name="body"/> is null.
        /// </exception>
        /// <remarks>
        /// The following table shows the initial overloaded arguments for <see cref="ForEach{TSource}(IEnumerable{TSource},Act{TSource})"/>.
        /// <list type="table">
        ///     <listheader>
        ///         <term>Argument</term>
        ///         <description>Initial Value</description>
        ///     </listheader>
        ///     <item>
        ///         <term>partitionSize</term>
        ///         <description><see cref="ThreadUtility.DefaultNumberOfConcurrentWorkerThreads"/></description>
        ///     </item>
        ///     <item>
        ///         <term>settings</term>
        ///         <description><see cref="ThreadUtility.CreateSettings()"/></description>
        ///     </item>
        ///     <item>
        ///         <term>timeout</term>
        ///         <description><see cref="TimeSpan.MaxValue"/></description>
        ///     </item>
        /// </list>
        /// </remarks>
        public static void ForEach<TSource>(IEnumerable<TSource> source, Act<TSource> body)
        {
            ForEach(ThreadUtility.DefaultNumberOfConcurrentWorkerThreads, source, body);
        }

        /// <summary>
        /// Provides a generic way of executing a parallel foreach-loop while providing ways to encapsulate and re-use existing code.
        /// </summary>
        /// <typeparam name="TSource">The type of the data in the <paramref name="source"/>.</typeparam>
        /// <param name="partitionSize">The maximum number of concurrent worker threads per partition.</param>
        /// <param name="source">The sequence to iterate over parallel.</param>
        /// <param name="body">The delegate that is invoked once per iteration.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="source"/> is null -or- <paramref name="body"/> is null.
        /// </exception>
        public static void ForEach<TSource>(int partitionSize, IEnumerable<TSource> source, Act<TSource> body)
        {
            ForEach(partitionSize, ThreadUtility.CreateSettings(), source, body);
        }

        /// <summary>
        /// Provides a generic way of executing a parallel foreach-loop while providing ways to encapsulate and re-use existing code.
        /// </summary>
        /// <typeparam name="TSource">The type of the data in the <paramref name="source"/>.</typeparam>
        /// <param name="partitionSize">The maximum number of concurrent worker threads per partition.</param>
        /// <param name="settings">The <see cref="ThreadSettings"/> object used to configure each new <see cref="Thread"/> instance.</param>
        /// <param name="source">The sequence to iterate over parallel.</param>
        /// <param name="body">The delegate that is invoked once per iteration.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="settings"/> is null -or- <paramref name="source"/> is null -or- <paramref name="body"/> is null.
        /// </exception>
        public static void ForEach<TSource>(int partitionSize, ThreadSettings settings, IEnumerable<TSource> source, Act<TSource> body)
        {
            ForEach(partitionSize, settings, TimeSpan.MaxValue, source, body);
        }

        /// <summary>
        /// Provides a generic way of executing a parallel foreach-loop while providing ways to encapsulate and re-use existing code.
        /// </summary>
        /// <typeparam name="TSource">The type of the data in the <paramref name="source"/>.</typeparam>
        /// <param name="partitionSize">The maximum number of concurrent worker threads per partition.</param>
        /// <param name="settings">The <see cref="ThreadSettings"/> object used to configure each new <see cref="Thread"/> instance.</param>
        /// <param name="timeout">A <see cref="TimeSpan" /> that represents the time to wait for the parallel foreach-loop operation to complete.</param>
        /// <param name="source">The sequence to iterate over parallel.</param>
        /// <param name="body">The delegate that is invoked once per iteration.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="settings"/> is null -or- <paramref name="source"/> is null -or- <paramref name="body"/> is null.
        /// </exception>
        public static void ForEach<TSource>(int partitionSize, ThreadSettings settings, TimeSpan timeout, IEnumerable<TSource> source, Act<TSource> body)
        {
            ValidateForEach(settings, source, body);
            var factory = ActFactory.Create(body, default(TSource));
            ForEachCore(factory, source, partitionSize, timeout, settings);
        }

        /// <summary>
        /// Provides a generic way of executing a parallel foreach-loop while providing ways to encapsulate and re-use existing code.
        /// </summary>
        /// <typeparam name="TSource">The type of the data in the <paramref name="source"/>.</typeparam>
        /// <typeparam name="T">The type of the parameter of the delegate <paramref name="body" />.</typeparam>
        /// <param name="source">The sequence to iterate over parallel.</param>
        /// <param name="body">The delegate that is invoked once per iteration.</param>
        /// <param name="arg">The parameter of the delegate <paramref name="body" />.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="source"/> is null -or- <paramref name="body"/> is null.
        /// </exception>
        public static void ForEach<TSource, T>(IEnumerable<TSource> source, Act<TSource, T> body, T arg)
        {
            ForEach(ThreadUtility.DefaultNumberOfConcurrentWorkerThreads, source, body, arg);
        }

        /// <summary>
        /// Provides a generic way of executing a parallel foreach-loop while providing ways to encapsulate and re-use existing code.
        /// </summary>
        /// <typeparam name="TSource">The type of the data in the <paramref name="source"/>.</typeparam>
        /// <typeparam name="T">The type of the parameter of the delegate <paramref name="body" />.</typeparam>
        /// <param name="partitionSize">The maximum number of concurrent worker threads per partition.</param>
        /// <param name="source">The sequence to iterate over parallel.</param>
        /// <param name="body">The delegate that is invoked once per iteration.</param>
        /// <param name="arg">The parameter of the delegate <paramref name="body" />.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="source"/> is null -or- <paramref name="body"/> is null.
        /// </exception>
        public static void ForEach<TSource, T>(int partitionSize, IEnumerable<TSource> source, Act<TSource, T> body, T arg)
        {
            ForEach(partitionSize, ThreadUtility.CreateSettings(), source, body, arg);
        }

        /// <summary>
        /// Provides a generic way of executing a parallel foreach-loop while providing ways to encapsulate and re-use existing code.
        /// </summary>
        /// <typeparam name="TSource">The type of the data in the <paramref name="source"/>.</typeparam>
        /// <typeparam name="T">The type of the parameter of the delegate <paramref name="body" />.</typeparam>
        /// <param name="partitionSize">The maximum number of concurrent worker threads per partition.</param>
        /// <param name="settings">The <see cref="ThreadSettings"/> object used to configure each new <see cref="Thread"/> instance.</param>
        /// <param name="source">The sequence to iterate over parallel.</param>
        /// <param name="body">The delegate that is invoked once per iteration.</param>
        /// <param name="arg">The parameter of the delegate <paramref name="body" />.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="settings"/> is null -or- <paramref name="source"/> is null -or- <paramref name="body"/> is null.
        /// </exception>
        public static void ForEach<TSource, T>(int partitionSize, ThreadSettings settings, IEnumerable<TSource> source, Act<TSource, T> body, T arg)
        {
            ForEach(partitionSize, settings, TimeSpan.MaxValue, source, body, arg);
        }

        /// <summary>
        /// Provides a generic way of executing a parallel foreach-loop while providing ways to encapsulate and re-use existing code.
        /// </summary>
        /// <typeparam name="TSource">The type of the data in the <paramref name="source"/>.</typeparam>
        /// <typeparam name="T">The type of the parameter of the delegate <paramref name="body" />.</typeparam>
        /// <param name="partitionSize">The maximum number of concurrent worker threads per partition.</param>
        /// <param name="settings">The <see cref="ThreadSettings"/> object used to configure each new <see cref="Thread"/> instance.</param>
        /// <param name="timeout">A <see cref="TimeSpan" /> that represents the time to wait for the parallel foreach-loop operation to complete.</param>
        /// <param name="source">The sequence to iterate over parallel.</param>
        /// <param name="body">The delegate that is invoked once per iteration.</param>
        /// <param name="arg">The parameter of the delegate <paramref name="body" />.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="settings"/> is null -or- <paramref name="source"/> is null -or- <paramref name="body"/> is null.
        /// </exception>
        public static void ForEach<TSource, T>(int partitionSize, ThreadSettings settings, TimeSpan timeout, IEnumerable<TSource> source, Act<TSource, T> body, T arg)
        {
            ValidateForEach(settings, source, body);
            var factory = ActFactory.Create(body, default(TSource), arg);
            ForEachCore(factory, source, partitionSize, timeout, settings);
        }

        /// <summary>
        /// Provides a generic way of executing a parallel foreach-loop while providing ways to encapsulate and re-use existing code.
        /// </summary>
        /// <typeparam name="TSource">The type of the data in the <paramref name="source"/>.</typeparam>
        /// <typeparam name="T1">The type of the first parameter of the delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the delegate <paramref name="body" />.</typeparam>
        /// <param name="source">The sequence to iterate over parallel.</param>
        /// <param name="body">The delegate that is invoked once per iteration.</param>
        /// <param name="arg1">The first parameter of the delegate <paramref name="body" />.</param>
        /// <param name="arg2">The second parameter of the delegate <paramref name="body" />.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="source"/> is null -or- <paramref name="body"/> is null.
        /// </exception>
        public static void ForEach<TSource, T1, T2>(IEnumerable<TSource> source, Act<TSource, T1, T2> body, T1 arg1, T2 arg2)
        {
            ForEach(ThreadUtility.DefaultNumberOfConcurrentWorkerThreads, source, body, arg1, arg2);
        }

        /// <summary>
        /// Provides a generic way of executing a parallel foreach-loop while providing ways to encapsulate and re-use existing code.
        /// </summary>
        /// <typeparam name="TSource">The type of the data in the <paramref name="source"/>.</typeparam>
        /// <typeparam name="T1">The type of the first parameter of the delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the delegate <paramref name="body" />.</typeparam>
        /// <param name="partitionSize">The maximum number of concurrent worker threads per partition.</param>
        /// <param name="source">The sequence to iterate over parallel.</param>
        /// <param name="body">The delegate that is invoked once per iteration.</param>
        /// <param name="arg1">The first parameter of the delegate <paramref name="body" />.</param>
        /// <param name="arg2">The second parameter of the delegate <paramref name="body" />.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="source"/> is null -or- <paramref name="body"/> is null.
        /// </exception>
        public static void ForEach<TSource, T1, T2>(int partitionSize, IEnumerable<TSource> source, Act<TSource, T1, T2> body, T1 arg1, T2 arg2)
        {
            ForEach(partitionSize, ThreadUtility.CreateSettings(), source, body, arg1, arg2);
        }

        /// <summary>
        /// Provides a generic way of executing a parallel foreach-loop while providing ways to encapsulate and re-use existing code.
        /// </summary>
        /// <typeparam name="TSource">The type of the data in the <paramref name="source"/>.</typeparam>
        /// <typeparam name="T1">The type of the first parameter of the delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the delegate <paramref name="body" />.</typeparam>
        /// <param name="partitionSize">The maximum number of concurrent worker threads per partition.</param>
        /// <param name="settings">The <see cref="ThreadSettings"/> object used to configure each new <see cref="Thread"/> instance.</param>
        /// <param name="source">The sequence to iterate over parallel.</param>
        /// <param name="body">The delegate that is invoked once per iteration.</param>
        /// <param name="arg1">The first parameter of the delegate <paramref name="body" />.</param>
        /// <param name="arg2">The second parameter of the delegate <paramref name="body" />.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="settings"/> is null -or- <paramref name="source"/> is null -or- <paramref name="body"/> is null.
        /// </exception>
        public static void ForEach<TSource, T1, T2>(int partitionSize, ThreadSettings settings, IEnumerable<TSource> source, Act<TSource, T1, T2> body, T1 arg1, T2 arg2)
        {
            ForEach(partitionSize, settings, TimeSpan.MaxValue, source, body, arg1, arg2);
        }

        /// <summary>
        /// Provides a generic way of executing a parallel foreach-loop while providing ways to encapsulate and re-use existing code.
        /// </summary>
        /// <typeparam name="TSource">The type of the data in the <paramref name="source"/>.</typeparam>
        /// <typeparam name="T1">The type of the first parameter of the delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the delegate <paramref name="body" />.</typeparam>
        /// <param name="partitionSize">The maximum number of concurrent worker threads per partition.</param>
        /// <param name="settings">The <see cref="ThreadSettings"/> object used to configure each new <see cref="Thread"/> instance.</param>
        /// <param name="timeout">A <see cref="TimeSpan" /> that represents the time to wait for the parallel foreach-loop operation to complete.</param>
        /// <param name="source">The sequence to iterate over parallel.</param>
        /// <param name="body">The delegate that is invoked once per iteration.</param>
        /// <param name="arg1">The first parameter of the delegate <paramref name="body" />.</param>
        /// <param name="arg2">The second parameter of the delegate <paramref name="body" />.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="settings"/> is null -or- <paramref name="source"/> is null -or- <paramref name="body"/> is null.
        /// </exception>
        public static void ForEach<TSource, T1, T2>(int partitionSize, ThreadSettings settings, TimeSpan timeout, IEnumerable<TSource> source, Act<TSource, T1, T2> body, T1 arg1, T2 arg2)
        {
            ValidateForEach(settings, source, body);
            var factory = ActFactory.Create(body, default(TSource), arg1, arg2);
            ForEachCore(factory, source, partitionSize, timeout, settings);
        }

        /// <summary>
        /// Provides a generic way of executing a parallel foreach-loop while providing ways to encapsulate and re-use existing code.
        /// </summary>
        /// <typeparam name="TSource">The type of the data in the <paramref name="source"/>.</typeparam>
        /// <typeparam name="T1">The type of the first parameter of the delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the delegate <paramref name="body" />.</typeparam>
        /// <param name="source">The sequence to iterate over parallel.</param>
        /// <param name="body">The delegate that is invoked once per iteration.</param>
        /// <param name="arg1">The first parameter of the delegate <paramref name="body" />.</param>
        /// <param name="arg2">The second parameter of the delegate <paramref name="body" />.</param>
        /// <param name="arg3">The third parameter of the delegate <paramref name="body" />.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="source"/> is null -or- <paramref name="body"/> is null.
        /// </exception>
        public static void ForEach<TSource, T1, T2, T3>(IEnumerable<TSource> source, Act<TSource, T1, T2, T3> body, T1 arg1, T2 arg2, T3 arg3)
        {
            ForEach(ThreadUtility.DefaultNumberOfConcurrentWorkerThreads, source, body, arg1, arg2, arg3);
        }

        /// <summary>
        /// Provides a generic way of executing a parallel foreach-loop while providing ways to encapsulate and re-use existing code.
        /// </summary>
        /// <typeparam name="TSource">The type of the data in the <paramref name="source"/>.</typeparam>
        /// <typeparam name="T1">The type of the first parameter of the delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the delegate <paramref name="body" />.</typeparam>
        /// <param name="partitionSize">The maximum number of concurrent worker threads per partition.</param>
        /// <param name="source">The sequence to iterate over parallel.</param>
        /// <param name="body">The delegate that is invoked once per iteration.</param>
        /// <param name="arg1">The first parameter of the delegate <paramref name="body" />.</param>
        /// <param name="arg2">The second parameter of the delegate <paramref name="body" />.</param>
        /// <param name="arg3">The third parameter of the delegate <paramref name="body" />.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="source"/> is null -or- <paramref name="body"/> is null.
        /// </exception>
        public static void ForEach<TSource, T1, T2, T3>(int partitionSize, IEnumerable<TSource> source, Act<TSource, T1, T2, T3> body, T1 arg1, T2 arg2, T3 arg3)
        {
            ForEach(partitionSize, ThreadUtility.CreateSettings(), source, body, arg1, arg2, arg3);
        }

        /// <summary>
        /// Provides a generic way of executing a parallel foreach-loop while providing ways to encapsulate and re-use existing code.
        /// </summary>
        /// <typeparam name="TSource">The type of the data in the <paramref name="source"/>.</typeparam>
        /// <typeparam name="T1">The type of the first parameter of the delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the delegate <paramref name="body" />.</typeparam>
        /// <param name="partitionSize">The maximum number of concurrent worker threads per partition.</param>
        /// <param name="settings">The <see cref="ThreadSettings"/> object used to configure each new <see cref="Thread"/> instance.</param>
        /// <param name="source">The sequence to iterate over parallel.</param>
        /// <param name="body">The delegate that is invoked once per iteration.</param>
        /// <param name="arg1">The first parameter of the delegate <paramref name="body" />.</param>
        /// <param name="arg2">The second parameter of the delegate <paramref name="body" />.</param>
        /// <param name="arg3">The third parameter of the delegate <paramref name="body" />.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="settings"/> is null -or- <paramref name="source"/> is null -or- <paramref name="body"/> is null.
        /// </exception>
        public static void ForEach<TSource, T1, T2, T3>(int partitionSize, ThreadSettings settings, IEnumerable<TSource> source, Act<TSource, T1, T2, T3> body, T1 arg1, T2 arg2, T3 arg3)
        {
            ForEach(partitionSize, settings, TimeSpan.MaxValue, source, body, arg1, arg2, arg3);
        }

        /// <summary>
        /// Provides a generic way of executing a parallel foreach-loop while providing ways to encapsulate and re-use existing code.
        /// </summary>
        /// <typeparam name="TSource">The type of the data in the <paramref name="source"/>.</typeparam>
        /// <typeparam name="T1">The type of the first parameter of the delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the delegate <paramref name="body" />.</typeparam>
        /// <param name="partitionSize">The maximum number of concurrent worker threads per partition.</param>
        /// <param name="settings">The <see cref="ThreadSettings"/> object used to configure each new <see cref="Thread"/> instance.</param>
        /// <param name="timeout">A <see cref="TimeSpan" /> that represents the time to wait for the parallel foreach-loop operation to complete.</param>
        /// <param name="source">The sequence to iterate over parallel.</param>
        /// <param name="body">The delegate that is invoked once per iteration.</param>
        /// <param name="arg1">The first parameter of the delegate <paramref name="body" />.</param>
        /// <param name="arg2">The second parameter of the delegate <paramref name="body" />.</param>
        /// <param name="arg3">The third parameter of the delegate <paramref name="body" />.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="settings"/> is null -or- <paramref name="source"/> is null -or- <paramref name="body"/> is null.
        /// </exception>
        public static void ForEach<TSource, T1, T2, T3>(int partitionSize, ThreadSettings settings, TimeSpan timeout, IEnumerable<TSource> source, Act<TSource, T1, T2, T3> body, T1 arg1, T2 arg2, T3 arg3)
        {
            ValidateForEach(settings, source, body);
            var factory = ActFactory.Create(body, default(TSource), arg1, arg2, arg3);
            ForEachCore(factory, source, partitionSize, timeout, settings);
        }

        /// <summary>
        /// Provides a generic way of executing a parallel foreach-loop while providing ways to encapsulate and re-use existing code.
        /// </summary>
        /// <typeparam name="TSource">The type of the data in the <paramref name="source"/>.</typeparam>
        /// <typeparam name="T1">The type of the first parameter of the delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of the delegate <paramref name="body" />.</typeparam>
        /// <param name="source">The sequence to iterate over parallel.</param>
        /// <param name="body">The delegate that is invoked once per iteration.</param>
        /// <param name="arg1">The first parameter of the delegate <paramref name="body" />.</param>
        /// <param name="arg2">The second parameter of the delegate <paramref name="body" />.</param>
        /// <param name="arg3">The third parameter of the delegate <paramref name="body" />.</param>
        /// <param name="arg4">The fourth parameter of the delegate <paramref name="body" />.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="source"/> is null -or- <paramref name="body"/> is null.
        /// </exception>
        public static void ForEach<TSource, T1, T2, T3, T4>(IEnumerable<TSource> source, Act<TSource, T1, T2, T3, T4> body, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            ForEach(ThreadUtility.DefaultNumberOfConcurrentWorkerThreads, source, body, arg1, arg2, arg3, arg4);
        }

        /// <summary>
        /// Provides a generic way of executing a parallel foreach-loop while providing ways to encapsulate and re-use existing code.
        /// </summary>
        /// <typeparam name="TSource">The type of the data in the <paramref name="source"/>.</typeparam>
        /// <typeparam name="T1">The type of the first parameter of the delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of the delegate <paramref name="body" />.</typeparam>
        /// <param name="partitionSize">The maximum number of concurrent worker threads per partition.</param>
        /// <param name="source">The sequence to iterate over parallel.</param>
        /// <param name="body">The delegate that is invoked once per iteration.</param>
        /// <param name="arg1">The first parameter of the delegate <paramref name="body" />.</param>
        /// <param name="arg2">The second parameter of the delegate <paramref name="body" />.</param>
        /// <param name="arg3">The third parameter of the delegate <paramref name="body" />.</param>
        /// <param name="arg4">The fourth parameter of the delegate <paramref name="body" />.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="source"/> is null -or- <paramref name="body"/> is null.
        /// </exception>
        public static void ForEach<TSource, T1, T2, T3, T4>(int partitionSize, IEnumerable<TSource> source, Act<TSource, T1, T2, T3, T4> body, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            ForEach(partitionSize, ThreadUtility.CreateSettings(), source, body, arg1, arg2, arg3, arg4);
        }

        /// <summary>
        /// Provides a generic way of executing a parallel foreach-loop while providing ways to encapsulate and re-use existing code.
        /// </summary>
        /// <typeparam name="TSource">The type of the data in the <paramref name="source"/>.</typeparam>
        /// <typeparam name="T1">The type of the first parameter of the delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of the delegate <paramref name="body" />.</typeparam>
        /// <param name="partitionSize">The maximum number of concurrent worker threads per partition.</param>
        /// <param name="settings">The <see cref="ThreadSettings"/> object used to configure each new <see cref="Thread"/> instance.</param>
        /// <param name="source">The sequence to iterate over parallel.</param>
        /// <param name="body">The delegate that is invoked once per iteration.</param>
        /// <param name="arg1">The first parameter of the delegate <paramref name="body" />.</param>
        /// <param name="arg2">The second parameter of the delegate <paramref name="body" />.</param>
        /// <param name="arg3">The third parameter of the delegate <paramref name="body" />.</param>
        /// <param name="arg4">The fourth parameter of the delegate <paramref name="body" />.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="settings"/> is null -or- <paramref name="source"/> is null -or- <paramref name="body"/> is null.
        /// </exception>
        public static void ForEach<TSource, T1, T2, T3, T4>(int partitionSize, ThreadSettings settings, IEnumerable<TSource> source, Act<TSource, T1, T2, T3, T4> body, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            ForEach(partitionSize, settings, TimeSpan.MaxValue, source, body, arg1, arg2, arg3, arg4);
        }

        /// <summary>
        /// Provides a generic way of executing a parallel foreach-loop while providing ways to encapsulate and re-use existing code.
        /// </summary>
        /// <typeparam name="TSource">The type of the data in the <paramref name="source"/>.</typeparam>
        /// <typeparam name="T1">The type of the first parameter of the delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of the delegate <paramref name="body" />.</typeparam>
        /// <param name="partitionSize">The maximum number of concurrent worker threads per partition.</param>
        /// <param name="settings">The <see cref="ThreadSettings"/> object used to configure each new <see cref="Thread"/> instance.</param>
        /// <param name="timeout">A <see cref="TimeSpan" /> that represents the time to wait for the parallel foreach-loop operation to complete.</param>
        /// <param name="source">The sequence to iterate over parallel.</param>
        /// <param name="body">The delegate that is invoked once per iteration.</param>
        /// <param name="arg1">The first parameter of the delegate <paramref name="body" />.</param>
        /// <param name="arg2">The second parameter of the delegate <paramref name="body" />.</param>
        /// <param name="arg3">The third parameter of the delegate <paramref name="body" />.</param>
        /// <param name="arg4">The fourth parameter of the delegate <paramref name="body" />.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="settings"/> is null -or- <paramref name="source"/> is null -or- <paramref name="body"/> is null.
        /// </exception>
        public static void ForEach<TSource, T1, T2, T3, T4>(int partitionSize, ThreadSettings settings, TimeSpan timeout, IEnumerable<TSource> source, Act<TSource, T1, T2, T3, T4> body, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            ValidateForEach(settings, source, body);
            var factory = ActFactory.Create(body, default(TSource), arg1, arg2, arg3, arg4);
            ForEachCore(factory, source, partitionSize, timeout, settings);
        }

        /// <summary>
        /// Provides a generic way of executing a parallel foreach-loop while providing ways to encapsulate and re-use existing code.
        /// </summary>
        /// <typeparam name="TSource">The type of the data in the <paramref name="source"/>.</typeparam>
        /// <typeparam name="T1">The type of the first parameter of the delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of the delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T5">The type of the fifth parameter of the delegate <paramref name="body" />.</typeparam>
        /// <param name="source">The sequence to iterate over parallel.</param>
        /// <param name="body">The delegate that is invoked once per iteration.</param>
        /// <param name="arg1">The first parameter of the delegate <paramref name="body" />.</param>
        /// <param name="arg2">The second parameter of the delegate <paramref name="body" />.</param>
        /// <param name="arg3">The third parameter of the delegate <paramref name="body" />.</param>
        /// <param name="arg4">The fourth parameter of the delegate <paramref name="body" />.</param>
        /// <param name="arg5">The fifth parameter of the delegate <paramref name="body" />.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="source"/> is null -or- <paramref name="body"/> is null.
        /// </exception>
        public static void ForEach<TSource, T1, T2, T3, T4, T5>(IEnumerable<TSource> source, Act<TSource, T1, T2, T3, T4, T5> body, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        {
            ForEach(ThreadUtility.DefaultNumberOfConcurrentWorkerThreads, source, body, arg1, arg2, arg3, arg4, arg5);
        }

        /// <summary>
        /// Provides a generic way of executing a parallel foreach-loop while providing ways to encapsulate and re-use existing code.
        /// </summary>
        /// <typeparam name="TSource">The type of the data in the <paramref name="source"/>.</typeparam>
        /// <typeparam name="T1">The type of the first parameter of the delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of the delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T5">The type of the fifth parameter of the delegate <paramref name="body" />.</typeparam>
        /// <param name="partitionSize">The maximum number of concurrent worker threads per partition.</param>
        /// <param name="source">The sequence to iterate over parallel.</param>
        /// <param name="body">The delegate that is invoked once per iteration.</param>
        /// <param name="arg1">The first parameter of the delegate <paramref name="body" />.</param>
        /// <param name="arg2">The second parameter of the delegate <paramref name="body" />.</param>
        /// <param name="arg3">The third parameter of the delegate <paramref name="body" />.</param>
        /// <param name="arg4">The fourth parameter of the delegate <paramref name="body" />.</param>
        /// <param name="arg5">The fifth parameter of the delegate <paramref name="body" />.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="source"/> is null -or- <paramref name="body"/> is null.
        /// </exception>
        public static void ForEach<TSource, T1, T2, T3, T4, T5>(int partitionSize, IEnumerable<TSource> source, Act<TSource, T1, T2, T3, T4, T5> body, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        {
            ForEach(partitionSize, ThreadUtility.CreateSettings(), source, body, arg1, arg2, arg3, arg4, arg5);
        }

        /// <summary>
        /// Provides a generic way of executing a parallel foreach-loop while providing ways to encapsulate and re-use existing code.
        /// </summary>
        /// <typeparam name="TSource">The type of the data in the <paramref name="source"/>.</typeparam>
        /// <typeparam name="T1">The type of the first parameter of the delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of the delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T5">The type of the fifth parameter of the delegate <paramref name="body" />.</typeparam>
        /// <param name="partitionSize">The maximum number of concurrent worker threads per partition.</param>
        /// <param name="settings">The <see cref="ThreadSettings"/> object used to configure each new <see cref="Thread"/> instance.</param>
        /// <param name="source">The sequence to iterate over parallel.</param>
        /// <param name="body">The delegate that is invoked once per iteration.</param>
        /// <param name="arg1">The first parameter of the delegate <paramref name="body" />.</param>
        /// <param name="arg2">The second parameter of the delegate <paramref name="body" />.</param>
        /// <param name="arg3">The third parameter of the delegate <paramref name="body" />.</param>
        /// <param name="arg4">The fourth parameter of the delegate <paramref name="body" />.</param>
        /// <param name="arg5">The fifth parameter of the delegate <paramref name="body" />.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="settings"/> is null -or- <paramref name="source"/> is null -or- <paramref name="body"/> is null.
        /// </exception>
        public static void ForEach<TSource, T1, T2, T3, T4, T5>(int partitionSize, ThreadSettings settings, IEnumerable<TSource> source, Act<TSource, T1, T2, T3, T4, T5> body, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        {
            ForEach(partitionSize, settings, TimeSpan.MaxValue, source, body, arg1, arg2, arg3, arg4, arg5);
        }

        /// <summary>
        /// Provides a generic way of executing a parallel foreach-loop while providing ways to encapsulate and re-use existing code.
        /// </summary>
        /// <typeparam name="TSource">The type of the data in the <paramref name="source"/>.</typeparam>
        /// <typeparam name="T1">The type of the first parameter of the delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of the delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T5">The type of the fifth parameter of the delegate <paramref name="body" />.</typeparam>
        /// <param name="partitionSize">The maximum number of concurrent worker threads per partition.</param>
        /// <param name="settings">The <see cref="ThreadSettings"/> object used to configure each new <see cref="Thread"/> instance.</param>
        /// <param name="timeout">A <see cref="TimeSpan" /> that represents the time to wait for the parallel foreach-loop operation to complete.</param>
        /// <param name="source">The sequence to iterate over parallel.</param>
        /// <param name="body">The delegate that is invoked once per iteration.</param>
        /// <param name="arg1">The first parameter of the delegate <paramref name="body" />.</param>
        /// <param name="arg2">The second parameter of the delegate <paramref name="body" />.</param>
        /// <param name="arg3">The third parameter of the delegate <paramref name="body" />.</param>
        /// <param name="arg4">The fourth parameter of the delegate <paramref name="body" />.</param>
        /// <param name="arg5">The fifth parameter of the delegate <paramref name="body" />.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="settings"/> is null -or- <paramref name="source"/> is null -or- <paramref name="body"/> is null.
        /// </exception>
        public static void ForEach<TSource, T1, T2, T3, T4, T5>(int partitionSize, ThreadSettings settings, TimeSpan timeout, IEnumerable<TSource> source, Act<TSource, T1, T2, T3, T4, T5> body, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        {
            ValidateForEach(settings, source, body);
            var factory = ActFactory.Create(body, default(TSource), arg1, arg2, arg3, arg4, arg5);
            ForEachCore(factory, source, partitionSize, timeout, settings);
        }

        /// <summary>
        /// Provides a generic way of executing a parallel foreach-loop while providing ways to encapsulate and re-use existing code.
        /// </summary>
        /// <typeparam name="TSource">The type of the data in the <paramref name="source"/>.</typeparam>
        /// <typeparam name="T1">The type of the first parameter of the delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of the delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T5">The type of the fifth parameter of the delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T6">The type of the sixth parameter of the delegate <paramref name="body" />.</typeparam>
        /// <param name="source">The sequence to iterate over parallel.</param>
        /// <param name="body">The delegate that is invoked once per iteration.</param>
        /// <param name="arg1">The first parameter of the delegate <paramref name="body" />.</param>
        /// <param name="arg2">The second parameter of the delegate <paramref name="body" />.</param>
        /// <param name="arg3">The third parameter of the delegate <paramref name="body" />.</param>
        /// <param name="arg4">The fourth parameter of the delegate <paramref name="body" />.</param>
        /// <param name="arg5">The fifth parameter of the delegate <paramref name="body" />.</param>
        /// <param name="arg6">The sixth parameter of the delegate <paramref name="body" />.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="source"/> is null -or- <paramref name="body"/> is null.
        /// </exception>
        public static void ForEach<TSource, T1, T2, T3, T4, T5, T6>(IEnumerable<TSource> source, Act<TSource, T1, T2, T3, T4, T5, T6> body, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6)
        {
            ForEach(ThreadUtility.DefaultNumberOfConcurrentWorkerThreads, source, body, arg1, arg2, arg3, arg4, arg5, arg6);
        }

        /// <summary>
        /// Provides a generic way of executing a parallel foreach-loop while providing ways to encapsulate and re-use existing code.
        /// </summary>
        /// <typeparam name="TSource">The type of the data in the <paramref name="source"/>.</typeparam>
        /// <typeparam name="T1">The type of the first parameter of the delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of the delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T5">The type of the fifth parameter of the delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T6">The type of the sixth parameter of the delegate <paramref name="body" />.</typeparam>
        /// <param name="partitionSize">The maximum number of concurrent worker threads per partition.</param>
        /// <param name="source">The sequence to iterate over parallel.</param>
        /// <param name="body">The delegate that is invoked once per iteration.</param>
        /// <param name="arg1">The first parameter of the delegate <paramref name="body" />.</param>
        /// <param name="arg2">The second parameter of the delegate <paramref name="body" />.</param>
        /// <param name="arg3">The third parameter of the delegate <paramref name="body" />.</param>
        /// <param name="arg4">The fourth parameter of the delegate <paramref name="body" />.</param>
        /// <param name="arg5">The fifth parameter of the delegate <paramref name="body" />.</param>
        /// <param name="arg6">The sixth parameter of the delegate <paramref name="body" />.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="source"/> is null -or- <paramref name="body"/> is null.
        /// </exception>
        public static void ForEach<TSource, T1, T2, T3, T4, T5, T6>(int partitionSize, IEnumerable<TSource> source, Act<TSource, T1, T2, T3, T4, T5, T6> body, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6)
        {
            ForEach(partitionSize, ThreadUtility.CreateSettings(), source, body, arg1, arg2, arg3, arg4, arg5, arg6);
        }

        /// <summary>
        /// Provides a generic way of executing a parallel foreach-loop while providing ways to encapsulate and re-use existing code.
        /// </summary>
        /// <typeparam name="TSource">The type of the data in the <paramref name="source"/>.</typeparam>
        /// <typeparam name="T1">The type of the first parameter of the delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of the delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T5">The type of the fifth parameter of the delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T6">The type of the sixth parameter of the delegate <paramref name="body" />.</typeparam>
        /// <param name="partitionSize">The maximum number of concurrent worker threads per partition.</param>
        /// <param name="settings">The <see cref="ThreadSettings"/> object used to configure each new <see cref="Thread"/> instance.</param>
        /// <param name="source">The sequence to iterate over parallel.</param>
        /// <param name="body">The delegate that is invoked once per iteration.</param>
        /// <param name="arg1">The first parameter of the delegate <paramref name="body" />.</param>
        /// <param name="arg2">The second parameter of the delegate <paramref name="body" />.</param>
        /// <param name="arg3">The third parameter of the delegate <paramref name="body" />.</param>
        /// <param name="arg4">The fourth parameter of the delegate <paramref name="body" />.</param>
        /// <param name="arg5">The fifth parameter of the delegate <paramref name="body" />.</param>
        /// <param name="arg6">The sixth parameter of the delegate <paramref name="body" />.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="settings"/> is null -or- <paramref name="source"/> is null -or- <paramref name="body"/> is null.
        /// </exception>
        public static void ForEach<TSource, T1, T2, T3, T4, T5, T6>(int partitionSize, ThreadSettings settings, IEnumerable<TSource> source, Act<TSource, T1, T2, T3, T4, T5, T6> body, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6)
        {
            ForEach(partitionSize, settings, TimeSpan.MaxValue, source, body, arg1, arg2, arg3, arg4, arg5, arg6);
        }

        /// <summary>
        /// Provides a generic way of executing a parallel foreach-loop while providing ways to encapsulate and re-use existing code.
        /// </summary>
        /// <typeparam name="TSource">The type of the data in the <paramref name="source"/>.</typeparam>
        /// <typeparam name="T1">The type of the first parameter of the delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of the delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T5">The type of the fifth parameter of the delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T6">The type of the sixth parameter of the delegate <paramref name="body" />.</typeparam>
        /// <param name="partitionSize">The maximum number of concurrent worker threads per partition.</param>
        /// <param name="settings">The <see cref="ThreadSettings"/> object used to configure each new <see cref="Thread"/> instance.</param>
        /// <param name="timeout">A <see cref="TimeSpan" /> that represents the time to wait for the parallel foreach-loop operation to complete.</param>
        /// <param name="source">The sequence to iterate over parallel.</param>
        /// <param name="body">The delegate that is invoked once per iteration.</param>
        /// <param name="arg1">The first parameter of the delegate <paramref name="body" />.</param>
        /// <param name="arg2">The second parameter of the delegate <paramref name="body" />.</param>
        /// <param name="arg3">The third parameter of the delegate <paramref name="body" />.</param>
        /// <param name="arg4">The fourth parameter of the delegate <paramref name="body" />.</param>
        /// <param name="arg5">The fifth parameter of the delegate <paramref name="body" />.</param>
        /// <param name="arg6">The sixth parameter of the delegate <paramref name="body" />.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="settings"/> is null -or- <paramref name="source"/> is null -or- <paramref name="body"/> is null.
        /// </exception>
        public static void ForEach<TSource, T1, T2, T3, T4, T5, T6>(int partitionSize, ThreadSettings settings, TimeSpan timeout, IEnumerable<TSource> source, Act<TSource, T1, T2, T3, T4, T5, T6> body, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6)
        {
            ValidateForEach(settings, source, body);
            var factory = ActFactory.Create(body, default(TSource), arg1, arg2, arg3, arg4, arg5, arg6);
            ForEachCore(factory, source, partitionSize, timeout, settings);
        }

        /// <summary>
        /// Provides a generic way of executing a parallel foreach-loop while providing ways to encapsulate and re-use existing code.
        /// </summary>
        /// <typeparam name="TSource">The type of the data in the <paramref name="source"/>.</typeparam>
        /// <typeparam name="T1">The type of the first parameter of the delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of the delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T5">The type of the fifth parameter of the delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T6">The type of the sixth parameter of the delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T7">The type of the seventh parameter of the delegate <paramref name="body" />.</typeparam>
        /// <param name="source">The sequence to iterate over parallel.</param>
        /// <param name="body">The delegate that is invoked once per iteration.</param>
        /// <param name="arg1">The first parameter of the delegate <paramref name="body" />.</param>
        /// <param name="arg2">The second parameter of the delegate <paramref name="body" />.</param>
        /// <param name="arg3">The third parameter of the delegate <paramref name="body" />.</param>
        /// <param name="arg4">The fourth parameter of the delegate <paramref name="body" />.</param>
        /// <param name="arg5">The fifth parameter of the delegate <paramref name="body" />.</param>
        /// <param name="arg6">The sixth parameter of the delegate <paramref name="body" />.</param>
        /// <param name="arg7">The seventh parameter of the delegate <paramref name="body" />.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="source"/> is null -or- <paramref name="body"/> is null.
        /// </exception>
        public static void ForEach<TSource, T1, T2, T3, T4, T5, T6, T7>(IEnumerable<TSource> source, Act<TSource, T1, T2, T3, T4, T5, T6, T7> body, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7)
        {
            ForEach(ThreadUtility.DefaultNumberOfConcurrentWorkerThreads, source, body, arg1, arg2, arg3, arg4, arg5, arg6, arg7);
        }

        /// <summary>
        /// Provides a generic way of executing a parallel foreach-loop while providing ways to encapsulate and re-use existing code.
        /// </summary>
        /// <typeparam name="TSource">The type of the data in the <paramref name="source"/>.</typeparam>
        /// <typeparam name="T1">The type of the first parameter of the delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of the delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T5">The type of the fifth parameter of the delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T6">The type of the sixth parameter of the delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T7">The type of the seventh parameter of the delegate <paramref name="body" />.</typeparam>
        /// <param name="partitionSize">The maximum number of concurrent worker threads per partition.</param>
        /// <param name="source">The sequence to iterate over parallel.</param>
        /// <param name="body">The delegate that is invoked once per iteration.</param>
        /// <param name="arg1">The first parameter of the delegate <paramref name="body" />.</param>
        /// <param name="arg2">The second parameter of the delegate <paramref name="body" />.</param>
        /// <param name="arg3">The third parameter of the delegate <paramref name="body" />.</param>
        /// <param name="arg4">The fourth parameter of the delegate <paramref name="body" />.</param>
        /// <param name="arg5">The fifth parameter of the delegate <paramref name="body" />.</param>
        /// <param name="arg6">The sixth parameter of the delegate <paramref name="body" />.</param>
        /// <param name="arg7">The seventh parameter of the delegate <paramref name="body" />.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="source"/> is null -or- <paramref name="body"/> is null.
        /// </exception>
        public static void ForEach<TSource, T1, T2, T3, T4, T5, T6, T7>(int partitionSize, IEnumerable<TSource> source, Act<TSource, T1, T2, T3, T4, T5, T6, T7> body, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7)
        {
            ForEach(partitionSize, ThreadUtility.CreateSettings(), source, body, arg1, arg2, arg3, arg4, arg5, arg6, arg7);
        }

        /// <summary>
        /// Provides a generic way of executing a parallel foreach-loop while providing ways to encapsulate and re-use existing code.
        /// </summary>
        /// <typeparam name="TSource">The type of the data in the <paramref name="source"/>.</typeparam>
        /// <typeparam name="T1">The type of the first parameter of the delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of the delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T5">The type of the fifth parameter of the delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T6">The type of the sixth parameter of the delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T7">The type of the seventh parameter of the delegate <paramref name="body" />.</typeparam>
        /// <param name="partitionSize">The maximum number of concurrent worker threads per partition.</param>
        /// <param name="settings">The <see cref="ThreadSettings"/> object used to configure each new <see cref="Thread"/> instance.</param>
        /// <param name="source">The sequence to iterate over parallel.</param>
        /// <param name="body">The delegate that is invoked once per iteration.</param>
        /// <param name="arg1">The first parameter of the delegate <paramref name="body" />.</param>
        /// <param name="arg2">The second parameter of the delegate <paramref name="body" />.</param>
        /// <param name="arg3">The third parameter of the delegate <paramref name="body" />.</param>
        /// <param name="arg4">The fourth parameter of the delegate <paramref name="body" />.</param>
        /// <param name="arg5">The fifth parameter of the delegate <paramref name="body" />.</param>
        /// <param name="arg6">The sixth parameter of the delegate <paramref name="body" />.</param>
        /// <param name="arg7">The seventh parameter of the delegate <paramref name="body" />.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="settings"/> is null -or- <paramref name="source"/> is null -or- <paramref name="body"/> is null.
        /// </exception>
        public static void ForEach<TSource, T1, T2, T3, T4, T5, T6, T7>(int partitionSize, ThreadSettings settings, IEnumerable<TSource> source, Act<TSource, T1, T2, T3, T4, T5, T6, T7> body, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7)
        {
            ForEach(partitionSize, settings, TimeSpan.MaxValue, source, body, arg1, arg2, arg3, arg4, arg5, arg6, arg7);
        }

        /// <summary>
        /// Provides a generic way of executing a parallel foreach-loop while providing ways to encapsulate and re-use existing code.
        /// </summary>
        /// <typeparam name="TSource">The type of the data in the <paramref name="source"/>.</typeparam>
        /// <typeparam name="T1">The type of the first parameter of the delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of the delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T5">The type of the fifth parameter of the delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T6">The type of the sixth parameter of the delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T7">The type of the seventh parameter of the delegate <paramref name="body" />.</typeparam>
        /// <param name="partitionSize">The maximum number of concurrent worker threads per partition.</param>
        /// <param name="settings">The <see cref="ThreadSettings"/> object used to configure each new <see cref="Thread"/> instance.</param>
        /// <param name="timeout">A <see cref="TimeSpan" /> that represents the time to wait for the parallel foreach-loop operation to complete.</param>
        /// <param name="source">The sequence to iterate over parallel.</param>
        /// <param name="body">The delegate that is invoked once per iteration.</param>
        /// <param name="arg1">The first parameter of the delegate <paramref name="body" />.</param>
        /// <param name="arg2">The second parameter of the delegate <paramref name="body" />.</param>
        /// <param name="arg3">The third parameter of the delegate <paramref name="body" />.</param>
        /// <param name="arg4">The fourth parameter of the delegate <paramref name="body" />.</param>
        /// <param name="arg5">The fifth parameter of the delegate <paramref name="body" />.</param>
        /// <param name="arg6">The sixth parameter of the delegate <paramref name="body" />.</param>
        /// <param name="arg7">The seventh parameter of the delegate <paramref name="body" />.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="settings"/> is null -or- <paramref name="source"/> is null -or- <paramref name="body"/> is null.
        /// </exception>
        public static void ForEach<TSource, T1, T2, T3, T4, T5, T6, T7>(int partitionSize, ThreadSettings settings, TimeSpan timeout, IEnumerable<TSource> source, Act<TSource, T1, T2, T3, T4, T5, T6, T7> body, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7)
        {
            ValidateForEach(settings, source, body);
            var factory = ActFactory.Create(body, default(TSource), arg1, arg2, arg3, arg4, arg5, arg6, arg7);
            ForEachCore(factory, source, partitionSize, timeout, settings);
        }

        /// <summary>
        /// Provides a generic way of executing a parallel foreach-loop while providing ways to encapsulate and re-use existing code.
        /// </summary>
        /// <typeparam name="TSource">The type of the data in the <paramref name="source"/>.</typeparam>
        /// <typeparam name="T1">The type of the first parameter of the delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of the delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T5">The type of the fifth parameter of the delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T6">The type of the sixth parameter of the delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T7">The type of the seventh parameter of the delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T8">The type of the eighth parameter of the delegate <paramref name="body" />.</typeparam>
        /// <param name="source">The sequence to iterate over parallel.</param>
        /// <param name="body">The delegate that is invoked once per iteration.</param>
        /// <param name="arg1">The first parameter of the delegate <paramref name="body" />.</param>
        /// <param name="arg2">The second parameter of the delegate <paramref name="body" />.</param>
        /// <param name="arg3">The third parameter of the delegate <paramref name="body" />.</param>
        /// <param name="arg4">The fourth parameter of the delegate <paramref name="body" />.</param>
        /// <param name="arg5">The fifth parameter of the delegate <paramref name="body" />.</param>
        /// <param name="arg6">The sixth parameter of the delegate <paramref name="body" />.</param>
        /// <param name="arg7">The seventh parameter of the delegate <paramref name="body" />.</param>
        /// <param name="arg8">The eighth parameter of the delegate <paramref name="body" />.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="source"/> is null -or- <paramref name="body"/> is null.
        /// </exception>
        public static void ForEach<TSource, T1, T2, T3, T4, T5, T6, T7, T8>(IEnumerable<TSource> source, Act<TSource, T1, T2, T3, T4, T5, T6, T7, T8> body, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8)
        {
            ForEach(ThreadUtility.DefaultNumberOfConcurrentWorkerThreads, source, body, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8);
        }

        /// <summary>
        /// Provides a generic way of executing a parallel foreach-loop while providing ways to encapsulate and re-use existing code.
        /// </summary>
        /// <typeparam name="TSource">The type of the data in the <paramref name="source"/>.</typeparam>
        /// <typeparam name="T1">The type of the first parameter of the delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of the delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T5">The type of the fifth parameter of the delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T6">The type of the sixth parameter of the delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T7">The type of the seventh parameter of the delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T8">The type of the eighth parameter of the delegate <paramref name="body" />.</typeparam>
        /// <param name="partitionSize">The maximum number of concurrent worker threads per partition.</param>
        /// <param name="source">The sequence to iterate over parallel.</param>
        /// <param name="body">The delegate that is invoked once per iteration.</param>
        /// <param name="arg1">The first parameter of the delegate <paramref name="body" />.</param>
        /// <param name="arg2">The second parameter of the delegate <paramref name="body" />.</param>
        /// <param name="arg3">The third parameter of the delegate <paramref name="body" />.</param>
        /// <param name="arg4">The fourth parameter of the delegate <paramref name="body" />.</param>
        /// <param name="arg5">The fifth parameter of the delegate <paramref name="body" />.</param>
        /// <param name="arg6">The sixth parameter of the delegate <paramref name="body" />.</param>
        /// <param name="arg7">The seventh parameter of the delegate <paramref name="body" />.</param>
        /// <param name="arg8">The eighth parameter of the delegate <paramref name="body" />.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="source"/> is null -or- <paramref name="body"/> is null.
        /// </exception>
        public static void ForEach<TSource, T1, T2, T3, T4, T5, T6, T7, T8>(int partitionSize, IEnumerable<TSource> source, Act<TSource, T1, T2, T3, T4, T5, T6, T7, T8> body, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8)
        {
            ForEach(partitionSize, ThreadUtility.CreateSettings(), source, body, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8);
        }

        /// <summary>
        /// Provides a generic way of executing a parallel foreach-loop while providing ways to encapsulate and re-use existing code.
        /// </summary>
        /// <typeparam name="TSource">The type of the data in the <paramref name="source"/>.</typeparam>
        /// <typeparam name="T1">The type of the first parameter of the delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of the delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T5">The type of the fifth parameter of the delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T6">The type of the sixth parameter of the delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T7">The type of the seventh parameter of the delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T8">The type of the eighth parameter of the delegate <paramref name="body" />.</typeparam>
        /// <param name="partitionSize">The maximum number of concurrent worker threads per partition.</param>
        /// <param name="settings">The <see cref="ThreadSettings"/> object used to configure each new <see cref="Thread"/> instance.</param>
        /// <param name="source">The sequence to iterate over parallel.</param>
        /// <param name="body">The delegate that is invoked once per iteration.</param>
        /// <param name="arg1">The first parameter of the delegate <paramref name="body" />.</param>
        /// <param name="arg2">The second parameter of the delegate <paramref name="body" />.</param>
        /// <param name="arg3">The third parameter of the delegate <paramref name="body" />.</param>
        /// <param name="arg4">The fourth parameter of the delegate <paramref name="body" />.</param>
        /// <param name="arg5">The fifth parameter of the delegate <paramref name="body" />.</param>
        /// <param name="arg6">The sixth parameter of the delegate <paramref name="body" />.</param>
        /// <param name="arg7">The seventh parameter of the delegate <paramref name="body" />.</param>
        /// <param name="arg8">The eighth parameter of the delegate <paramref name="body" />.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="settings"/> is null -or- <paramref name="source"/> is null -or- <paramref name="body"/> is null.
        /// </exception>
        public static void ForEach<TSource, T1, T2, T3, T4, T5, T6, T7, T8>(int partitionSize, ThreadSettings settings, IEnumerable<TSource> source, Act<TSource, T1, T2, T3, T4, T5, T6, T7, T8> body, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8)
        {
            ForEach(partitionSize, settings, TimeSpan.MaxValue, source, body, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8);
        }

        /// <summary>
        /// Provides a generic way of executing a parallel foreach-loop while providing ways to encapsulate and re-use existing code.
        /// </summary>
        /// <typeparam name="TSource">The type of the data in the <paramref name="source"/>.</typeparam>
        /// <typeparam name="T1">The type of the first parameter of the delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of the delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T5">The type of the fifth parameter of the delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T6">The type of the sixth parameter of the delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T7">The type of the seventh parameter of the delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T8">The type of the eighth parameter of the delegate <paramref name="body" />.</typeparam>
        /// <param name="partitionSize">The maximum number of concurrent worker threads per partition.</param>
        /// <param name="settings">The <see cref="ThreadSettings"/> object used to configure each new <see cref="Thread"/> instance.</param>
        /// <param name="timeout">A <see cref="TimeSpan" /> that represents the time to wait for the parallel foreach-loop operation to complete.</param>
        /// <param name="source">The sequence to iterate over parallel.</param>
        /// <param name="body">The delegate that is invoked once per iteration.</param>
        /// <param name="arg1">The first parameter of the delegate <paramref name="body" />.</param>
        /// <param name="arg2">The second parameter of the delegate <paramref name="body" />.</param>
        /// <param name="arg3">The third parameter of the delegate <paramref name="body" />.</param>
        /// <param name="arg4">The fourth parameter of the delegate <paramref name="body" />.</param>
        /// <param name="arg5">The fifth parameter of the delegate <paramref name="body" />.</param>
        /// <param name="arg6">The sixth parameter of the delegate <paramref name="body" />.</param>
        /// <param name="arg7">The seventh parameter of the delegate <paramref name="body" />.</param>
        /// <param name="arg8">The eighth parameter of the delegate <paramref name="body" />.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="settings"/> is null -or- <paramref name="source"/> is null -or- <paramref name="body"/> is null.
        /// </exception>
        public static void ForEach<TSource, T1, T2, T3, T4, T5, T6, T7, T8>(int partitionSize, ThreadSettings settings, TimeSpan timeout, IEnumerable<TSource> source, Act<TSource, T1, T2, T3, T4, T5, T6, T7, T8> body, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8)
        {
            ValidateForEach(settings, source, body);
            var factory = ActFactory.Create(body, default(TSource), arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8);
            ForEachCore(factory, source, partitionSize, timeout, settings);
        }

        /// <summary>
        /// Provides a generic way of executing a parallel foreach-loop while providing ways to encapsulate and re-use existing code.
        /// </summary>
        /// <typeparam name="TSource">The type of the data in the <paramref name="source"/>.</typeparam>
        /// <typeparam name="T1">The type of the first parameter of the delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of the delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T5">The type of the fifth parameter of the delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T6">The type of the sixth parameter of the delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T7">The type of the seventh parameter of the delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T8">The type of the eighth parameter of the delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T9">The type of the ninth parameter of the delegate <paramref name="body" />.</typeparam>
        /// <param name="source">The sequence to iterate over parallel.</param>
        /// <param name="body">The delegate that is invoked once per iteration.</param>
        /// <param name="arg1">The first parameter of the delegate <paramref name="body" />.</param>
        /// <param name="arg2">The second parameter of the delegate <paramref name="body" />.</param>
        /// <param name="arg3">The third parameter of the delegate <paramref name="body" />.</param>
        /// <param name="arg4">The fourth parameter of the delegate <paramref name="body" />.</param>
        /// <param name="arg5">The fifth parameter of the delegate <paramref name="body" />.</param>
        /// <param name="arg6">The sixth parameter of the delegate <paramref name="body" />.</param>
        /// <param name="arg7">The seventh parameter of the delegate <paramref name="body" />.</param>
        /// <param name="arg8">The eighth parameter of the delegate <paramref name="body" />.</param>
        /// <param name="arg9">The ninth parameter of the delegate <paramref name="body" />.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="source"/> is null -or- <paramref name="body"/> is null.
        /// </exception>
        public static void ForEach<TSource, T1, T2, T3, T4, T5, T6, T7, T8, T9>(IEnumerable<TSource> source, Act<TSource, T1, T2, T3, T4, T5, T6, T7, T8, T9> body, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9)
        {
            ForEach(ThreadUtility.DefaultNumberOfConcurrentWorkerThreads, source, body, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9);
        }

        /// <summary>
        /// Provides a generic way of executing a parallel foreach-loop while providing ways to encapsulate and re-use existing code.
        /// </summary>
        /// <typeparam name="TSource">The type of the data in the <paramref name="source"/>.</typeparam>
        /// <typeparam name="T1">The type of the first parameter of the delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of the delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T5">The type of the fifth parameter of the delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T6">The type of the sixth parameter of the delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T7">The type of the seventh parameter of the delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T8">The type of the eighth parameter of the delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T9">The type of the ninth parameter of the delegate <paramref name="body" />.</typeparam>
        /// <param name="partitionSize">The maximum number of concurrent worker threads per partition.</param>
        /// <param name="source">The sequence to iterate over parallel.</param>
        /// <param name="body">The delegate that is invoked once per iteration.</param>
        /// <param name="arg1">The first parameter of the delegate <paramref name="body" />.</param>
        /// <param name="arg2">The second parameter of the delegate <paramref name="body" />.</param>
        /// <param name="arg3">The third parameter of the delegate <paramref name="body" />.</param>
        /// <param name="arg4">The fourth parameter of the delegate <paramref name="body" />.</param>
        /// <param name="arg5">The fifth parameter of the delegate <paramref name="body" />.</param>
        /// <param name="arg6">The sixth parameter of the delegate <paramref name="body" />.</param>
        /// <param name="arg7">The seventh parameter of the delegate <paramref name="body" />.</param>
        /// <param name="arg8">The eighth parameter of the delegate <paramref name="body" />.</param>
        /// <param name="arg9">The ninth parameter of the delegate <paramref name="body" />.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="source"/> is null -or- <paramref name="body"/> is null.
        /// </exception>
        public static void ForEach<TSource, T1, T2, T3, T4, T5, T6, T7, T8, T9>(int partitionSize, IEnumerable<TSource> source, Act<TSource, T1, T2, T3, T4, T5, T6, T7, T8, T9> body, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9)
        {
            ForEach(partitionSize, ThreadUtility.CreateSettings(), source, body, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9);
        }

        /// <summary>
        /// Provides a generic way of executing a parallel foreach-loop while providing ways to encapsulate and re-use existing code.
        /// </summary>
        /// <typeparam name="TSource">The type of the data in the <paramref name="source"/>.</typeparam>
        /// <typeparam name="T1">The type of the first parameter of the delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of the delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T5">The type of the fifth parameter of the delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T6">The type of the sixth parameter of the delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T7">The type of the seventh parameter of the delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T8">The type of the eighth parameter of the delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T9">The type of the ninth parameter of the delegate <paramref name="body" />.</typeparam>
        /// <param name="partitionSize">The maximum number of concurrent worker threads per partition.</param>
        /// <param name="settings">The <see cref="ThreadSettings"/> object used to configure each new <see cref="Thread"/> instance.</param>
        /// <param name="source">The sequence to iterate over parallel.</param>
        /// <param name="body">The delegate that is invoked once per iteration.</param>
        /// <param name="arg1">The first parameter of the delegate <paramref name="body" />.</param>
        /// <param name="arg2">The second parameter of the delegate <paramref name="body" />.</param>
        /// <param name="arg3">The third parameter of the delegate <paramref name="body" />.</param>
        /// <param name="arg4">The fourth parameter of the delegate <paramref name="body" />.</param>
        /// <param name="arg5">The fifth parameter of the delegate <paramref name="body" />.</param>
        /// <param name="arg6">The sixth parameter of the delegate <paramref name="body" />.</param>
        /// <param name="arg7">The seventh parameter of the delegate <paramref name="body" />.</param>
        /// <param name="arg8">The eighth parameter of the delegate <paramref name="body" />.</param>
        /// <param name="arg9">The ninth parameter of the delegate <paramref name="body" />.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="settings"/> is null -or- <paramref name="source"/> is null -or- <paramref name="body"/> is null.
        /// </exception>
        public static void ForEach<TSource, T1, T2, T3, T4, T5, T6, T7, T8, T9>(int partitionSize, ThreadSettings settings, IEnumerable<TSource> source, Act<TSource, T1, T2, T3, T4, T5, T6, T7, T8, T9> body, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9)
        {
            ForEach(partitionSize, settings, TimeSpan.MaxValue, source, body, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9);
        }

        /// <summary>
        /// Provides a generic way of executing a parallel foreach-loop while providing ways to encapsulate and re-use existing code.
        /// </summary>
        /// <typeparam name="TSource">The type of the data in the <paramref name="source"/>.</typeparam>
        /// <typeparam name="T1">The type of the first parameter of the delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of the delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T5">The type of the fifth parameter of the delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T6">The type of the sixth parameter of the delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T7">The type of the seventh parameter of the delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T8">The type of the eighth parameter of the delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T9">The type of the ninth parameter of the delegate <paramref name="body" />.</typeparam>
        /// <param name="partitionSize">The maximum number of concurrent worker threads per partition.</param>
        /// <param name="settings">The <see cref="ThreadSettings"/> object used to configure each new <see cref="Thread"/> instance.</param>
        /// <param name="timeout">A <see cref="TimeSpan" /> that represents the time to wait for the parallel foreach-loop operation to complete.</param>
        /// <param name="source">The sequence to iterate over parallel.</param>
        /// <param name="body">The delegate that is invoked once per iteration.</param>
        /// <param name="arg1">The first parameter of the delegate <paramref name="body" />.</param>
        /// <param name="arg2">The second parameter of the delegate <paramref name="body" />.</param>
        /// <param name="arg3">The third parameter of the delegate <paramref name="body" />.</param>
        /// <param name="arg4">The fourth parameter of the delegate <paramref name="body" />.</param>
        /// <param name="arg5">The fifth parameter of the delegate <paramref name="body" />.</param>
        /// <param name="arg6">The sixth parameter of the delegate <paramref name="body" />.</param>
        /// <param name="arg7">The seventh parameter of the delegate <paramref name="body" />.</param>
        /// <param name="arg8">The eighth parameter of the delegate <paramref name="body" />.</param>
        /// <param name="arg9">The ninth parameter of the delegate <paramref name="body" />.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="settings"/> is null -or- <paramref name="source"/> is null -or- <paramref name="body"/> is null.
        /// </exception>
        public static void ForEach<TSource, T1, T2, T3, T4, T5, T6, T7, T8, T9>(int partitionSize, ThreadSettings settings, TimeSpan timeout, IEnumerable<TSource> source, Act<TSource, T1, T2, T3, T4, T5, T6, T7, T8, T9> body, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9)
        {
            ValidateForEach(settings, source, body);
            var factory = ActFactory.Create(body, default(TSource), arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9);
            ForEachCore(factory, source, partitionSize, timeout, settings);
        }

        /// <summary>
        /// Provides a generic way of executing a parallel foreach-loop while providing ways to encapsulate and re-use existing code.
        /// </summary>
        /// <typeparam name="TSource">The type of the data in the <paramref name="source"/>.</typeparam>
        /// <typeparam name="T1">The type of the first parameter of the delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of the delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T5">The type of the fifth parameter of the delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T6">The type of the sixth parameter of the delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T7">The type of the seventh parameter of the delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T8">The type of the eighth parameter of the delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T9">The type of the ninth parameter of the delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T10">The type of the tenth parameter of the delegate <paramref name="body" />.</typeparam>
        /// <param name="source">The sequence to iterate over parallel.</param>
        /// <param name="body">The delegate that is invoked once per iteration.</param>
        /// <param name="arg1">The first parameter of the delegate <paramref name="body" />.</param>
        /// <param name="arg2">The second parameter of the delegate <paramref name="body" />.</param>
        /// <param name="arg3">The third parameter of the delegate <paramref name="body" />.</param>
        /// <param name="arg4">The fourth parameter of the delegate <paramref name="body" />.</param>
        /// <param name="arg5">The fifth parameter of the delegate <paramref name="body" />.</param>
        /// <param name="arg6">The sixth parameter of the delegate <paramref name="body" />.</param>
        /// <param name="arg7">The seventh parameter of the delegate <paramref name="body" />.</param>
        /// <param name="arg8">The eighth parameter of the delegate <paramref name="body" />.</param>
        /// <param name="arg9">The ninth parameter of the delegate <paramref name="body" />.</param>
        /// <param name="arg10">The tenth parameter of the delegate <paramref name="body" />.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="source"/> is null -or- <paramref name="body"/> is null.
        /// </exception>
        public static void ForEach<TSource, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(IEnumerable<TSource> source, Act<TSource, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> body, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10)
        {
            ForEach(ThreadUtility.DefaultNumberOfConcurrentWorkerThreads, source, body, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10);
        }

        /// <summary>
        /// Provides a generic way of executing a parallel foreach-loop while providing ways to encapsulate and re-use existing code.
        /// </summary>
        /// <typeparam name="TSource">The type of the data in the <paramref name="source"/>.</typeparam>
        /// <typeparam name="T1">The type of the first parameter of the delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of the delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T5">The type of the fifth parameter of the delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T6">The type of the sixth parameter of the delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T7">The type of the seventh parameter of the delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T8">The type of the eighth parameter of the delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T9">The type of the ninth parameter of the delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T10">The type of the tenth parameter of the delegate <paramref name="body" />.</typeparam>
        /// <param name="partitionSize">The maximum number of concurrent worker threads per partition.</param>
        /// <param name="source">The sequence to iterate over parallel.</param>
        /// <param name="body">The delegate that is invoked once per iteration.</param>
        /// <param name="arg1">The first parameter of the delegate <paramref name="body" />.</param>
        /// <param name="arg2">The second parameter of the delegate <paramref name="body" />.</param>
        /// <param name="arg3">The third parameter of the delegate <paramref name="body" />.</param>
        /// <param name="arg4">The fourth parameter of the delegate <paramref name="body" />.</param>
        /// <param name="arg5">The fifth parameter of the delegate <paramref name="body" />.</param>
        /// <param name="arg6">The sixth parameter of the delegate <paramref name="body" />.</param>
        /// <param name="arg7">The seventh parameter of the delegate <paramref name="body" />.</param>
        /// <param name="arg8">The eighth parameter of the delegate <paramref name="body" />.</param>
        /// <param name="arg9">The ninth parameter of the delegate <paramref name="body" />.</param>
        /// <param name="arg10">The tenth parameter of the delegate <paramref name="body" />.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="source"/> is null -or- <paramref name="body"/> is null.
        /// </exception>
        public static void ForEach<TSource, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(int partitionSize, IEnumerable<TSource> source, Act<TSource, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> body, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10)
        {
            ForEach(partitionSize, ThreadUtility.CreateSettings(), source, body, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10);
        }

        /// <summary>
        /// Provides a generic way of executing a parallel foreach-loop while providing ways to encapsulate and re-use existing code.
        /// </summary>
        /// <typeparam name="TSource">The type of the data in the <paramref name="source"/>.</typeparam>
        /// <typeparam name="T1">The type of the first parameter of the delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of the delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T5">The type of the fifth parameter of the delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T6">The type of the sixth parameter of the delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T7">The type of the seventh parameter of the delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T8">The type of the eighth parameter of the delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T9">The type of the ninth parameter of the delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T10">The type of the tenth parameter of the delegate <paramref name="body" />.</typeparam>
        /// <param name="partitionSize">The maximum number of concurrent worker threads per partition.</param>
        /// <param name="settings">The <see cref="ThreadSettings"/> object used to configure each new <see cref="Thread"/> instance.</param>
        /// <param name="source">The sequence to iterate over parallel.</param>
        /// <param name="body">The delegate that is invoked once per iteration.</param>
        /// <param name="arg1">The first parameter of the delegate <paramref name="body" />.</param>
        /// <param name="arg2">The second parameter of the delegate <paramref name="body" />.</param>
        /// <param name="arg3">The third parameter of the delegate <paramref name="body" />.</param>
        /// <param name="arg4">The fourth parameter of the delegate <paramref name="body" />.</param>
        /// <param name="arg5">The fifth parameter of the delegate <paramref name="body" />.</param>
        /// <param name="arg6">The sixth parameter of the delegate <paramref name="body" />.</param>
        /// <param name="arg7">The seventh parameter of the delegate <paramref name="body" />.</param>
        /// <param name="arg8">The eighth parameter of the delegate <paramref name="body" />.</param>
        /// <param name="arg9">The ninth parameter of the delegate <paramref name="body" />.</param>
        /// <param name="arg10">The tenth parameter of the delegate <paramref name="body" />.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="settings"/> is null -or- <paramref name="source"/> is null -or- <paramref name="body"/> is null.
        /// </exception>
        public static void ForEach<TSource, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(int partitionSize, ThreadSettings settings, IEnumerable<TSource> source, Act<TSource, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> body, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10)
        {
            ForEach(partitionSize, settings, TimeSpan.MaxValue, source, body, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10);
        }

        /// <summary>
        /// Provides a generic way of executing a parallel foreach-loop while providing ways to encapsulate and re-use existing code.
        /// </summary>
        /// <typeparam name="TSource">The type of the data in the <paramref name="source"/>.</typeparam>
        /// <typeparam name="T1">The type of the first parameter of the delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of the delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T5">The type of the fifth parameter of the delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T6">The type of the sixth parameter of the delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T7">The type of the seventh parameter of the delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T8">The type of the eighth parameter of the delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T9">The type of the ninth parameter of the delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T10">The type of the tenth parameter of the delegate <paramref name="body" />.</typeparam>
        /// <param name="partitionSize">The maximum number of concurrent worker threads per partition.</param>
        /// <param name="settings">The <see cref="ThreadSettings"/> object used to configure each new <see cref="Thread"/> instance.</param>
        /// <param name="timeout">A <see cref="TimeSpan" /> that represents the time to wait for the parallel foreach-loop operation to complete.</param>
        /// <param name="source">The sequence to iterate over parallel.</param>
        /// <param name="body">The delegate that is invoked once per iteration.</param>
        /// <param name="arg1">The first parameter of the delegate <paramref name="body" />.</param>
        /// <param name="arg2">The second parameter of the delegate <paramref name="body" />.</param>
        /// <param name="arg3">The third parameter of the delegate <paramref name="body" />.</param>
        /// <param name="arg4">The fourth parameter of the delegate <paramref name="body" />.</param>
        /// <param name="arg5">The fifth parameter of the delegate <paramref name="body" />.</param>
        /// <param name="arg6">The sixth parameter of the delegate <paramref name="body" />.</param>
        /// <param name="arg7">The seventh parameter of the delegate <paramref name="body" />.</param>
        /// <param name="arg8">The eighth parameter of the delegate <paramref name="body" />.</param>
        /// <param name="arg9">The ninth parameter of the delegate <paramref name="body" />.</param>
        /// <param name="arg10">The tenth parameter of the delegate <paramref name="body" />.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="settings"/> is null -or- <paramref name="source"/> is null -or- <paramref name="body"/> is null.
        /// </exception>
        public static void ForEach<TSource, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(int partitionSize, ThreadSettings settings, TimeSpan timeout, IEnumerable<TSource> source, Act<TSource, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> body, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10)
        {
            ValidateForEach(settings, source, body);
            var factory = ActFactory.Create(body, default(TSource), arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10);
            ForEachCore(factory, source, partitionSize, timeout, settings);
        }

        private static void ValidateForEach<TSource>(ThreadSettings settings, IEnumerable<TSource> source, object body)
        {
            Validator.ThrowIfNull(settings, nameof(settings));
            Validator.ThrowIfNull(source, nameof(source));
            Validator.ThrowIfNull(body, nameof(body));
        }

        private static void ForEachCore<TTuple, TSource>(ActFactory<TTuple> factory, IEnumerable<TSource> source, int partitionSize, TimeSpan timeout, ThreadSettings settings) where TTuple : Template<TSource>
        {
            List<Thread> threads = new List<Thread>();
            PartitionCollection<TSource> partition = new PartitionCollection<TSource>(source, partitionSize);
            List<Exception> aggregatedExceptions = new List<Exception>();
            while (partition.HasPartitions)
            {
                foreach (TSource element in partition)
                {
                    factory.GenericArguments.Arg1 = element;
                    var shallowFactory = factory.Clone();
                    Thread elementThread = ThreadUtility.StartNew(() =>
                    {
                        try
                        {
                            shallowFactory.ExecuteMethod();
                        }
                        catch (Exception te)
                        {
                            lock (aggregatedExceptions)
                            {
                                aggregatedExceptions.Add(te);
                            }
                        }
                    });
                    threads.Add(elementThread);
                }

                bool useParameterlessJoin = timeout == TimeSpan.MaxValue;
                foreach (Thread thread in threads)
                {
                    if (useParameterlessJoin)
                    {
                        thread.Join();
                    }
                    else
                    {
                        thread.Join(timeout);
                    }
                }
                threads.Clear();
            }
        }
    }
}