using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Text;

namespace Cuemon.Diagnostics
{
    /// <summary>
    /// This utility class is designed to make stand-alone time measuring operations easier to work with.
    /// </summary>
    public static class TimeMeasureUtility
    {
        /// <summary>
        /// Time measure the specified method.
        /// </summary>
        /// <param name="method">The method to time measure.</param>
        /// <returns>A <see cref="TimeSpan"/> holding the elapsed execution time of <paramref name="method"/>.</returns>
        public static TimeSpan Measure(Act method)
        {
            if (method == null) { throw new ArgumentNullException("method"); }
            Stopwatch timing = Stopwatch.StartNew();
            method();
            timing.Stop();
            return timing.Elapsed;
        }

        /// <summary>
        /// Time measure the specified method.
        /// </summary>
        /// <typeparam name="T">The type of the parameter of the method.</typeparam>
        /// <param name="method">The method to time measure.</param>
        /// <param name="arg">The parameter of the method that this delegate encapsulates.</param>
        /// <returns>A <see cref="TimeSpan"/> holding the elapsed execution time of <paramref name="method"/>.</returns>
        public static TimeSpan Measure<T>(Act<T> method, T arg)
        {
            if (method == null) { throw new ArgumentNullException("method"); }
            Stopwatch timing = Stopwatch.StartNew();
            method(arg);
            timing.Stop();
            return timing.Elapsed;
        }

        /// <summary>
        /// Time measure the specified method.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter of the method.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the method.</typeparam>
        /// <param name="method">The method to time measure.</param>
        /// <param name="arg1">The first parameter of the method that this delegate encapsulates.</param>
        /// <param name="arg2">The second parameter of the method that this delegate encapsulates.</param>
        /// <returns>A <see cref="TimeSpan"/> holding the elapsed execution time of <paramref name="method"/>.</returns>
        public static TimeSpan Measure<T1, T2>(Act<T1, T2> method, T1 arg1, T2 arg2)
        {
            if (method == null) { throw new ArgumentNullException("method"); }
            Stopwatch timing = Stopwatch.StartNew();
            method(arg1, arg2);
            timing.Stop();
            return timing.Elapsed;
        }

        /// <summary>
        /// Time measure the specified method.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter of the method.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the method.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the method.</typeparam>
        /// <param name="method">The method to time measure.</param>
        /// <param name="arg1">The first parameter of the method that this delegate encapsulates.</param>
        /// <param name="arg2">The second parameter of the method that this delegate encapsulates.</param>
        /// <param name="arg3">The third parameter of the method that this delegate encapsulates.</param>
        /// <returns>A <see cref="TimeSpan"/> holding the elapsed execution time of <paramref name="method"/>.</returns>
        public static TimeSpan Measure<T1, T2, T3>(Act<T1, T2, T3> method, T1 arg1, T2 arg2, T3 arg3)
        {
            if (method == null) { throw new ArgumentNullException("method"); }
            Stopwatch timing = Stopwatch.StartNew();
            method(arg1, arg2, arg3);
            timing.Stop();
            return timing.Elapsed;
        }

        /// <summary>
        /// Time measure the specified method.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter of the method.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the method.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the method.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of the method.</typeparam>
        /// <param name="method">The method to time measure.</param>
        /// <param name="arg1">The first parameter of the method that this delegate encapsulates.</param>
        /// <param name="arg2">The second parameter of the method that this delegate encapsulates.</param>
        /// <param name="arg3">The third parameter of the method that this delegate encapsulates.</param>
        /// <param name="arg4">The fourth parameter of the method that this delegate encapsulates.</param>
        /// <returns>A <see cref="TimeSpan"/> holding the elapsed execution time of <paramref name="method"/>.</returns>
        public static TimeSpan Measure<T1, T2, T3, T4>(Act<T1, T2, T3, T4> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            if (method == null) { throw new ArgumentNullException("method"); }
            Stopwatch timing = Stopwatch.StartNew();
            method(arg1, arg2, arg3, arg4);
            timing.Stop();
            return timing.Elapsed;
        }

        /// <summary>
        /// Time measure the specified method.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter of the method.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the method.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the method.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of the method.</typeparam>
        /// <typeparam name="T5">The type of the fifth parameter of the method.</typeparam>
        /// <param name="method">The method to time measure.</param>
        /// <param name="arg1">The first parameter of the method that this delegate encapsulates.</param>
        /// <param name="arg2">The second parameter of the method that this delegate encapsulates.</param>
        /// <param name="arg3">The third parameter of the method that this delegate encapsulates.</param>
        /// <param name="arg4">The fourth parameter of the method that this delegate encapsulates.</param>
        /// <param name="arg5">The fifth parameter of the method that this delegate encapsulates.</param>
        /// <returns>A <see cref="TimeSpan"/> holding the elapsed execution time of <paramref name="method"/>.</returns>
        public static TimeSpan Measure<T1, T2, T3, T4, T5>(Act<T1, T2, T3, T4, T5> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        {
            if (method == null) { throw new ArgumentNullException("method"); }
            Stopwatch timing = Stopwatch.StartNew();
            method(arg1, arg2, arg3, arg4, arg5);
            timing.Stop();
            return timing.Elapsed;
        }
    }
}
