﻿using System;
using System.Collections.Generic;
using System.Threading;
using Cuemon.Collections.Generic;

namespace Cuemon.Threading
{
    public static partial class ParallelThreadPool
    {
        /// <summary>
        /// Provides a generic way of executing a parallel foreach-loop while providing ways to encapsulate and re-use existing code having a return value.
        /// </summary>
        /// <typeparam name="TSource">The type of the data in the <paramref name="source"/>.</typeparam>
        /// <typeparam name="TResult">The type of the return value of the <paramref name="body"/> that this function delegate encapsulates.</typeparam>
        /// <param name="source">The sequence to iterate over parallel.</param>
        /// <param name="body">The delegate that is invoked once per iteration.</param>
        /// <returns>A <see cref="IReadOnlyCollection{TResult}"/> where the result of <paramref name="body"/> has been stored in the same sequential order as <paramref name="source"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="source"/> is null -or- <paramref name="body"/> is null.
        /// </exception>
        /// <remarks>
        /// The following table shows the initial overloaded arguments for <see cref="ForEach{TSource}(System.Collections.Generic.IEnumerable{TSource},Cuemon.Act{TSource})"/>.
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
        ///         <description><see cref="ThreadPoolUtility.GetSettings"/> with some adaption to <c>partitionSize</c>.</description>
        ///     </item>
        ///     <item>
        ///         <term>timeout</term>
        ///         <description><see cref="TimeSpan.MaxValue"/></description>
        ///     </item>
        /// </list>
        /// </remarks>
        public static IReadOnlyCollection<TResult> ForEachTask<TSource, TResult>(IEnumerable<TSource> source, Doer<TSource, TResult> body)
        {
            return ForEachTask(ThreadUtility.DefaultNumberOfConcurrentWorkerThreads, source, body);
        }

        /// <summary>
        /// Provides a generic way of executing a parallel foreach-loop while providing ways to encapsulate and re-use existing code having a return value.
        /// </summary>
        /// <typeparam name="TSource">The type of the data in the <paramref name="source"/>.</typeparam>
        /// <typeparam name="TResult">The type of the return value of the <paramref name="body"/> that this function delegate encapsulates.</typeparam>
        /// <param name="partitionSize">The maximum number of concurrent worker threads per partition.</param>
        /// <param name="source">The sequence to iterate over parallel.</param>
        /// <param name="body">The delegate that is invoked once per iteration.</param>
        /// <returns>A <see cref="IReadOnlyCollection{TResult}"/> where the result of <paramref name="body"/> has been stored in the same sequential order as <paramref name="source"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="source"/> is null -or- <paramref name="body"/> is null.
        /// </exception>
        public static IReadOnlyCollection<TResult> ForEachTask<TSource, TResult>(int partitionSize, IEnumerable<TSource> source, Doer<TSource, TResult> body)
        {
            return ForEachTask(partitionSize, GetSettingsPreInitialized(partitionSize), source, body);
        }

        /// <summary>
        /// Provides a generic way of executing a parallel foreach-loop while providing ways to encapsulate and re-use existing code having a return value.
        /// </summary>
        /// <typeparam name="TSource">The type of the data in the <paramref name="source"/>.</typeparam>
        /// <typeparam name="TResult">The type of the return value of the <paramref name="body"/> that this function delegate encapsulates.</typeparam>
        /// <param name="partitionSize">The maximum number of concurrent worker threads per partition.</param>
        /// <param name="settings">The <see cref="ThreadPoolSettings"/> object used to temporary configure the static <see cref="ThreadPool"/> instance.</param>
        /// <param name="source">The sequence to iterate over parallel.</param>
        /// <param name="body">The delegate that is invoked once per iteration.</param>
        /// <returns>A <see cref="IReadOnlyCollection{TResult}"/> where the result of <paramref name="body"/> has been stored in the same sequential order as <paramref name="source"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="settings"/> is null -or- <paramref name="source"/> is null -or- <paramref name="body"/> is null.
        /// </exception>
        public static IReadOnlyCollection<TResult> ForEachTask<TSource, TResult>(int partitionSize, ThreadPoolSettings settings, IEnumerable<TSource> source, Doer<TSource, TResult> body)
        {
            return ForEachTask(partitionSize, settings, TimeSpan.MaxValue, source, body);
        }

        /// <summary>
        /// Provides a generic way of executing a parallel foreach-loop while providing ways to encapsulate and re-use existing code having a return value.
        /// </summary>
        /// <typeparam name="TSource">The type of the data in the <paramref name="source"/>.</typeparam>
        /// <typeparam name="TResult">The type of the return value of the <paramref name="body"/> that this function delegate encapsulates.</typeparam>
        /// <param name="partitionSize">The maximum number of concurrent worker threads per partition.</param>
        /// <param name="settings">The <see cref="ThreadPoolSettings"/> object used to temporary configure the static <see cref="ThreadPool"/> instance.</param>
        /// <param name="timeout">A <see cref="TimeSpan" /> that represents the time to wait for the parallel foreach-loop operation to complete.</param>
        /// <param name="source">The sequence to iterate over parallel.</param>
        /// <param name="body">The delegate that is invoked once per iteration.</param>
        /// <returns>A <see cref="IReadOnlyCollection{TResult}"/> where the result of <paramref name="body"/> has been stored in the same sequential order as <paramref name="source"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="settings"/> is null -or- <paramref name="source"/> is null -or- <paramref name="body"/> is null.
        /// </exception>
        public static IReadOnlyCollection<TResult> ForEachTask<TSource, TResult>(int partitionSize, ThreadPoolSettings settings, TimeSpan timeout, IEnumerable<TSource> source, Doer<TSource, TResult> body)
        {
            Validator.ThrowIfNull(settings, "settings");
            Validator.ThrowIfNull(source, "source");
            Validator.ThrowIfNull(body, "body");
            DoerFactory<TSource, TResult> factory = new DoerFactory<TSource, TResult>(body, default(TSource));
            return ForEachTaskCore(factory, source, partitionSize, timeout, settings);
        }

        /// <summary>
        /// Provides a generic way of executing a parallel foreach-loop while providing ways to encapsulate and re-use existing code having a return value.
        /// </summary>
        /// <typeparam name="TSource">The type of the data in the <paramref name="source"/>.</typeparam>
        /// <typeparam name="TResult">The type of the return value of the <paramref name="body"/> that this function delegate encapsulates.</typeparam>
        /// <typeparam name="T">The type of the parameter of the function delegate <paramref name="body" />.</typeparam>
        /// <param name="source">The sequence to iterate over parallel.</param>
        /// <param name="body">The delegate that is invoked once per iteration.</param>
        /// <param name="arg">The parameter of the function delegate <paramref name="body" />.</param>
        /// <returns>A <see cref="IReadOnlyCollection{TResult}"/> where the result of <paramref name="body"/> has been stored in the same sequential order as <paramref name="source"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="source"/> is null -or- <paramref name="body"/> is null.
        /// </exception>
        public static IReadOnlyCollection<TResult> ForEachTask<TSource, T, TResult>(IEnumerable<TSource> source, Doer<TSource, T, TResult> body, T arg)
        {
            return ForEachTask(ThreadUtility.DefaultNumberOfConcurrentWorkerThreads, source, body, arg);
        }

        /// <summary>
        /// Provides a generic way of executing a parallel foreach-loop while providing ways to encapsulate and re-use existing code having a return value.
        /// </summary>
        /// <typeparam name="TSource">The type of the data in the <paramref name="source"/>.</typeparam>
        /// <typeparam name="TResult">The type of the return value of the <paramref name="body"/> that this function delegate encapsulates.</typeparam>
        /// <typeparam name="T">The type of the parameter of the function delegate <paramref name="body" />.</typeparam>
        /// <param name="partitionSize">The maximum number of concurrent worker threads per partition.</param>
        /// <param name="source">The sequence to iterate over parallel.</param>
        /// <param name="body">The delegate that is invoked once per iteration.</param>
        /// <param name="arg">The parameter of the function delegate <paramref name="body" />.</param>
        /// <returns>A <see cref="IReadOnlyCollection{TResult}"/> where the result of <paramref name="body"/> has been stored in the same sequential order as <paramref name="source"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="source"/> is null -or- <paramref name="body"/> is null.
        /// </exception>
        public static IReadOnlyCollection<TResult> ForEachTask<TSource, T, TResult>(int partitionSize, IEnumerable<TSource> source, Doer<TSource, T, TResult> body, T arg)
        {
            return ForEachTask(partitionSize, GetSettingsPreInitialized(partitionSize), source, body, arg);
        }

        /// <summary>
        /// Provides a generic way of executing a parallel foreach-loop while providing ways to encapsulate and re-use existing code having a return value.
        /// </summary>
        /// <typeparam name="TSource">The type of the data in the <paramref name="source"/>.</typeparam>
        /// <typeparam name="TResult">The type of the return value of the <paramref name="body"/> that this function delegate encapsulates.</typeparam>
        /// <typeparam name="T">The type of the parameter of the function delegate <paramref name="body" />.</typeparam>
        /// <param name="partitionSize">The maximum number of concurrent worker threads per partition.</param>
        /// <param name="settings">The <see cref="ThreadPoolSettings"/> object used to temporary configure the static <see cref="ThreadPool"/> instance.</param>
        /// <param name="source">The sequence to iterate over parallel.</param>
        /// <param name="body">The delegate that is invoked once per iteration.</param>
        /// <param name="arg">The parameter of the function delegate <paramref name="body" />.</param>
        /// <returns>A <see cref="IReadOnlyCollection{TResult}"/> where the result of <paramref name="body"/> has been stored in the same sequential order as <paramref name="source"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="settings"/> is null -or- <paramref name="source"/> is null -or- <paramref name="body"/> is null.
        /// </exception>
        public static IReadOnlyCollection<TResult> ForEachTask<TSource, T, TResult>(int partitionSize, ThreadPoolSettings settings, IEnumerable<TSource> source, Doer<TSource, T, TResult> body, T arg)
        {
            return ForEachTask(partitionSize, settings, TimeSpan.MaxValue, source, body, arg);
        }

        /// <summary>
        /// Provides a generic way of executing a parallel foreach-loop while providing ways to encapsulate and re-use existing code having a return value.
        /// </summary>
        /// <typeparam name="TSource">The type of the data in the <paramref name="source"/>.</typeparam>
        /// <typeparam name="TResult">The type of the return value of the <paramref name="body"/> that this function delegate encapsulates.</typeparam>
        /// <typeparam name="T">The type of the parameter of the function delegate <paramref name="body" />.</typeparam>
        /// <param name="partitionSize">The maximum number of concurrent worker threads per partition.</param>
        /// <param name="settings">The <see cref="ThreadPoolSettings"/> object used to temporary configure the static <see cref="ThreadPool"/> instance.</param>
        /// <param name="timeout">A <see cref="TimeSpan" /> that represents the time to wait for the parallel foreach-loop operation to complete.</param>
        /// <param name="source">The sequence to iterate over parallel.</param>
        /// <param name="body">The delegate that is invoked once per iteration.</param>
        /// <param name="arg">The parameter of the function delegate <paramref name="body" />.</param>
        /// <returns>A <see cref="IReadOnlyCollection{TResult}"/> where the result of <paramref name="body"/> has been stored in the same sequential order as <paramref name="source"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="settings"/> is null -or- <paramref name="source"/> is null -or- <paramref name="body"/> is null.
        /// </exception>
        public static IReadOnlyCollection<TResult> ForEachTask<TSource, T, TResult>(int partitionSize, ThreadPoolSettings settings, TimeSpan timeout, IEnumerable<TSource> source, Doer<TSource, T, TResult> body, T arg)
        {
            Validator.ThrowIfNull(settings, "settings");
            Validator.ThrowIfNull(source, "source");
            Validator.ThrowIfNull(body, "body");
            DoerFactory<TSource, T, TResult> factory = new DoerFactory<TSource, T, TResult>(body, default(TSource), arg);
            return ForEachTaskCore(factory, source, partitionSize, timeout, settings);
        }

        /// <summary>
        /// Provides a generic way of executing a parallel foreach-loop while providing ways to encapsulate and re-use existing code having a return value.
        /// </summary>
        /// <typeparam name="TSource">The type of the data in the <paramref name="source"/>.</typeparam>
        /// <typeparam name="TResult">The type of the return value of the <paramref name="body"/> that this function delegate encapsulates.</typeparam>
        /// <typeparam name="T1">The type of the first parameter of the function delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the function delegate <paramref name="body" />.</typeparam>
        /// <param name="source">The sequence to iterate over parallel.</param>
        /// <param name="body">The delegate that is invoked once per iteration.</param>
        /// <param name="arg1">The first parameter of the function delegate <paramref name="body" />.</param>
        /// <param name="arg2">The second parameter of the function delegate <paramref name="body" />.</param>
        /// <returns>A <see cref="IReadOnlyCollection{TResult}"/> where the result of <paramref name="body"/> has been stored in the same sequential order as <paramref name="source"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="source"/> is null -or- <paramref name="body"/> is null.
        /// </exception>
        public static IReadOnlyCollection<TResult> ForEachTask<TSource, T1, T2, TResult>(IEnumerable<TSource> source, Doer<TSource, T1, T2, TResult> body, T1 arg1, T2 arg2)
        {
            return ForEachTask(ThreadUtility.DefaultNumberOfConcurrentWorkerThreads, source, body, arg1, arg2);
        }

        /// <summary>
        /// Provides a generic way of executing a parallel foreach-loop while providing ways to encapsulate and re-use existing code having a return value.
        /// </summary>
        /// <typeparam name="TSource">The type of the data in the <paramref name="source"/>.</typeparam>
        /// <typeparam name="TResult">The type of the return value of the <paramref name="body"/> that this function delegate encapsulates.</typeparam>
        /// <typeparam name="T1">The type of the first parameter of the function delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the function delegate <paramref name="body" />.</typeparam>
        /// <param name="partitionSize">The maximum number of concurrent worker threads per partition.</param>
        /// <param name="source">The sequence to iterate over parallel.</param>
        /// <param name="body">The delegate that is invoked once per iteration.</param>
        /// <param name="arg1">The first parameter of the function delegate <paramref name="body" />.</param>
        /// <param name="arg2">The second parameter of the function delegate <paramref name="body" />.</param>
        /// <returns>A <see cref="IReadOnlyCollection{TResult}"/> where the result of <paramref name="body"/> has been stored in the same sequential order as <paramref name="source"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="source"/> is null -or- <paramref name="body"/> is null.
        /// </exception>
        public static IReadOnlyCollection<TResult> ForEachTask<TSource, T1, T2, TResult>(int partitionSize, IEnumerable<TSource> source, Doer<TSource, T1, T2, TResult> body, T1 arg1, T2 arg2)
        {
            return ForEachTask(partitionSize, GetSettingsPreInitialized(partitionSize), source, body, arg1, arg2);
        }

        /// <summary>
        /// Provides a generic way of executing a parallel foreach-loop while providing ways to encapsulate and re-use existing code having a return value.
        /// </summary>
        /// <typeparam name="TSource">The type of the data in the <paramref name="source"/>.</typeparam>
        /// <typeparam name="TResult">The type of the return value of the <paramref name="body"/> that this function delegate encapsulates.</typeparam>
        /// <typeparam name="T1">The type of the first parameter of the function delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the function delegate <paramref name="body" />.</typeparam>
        /// <param name="partitionSize">The maximum number of concurrent worker threads per partition.</param>
        /// <param name="settings">The <see cref="ThreadPoolSettings"/> object used to temporary configure the static <see cref="ThreadPool"/> instance.</param>
        /// <param name="source">The sequence to iterate over parallel.</param>
        /// <param name="body">The delegate that is invoked once per iteration.</param>
        /// <param name="arg1">The first parameter of the function delegate <paramref name="body" />.</param>
        /// <param name="arg2">The second parameter of the function delegate <paramref name="body" />.</param>
        /// <returns>A <see cref="IReadOnlyCollection{TResult}"/> where the result of <paramref name="body"/> has been stored in the same sequential order as <paramref name="source"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="settings"/> is null -or- <paramref name="source"/> is null -or- <paramref name="body"/> is null.
        /// </exception>
        public static IReadOnlyCollection<TResult> ForEachTask<TSource, T1, T2, TResult>(int partitionSize, ThreadPoolSettings settings, IEnumerable<TSource> source, Doer<TSource, T1, T2, TResult> body, T1 arg1, T2 arg2)
        {
            return ForEachTask(partitionSize, settings, TimeSpan.MaxValue, source, body, arg1, arg2);
        }

        /// <summary>
        /// Provides a generic way of executing a parallel foreach-loop while providing ways to encapsulate and re-use existing code having a return value.
        /// </summary>
        /// <typeparam name="TSource">The type of the data in the <paramref name="source"/>.</typeparam>
        /// <typeparam name="TResult">The type of the return value of the <paramref name="body"/> that this function delegate encapsulates.</typeparam>
        /// <typeparam name="T1">The type of the first parameter of the function delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the function delegate <paramref name="body" />.</typeparam>
        /// <param name="partitionSize">The maximum number of concurrent worker threads per partition.</param>
        /// <param name="settings">The <see cref="ThreadPoolSettings"/> object used to temporary configure the static <see cref="ThreadPool"/> instance.</param>
        /// <param name="timeout">A <see cref="TimeSpan" /> that represents the time to wait for the parallel foreach-loop operation to complete.</param>
        /// <param name="source">The sequence to iterate over parallel.</param>
        /// <param name="body">The delegate that is invoked once per iteration.</param>
        /// <param name="arg1">The first parameter of the function delegate <paramref name="body" />.</param>
        /// <param name="arg2">The second parameter of the function delegate <paramref name="body" />.</param>
        /// <returns>A <see cref="IReadOnlyCollection{TResult}"/> where the result of <paramref name="body"/> has been stored in the same sequential order as <paramref name="source"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="settings"/> is null -or- <paramref name="source"/> is null -or- <paramref name="body"/> is null.
        /// </exception>
        public static IReadOnlyCollection<TResult> ForEachTask<TSource, T1, T2, TResult>(int partitionSize, ThreadPoolSettings settings, TimeSpan timeout, IEnumerable<TSource> source, Doer<TSource, T1, T2, TResult> body, T1 arg1, T2 arg2)
        {
            Validator.ThrowIfNull(settings, "settings");
            Validator.ThrowIfNull(source, "source");
            Validator.ThrowIfNull(body, "body");
            DoerFactory<TSource, T1, T2, TResult> factory = new DoerFactory<TSource, T1, T2, TResult>(body, default(TSource), arg1, arg2);
            return ForEachTaskCore(factory, source, partitionSize, timeout, settings);
        }

        /// <summary>
        /// Provides a generic way of executing a parallel foreach-loop while providing ways to encapsulate and re-use existing code having a return value.
        /// </summary>
        /// <typeparam name="TSource">The type of the data in the <paramref name="source"/>.</typeparam>
        /// <typeparam name="TResult">The type of the return value of the <paramref name="body"/> that this function delegate encapsulates.</typeparam>
        /// <typeparam name="T1">The type of the first parameter of the function delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the function delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the function delegate <paramref name="body" />.</typeparam>
        /// <param name="source">The sequence to iterate over parallel.</param>
        /// <param name="body">The delegate that is invoked once per iteration.</param>
        /// <param name="arg1">The first parameter of the function delegate <paramref name="body" />.</param>
        /// <param name="arg2">The second parameter of the function delegate <paramref name="body" />.</param>
        /// <param name="arg3">The third parameter of the function delegate <paramref name="body" />.</param>
        /// <returns>A <see cref="IReadOnlyCollection{TResult}"/> where the result of <paramref name="body"/> has been stored in the same sequential order as <paramref name="source"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="source"/> is null -or- <paramref name="body"/> is null.
        /// </exception>
        public static IReadOnlyCollection<TResult> ForEachTask<TSource, T1, T2, T3, TResult>(IEnumerable<TSource> source, Doer<TSource, T1, T2, T3, TResult> body, T1 arg1, T2 arg2, T3 arg3)
        {
            return ForEachTask(ThreadUtility.DefaultNumberOfConcurrentWorkerThreads, source, body, arg1, arg2, arg3);
        }

        /// <summary>
        /// Provides a generic way of executing a parallel foreach-loop while providing ways to encapsulate and re-use existing code having a return value.
        /// </summary>
        /// <typeparam name="TSource">The type of the data in the <paramref name="source"/>.</typeparam>
        /// <typeparam name="TResult">The type of the return value of the <paramref name="body"/> that this function delegate encapsulates.</typeparam>
        /// <typeparam name="T1">The type of the first parameter of the function delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the function delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the function delegate <paramref name="body" />.</typeparam>
        /// <param name="partitionSize">The maximum number of concurrent worker threads per partition.</param>
        /// <param name="source">The sequence to iterate over parallel.</param>
        /// <param name="body">The delegate that is invoked once per iteration.</param>
        /// <param name="arg1">The first parameter of the function delegate <paramref name="body" />.</param>
        /// <param name="arg2">The second parameter of the function delegate <paramref name="body" />.</param>
        /// <param name="arg3">The third parameter of the function delegate <paramref name="body" />.</param>
        /// <returns>A <see cref="IReadOnlyCollection{TResult}"/> where the result of <paramref name="body"/> has been stored in the same sequential order as <paramref name="source"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="source"/> is null -or- <paramref name="body"/> is null.
        /// </exception>
        public static IReadOnlyCollection<TResult> ForEachTask<TSource, T1, T2, T3, TResult>(int partitionSize, IEnumerable<TSource> source, Doer<TSource, T1, T2, T3, TResult> body, T1 arg1, T2 arg2, T3 arg3)
        {
            return ForEachTask(partitionSize, GetSettingsPreInitialized(partitionSize), source, body, arg1, arg2, arg3);
        }

        /// <summary>
        /// Provides a generic way of executing a parallel foreach-loop while providing ways to encapsulate and re-use existing code having a return value.
        /// </summary>
        /// <typeparam name="TSource">The type of the data in the <paramref name="source"/>.</typeparam>
        /// <typeparam name="TResult">The type of the return value of the <paramref name="body"/> that this function delegate encapsulates.</typeparam>
        /// <typeparam name="T1">The type of the first parameter of the function delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the function delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the function delegate <paramref name="body" />.</typeparam>
        /// <param name="partitionSize">The maximum number of concurrent worker threads per partition.</param>
        /// <param name="settings">The <see cref="ThreadPoolSettings"/> object used to temporary configure the static <see cref="ThreadPool"/> instance.</param>
        /// <param name="source">The sequence to iterate over parallel.</param>
        /// <param name="body">The delegate that is invoked once per iteration.</param>
        /// <param name="arg1">The first parameter of the function delegate <paramref name="body" />.</param>
        /// <param name="arg2">The second parameter of the function delegate <paramref name="body" />.</param>
        /// <param name="arg3">The third parameter of the function delegate <paramref name="body" />.</param>
        /// <returns>A <see cref="IReadOnlyCollection{TResult}"/> where the result of <paramref name="body"/> has been stored in the same sequential order as <paramref name="source"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="settings"/> is null -or- <paramref name="source"/> is null -or- <paramref name="body"/> is null.
        /// </exception>
        public static IReadOnlyCollection<TResult> ForEachTask<TSource, T1, T2, T3, TResult>(int partitionSize, ThreadPoolSettings settings, IEnumerable<TSource> source, Doer<TSource, T1, T2, T3, TResult> body, T1 arg1, T2 arg2, T3 arg3)
        {
            return ForEachTask(partitionSize, settings, TimeSpan.MaxValue, source, body, arg1, arg2, arg3);
        }

        /// <summary>
        /// Provides a generic way of executing a parallel foreach-loop while providing ways to encapsulate and re-use existing code having a return value.
        /// </summary>
        /// <typeparam name="TSource">The type of the data in the <paramref name="source"/>.</typeparam>
        /// <typeparam name="TResult">The type of the return value of the <paramref name="body"/> that this function delegate encapsulates.</typeparam>
        /// <typeparam name="T1">The type of the first parameter of the function delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the function delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the function delegate <paramref name="body" />.</typeparam>
        /// <param name="partitionSize">The maximum number of concurrent worker threads per partition.</param>
        /// <param name="settings">The <see cref="ThreadPoolSettings"/> object used to temporary configure the static <see cref="ThreadPool"/> instance.</param>
        /// <param name="timeout">A <see cref="TimeSpan" /> that represents the time to wait for the parallel foreach-loop operation to complete.</param>
        /// <param name="source">The sequence to iterate over parallel.</param>
        /// <param name="body">The delegate that is invoked once per iteration.</param>
        /// <param name="arg1">The first parameter of the function delegate <paramref name="body" />.</param>
        /// <param name="arg2">The second parameter of the function delegate <paramref name="body" />.</param>
        /// <param name="arg3">The third parameter of the function delegate <paramref name="body" />.</param>
        /// <returns>A <see cref="IReadOnlyCollection{TResult}"/> where the result of <paramref name="body"/> has been stored in the same sequential order as <paramref name="source"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="settings"/> is null -or- <paramref name="source"/> is null -or- <paramref name="body"/> is null.
        /// </exception>
        public static IReadOnlyCollection<TResult> ForEachTask<TSource, T1, T2, T3, TResult>(int partitionSize, ThreadPoolSettings settings, TimeSpan timeout, IEnumerable<TSource> source, Doer<TSource, T1, T2, T3, TResult> body, T1 arg1, T2 arg2, T3 arg3)
        {
            Validator.ThrowIfNull(settings, "settings");
            Validator.ThrowIfNull(source, "source");
            Validator.ThrowIfNull(body, "body");
            DoerFactory<TSource, T1, T2, T3, TResult> factory = new DoerFactory<TSource, T1, T2, T3, TResult>(body, default(TSource), arg1, arg2, arg3);
            return ForEachTaskCore(factory, source, partitionSize, timeout, settings);
        }

        /// <summary>
        /// Provides a generic way of executing a parallel foreach-loop while providing ways to encapsulate and re-use existing code having a return value.
        /// </summary>
        /// <typeparam name="TSource">The type of the data in the <paramref name="source"/>.</typeparam>
        /// <typeparam name="TResult">The type of the return value of the <paramref name="body"/> that this function delegate encapsulates.</typeparam>
        /// <typeparam name="T1">The type of the first parameter of the function delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the function delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the function delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of the function delegate <paramref name="body" />.</typeparam>
        /// <param name="source">The sequence to iterate over parallel.</param>
        /// <param name="body">The delegate that is invoked once per iteration.</param>
        /// <param name="arg1">The first parameter of the function delegate <paramref name="body" />.</param>
        /// <param name="arg2">The second parameter of the function delegate <paramref name="body" />.</param>
        /// <param name="arg3">The third parameter of the function delegate <paramref name="body" />.</param>
        /// <param name="arg4">The fourth parameter of the function delegate <paramref name="body" />.</param>
        /// <returns>A <see cref="IReadOnlyCollection{TResult}"/> where the result of <paramref name="body"/> has been stored in the same sequential order as <paramref name="source"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="source"/> is null -or- <paramref name="body"/> is null.
        /// </exception>
        public static IReadOnlyCollection<TResult> ForEachTask<TSource, T1, T2, T3, T4, TResult>(IEnumerable<TSource> source, Doer<TSource, T1, T2, T3, T4, TResult> body, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            return ForEachTask(ThreadUtility.DefaultNumberOfConcurrentWorkerThreads, source, body, arg1, arg2, arg3, arg4);
        }

        /// <summary>
        /// Provides a generic way of executing a parallel foreach-loop while providing ways to encapsulate and re-use existing code having a return value.
        /// </summary>
        /// <typeparam name="TSource">The type of the data in the <paramref name="source"/>.</typeparam>
        /// <typeparam name="TResult">The type of the return value of the <paramref name="body"/> that this function delegate encapsulates.</typeparam>
        /// <typeparam name="T1">The type of the first parameter of the function delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the function delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the function delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of the function delegate <paramref name="body" />.</typeparam>
        /// <param name="partitionSize">The maximum number of concurrent worker threads per partition.</param>
        /// <param name="source">The sequence to iterate over parallel.</param>
        /// <param name="body">The delegate that is invoked once per iteration.</param>
        /// <param name="arg1">The first parameter of the function delegate <paramref name="body" />.</param>
        /// <param name="arg2">The second parameter of the function delegate <paramref name="body" />.</param>
        /// <param name="arg3">The third parameter of the function delegate <paramref name="body" />.</param>
        /// <param name="arg4">The fourth parameter of the function delegate <paramref name="body" />.</param>
        /// <returns>A <see cref="IReadOnlyCollection{TResult}"/> where the result of <paramref name="body"/> has been stored in the same sequential order as <paramref name="source"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="source"/> is null -or- <paramref name="body"/> is null.
        /// </exception>
        public static IReadOnlyCollection<TResult> ForEachTask<TSource, T1, T2, T3, T4, TResult>(int partitionSize, IEnumerable<TSource> source, Doer<TSource, T1, T2, T3, T4, TResult> body, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            return ForEachTask(partitionSize, GetSettingsPreInitialized(partitionSize), source, body, arg1, arg2, arg3, arg4);
        }

        /// <summary>
        /// Provides a generic way of executing a parallel foreach-loop while providing ways to encapsulate and re-use existing code having a return value.
        /// </summary>
        /// <typeparam name="TSource">The type of the data in the <paramref name="source"/>.</typeparam>
        /// <typeparam name="TResult">The type of the return value of the <paramref name="body"/> that this function delegate encapsulates.</typeparam>
        /// <typeparam name="T1">The type of the first parameter of the function delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the function delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the function delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of the function delegate <paramref name="body" />.</typeparam>
        /// <param name="partitionSize">The maximum number of concurrent worker threads per partition.</param>
        /// <param name="settings">The <see cref="ThreadPoolSettings"/> object used to temporary configure the static <see cref="ThreadPool"/> instance.</param>
        /// <param name="source">The sequence to iterate over parallel.</param>
        /// <param name="body">The delegate that is invoked once per iteration.</param>
        /// <param name="arg1">The first parameter of the function delegate <paramref name="body" />.</param>
        /// <param name="arg2">The second parameter of the function delegate <paramref name="body" />.</param>
        /// <param name="arg3">The third parameter of the function delegate <paramref name="body" />.</param>
        /// <param name="arg4">The fourth parameter of the function delegate <paramref name="body" />.</param>
        /// <returns>A <see cref="IReadOnlyCollection{TResult}"/> where the result of <paramref name="body"/> has been stored in the same sequential order as <paramref name="source"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="settings"/> is null -or- <paramref name="source"/> is null -or- <paramref name="body"/> is null.
        /// </exception>
        public static IReadOnlyCollection<TResult> ForEachTask<TSource, T1, T2, T3, T4, TResult>(int partitionSize, ThreadPoolSettings settings, IEnumerable<TSource> source, Doer<TSource, T1, T2, T3, T4, TResult> body, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            return ForEachTask(partitionSize, settings, TimeSpan.MaxValue, source, body, arg1, arg2, arg3, arg4);
        }

        /// <summary>
        /// Provides a generic way of executing a parallel foreach-loop while providing ways to encapsulate and re-use existing code having a return value.
        /// </summary>
        /// <typeparam name="TSource">The type of the data in the <paramref name="source"/>.</typeparam>
        /// <typeparam name="TResult">The type of the return value of the <paramref name="body"/> that this function delegate encapsulates.</typeparam>
        /// <typeparam name="T1">The type of the first parameter of the function delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the function delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the function delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of the function delegate <paramref name="body" />.</typeparam>
        /// <param name="partitionSize">The maximum number of concurrent worker threads per partition.</param>
        /// <param name="settings">The <see cref="ThreadPoolSettings"/> object used to temporary configure the static <see cref="ThreadPool"/> instance.</param>
        /// <param name="timeout">A <see cref="TimeSpan" /> that represents the time to wait for the parallel foreach-loop operation to complete.</param>
        /// <param name="source">The sequence to iterate over parallel.</param>
        /// <param name="body">The delegate that is invoked once per iteration.</param>
        /// <param name="arg1">The first parameter of the function delegate <paramref name="body" />.</param>
        /// <param name="arg2">The second parameter of the function delegate <paramref name="body" />.</param>
        /// <param name="arg3">The third parameter of the function delegate <paramref name="body" />.</param>
        /// <param name="arg4">The fourth parameter of the function delegate <paramref name="body" />.</param>
        /// <returns>A <see cref="IReadOnlyCollection{TResult}"/> where the result of <paramref name="body"/> has been stored in the same sequential order as <paramref name="source"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="settings"/> is null -or- <paramref name="source"/> is null -or- <paramref name="body"/> is null.
        /// </exception>
        public static IReadOnlyCollection<TResult> ForEachTask<TSource, T1, T2, T3, T4, TResult>(int partitionSize, ThreadPoolSettings settings, TimeSpan timeout, IEnumerable<TSource> source, Doer<TSource, T1, T2, T3, T4, TResult> body, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            Validator.ThrowIfNull(settings, "settings");
            Validator.ThrowIfNull(source, "source");
            Validator.ThrowIfNull(body, "body");
            DoerFactory<TSource, T1, T2, T3, T4, TResult> factory = new DoerFactory<TSource, T1, T2, T3, T4, TResult>(body, default(TSource), arg1, arg2, arg3, arg4);
            return ForEachTaskCore(factory, source, partitionSize, timeout, settings);
        }

        /// <summary>
        /// Provides a generic way of executing a parallel foreach-loop while providing ways to encapsulate and re-use existing code having a return value.
        /// </summary>
        /// <typeparam name="TSource">The type of the data in the <paramref name="source"/>.</typeparam>
        /// <typeparam name="TResult">The type of the return value of the <paramref name="body"/> that this function delegate encapsulates.</typeparam>
        /// <typeparam name="T1">The type of the first parameter of the function delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the function delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the function delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of the function delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T5">The type of the fifth parameter of the function delegate <paramref name="body" />.</typeparam>
        /// <param name="source">The sequence to iterate over parallel.</param>
        /// <param name="body">The delegate that is invoked once per iteration.</param>
        /// <param name="arg1">The first parameter of the function delegate <paramref name="body" />.</param>
        /// <param name="arg2">The second parameter of the function delegate <paramref name="body" />.</param>
        /// <param name="arg3">The third parameter of the function delegate <paramref name="body" />.</param>
        /// <param name="arg4">The fourth parameter of the function delegate <paramref name="body" />.</param>
        /// <param name="arg5">The fifth parameter of the function delegate <paramref name="body" />.</param>
        /// <returns>A <see cref="IReadOnlyCollection{TResult}"/> where the result of <paramref name="body"/> has been stored in the same sequential order as <paramref name="source"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="source"/> is null -or- <paramref name="body"/> is null.
        /// </exception>
        public static IReadOnlyCollection<TResult> ForEachTask<TSource, T1, T2, T3, T4, T5, TResult>(IEnumerable<TSource> source, Doer<TSource, T1, T2, T3, T4, T5, TResult> body, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        {
            return ForEachTask(ThreadUtility.DefaultNumberOfConcurrentWorkerThreads, source, body, arg1, arg2, arg3, arg4, arg5);
        }

        /// <summary>
        /// Provides a generic way of executing a parallel foreach-loop while providing ways to encapsulate and re-use existing code having a return value.
        /// </summary>
        /// <typeparam name="TSource">The type of the data in the <paramref name="source"/>.</typeparam>
        /// <typeparam name="TResult">The type of the return value of the <paramref name="body"/> that this function delegate encapsulates.</typeparam>
        /// <typeparam name="T1">The type of the first parameter of the function delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the function delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the function delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of the function delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T5">The type of the fifth parameter of the function delegate <paramref name="body" />.</typeparam>
        /// <param name="partitionSize">The maximum number of concurrent worker threads per partition.</param>
        /// <param name="source">The sequence to iterate over parallel.</param>
        /// <param name="body">The delegate that is invoked once per iteration.</param>
        /// <param name="arg1">The first parameter of the function delegate <paramref name="body" />.</param>
        /// <param name="arg2">The second parameter of the function delegate <paramref name="body" />.</param>
        /// <param name="arg3">The third parameter of the function delegate <paramref name="body" />.</param>
        /// <param name="arg4">The fourth parameter of the function delegate <paramref name="body" />.</param>
        /// <param name="arg5">The fifth parameter of the function delegate <paramref name="body" />.</param>
        /// <returns>A <see cref="IReadOnlyCollection{TResult}"/> where the result of <paramref name="body"/> has been stored in the same sequential order as <paramref name="source"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="source"/> is null -or- <paramref name="body"/> is null.
        /// </exception>
        public static IReadOnlyCollection<TResult> ForEachTask<TSource, T1, T2, T3, T4, T5, TResult>(int partitionSize, IEnumerable<TSource> source, Doer<TSource, T1, T2, T3, T4, T5, TResult> body, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        {
            return ForEachTask(partitionSize, GetSettingsPreInitialized(partitionSize), source, body, arg1, arg2, arg3, arg4, arg5);
        }

        /// <summary>
        /// Provides a generic way of executing a parallel foreach-loop while providing ways to encapsulate and re-use existing code having a return value.
        /// </summary>
        /// <typeparam name="TSource">The type of the data in the <paramref name="source"/>.</typeparam>
        /// <typeparam name="TResult">The type of the return value of the <paramref name="body"/> that this function delegate encapsulates.</typeparam>
        /// <typeparam name="T1">The type of the first parameter of the function delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the function delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the function delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of the function delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T5">The type of the fifth parameter of the function delegate <paramref name="body" />.</typeparam>
        /// <param name="partitionSize">The maximum number of concurrent worker threads per partition.</param>
        /// <param name="settings">The <see cref="ThreadPoolSettings"/> object used to temporary configure the static <see cref="ThreadPool"/> instance.</param>
        /// <param name="source">The sequence to iterate over parallel.</param>
        /// <param name="body">The delegate that is invoked once per iteration.</param>
        /// <param name="arg1">The first parameter of the function delegate <paramref name="body" />.</param>
        /// <param name="arg2">The second parameter of the function delegate <paramref name="body" />.</param>
        /// <param name="arg3">The third parameter of the function delegate <paramref name="body" />.</param>
        /// <param name="arg4">The fourth parameter of the function delegate <paramref name="body" />.</param>
        /// <param name="arg5">The fifth parameter of the function delegate <paramref name="body" />.</param>
        /// <returns>A <see cref="IReadOnlyCollection{TResult}"/> where the result of <paramref name="body"/> has been stored in the same sequential order as <paramref name="source"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="settings"/> is null -or- <paramref name="source"/> is null -or- <paramref name="body"/> is null.
        /// </exception>
        public static IReadOnlyCollection<TResult> ForEachTask<TSource, T1, T2, T3, T4, T5, TResult>(int partitionSize, ThreadPoolSettings settings, IEnumerable<TSource> source, Doer<TSource, T1, T2, T3, T4, T5, TResult> body, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        {
            return ForEachTask(partitionSize, settings, TimeSpan.MaxValue, source, body, arg1, arg2, arg3, arg4, arg5);
        }

        /// <summary>
        /// Provides a generic way of executing a parallel foreach-loop while providing ways to encapsulate and re-use existing code having a return value.
        /// </summary>
        /// <typeparam name="TSource">The type of the data in the <paramref name="source"/>.</typeparam>
        /// <typeparam name="TResult">The type of the return value of the <paramref name="body"/> that this function delegate encapsulates.</typeparam>
        /// <typeparam name="T1">The type of the first parameter of the function delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the function delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the function delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of the function delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T5">The type of the fifth parameter of the function delegate <paramref name="body" />.</typeparam>
        /// <param name="partitionSize">The maximum number of concurrent worker threads per partition.</param>
        /// <param name="settings">The <see cref="ThreadPoolSettings"/> object used to temporary configure the static <see cref="ThreadPool"/> instance.</param>
        /// <param name="timeout">A <see cref="TimeSpan" /> that represents the time to wait for the parallel foreach-loop operation to complete.</param>
        /// <param name="source">The sequence to iterate over parallel.</param>
        /// <param name="body">The delegate that is invoked once per iteration.</param>
        /// <param name="arg1">The first parameter of the function delegate <paramref name="body" />.</param>
        /// <param name="arg2">The second parameter of the function delegate <paramref name="body" />.</param>
        /// <param name="arg3">The third parameter of the function delegate <paramref name="body" />.</param>
        /// <param name="arg4">The fourth parameter of the function delegate <paramref name="body" />.</param>
        /// <param name="arg5">The fifth parameter of the function delegate <paramref name="body" />.</param>
        /// <returns>A <see cref="IReadOnlyCollection{TResult}"/> where the result of <paramref name="body"/> has been stored in the same sequential order as <paramref name="source"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="settings"/> is null -or- <paramref name="source"/> is null -or- <paramref name="body"/> is null.
        /// </exception>
        public static IReadOnlyCollection<TResult> ForEachTask<TSource, T1, T2, T3, T4, T5, TResult>(int partitionSize, ThreadPoolSettings settings, TimeSpan timeout, IEnumerable<TSource> source, Doer<TSource, T1, T2, T3, T4, T5, TResult> body, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        {
            Validator.ThrowIfNull(settings, "settings");
            Validator.ThrowIfNull(source, "source");
            Validator.ThrowIfNull(body, "body");
            DoerFactory<TSource, T1, T2, T3, T4, T5, TResult> factory = new DoerFactory<TSource, T1, T2, T3, T4, T5, TResult>(body, default(TSource), arg1, arg2, arg3, arg4, arg5);
            return ForEachTaskCore(factory, source, partitionSize, timeout, settings);
        }

        /// <summary>
        /// Provides a generic way of executing a parallel foreach-loop while providing ways to encapsulate and re-use existing code having a return value.
        /// </summary>
        /// <typeparam name="TSource">The type of the data in the <paramref name="source"/>.</typeparam>
        /// <typeparam name="TResult">The type of the return value of the <paramref name="body"/> that this function delegate encapsulates.</typeparam>
        /// <typeparam name="T1">The type of the first parameter of the function delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the function delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the function delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of the function delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T5">The type of the fifth parameter of the function delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T6">The type of the sixth parameter of the function delegate <paramref name="body" />.</typeparam>
        /// <param name="source">The sequence to iterate over parallel.</param>
        /// <param name="body">The delegate that is invoked once per iteration.</param>
        /// <param name="arg1">The first parameter of the function delegate <paramref name="body" />.</param>
        /// <param name="arg2">The second parameter of the function delegate <paramref name="body" />.</param>
        /// <param name="arg3">The third parameter of the function delegate <paramref name="body" />.</param>
        /// <param name="arg4">The fourth parameter of the function delegate <paramref name="body" />.</param>
        /// <param name="arg5">The fifth parameter of the function delegate <paramref name="body" />.</param>
        /// <param name="arg6">The sixth parameter of the function delegate <paramref name="body" />.</param>
        /// <returns>A <see cref="IReadOnlyCollection{TResult}"/> where the result of <paramref name="body"/> has been stored in the same sequential order as <paramref name="source"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="source"/> is null -or- <paramref name="body"/> is null.
        /// </exception>
        public static IReadOnlyCollection<TResult> ForEachTask<TSource, T1, T2, T3, T4, T5, T6, TResult>(IEnumerable<TSource> source, Doer<TSource, T1, T2, T3, T4, T5, T6, TResult> body, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6)
        {
            return ForEachTask(ThreadUtility.DefaultNumberOfConcurrentWorkerThreads, source, body, arg1, arg2, arg3, arg4, arg5, arg6);
        }

        /// <summary>
        /// Provides a generic way of executing a parallel foreach-loop while providing ways to encapsulate and re-use existing code having a return value.
        /// </summary>
        /// <typeparam name="TSource">The type of the data in the <paramref name="source"/>.</typeparam>
        /// <typeparam name="TResult">The type of the return value of the <paramref name="body"/> that this function delegate encapsulates.</typeparam>
        /// <typeparam name="T1">The type of the first parameter of the function delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the function delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the function delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of the function delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T5">The type of the fifth parameter of the function delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T6">The type of the sixth parameter of the function delegate <paramref name="body" />.</typeparam>
        /// <param name="partitionSize">The maximum number of concurrent worker threads per partition.</param>
        /// <param name="source">The sequence to iterate over parallel.</param>
        /// <param name="body">The delegate that is invoked once per iteration.</param>
        /// <param name="arg1">The first parameter of the function delegate <paramref name="body" />.</param>
        /// <param name="arg2">The second parameter of the function delegate <paramref name="body" />.</param>
        /// <param name="arg3">The third parameter of the function delegate <paramref name="body" />.</param>
        /// <param name="arg4">The fourth parameter of the function delegate <paramref name="body" />.</param>
        /// <param name="arg5">The fifth parameter of the function delegate <paramref name="body" />.</param>
        /// <param name="arg6">The sixth parameter of the function delegate <paramref name="body" />.</param>
        /// <returns>A <see cref="IReadOnlyCollection{TResult}"/> where the result of <paramref name="body"/> has been stored in the same sequential order as <paramref name="source"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="source"/> is null -or- <paramref name="body"/> is null.
        /// </exception>
        public static IReadOnlyCollection<TResult> ForEachTask<TSource, T1, T2, T3, T4, T5, T6, TResult>(int partitionSize, IEnumerable<TSource> source, Doer<TSource, T1, T2, T3, T4, T5, T6, TResult> body, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6)
        {
            return ForEachTask(partitionSize, GetSettingsPreInitialized(partitionSize), source, body, arg1, arg2, arg3, arg4, arg5, arg6);
        }

        /// <summary>
        /// Provides a generic way of executing a parallel foreach-loop while providing ways to encapsulate and re-use existing code having a return value.
        /// </summary>
        /// <typeparam name="TSource">The type of the data in the <paramref name="source"/>.</typeparam>
        /// <typeparam name="TResult">The type of the return value of the <paramref name="body"/> that this function delegate encapsulates.</typeparam>
        /// <typeparam name="T1">The type of the first parameter of the function delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the function delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the function delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of the function delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T5">The type of the fifth parameter of the function delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T6">The type of the sixth parameter of the function delegate <paramref name="body" />.</typeparam>
        /// <param name="partitionSize">The maximum number of concurrent worker threads per partition.</param>
        /// <param name="settings">The <see cref="ThreadPoolSettings"/> object used to temporary configure the static <see cref="ThreadPool"/> instance.</param>
        /// <param name="source">The sequence to iterate over parallel.</param>
        /// <param name="body">The delegate that is invoked once per iteration.</param>
        /// <param name="arg1">The first parameter of the function delegate <paramref name="body" />.</param>
        /// <param name="arg2">The second parameter of the function delegate <paramref name="body" />.</param>
        /// <param name="arg3">The third parameter of the function delegate <paramref name="body" />.</param>
        /// <param name="arg4">The fourth parameter of the function delegate <paramref name="body" />.</param>
        /// <param name="arg5">The fifth parameter of the function delegate <paramref name="body" />.</param>
        /// <param name="arg6">The sixth parameter of the function delegate <paramref name="body" />.</param>
        /// <returns>A <see cref="IReadOnlyCollection{TResult}"/> where the result of <paramref name="body"/> has been stored in the same sequential order as <paramref name="source"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="settings"/> is null -or- <paramref name="source"/> is null -or- <paramref name="body"/> is null.
        /// </exception>
        public static IReadOnlyCollection<TResult> ForEachTask<TSource, T1, T2, T3, T4, T5, T6, TResult>(int partitionSize, ThreadPoolSettings settings, IEnumerable<TSource> source, Doer<TSource, T1, T2, T3, T4, T5, T6, TResult> body, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6)
        {
            return ForEachTask(partitionSize, settings, TimeSpan.MaxValue, source, body, arg1, arg2, arg3, arg4, arg5, arg6);
        }

        /// <summary>
        /// Provides a generic way of executing a parallel foreach-loop while providing ways to encapsulate and re-use existing code having a return value.
        /// </summary>
        /// <typeparam name="TSource">The type of the data in the <paramref name="source"/>.</typeparam>
        /// <typeparam name="TResult">The type of the return value of the <paramref name="body"/> that this function delegate encapsulates.</typeparam>
        /// <typeparam name="T1">The type of the first parameter of the function delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the function delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the function delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of the function delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T5">The type of the fifth parameter of the function delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T6">The type of the sixth parameter of the function delegate <paramref name="body" />.</typeparam>
        /// <param name="partitionSize">The maximum number of concurrent worker threads per partition.</param>
        /// <param name="settings">The <see cref="ThreadPoolSettings"/> object used to temporary configure the static <see cref="ThreadPool"/> instance.</param>
        /// <param name="timeout">A <see cref="TimeSpan" /> that represents the time to wait for the parallel foreach-loop operation to complete.</param>
        /// <param name="source">The sequence to iterate over parallel.</param>
        /// <param name="body">The delegate that is invoked once per iteration.</param>
        /// <param name="arg1">The first parameter of the function delegate <paramref name="body" />.</param>
        /// <param name="arg2">The second parameter of the function delegate <paramref name="body" />.</param>
        /// <param name="arg3">The third parameter of the function delegate <paramref name="body" />.</param>
        /// <param name="arg4">The fourth parameter of the function delegate <paramref name="body" />.</param>
        /// <param name="arg5">The fifth parameter of the function delegate <paramref name="body" />.</param>
        /// <param name="arg6">The sixth parameter of the function delegate <paramref name="body" />.</param>
        /// <returns>A <see cref="IReadOnlyCollection{TResult}"/> where the result of <paramref name="body"/> has been stored in the same sequential order as <paramref name="source"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="settings"/> is null -or- <paramref name="source"/> is null -or- <paramref name="body"/> is null.
        /// </exception>
        public static IReadOnlyCollection<TResult> ForEachTask<TSource, T1, T2, T3, T4, T5, T6, TResult>(int partitionSize, ThreadPoolSettings settings, TimeSpan timeout, IEnumerable<TSource> source, Doer<TSource, T1, T2, T3, T4, T5, T6, TResult> body, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6)
        {
            Validator.ThrowIfNull(settings, "settings");
            Validator.ThrowIfNull(source, "source");
            Validator.ThrowIfNull(body, "body");
            DoerFactory<TSource, T1, T2, T3, T4, T5, T6, TResult> factory = new DoerFactory<TSource, T1, T2, T3, T4, T5, T6, TResult>(body, default(TSource), arg1, arg2, arg3, arg4, arg5, arg6);
            return ForEachTaskCore(factory, source, partitionSize, timeout, settings);
        }

        /// <summary>
        /// Provides a generic way of executing a parallel foreach-loop while providing ways to encapsulate and re-use existing code having a return value.
        /// </summary>
        /// <typeparam name="TSource">The type of the data in the <paramref name="source"/>.</typeparam>
        /// <typeparam name="TResult">The type of the return value of the <paramref name="body"/> that this function delegate encapsulates.</typeparam>
        /// <typeparam name="T1">The type of the first parameter of the function delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the function delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the function delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of the function delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T5">The type of the fifth parameter of the function delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T6">The type of the sixth parameter of the function delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T7">The type of the seventh parameter of the function delegate <paramref name="body" />.</typeparam>
        /// <param name="source">The sequence to iterate over parallel.</param>
        /// <param name="body">The delegate that is invoked once per iteration.</param>
        /// <param name="arg1">The first parameter of the function delegate <paramref name="body" />.</param>
        /// <param name="arg2">The second parameter of the function delegate <paramref name="body" />.</param>
        /// <param name="arg3">The third parameter of the function delegate <paramref name="body" />.</param>
        /// <param name="arg4">The fourth parameter of the function delegate <paramref name="body" />.</param>
        /// <param name="arg5">The fifth parameter of the function delegate <paramref name="body" />.</param>
        /// <param name="arg6">The sixth parameter of the function delegate <paramref name="body" />.</param>
        /// <param name="arg7">The seventh parameter of the function delegate <paramref name="body" />.</param>
        /// <returns>A <see cref="IReadOnlyCollection{TResult}"/> where the result of <paramref name="body"/> has been stored in the same sequential order as <paramref name="source"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="source"/> is null -or- <paramref name="body"/> is null.
        /// </exception>
        public static IReadOnlyCollection<TResult> ForEachTask<TSource, T1, T2, T3, T4, T5, T6, T7, TResult>(IEnumerable<TSource> source, Doer<TSource, T1, T2, T3, T4, T5, T6, T7, TResult> body, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7)
        {
            return ForEachTask(ThreadUtility.DefaultNumberOfConcurrentWorkerThreads, source, body, arg1, arg2, arg3, arg4, arg5, arg6, arg7);
        }

        /// <summary>
        /// Provides a generic way of executing a parallel foreach-loop while providing ways to encapsulate and re-use existing code having a return value.
        /// </summary>
        /// <typeparam name="TSource">The type of the data in the <paramref name="source"/>.</typeparam>
        /// <typeparam name="TResult">The type of the return value of the <paramref name="body"/> that this function delegate encapsulates.</typeparam>
        /// <typeparam name="T1">The type of the first parameter of the function delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the function delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the function delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of the function delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T5">The type of the fifth parameter of the function delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T6">The type of the sixth parameter of the function delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T7">The type of the seventh parameter of the function delegate <paramref name="body" />.</typeparam>
        /// <param name="partitionSize">The maximum number of concurrent worker threads per partition.</param>
        /// <param name="source">The sequence to iterate over parallel.</param>
        /// <param name="body">The delegate that is invoked once per iteration.</param>
        /// <param name="arg1">The first parameter of the function delegate <paramref name="body" />.</param>
        /// <param name="arg2">The second parameter of the function delegate <paramref name="body" />.</param>
        /// <param name="arg3">The third parameter of the function delegate <paramref name="body" />.</param>
        /// <param name="arg4">The fourth parameter of the function delegate <paramref name="body" />.</param>
        /// <param name="arg5">The fifth parameter of the function delegate <paramref name="body" />.</param>
        /// <param name="arg6">The sixth parameter of the function delegate <paramref name="body" />.</param>
        /// <param name="arg7">The seventh parameter of the function delegate <paramref name="body" />.</param>
        /// <returns>A <see cref="IReadOnlyCollection{TResult}"/> where the result of <paramref name="body"/> has been stored in the same sequential order as <paramref name="source"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="source"/> is null -or- <paramref name="body"/> is null.
        /// </exception>
        public static IReadOnlyCollection<TResult> ForEachTask<TSource, T1, T2, T3, T4, T5, T6, T7, TResult>(int partitionSize, IEnumerable<TSource> source, Doer<TSource, T1, T2, T3, T4, T5, T6, T7, TResult> body, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7)
        {
            return ForEachTask(partitionSize, GetSettingsPreInitialized(partitionSize), source, body, arg1, arg2, arg3, arg4, arg5, arg6, arg7);
        }

        /// <summary>
        /// Provides a generic way of executing a parallel foreach-loop while providing ways to encapsulate and re-use existing code having a return value.
        /// </summary>
        /// <typeparam name="TSource">The type of the data in the <paramref name="source"/>.</typeparam>
        /// <typeparam name="TResult">The type of the return value of the <paramref name="body"/> that this function delegate encapsulates.</typeparam>
        /// <typeparam name="T1">The type of the first parameter of the function delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the function delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the function delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of the function delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T5">The type of the fifth parameter of the function delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T6">The type of the sixth parameter of the function delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T7">The type of the seventh parameter of the function delegate <paramref name="body" />.</typeparam>
        /// <param name="partitionSize">The maximum number of concurrent worker threads per partition.</param>
        /// <param name="settings">The <see cref="ThreadPoolSettings"/> object used to temporary configure the static <see cref="ThreadPool"/> instance.</param>
        /// <param name="source">The sequence to iterate over parallel.</param>
        /// <param name="body">The delegate that is invoked once per iteration.</param>
        /// <param name="arg1">The first parameter of the function delegate <paramref name="body" />.</param>
        /// <param name="arg2">The second parameter of the function delegate <paramref name="body" />.</param>
        /// <param name="arg3">The third parameter of the function delegate <paramref name="body" />.</param>
        /// <param name="arg4">The fourth parameter of the function delegate <paramref name="body" />.</param>
        /// <param name="arg5">The fifth parameter of the function delegate <paramref name="body" />.</param>
        /// <param name="arg6">The sixth parameter of the function delegate <paramref name="body" />.</param>
        /// <param name="arg7">The seventh parameter of the function delegate <paramref name="body" />.</param>
        /// <returns>A <see cref="IReadOnlyCollection{TResult}"/> where the result of <paramref name="body"/> has been stored in the same sequential order as <paramref name="source"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="settings"/> is null -or- <paramref name="source"/> is null -or- <paramref name="body"/> is null.
        /// </exception>
        public static IReadOnlyCollection<TResult> ForEachTask<TSource, T1, T2, T3, T4, T5, T6, T7, TResult>(int partitionSize, ThreadPoolSettings settings, IEnumerable<TSource> source, Doer<TSource, T1, T2, T3, T4, T5, T6, T7, TResult> body, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7)
        {
            return ForEachTask(partitionSize, settings, TimeSpan.MaxValue, source, body, arg1, arg2, arg3, arg4, arg5, arg6, arg7);
        }

        /// <summary>
        /// Provides a generic way of executing a parallel foreach-loop while providing ways to encapsulate and re-use existing code having a return value.
        /// </summary>
        /// <typeparam name="TSource">The type of the data in the <paramref name="source"/>.</typeparam>
        /// <typeparam name="TResult">The type of the return value of the <paramref name="body"/> that this function delegate encapsulates.</typeparam>
        /// <typeparam name="T1">The type of the first parameter of the function delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the function delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the function delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of the function delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T5">The type of the fifth parameter of the function delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T6">The type of the sixth parameter of the function delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T7">The type of the seventh parameter of the function delegate <paramref name="body" />.</typeparam>
        /// <param name="partitionSize">The maximum number of concurrent worker threads per partition.</param>
        /// <param name="settings">The <see cref="ThreadPoolSettings"/> object used to temporary configure the static <see cref="ThreadPool"/> instance.</param>
        /// <param name="timeout">A <see cref="TimeSpan" /> that represents the time to wait for the parallel foreach-loop operation to complete.</param>
        /// <param name="source">The sequence to iterate over parallel.</param>
        /// <param name="body">The delegate that is invoked once per iteration.</param>
        /// <param name="arg1">The first parameter of the function delegate <paramref name="body" />.</param>
        /// <param name="arg2">The second parameter of the function delegate <paramref name="body" />.</param>
        /// <param name="arg3">The third parameter of the function delegate <paramref name="body" />.</param>
        /// <param name="arg4">The fourth parameter of the function delegate <paramref name="body" />.</param>
        /// <param name="arg5">The fifth parameter of the function delegate <paramref name="body" />.</param>
        /// <param name="arg6">The sixth parameter of the function delegate <paramref name="body" />.</param>
        /// <param name="arg7">The seventh parameter of the function delegate <paramref name="body" />.</param>
        /// <returns>A <see cref="IReadOnlyCollection{TResult}"/> where the result of <paramref name="body"/> has been stored in the same sequential order as <paramref name="source"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="settings"/> is null -or- <paramref name="source"/> is null -or- <paramref name="body"/> is null.
        /// </exception>
        public static IReadOnlyCollection<TResult> ForEachTask<TSource, T1, T2, T3, T4, T5, T6, T7, TResult>(int partitionSize, ThreadPoolSettings settings, TimeSpan timeout, IEnumerable<TSource> source, Doer<TSource, T1, T2, T3, T4, T5, T6, T7, TResult> body, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7)
        {
            Validator.ThrowIfNull(settings, "settings");
            Validator.ThrowIfNull(source, "source");
            Validator.ThrowIfNull(body, "body");
            DoerFactory<TSource, T1, T2, T3, T4, T5, T6, T7, TResult> factory = new DoerFactory<TSource, T1, T2, T3, T4, T5, T6, T7, TResult>(body, default(TSource), arg1, arg2, arg3, arg4, arg5, arg6, arg7);
            return ForEachTaskCore(factory, source, partitionSize, timeout, settings);
        }

        /// <summary>
        /// Provides a generic way of executing a parallel foreach-loop while providing ways to encapsulate and re-use existing code having a return value.
        /// </summary>
        /// <typeparam name="TSource">The type of the data in the <paramref name="source"/>.</typeparam>
        /// <typeparam name="TResult">The type of the return value of the <paramref name="body"/> that this function delegate encapsulates.</typeparam>
        /// <typeparam name="T1">The type of the first parameter of the function delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the function delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the function delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of the function delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T5">The type of the fifth parameter of the function delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T6">The type of the sixth parameter of the function delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T7">The type of the seventh parameter of the function delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T8">The type of the eighth parameter of the function delegate <paramref name="body" />.</typeparam>
        /// <param name="source">The sequence to iterate over parallel.</param>
        /// <param name="body">The delegate that is invoked once per iteration.</param>
        /// <param name="arg1">The first parameter of the function delegate <paramref name="body" />.</param>
        /// <param name="arg2">The second parameter of the function delegate <paramref name="body" />.</param>
        /// <param name="arg3">The third parameter of the function delegate <paramref name="body" />.</param>
        /// <param name="arg4">The fourth parameter of the function delegate <paramref name="body" />.</param>
        /// <param name="arg5">The fifth parameter of the function delegate <paramref name="body" />.</param>
        /// <param name="arg6">The sixth parameter of the function delegate <paramref name="body" />.</param>
        /// <param name="arg7">The seventh parameter of the function delegate <paramref name="body" />.</param>
        /// <param name="arg8">The eighth parameter of the function delegate <paramref name="body" />.</param>
        /// <returns>A <see cref="IReadOnlyCollection{TResult}"/> where the result of <paramref name="body"/> has been stored in the same sequential order as <paramref name="source"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="source"/> is null -or- <paramref name="body"/> is null.
        /// </exception>
        public static IReadOnlyCollection<TResult> ForEachTask<TSource, T1, T2, T3, T4, T5, T6, T7, T8, TResult>(IEnumerable<TSource> source, Doer<TSource, T1, T2, T3, T4, T5, T6, T7, T8, TResult> body, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8)
        {
            return ForEachTask(ThreadUtility.DefaultNumberOfConcurrentWorkerThreads, source, body, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8);
        }

        /// <summary>
        /// Provides a generic way of executing a parallel foreach-loop while providing ways to encapsulate and re-use existing code having a return value.
        /// </summary>
        /// <typeparam name="TSource">The type of the data in the <paramref name="source"/>.</typeparam>
        /// <typeparam name="TResult">The type of the return value of the <paramref name="body"/> that this function delegate encapsulates.</typeparam>
        /// <typeparam name="T1">The type of the first parameter of the function delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the function delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the function delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of the function delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T5">The type of the fifth parameter of the function delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T6">The type of the sixth parameter of the function delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T7">The type of the seventh parameter of the function delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T8">The type of the eighth parameter of the function delegate <paramref name="body" />.</typeparam>
        /// <param name="partitionSize">The maximum number of concurrent worker threads per partition.</param>
        /// <param name="source">The sequence to iterate over parallel.</param>
        /// <param name="body">The delegate that is invoked once per iteration.</param>
        /// <param name="arg1">The first parameter of the function delegate <paramref name="body" />.</param>
        /// <param name="arg2">The second parameter of the function delegate <paramref name="body" />.</param>
        /// <param name="arg3">The third parameter of the function delegate <paramref name="body" />.</param>
        /// <param name="arg4">The fourth parameter of the function delegate <paramref name="body" />.</param>
        /// <param name="arg5">The fifth parameter of the function delegate <paramref name="body" />.</param>
        /// <param name="arg6">The sixth parameter of the function delegate <paramref name="body" />.</param>
        /// <param name="arg7">The seventh parameter of the function delegate <paramref name="body" />.</param>
        /// <param name="arg8">The eighth parameter of the function delegate <paramref name="body" />.</param>
        /// <returns>A <see cref="IReadOnlyCollection{TResult}"/> where the result of <paramref name="body"/> has been stored in the same sequential order as <paramref name="source"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="source"/> is null -or- <paramref name="body"/> is null.
        /// </exception>
        public static IReadOnlyCollection<TResult> ForEachTask<TSource, T1, T2, T3, T4, T5, T6, T7, T8, TResult>(int partitionSize, IEnumerable<TSource> source, Doer<TSource, T1, T2, T3, T4, T5, T6, T7, T8, TResult> body, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8)
        {
            return ForEachTask(partitionSize, GetSettingsPreInitialized(partitionSize), source, body, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8);
        }

        /// <summary>
        /// Provides a generic way of executing a parallel foreach-loop while providing ways to encapsulate and re-use existing code having a return value.
        /// </summary>
        /// <typeparam name="TSource">The type of the data in the <paramref name="source"/>.</typeparam>
        /// <typeparam name="TResult">The type of the return value of the <paramref name="body"/> that this function delegate encapsulates.</typeparam>
        /// <typeparam name="T1">The type of the first parameter of the function delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the function delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the function delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of the function delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T5">The type of the fifth parameter of the function delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T6">The type of the sixth parameter of the function delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T7">The type of the seventh parameter of the function delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T8">The type of the eighth parameter of the function delegate <paramref name="body" />.</typeparam>
        /// <param name="partitionSize">The maximum number of concurrent worker threads per partition.</param>
        /// <param name="settings">The <see cref="ThreadPoolSettings"/> object used to temporary configure the static <see cref="ThreadPool"/> instance.</param>
        /// <param name="source">The sequence to iterate over parallel.</param>
        /// <param name="body">The delegate that is invoked once per iteration.</param>
        /// <param name="arg1">The first parameter of the function delegate <paramref name="body" />.</param>
        /// <param name="arg2">The second parameter of the function delegate <paramref name="body" />.</param>
        /// <param name="arg3">The third parameter of the function delegate <paramref name="body" />.</param>
        /// <param name="arg4">The fourth parameter of the function delegate <paramref name="body" />.</param>
        /// <param name="arg5">The fifth parameter of the function delegate <paramref name="body" />.</param>
        /// <param name="arg6">The sixth parameter of the function delegate <paramref name="body" />.</param>
        /// <param name="arg7">The seventh parameter of the function delegate <paramref name="body" />.</param>
        /// <param name="arg8">The eighth parameter of the function delegate <paramref name="body" />.</param>
        /// <returns>A <see cref="IReadOnlyCollection{TResult}"/> where the result of <paramref name="body"/> has been stored in the same sequential order as <paramref name="source"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="settings"/> is null -or- <paramref name="source"/> is null -or- <paramref name="body"/> is null.
        /// </exception>
        public static IReadOnlyCollection<TResult> ForEachTask<TSource, T1, T2, T3, T4, T5, T6, T7, T8, TResult>(int partitionSize, ThreadPoolSettings settings, IEnumerable<TSource> source, Doer<TSource, T1, T2, T3, T4, T5, T6, T7, T8, TResult> body, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8)
        {
            return ForEachTask(partitionSize, settings, TimeSpan.MaxValue, source, body, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8);
        }

        /// <summary>
        /// Provides a generic way of executing a parallel foreach-loop while providing ways to encapsulate and re-use existing code having a return value.
        /// </summary>
        /// <typeparam name="TSource">The type of the data in the <paramref name="source"/>.</typeparam>
        /// <typeparam name="TResult">The type of the return value of the <paramref name="body"/> that this function delegate encapsulates.</typeparam>
        /// <typeparam name="T1">The type of the first parameter of the function delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the function delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the function delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of the function delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T5">The type of the fifth parameter of the function delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T6">The type of the sixth parameter of the function delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T7">The type of the seventh parameter of the function delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T8">The type of the eighth parameter of the function delegate <paramref name="body" />.</typeparam>
        /// <param name="partitionSize">The maximum number of concurrent worker threads per partition.</param>
        /// <param name="settings">The <see cref="ThreadPoolSettings"/> object used to temporary configure the static <see cref="ThreadPool"/> instance.</param>
        /// <param name="timeout">A <see cref="TimeSpan" /> that represents the time to wait for the parallel foreach-loop operation to complete.</param>
        /// <param name="source">The sequence to iterate over parallel.</param>
        /// <param name="body">The delegate that is invoked once per iteration.</param>
        /// <param name="arg1">The first parameter of the function delegate <paramref name="body" />.</param>
        /// <param name="arg2">The second parameter of the function delegate <paramref name="body" />.</param>
        /// <param name="arg3">The third parameter of the function delegate <paramref name="body" />.</param>
        /// <param name="arg4">The fourth parameter of the function delegate <paramref name="body" />.</param>
        /// <param name="arg5">The fifth parameter of the function delegate <paramref name="body" />.</param>
        /// <param name="arg6">The sixth parameter of the function delegate <paramref name="body" />.</param>
        /// <param name="arg7">The seventh parameter of the function delegate <paramref name="body" />.</param>
        /// <param name="arg8">The eighth parameter of the function delegate <paramref name="body" />.</param>
        /// <returns>A <see cref="IReadOnlyCollection{TResult}"/> where the result of <paramref name="body"/> has been stored in the same sequential order as <paramref name="source"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="settings"/> is null -or- <paramref name="source"/> is null -or- <paramref name="body"/> is null.
        /// </exception>
        public static IReadOnlyCollection<TResult> ForEachTask<TSource, T1, T2, T3, T4, T5, T6, T7, T8, TResult>(int partitionSize, ThreadPoolSettings settings, TimeSpan timeout, IEnumerable<TSource> source, Doer<TSource, T1, T2, T3, T4, T5, T6, T7, T8, TResult> body, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8)
        {
            Validator.ThrowIfNull(settings, "settings");
            Validator.ThrowIfNull(source, "source");
            Validator.ThrowIfNull(body, "body");
            DoerFactory<TSource, T1, T2, T3, T4, T5, T6, T7, T8, TResult> factory = new DoerFactory<TSource, T1, T2, T3, T4, T5, T6, T7, T8, TResult>(body, default(TSource), arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8);
            return ForEachTaskCore(factory, source, partitionSize, timeout, settings);
        }

        /// <summary>
        /// Provides a generic way of executing a parallel foreach-loop while providing ways to encapsulate and re-use existing code having a return value.
        /// </summary>
        /// <typeparam name="TSource">The type of the data in the <paramref name="source"/>.</typeparam>
        /// <typeparam name="TResult">The type of the return value of the <paramref name="body"/> that this function delegate encapsulates.</typeparam>
        /// <typeparam name="T1">The type of the first parameter of the function delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the function delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the function delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of the function delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T5">The type of the fifth parameter of the function delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T6">The type of the sixth parameter of the function delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T7">The type of the seventh parameter of the function delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T8">The type of the eighth parameter of the function delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T9">The type of the ninth parameter of the function delegate <paramref name="body" />.</typeparam>
        /// <param name="source">The sequence to iterate over parallel.</param>
        /// <param name="body">The delegate that is invoked once per iteration.</param>
        /// <param name="arg1">The first parameter of the function delegate <paramref name="body" />.</param>
        /// <param name="arg2">The second parameter of the function delegate <paramref name="body" />.</param>
        /// <param name="arg3">The third parameter of the function delegate <paramref name="body" />.</param>
        /// <param name="arg4">The fourth parameter of the function delegate <paramref name="body" />.</param>
        /// <param name="arg5">The fifth parameter of the function delegate <paramref name="body" />.</param>
        /// <param name="arg6">The sixth parameter of the function delegate <paramref name="body" />.</param>
        /// <param name="arg7">The seventh parameter of the function delegate <paramref name="body" />.</param>
        /// <param name="arg8">The eighth parameter of the function delegate <paramref name="body" />.</param>
        /// <param name="arg9">The ninth parameter of the function delegate <paramref name="body" />.</param>
        /// <returns>A <see cref="IReadOnlyCollection{TResult}"/> where the result of <paramref name="body"/> has been stored in the same sequential order as <paramref name="source"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="source"/> is null -or- <paramref name="body"/> is null.
        /// </exception>
        public static IReadOnlyCollection<TResult> ForEachTask<TSource, T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>(IEnumerable<TSource> source, Doer<TSource, T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult> body, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9)
        {
            return ForEachTask(ThreadUtility.DefaultNumberOfConcurrentWorkerThreads, source, body, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9);
        }

        /// <summary>
        /// Provides a generic way of executing a parallel foreach-loop while providing ways to encapsulate and re-use existing code having a return value.
        /// </summary>
        /// <typeparam name="TSource">The type of the data in the <paramref name="source"/>.</typeparam>
        /// <typeparam name="TResult">The type of the return value of the <paramref name="body"/> that this function delegate encapsulates.</typeparam>
        /// <typeparam name="T1">The type of the first parameter of the function delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the function delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the function delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of the function delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T5">The type of the fifth parameter of the function delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T6">The type of the sixth parameter of the function delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T7">The type of the seventh parameter of the function delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T8">The type of the eighth parameter of the function delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T9">The type of the ninth parameter of the function delegate <paramref name="body" />.</typeparam>
        /// <param name="partitionSize">The maximum number of concurrent worker threads per partition.</param>
        /// <param name="source">The sequence to iterate over parallel.</param>
        /// <param name="body">The delegate that is invoked once per iteration.</param>
        /// <param name="arg1">The first parameter of the function delegate <paramref name="body" />.</param>
        /// <param name="arg2">The second parameter of the function delegate <paramref name="body" />.</param>
        /// <param name="arg3">The third parameter of the function delegate <paramref name="body" />.</param>
        /// <param name="arg4">The fourth parameter of the function delegate <paramref name="body" />.</param>
        /// <param name="arg5">The fifth parameter of the function delegate <paramref name="body" />.</param>
        /// <param name="arg6">The sixth parameter of the function delegate <paramref name="body" />.</param>
        /// <param name="arg7">The seventh parameter of the function delegate <paramref name="body" />.</param>
        /// <param name="arg8">The eighth parameter of the function delegate <paramref name="body" />.</param>
        /// <param name="arg9">The ninth parameter of the function delegate <paramref name="body" />.</param>
        /// <returns>A <see cref="IReadOnlyCollection{TResult}"/> where the result of <paramref name="body"/> has been stored in the same sequential order as <paramref name="source"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="source"/> is null -or- <paramref name="body"/> is null.
        /// </exception>
        public static IReadOnlyCollection<TResult> ForEachTask<TSource, T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>(int partitionSize, IEnumerable<TSource> source, Doer<TSource, T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult> body, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9)
        {
            return ForEachTask(partitionSize, GetSettingsPreInitialized(partitionSize), source, body, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9);
        }

        /// <summary>
        /// Provides a generic way of executing a parallel foreach-loop while providing ways to encapsulate and re-use existing code having a return value.
        /// </summary>
        /// <typeparam name="TSource">The type of the data in the <paramref name="source"/>.</typeparam>
        /// <typeparam name="TResult">The type of the return value of the <paramref name="body"/> that this function delegate encapsulates.</typeparam>
        /// <typeparam name="T1">The type of the first parameter of the function delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the function delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the function delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of the function delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T5">The type of the fifth parameter of the function delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T6">The type of the sixth parameter of the function delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T7">The type of the seventh parameter of the function delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T8">The type of the eighth parameter of the function delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T9">The type of the ninth parameter of the function delegate <paramref name="body" />.</typeparam>
        /// <param name="partitionSize">The maximum number of concurrent worker threads per partition.</param>
        /// <param name="settings">The <see cref="ThreadPoolSettings"/> object used to temporary configure the static <see cref="ThreadPool"/> instance.</param>
        /// <param name="source">The sequence to iterate over parallel.</param>
        /// <param name="body">The delegate that is invoked once per iteration.</param>
        /// <param name="arg1">The first parameter of the function delegate <paramref name="body" />.</param>
        /// <param name="arg2">The second parameter of the function delegate <paramref name="body" />.</param>
        /// <param name="arg3">The third parameter of the function delegate <paramref name="body" />.</param>
        /// <param name="arg4">The fourth parameter of the function delegate <paramref name="body" />.</param>
        /// <param name="arg5">The fifth parameter of the function delegate <paramref name="body" />.</param>
        /// <param name="arg6">The sixth parameter of the function delegate <paramref name="body" />.</param>
        /// <param name="arg7">The seventh parameter of the function delegate <paramref name="body" />.</param>
        /// <param name="arg8">The eighth parameter of the function delegate <paramref name="body" />.</param>
        /// <param name="arg9">The ninth parameter of the function delegate <paramref name="body" />.</param>
        /// <returns>A <see cref="IReadOnlyCollection{TResult}"/> where the result of <paramref name="body"/> has been stored in the same sequential order as <paramref name="source"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="settings"/> is null -or- <paramref name="source"/> is null -or- <paramref name="body"/> is null.
        /// </exception>
        public static IReadOnlyCollection<TResult> ForEachTask<TSource, T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>(int partitionSize, ThreadPoolSettings settings, IEnumerable<TSource> source, Doer<TSource, T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult> body, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9)
        {
            return ForEachTask(partitionSize, settings, TimeSpan.MaxValue, source, body, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9);
        }

        /// <summary>
        /// Provides a generic way of executing a parallel foreach-loop while providing ways to encapsulate and re-use existing code having a return value.
        /// </summary>
        /// <typeparam name="TSource">The type of the data in the <paramref name="source"/>.</typeparam>
        /// <typeparam name="TResult">The type of the return value of the <paramref name="body"/> that this function delegate encapsulates.</typeparam>
        /// <typeparam name="T1">The type of the first parameter of the function delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the function delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the function delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of the function delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T5">The type of the fifth parameter of the function delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T6">The type of the sixth parameter of the function delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T7">The type of the seventh parameter of the function delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T8">The type of the eighth parameter of the function delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T9">The type of the ninth parameter of the function delegate <paramref name="body" />.</typeparam>
        /// <param name="partitionSize">The maximum number of concurrent worker threads per partition.</param>
        /// <param name="settings">The <see cref="ThreadPoolSettings"/> object used to temporary configure the static <see cref="ThreadPool"/> instance.</param>
        /// <param name="timeout">A <see cref="TimeSpan" /> that represents the time to wait for the parallel foreach-loop operation to complete.</param>
        /// <param name="source">The sequence to iterate over parallel.</param>
        /// <param name="body">The delegate that is invoked once per iteration.</param>
        /// <param name="arg1">The first parameter of the function delegate <paramref name="body" />.</param>
        /// <param name="arg2">The second parameter of the function delegate <paramref name="body" />.</param>
        /// <param name="arg3">The third parameter of the function delegate <paramref name="body" />.</param>
        /// <param name="arg4">The fourth parameter of the function delegate <paramref name="body" />.</param>
        /// <param name="arg5">The fifth parameter of the function delegate <paramref name="body" />.</param>
        /// <param name="arg6">The sixth parameter of the function delegate <paramref name="body" />.</param>
        /// <param name="arg7">The seventh parameter of the function delegate <paramref name="body" />.</param>
        /// <param name="arg8">The eighth parameter of the function delegate <paramref name="body" />.</param>
        /// <param name="arg9">The ninth parameter of the function delegate <paramref name="body" />.</param>
        /// <returns>A <see cref="IReadOnlyCollection{TResult}"/> where the result of <paramref name="body"/> has been stored in the same sequential order as <paramref name="source"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="settings"/> is null -or- <paramref name="source"/> is null -or- <paramref name="body"/> is null.
        /// </exception>
        public static IReadOnlyCollection<TResult> ForEachTask<TSource, T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>(int partitionSize, ThreadPoolSettings settings, TimeSpan timeout, IEnumerable<TSource> source, Doer<TSource, T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult> body, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9)
        {
            Validator.ThrowIfNull(settings, "settings");
            Validator.ThrowIfNull(source, "source");
            Validator.ThrowIfNull(body, "body");
            DoerFactory<TSource, T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult> factory = new DoerFactory<TSource, T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>(body, default(TSource), arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9);
            return ForEachTaskCore(factory, source, partitionSize, timeout, settings);
        }

        /// <summary>
        /// Provides a generic way of executing a parallel foreach-loop while providing ways to encapsulate and re-use existing code having a return value.
        /// </summary>
        /// <typeparam name="TSource">The type of the data in the <paramref name="source"/>.</typeparam>
        /// <typeparam name="TResult">The type of the return value of the <paramref name="body"/> that this function delegate encapsulates.</typeparam>
        /// <typeparam name="T1">The type of the first parameter of the function delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the function delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the function delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of the function delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T5">The type of the fifth parameter of the function delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T6">The type of the sixth parameter of the function delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T7">The type of the seventh parameter of the function delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T8">The type of the eighth parameter of the function delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T9">The type of the ninth parameter of the function delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T10">The type of the tenth parameter of the function delegate <paramref name="body" />.</typeparam>
        /// <param name="source">The sequence to iterate over parallel.</param>
        /// <param name="body">The delegate that is invoked once per iteration.</param>
        /// <param name="arg1">The first parameter of the function delegate <paramref name="body" />.</param>
        /// <param name="arg2">The second parameter of the function delegate <paramref name="body" />.</param>
        /// <param name="arg3">The third parameter of the function delegate <paramref name="body" />.</param>
        /// <param name="arg4">The fourth parameter of the function delegate <paramref name="body" />.</param>
        /// <param name="arg5">The fifth parameter of the function delegate <paramref name="body" />.</param>
        /// <param name="arg6">The sixth parameter of the function delegate <paramref name="body" />.</param>
        /// <param name="arg7">The seventh parameter of the function delegate <paramref name="body" />.</param>
        /// <param name="arg8">The eighth parameter of the function delegate <paramref name="body" />.</param>
        /// <param name="arg9">The ninth parameter of the function delegate <paramref name="body" />.</param>
        /// <param name="arg10">The tenth parameter of the function delegate <paramref name="body" />.</param>
        /// <returns>A <see cref="IReadOnlyCollection{TResult}"/> where the result of <paramref name="body"/> has been stored in the same sequential order as <paramref name="source"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="source"/> is null -or- <paramref name="body"/> is null.
        /// </exception>
        public static IReadOnlyCollection<TResult> ForEachTask<TSource, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>(IEnumerable<TSource> source, Doer<TSource, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult> body, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10)
        {
            return ForEachTask(ThreadUtility.DefaultNumberOfConcurrentWorkerThreads, source, body, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10);
        }

        /// <summary>
        /// Provides a generic way of executing a parallel foreach-loop while providing ways to encapsulate and re-use existing code having a return value.
        /// </summary>
        /// <typeparam name="TSource">The type of the data in the <paramref name="source"/>.</typeparam>
        /// <typeparam name="TResult">The type of the return value of the <paramref name="body"/> that this function delegate encapsulates.</typeparam>
        /// <typeparam name="T1">The type of the first parameter of the function delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the function delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the function delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of the function delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T5">The type of the fifth parameter of the function delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T6">The type of the sixth parameter of the function delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T7">The type of the seventh parameter of the function delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T8">The type of the eighth parameter of the function delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T9">The type of the ninth parameter of the function delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T10">The type of the tenth parameter of the function delegate <paramref name="body" />.</typeparam>
        /// <param name="partitionSize">The maximum number of concurrent worker threads per partition.</param>
        /// <param name="source">The sequence to iterate over parallel.</param>
        /// <param name="body">The delegate that is invoked once per iteration.</param>
        /// <param name="arg1">The first parameter of the function delegate <paramref name="body" />.</param>
        /// <param name="arg2">The second parameter of the function delegate <paramref name="body" />.</param>
        /// <param name="arg3">The third parameter of the function delegate <paramref name="body" />.</param>
        /// <param name="arg4">The fourth parameter of the function delegate <paramref name="body" />.</param>
        /// <param name="arg5">The fifth parameter of the function delegate <paramref name="body" />.</param>
        /// <param name="arg6">The sixth parameter of the function delegate <paramref name="body" />.</param>
        /// <param name="arg7">The seventh parameter of the function delegate <paramref name="body" />.</param>
        /// <param name="arg8">The eighth parameter of the function delegate <paramref name="body" />.</param>
        /// <param name="arg9">The ninth parameter of the function delegate <paramref name="body" />.</param>
        /// <param name="arg10">The tenth parameter of the function delegate <paramref name="body" />.</param>
        /// <returns>A <see cref="IReadOnlyCollection{TResult}"/> where the result of <paramref name="body"/> has been stored in the same sequential order as <paramref name="source"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="source"/> is null -or- <paramref name="body"/> is null.
        /// </exception>
        public static IReadOnlyCollection<TResult> ForEachTask<TSource, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>(int partitionSize, IEnumerable<TSource> source, Doer<TSource, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult> body, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10)
        {
            return ForEachTask(partitionSize, GetSettingsPreInitialized(partitionSize), source, body, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10);
        }

        /// <summary>
        /// Provides a generic way of executing a parallel foreach-loop while providing ways to encapsulate and re-use existing code having a return value.
        /// </summary>
        /// <typeparam name="TSource">The type of the data in the <paramref name="source"/>.</typeparam>
        /// <typeparam name="TResult">The type of the return value of the <paramref name="body"/> that this function delegate encapsulates.</typeparam>
        /// <typeparam name="T1">The type of the first parameter of the function delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the function delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the function delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of the function delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T5">The type of the fifth parameter of the function delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T6">The type of the sixth parameter of the function delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T7">The type of the seventh parameter of the function delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T8">The type of the eighth parameter of the function delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T9">The type of the ninth parameter of the function delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T10">The type of the tenth parameter of the function delegate <paramref name="body" />.</typeparam>
        /// <param name="partitionSize">The maximum number of concurrent worker threads per partition.</param>
        /// <param name="settings">The <see cref="ThreadPoolSettings"/> object used to temporary configure the static <see cref="ThreadPool"/> instance.</param>
        /// <param name="source">The sequence to iterate over parallel.</param>
        /// <param name="body">The delegate that is invoked once per iteration.</param>
        /// <param name="arg1">The first parameter of the function delegate <paramref name="body" />.</param>
        /// <param name="arg2">The second parameter of the function delegate <paramref name="body" />.</param>
        /// <param name="arg3">The third parameter of the function delegate <paramref name="body" />.</param>
        /// <param name="arg4">The fourth parameter of the function delegate <paramref name="body" />.</param>
        /// <param name="arg5">The fifth parameter of the function delegate <paramref name="body" />.</param>
        /// <param name="arg6">The sixth parameter of the function delegate <paramref name="body" />.</param>
        /// <param name="arg7">The seventh parameter of the function delegate <paramref name="body" />.</param>
        /// <param name="arg8">The eighth parameter of the function delegate <paramref name="body" />.</param>
        /// <param name="arg9">The ninth parameter of the function delegate <paramref name="body" />.</param>
        /// <param name="arg10">The tenth parameter of the function delegate <paramref name="body" />.</param>
        /// <returns>A <see cref="IReadOnlyCollection{TResult}"/> where the result of <paramref name="body"/> has been stored in the same sequential order as <paramref name="source"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="settings"/> is null -or- <paramref name="source"/> is null -or- <paramref name="body"/> is null.
        /// </exception>
        public static IReadOnlyCollection<TResult> ForEachTask<TSource, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>(int partitionSize, ThreadPoolSettings settings, IEnumerable<TSource> source, Doer<TSource, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult> body, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10)
        {
            return ForEachTask(partitionSize, settings, TimeSpan.MaxValue, source, body, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10);
        }

        /// <summary>
        /// Provides a generic way of executing a parallel foreach-loop while providing ways to encapsulate and re-use existing code having a return value.
        /// </summary>
        /// <typeparam name="TSource">The type of the data in the <paramref name="source"/>.</typeparam>
        /// <typeparam name="TResult">The type of the return value of the <paramref name="body"/> that this function delegate encapsulates.</typeparam>
        /// <typeparam name="T1">The type of the first parameter of the function delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the function delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the function delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of the function delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T5">The type of the fifth parameter of the function delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T6">The type of the sixth parameter of the function delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T7">The type of the seventh parameter of the function delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T8">The type of the eighth parameter of the function delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T9">The type of the ninth parameter of the function delegate <paramref name="body" />.</typeparam>
        /// <typeparam name="T10">The type of the tenth parameter of the function delegate <paramref name="body" />.</typeparam>
        /// <param name="partitionSize">The maximum number of concurrent worker threads per partition.</param>
        /// <param name="settings">The <see cref="ThreadPoolSettings"/> object used to temporary configure the static <see cref="ThreadPool"/> instance.</param>
        /// <param name="timeout">A <see cref="TimeSpan" /> that represents the time to wait for the parallel foreach-loop operation to complete.</param>
        /// <param name="source">The sequence to iterate over parallel.</param>
        /// <param name="body">The delegate that is invoked once per iteration.</param>
        /// <param name="arg1">The first parameter of the function delegate <paramref name="body" />.</param>
        /// <param name="arg2">The second parameter of the function delegate <paramref name="body" />.</param>
        /// <param name="arg3">The third parameter of the function delegate <paramref name="body" />.</param>
        /// <param name="arg4">The fourth parameter of the function delegate <paramref name="body" />.</param>
        /// <param name="arg5">The fifth parameter of the function delegate <paramref name="body" />.</param>
        /// <param name="arg6">The sixth parameter of the function delegate <paramref name="body" />.</param>
        /// <param name="arg7">The seventh parameter of the function delegate <paramref name="body" />.</param>
        /// <param name="arg8">The eighth parameter of the function delegate <paramref name="body" />.</param>
        /// <param name="arg9">The ninth parameter of the function delegate <paramref name="body" />.</param>
        /// <param name="arg10">The tenth parameter of the function delegate <paramref name="body" />.</param>
        /// <returns>A <see cref="IReadOnlyCollection{TResult}"/> where the result of <paramref name="body"/> has been stored in the same sequential order as <paramref name="source"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="settings"/> is null -or- <paramref name="source"/> is null -or- <paramref name="body"/> is null.
        /// </exception>
        public static IReadOnlyCollection<TResult> ForEachTask<TSource, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>(int partitionSize, ThreadPoolSettings settings, TimeSpan timeout, IEnumerable<TSource> source, Doer<TSource, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult> body, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10)
        {
            Validator.ThrowIfNull(settings, "settings");
            Validator.ThrowIfNull(source, "source");
            Validator.ThrowIfNull(body, "body");
            DoerFactory<TSource, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult> factory = new DoerFactory<TSource, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>(body, default(TSource), arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10);
            return ForEachTaskCore(factory, source, partitionSize, timeout, settings);
        }

        private static IReadOnlyCollection<TResult> ForEachTaskCore<TSource, TResult>(DoerFactory<TSource, TResult> factory, IEnumerable<TSource> source, int partitionSize, TimeSpan timeout, ThreadPoolSettings settings)
        {
            CountdownEvent sync = null;
            SortedDoerWorkItemPool<long, TResult> pool = new SortedDoerWorkItemPool<long, TResult>();
            try
            {
                long sorter = 0;
                PartitionCollection<TSource> partition = new PartitionCollection<TSource>(source, partitionSize);
                while (partition.HasPartitions)
                {
                    try
                    {
                        int currentPartitionSize = partition.PartitionSize > partition.Remaining ? partition.Remaining : partition.PartitionSize;
                        sync = new CountdownEvent(currentPartitionSize);
                        foreach (TSource element in partition)
                        {
                            factory.Arg1 = element;
                            ISortedDoerWorkItem<long, TResult> work = SortedDoerWorkItem.Create(sorter, sync, ForEachElementTaskCore, factory.Clone());
                            pool.ProcessWork(work);
                            sorter++;
                        }
                    }
                    finally
                    {
                        sync.Wait(timeout);
                        sync.Dispose();
                        sync = null;
                    }
                }
            }
            finally
            {
                settings.Rollback();
            }

            return pool.Result;
        }

        private static TResult ForEachElementTaskCore<TResult>(DoerFactory<TResult> factory)
        {
            try
            {
                return factory.ExecuteMethod();
            }
            finally
            {
                factory = null;
            }
        }
    }
}
