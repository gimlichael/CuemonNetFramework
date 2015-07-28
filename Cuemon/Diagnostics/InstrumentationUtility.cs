
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Text;

namespace Cuemon.Diagnostics
{
    /// <summary>
    /// Provides a way to diagnostic, monitor and measure performance through <see cref="Act"/> and <see cref="Doer{TResult}"/> delegate overloads.
    /// </summary>
    public static class InstrumentationUtility
    {
        private static readonly FactoryInstrumentation Instrumentation = new FactoryInstrumentation();
        private static Act<string, TimeSpan, IDictionary<string, object>> timeMeasureCompletedValue = DefaultTimeMeasureCompletedCallback;

        /// <summary>
        /// Gets or sets the delegate that is invoked when a time measured instrumentation is completed.
        /// </summary>
        /// <value>The delegate that is invoked when a time measured instrumentation is completed.</value>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="value"/> is null.
        /// </exception>
        public static Act<string, TimeSpan, IDictionary<string, object>> TimeMeasureCompleted
        {
            get { return timeMeasureCompletedValue; }
            set
            {
                if (value == null) { throw new ArgumentNullException("value"); }
                timeMeasureCompletedValue = value;
            }
        }

        /// <summary>
        /// Instrument and executes the specified delegate <paramref name="method"/>.
        /// </summary>
        /// <param name="caller">A <see cref="MethodBase"/> object representing the executing caller method.</param>
        /// <param name="method">The delegate to instrument and invoke.</param>
        /// <remarks>The instrumentation of <paramref name="method"/> is thread safe and the instrumented result is passed to <see cref="TimeMeasureCompleted"/>.</remarks>
        public static void ExecuteAction(MethodBase caller, Act method)
        {
            Instrumentation.ExecuteAction(caller, method);
        }

        /// <summary>
        /// Instrument and executes the specified delegate <paramref name="method"/>.
        /// </summary>
        /// <typeparam name="T">The type of the parameter of the delegate <paramref name="method"/>.</typeparam>
        /// <param name="caller">A <see cref="MethodBase"/> object representing the executing caller method.</param>
        /// <param name="method">The delegate to instrument and invoke.</param>
        /// <param name="arg">The parameter of the delegate <paramref name="method"/>.</param>
        /// <remarks>The instrumentation of <paramref name="method"/> is thread safe and the instrumented result is passed to <see cref="TimeMeasureCompleted"/>.</remarks>
        public static void ExecuteAction<T>(MethodBase caller, Act<T> method, T arg)
        {
            Instrumentation.ExecuteAction(caller, method, arg);
        }

        /// <summary>
        /// Instrument and executes the specified delegate <paramref name="method"/>.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter of the delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the delegate <paramref name="method"/>.</typeparam>
        /// <param name="caller">A <see cref="MethodBase"/> object representing the executing caller method.</param>
        /// <param name="method">The delegate to instrument and invoke.</param>
        /// <param name="arg1">The first parameter of the delegate <paramref name="method"/>.</param>
        /// <param name="arg2">The second parameter of the delegate <paramref name="method"/>.</param>
        /// <remarks>The instrumentation of <paramref name="method"/> is thread safe and the instrumented result is passed to <see cref="TimeMeasureCompleted"/>.</remarks>
        public static void ExecuteAction<T1, T2>(MethodBase caller, Act<T1, T2> method, T1 arg1, T2 arg2)
        {
            Instrumentation.ExecuteAction(caller, method, arg1, arg2);
        }

        /// <summary>
        /// Instrument and executes the specified delegate <paramref name="method"/>.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter of the delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the delegate <paramref name="method"/>.</typeparam>
        /// <param name="caller">A <see cref="MethodBase"/> object representing the executing caller method.</param>
        /// <param name="method">The delegate to instrument and invoke.</param>
        /// <param name="arg1">The first parameter of the delegate <paramref name="method"/>.</param>
        /// <param name="arg2">The second parameter of the delegate <paramref name="method"/>.</param>
        /// <param name="arg3">The third parameter of the delegate <paramref name="method"/>.</param>
        /// <remarks>The instrumentation of <paramref name="method"/> is thread safe and the instrumented result is passed to <see cref="TimeMeasureCompleted"/>.</remarks>
        public static void ExecuteAction<T1, T2, T3>(MethodBase caller, Act<T1, T2, T3> method, T1 arg1, T2 arg2, T3 arg3)
        {
            Instrumentation.ExecuteAction(caller, method, arg1, arg2, arg3);
        }

        /// <summary>
        /// Instrument and executes the specified delegate <paramref name="method"/>.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter of the delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of the delegate <paramref name="method"/>.</typeparam>
        /// <param name="caller">A <see cref="MethodBase"/> object representing the executing caller method.</param>
        /// <param name="method">The delegate to instrument and invoke.</param>
        /// <param name="arg1">The first parameter of the delegate <paramref name="method"/>.</param>
        /// <param name="arg2">The second parameter of the delegate <paramref name="method"/>.</param>
        /// <param name="arg3">The third parameter of the delegate <paramref name="method"/>.</param>
        /// <param name="arg4">The fourth parameter of the delegate <paramref name="method"/>.</param>
        /// <remarks>The instrumentation of <paramref name="method"/> is thread safe and the instrumented result is passed to <see cref="TimeMeasureCompleted"/>.</remarks>
        public static void ExecuteAction<T1, T2, T3, T4>(MethodBase caller, Act<T1, T2, T3, T4> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            Instrumentation.ExecuteAction(caller, method, arg1, arg2, arg3, arg4);
        }

        /// <summary>
        /// Instrument and executes the specified delegate <paramref name="method"/>.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter of the delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of the delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T5">The type of the fifth parameter of the delegate <paramref name="method"/>.</typeparam>
        /// <param name="caller">A <see cref="MethodBase"/> object representing the executing caller method.</param>
        /// <param name="method">The delegate to instrument and invoke.</param>
        /// <param name="arg1">The first parameter of the delegate <paramref name="method"/>.</param>
        /// <param name="arg2">The second parameter of the delegate <paramref name="method"/>.</param>
        /// <param name="arg3">The third parameter of the delegate <paramref name="method"/>.</param>
        /// <param name="arg4">The fourth parameter of the delegate <paramref name="method"/>.</param>
        /// <param name="arg5">The fifth parameter of the delegate <paramref name="method"/>.</param>
        /// <remarks>The instrumentation of <paramref name="method"/> is thread safe and the instrumented result is passed to <see cref="TimeMeasureCompleted"/>.</remarks>
        public static void ExecuteAction<T1, T2, T3, T4, T5>(MethodBase caller, Act<T1, T2, T3, T4, T5> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        {
            Instrumentation.ExecuteAction(caller, method, arg1, arg2, arg3, arg4, arg5);
        }

        /// <summary>
        /// Instrument and executes the specified delegate <paramref name="method"/>.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter of the delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of the delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T5">The type of the fifth parameter of the delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T6">The type of the sixth parameter of the delegate <paramref name="method"/>.</typeparam>
        /// <param name="caller">A <see cref="MethodBase"/> object representing the executing caller method.</param>
        /// <param name="method">The delegate to instrument and invoke.</param>
        /// <param name="arg1">The first parameter of the delegate <paramref name="method"/>.</param>
        /// <param name="arg2">The second parameter of the delegate <paramref name="method"/>.</param>
        /// <param name="arg3">The third parameter of the delegate <paramref name="method"/>.</param>
        /// <param name="arg4">The fourth parameter of the delegate <paramref name="method"/>.</param>
        /// <param name="arg5">The fifth parameter of the delegate <paramref name="method"/>.</param>
        /// <param name="arg6">The sixth parameter of the delegate <paramref name="method"/>.</param>
        /// <remarks>The instrumentation of <paramref name="method"/> is thread safe and the instrumented result is passed to <see cref="TimeMeasureCompleted"/>.</remarks>
        public static void ExecuteAction<T1, T2, T3, T4, T5, T6>(MethodBase caller, Act<T1, T2, T3, T4, T5, T6> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6)
        {
            Instrumentation.ExecuteAction(caller, method, arg1, arg2, arg3, arg4, arg5, arg6);
        }

        /// <summary>
        /// Instrument and executes the specified delegate <paramref name="method"/>.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter of the delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of the delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T5">The type of the fifth parameter of the delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T6">The type of the sixth parameter of the delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T7">The type of the seventh parameter of the delegate <paramref name="method"/>.</typeparam>
        /// <param name="caller">A <see cref="MethodBase"/> object representing the executing caller method.</param>
        /// <param name="method">The delegate to instrument and invoke.</param>
        /// <param name="arg1">The first parameter of the delegate <paramref name="method"/>.</param>
        /// <param name="arg2">The second parameter of the delegate <paramref name="method"/>.</param>
        /// <param name="arg3">The third parameter of the delegate <paramref name="method"/>.</param>
        /// <param name="arg4">The fourth parameter of the delegate <paramref name="method"/>.</param>
        /// <param name="arg5">The fifth parameter of the delegate <paramref name="method"/>.</param>
        /// <param name="arg6">The sixth parameter of the delegate <paramref name="method"/>.</param>
        /// <param name="arg7">The seventh parameter of the delegate <paramref name="method"/>.</param>
        /// <remarks>The instrumentation of <paramref name="method"/> is thread safe and the instrumented result is passed to <see cref="TimeMeasureCompleted"/>.</remarks>
        public static void ExecuteAction<T1, T2, T3, T4, T5, T6, T7>(MethodBase caller, Act<T1, T2, T3, T4, T5, T6, T7> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7)
        {
            Instrumentation.ExecuteAction(caller, method, arg1, arg2, arg3, arg4, arg5, arg6, arg7);
        }

        /// <summary>
        /// Instrument and executes the specified delegate <paramref name="method"/>.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter of the delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of the delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T5">The type of the fifth parameter of the delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T6">The type of the sixth parameter of the delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T7">The type of the seventh parameter of the delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T8">The type of the eighth parameter of the delegate <paramref name="method"/>.</typeparam>
        /// <param name="caller">A <see cref="MethodBase"/> object representing the executing caller method.</param>
        /// <param name="method">The delegate to instrument and invoke.</param>
        /// <param name="arg1">The first parameter of the delegate <paramref name="method"/>.</param>
        /// <param name="arg2">The second parameter of the delegate <paramref name="method"/>.</param>
        /// <param name="arg3">The third parameter of the delegate <paramref name="method"/>.</param>
        /// <param name="arg4">The fourth parameter of the delegate <paramref name="method"/>.</param>
        /// <param name="arg5">The fifth parameter of the delegate <paramref name="method"/>.</param>
        /// <param name="arg6">The sixth parameter of the delegate <paramref name="method"/>.</param>
        /// <param name="arg7">The seventh parameter of the delegate <paramref name="method"/>.</param>
        /// <param name="arg8">The eighth parameter of the delegate <paramref name="method"/>.</param>
        /// <remarks>The instrumentation of <paramref name="method"/> is thread safe and the instrumented result is passed to <see cref="TimeMeasureCompleted"/>.</remarks>
        public static void ExecuteAction<T1, T2, T3, T4, T5, T6, T7, T8>(MethodBase caller, Act<T1, T2, T3, T4, T5, T6, T7, T8> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8)
        {
            Instrumentation.ExecuteAction(caller, method, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8);
        }

        /// <summary>
        /// Instrument and executes the specified delegate <paramref name="method"/>.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter of the delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of the delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T5">The type of the fifth parameter of the delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T6">The type of the sixth parameter of the delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T7">The type of the seventh parameter of the delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T8">The type of the eighth parameter of the delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T9">The type of the ninth parameter of the delegate <paramref name="method"/>.</typeparam>
        /// <param name="caller">A <see cref="MethodBase"/> object representing the executing caller method.</param>
        /// <param name="method">The delegate to instrument and invoke.</param>
        /// <param name="arg1">The first parameter of the delegate <paramref name="method"/>.</param>
        /// <param name="arg2">The second parameter of the delegate <paramref name="method"/>.</param>
        /// <param name="arg3">The third parameter of the delegate <paramref name="method"/>.</param>
        /// <param name="arg4">The fourth parameter of the delegate <paramref name="method"/>.</param>
        /// <param name="arg5">The fifth parameter of the delegate <paramref name="method"/>.</param>
        /// <param name="arg6">The sixth parameter of the delegate <paramref name="method"/>.</param>
        /// <param name="arg7">The seventh parameter of the delegate <paramref name="method"/>.</param>
        /// <param name="arg8">The eighth parameter of the delegate <paramref name="method"/>.</param>
        /// <param name="arg9">The ninth parameter of the delegate <paramref name="method"/>.</param>
        /// <remarks>The instrumentation of <paramref name="method"/> is thread safe and the instrumented result is passed to <see cref="TimeMeasureCompleted"/>.</remarks>
        public static void ExecuteAction<T1, T2, T3, T4, T5, T6, T7, T8, T9>(MethodBase caller, Act<T1, T2, T3, T4, T5, T6, T7, T8, T9> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9)
        {
            Instrumentation.ExecuteAction(caller, method, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9);
        }

        /// <summary>
        /// Instrument and executes the specified delegate <paramref name="method"/>.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter of the delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of the delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T5">The type of the fifth parameter of the delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T6">The type of the sixth parameter of the delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T7">The type of the seventh parameter of the delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T8">The type of the eighth parameter of the delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T9">The type of the ninth parameter of the delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T10">The type of the tenth parameter of the delegate <paramref name="method"/>.</typeparam>
        /// <param name="caller">A <see cref="MethodBase"/> object representing the executing caller method.</param>
        /// <param name="method">The delegate to instrument and invoke.</param>
        /// <param name="arg1">The first parameter of the delegate <paramref name="method"/>.</param>
        /// <param name="arg2">The second parameter of the delegate <paramref name="method"/>.</param>
        /// <param name="arg3">The third parameter of the delegate <paramref name="method"/>.</param>
        /// <param name="arg4">The fourth parameter of the delegate <paramref name="method"/>.</param>
        /// <param name="arg5">The fifth parameter of the delegate <paramref name="method"/>.</param>
        /// <param name="arg6">The sixth parameter of the delegate <paramref name="method"/>.</param>
        /// <param name="arg7">The seventh parameter of the delegate <paramref name="method"/>.</param>
        /// <param name="arg8">The eighth parameter of the delegate <paramref name="method"/>.</param>
        /// <param name="arg9">The ninth parameter of the delegate <paramref name="method"/>.</param>
        /// <param name="arg10">The tenth parameter of the delegate <paramref name="method"/>.</param>
        /// <remarks>The instrumentation of <paramref name="method"/> is thread safe and the instrumented result is passed to <see cref="TimeMeasureCompleted"/>.</remarks>
        public static void ExecuteAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(MethodBase caller, Act<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10)
        {
            Instrumentation.ExecuteAction(caller, method, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10);
        }

        /// <summary>
        /// Instrument and executes the specified delegate <paramref name="method"/>.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter of the delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of the delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T5">The type of the fifth parameter of the delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T6">The type of the sixth parameter of the delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T7">The type of the seventh parameter of the delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T8">The type of the eighth parameter of the delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T9">The type of the ninth parameter of the delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T10">The type of the tenth parameter of the delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T11">The type of the eleventh parameter of the delegate <paramref name="method"/>.</typeparam>
        /// <param name="caller">A <see cref="MethodBase"/> object representing the executing caller method.</param>
        /// <param name="method">The delegate to instrument and invoke.</param>
        /// <param name="arg1">The first parameter of the delegate <paramref name="method"/>.</param>
        /// <param name="arg2">The second parameter of the delegate <paramref name="method"/>.</param>
        /// <param name="arg3">The third parameter of the delegate <paramref name="method"/>.</param>
        /// <param name="arg4">The fourth parameter of the delegate <paramref name="method"/>.</param>
        /// <param name="arg5">The fifth parameter of the delegate <paramref name="method"/>.</param>
        /// <param name="arg6">The sixth parameter of the delegate <paramref name="method"/>.</param>
        /// <param name="arg7">The seventh parameter of the delegate <paramref name="method"/>.</param>
        /// <param name="arg8">The eighth parameter of the delegate <paramref name="method"/>.</param>
        /// <param name="arg9">The ninth parameter of the delegate <paramref name="method"/>.</param>
        /// <param name="arg10">The tenth parameter of the delegate <paramref name="method"/>.</param>
        /// <param name="arg11">The eleventh parameter of the delegate <paramref name="method"/>.</param>
        /// <remarks>The instrumentation of <paramref name="method"/> is thread safe and the instrumented result is passed to <see cref="TimeMeasureCompleted"/>.</remarks>
        public static void ExecuteAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(MethodBase caller, Act<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11)
        {
            Instrumentation.ExecuteAction(caller, method, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11);
        }

        /// <summary>
        /// Instrument and executes the specified delegate <paramref name="method"/>.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter of the delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of the delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T5">The type of the fifth parameter of the delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T6">The type of the sixth parameter of the delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T7">The type of the seventh parameter of the delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T8">The type of the eighth parameter of the delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T9">The type of the ninth parameter of the delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T10">The type of the tenth parameter of the delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T11">The type of the eleventh parameter of the delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T12">The type of the twelfth parameter of the delegate <paramref name="method"/>.</typeparam>
        /// <param name="caller">A <see cref="MethodBase"/> object representing the executing caller method.</param>
        /// <param name="method">The delegate to instrument and invoke.</param>
        /// <param name="arg1">The first parameter of the delegate <paramref name="method"/>.</param>
        /// <param name="arg2">The second parameter of the delegate <paramref name="method"/>.</param>
        /// <param name="arg3">The third parameter of the delegate <paramref name="method"/>.</param>
        /// <param name="arg4">The fourth parameter of the delegate <paramref name="method"/>.</param>
        /// <param name="arg5">The fifth parameter of the delegate <paramref name="method"/>.</param>
        /// <param name="arg6">The sixth parameter of the delegate <paramref name="method"/>.</param>
        /// <param name="arg7">The seventh parameter of the delegate <paramref name="method"/>.</param>
        /// <param name="arg8">The eighth parameter of the delegate <paramref name="method"/>.</param>
        /// <param name="arg9">The ninth parameter of the delegate <paramref name="method"/>.</param>
        /// <param name="arg10">The tenth parameter of the delegate <paramref name="method"/>.</param>
        /// <param name="arg11">The eleventh parameter of the delegate <paramref name="method"/>.</param>
        /// <param name="arg12">The twelfth parameter of the delegate <paramref name="method"/>.</param>
        /// <remarks>The instrumentation of <paramref name="method"/> is thread safe and the instrumented result is passed to <see cref="TimeMeasureCompleted"/>.</remarks>
        public static void ExecuteAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(MethodBase caller, Act<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12)
        {
            Instrumentation.ExecuteAction(caller, method, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12);
        }

        /// <summary>
        /// Instrument and executes the specified delegate <paramref name="method"/>.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter of the delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of the delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T5">The type of the fifth parameter of the delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T6">The type of the sixth parameter of the delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T7">The type of the seventh parameter of the delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T8">The type of the eighth parameter of the delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T9">The type of the ninth parameter of the delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T10">The type of the tenth parameter of the delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T11">The type of the eleventh parameter of the delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T12">The type of the twelfth parameter of the delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T13">The type of the thirteenth parameter of the delegate <paramref name="method"/>.</typeparam>
        /// <param name="caller">A <see cref="MethodBase"/> object representing the executing caller method.</param>
        /// <param name="method">The delegate to instrument and invoke.</param>
        /// <param name="arg1">The first parameter of the delegate <paramref name="method"/>.</param>
        /// <param name="arg2">The second parameter of the delegate <paramref name="method"/>.</param>
        /// <param name="arg3">The third parameter of the delegate <paramref name="method"/>.</param>
        /// <param name="arg4">The fourth parameter of the delegate <paramref name="method"/>.</param>
        /// <param name="arg5">The fifth parameter of the delegate <paramref name="method"/>.</param>
        /// <param name="arg6">The sixth parameter of the delegate <paramref name="method"/>.</param>
        /// <param name="arg7">The seventh parameter of the delegate <paramref name="method"/>.</param>
        /// <param name="arg8">The eighth parameter of the delegate <paramref name="method"/>.</param>
        /// <param name="arg9">The ninth parameter of the delegate <paramref name="method"/>.</param>
        /// <param name="arg10">The tenth parameter of the delegate <paramref name="method"/>.</param>
        /// <param name="arg11">The eleventh parameter of the delegate <paramref name="method"/>.</param>
        /// <param name="arg12">The twelfth parameter of the delegate <paramref name="method"/>.</param>
        /// <param name="arg13">The thirteenth parameter of the delegate <paramref name="method"/>.</param>
        /// <remarks>The instrumentation of <paramref name="method"/> is thread safe and the instrumented result is passed to <see cref="TimeMeasureCompleted"/>.</remarks>
        public static void ExecuteAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(MethodBase caller, Act<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13)
        {
            Instrumentation.ExecuteAction(caller, method, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13);
        }

        /// <summary>
        /// Instrument and executes the specified delegate <paramref name="method"/>.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter of the delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of the delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T5">The type of the fifth parameter of the delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T6">The type of the sixth parameter of the delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T7">The type of the seventh parameter of the delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T8">The type of the eighth parameter of the delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T9">The type of the ninth parameter of the delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T10">The type of the tenth parameter of the delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T11">The type of the eleventh parameter of the delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T12">The type of the twelfth parameter of the delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T13">The type of the thirteenth parameter of the delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T14">The type of the fourteenth parameter of the delegate <paramref name="method"/>.</typeparam>
        /// <param name="caller">A <see cref="MethodBase"/> object representing the executing caller method.</param>
        /// <param name="method">The delegate to instrument and invoke.</param>
        /// <param name="arg1">The first parameter of the delegate <paramref name="method"/>.</param>
        /// <param name="arg2">The second parameter of the delegate <paramref name="method"/>.</param>
        /// <param name="arg3">The third parameter of the delegate <paramref name="method"/>.</param>
        /// <param name="arg4">The fourth parameter of the delegate <paramref name="method"/>.</param>
        /// <param name="arg5">The fifth parameter of the delegate <paramref name="method"/>.</param>
        /// <param name="arg6">The sixth parameter of the delegate <paramref name="method"/>.</param>
        /// <param name="arg7">The seventh parameter of the delegate <paramref name="method"/>.</param>
        /// <param name="arg8">The eighth parameter of the delegate <paramref name="method"/>.</param>
        /// <param name="arg9">The ninth parameter of the delegate <paramref name="method"/>.</param>
        /// <param name="arg10">The tenth parameter of the delegate <paramref name="method"/>.</param>
        /// <param name="arg11">The eleventh parameter of the delegate <paramref name="method"/>.</param>
        /// <param name="arg12">The twelfth parameter of the delegate <paramref name="method"/>.</param>
        /// <param name="arg13">The thirteenth parameter of the delegate <paramref name="method"/>.</param>
        /// <param name="arg14">The fourteenth parameter of the delegate <paramref name="method"/>.</param>
        /// <remarks>The instrumentation of <paramref name="method"/> is thread safe and the instrumented result is passed to <see cref="TimeMeasureCompleted"/>.</remarks>
        public static void ExecuteAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(MethodBase caller, Act<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14)
        {
            Instrumentation.ExecuteAction(caller, method, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14);
        }

        /// <summary>
        /// Instrument and executes the specified delegate <paramref name="method"/>.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter of the delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of the delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T5">The type of the fifth parameter of the delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T6">The type of the sixth parameter of the delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T7">The type of the seventh parameter of the delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T8">The type of the eighth parameter of the delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T9">The type of the ninth parameter of the delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T10">The type of the tenth parameter of the delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T11">The type of the eleventh parameter of the delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T12">The type of the twelfth parameter of the delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T13">The type of the thirteenth parameter of the delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T14">The type of the fourteenth parameter of the delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T15">The type of the fifteenth parameter of the delegate <paramref name="method"/>.</typeparam>
        /// <param name="caller">A <see cref="MethodBase"/> object representing the executing caller method.</param>
        /// <param name="method">The delegate to instrument and invoke.</param>
        /// <param name="arg1">The first parameter of the delegate <paramref name="method"/>.</param>
        /// <param name="arg2">The second parameter of the delegate <paramref name="method"/>.</param>
        /// <param name="arg3">The third parameter of the delegate <paramref name="method"/>.</param>
        /// <param name="arg4">The fourth parameter of the delegate <paramref name="method"/>.</param>
        /// <param name="arg5">The fifth parameter of the delegate <paramref name="method"/>.</param>
        /// <param name="arg6">The sixth parameter of the delegate <paramref name="method"/>.</param>
        /// <param name="arg7">The seventh parameter of the delegate <paramref name="method"/>.</param>
        /// <param name="arg8">The eighth parameter of the delegate <paramref name="method"/>.</param>
        /// <param name="arg9">The ninth parameter of the delegate <paramref name="method"/>.</param>
        /// <param name="arg10">The tenth parameter of the delegate <paramref name="method"/>.</param>
        /// <param name="arg11">The eleventh parameter of the delegate <paramref name="method"/>.</param>
        /// <param name="arg12">The twelfth parameter of the delegate <paramref name="method"/>.</param>
        /// <param name="arg13">The thirteenth parameter of the delegate <paramref name="method"/>.</param>
        /// <param name="arg14">The fourteenth parameter of the delegate <paramref name="method"/>.</param>
        /// <param name="arg15">The fifteenth parameter of the delegate <paramref name="method"/>.</param>
        /// <remarks>The instrumentation of <paramref name="method"/> is thread safe and the instrumented result is passed to <see cref="TimeMeasureCompleted"/>.</remarks>
        public static void ExecuteAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(MethodBase caller, Act<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15)
        {
            Instrumentation.ExecuteAction(caller, method, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15);
        }

        /// <summary>
        /// Instrument and executes the specified delegate <paramref name="method"/>.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter of the delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of the delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T5">The type of the fifth parameter of the delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T6">The type of the sixth parameter of the delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T7">The type of the seventh parameter of the delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T8">The type of the eighth parameter of the delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T9">The type of the ninth parameter of the delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T10">The type of the tenth parameter of the delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T11">The type of the eleventh parameter of the delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T12">The type of the twelfth parameter of the delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T13">The type of the thirteenth parameter of the delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T14">The type of the fourteenth parameter of the delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T15">The type of the fifteenth parameter of the delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T16">The type of the sixteenth parameter of the delegate <paramref name="method"/>.</typeparam>
        /// <param name="caller">A <see cref="MethodBase"/> object representing the executing caller method.</param>
        /// <param name="method">The delegate to instrument and invoke.</param>
        /// <param name="arg1">The first parameter of the delegate <paramref name="method"/>.</param>
        /// <param name="arg2">The second parameter of the delegate <paramref name="method"/>.</param>
        /// <param name="arg3">The third parameter of the delegate <paramref name="method"/>.</param>
        /// <param name="arg4">The fourth parameter of the delegate <paramref name="method"/>.</param>
        /// <param name="arg5">The fifth parameter of the delegate <paramref name="method"/>.</param>
        /// <param name="arg6">The sixth parameter of the delegate <paramref name="method"/>.</param>
        /// <param name="arg7">The seventh parameter of the delegate <paramref name="method"/>.</param>
        /// <param name="arg8">The eighth parameter of the delegate <paramref name="method"/>.</param>
        /// <param name="arg9">The ninth parameter of the delegate <paramref name="method"/>.</param>
        /// <param name="arg10">The tenth parameter of the delegate <paramref name="method"/>.</param>
        /// <param name="arg11">The eleventh parameter of the delegate <paramref name="method"/>.</param>
        /// <param name="arg12">The twelfth parameter of the delegate <paramref name="method"/>.</param>
        /// <param name="arg13">The thirteenth parameter of the delegate <paramref name="method"/>.</param>
        /// <param name="arg14">The fourteenth parameter of the delegate <paramref name="method"/>.</param>
        /// <param name="arg15">The fifteenth parameter of the delegate <paramref name="method"/>.</param>
        /// <param name="arg16">The sixteenth parameter of the delegate <paramref name="method"/>.</param>
        /// <remarks>The instrumentation of <paramref name="method"/> is thread safe and the instrumented result is passed to <see cref="TimeMeasureCompleted"/>.</remarks>
        public static void ExecuteAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(MethodBase caller, Act<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, T16 arg16)
        {
            Instrumentation.ExecuteAction(caller, method, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15, arg16);
        }

        /// <summary>
        /// Instrument and executes the specified delegate <paramref name="method"/>.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter of the delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of the delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T5">The type of the fifth parameter of the delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T6">The type of the sixth parameter of the delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T7">The type of the seventh parameter of the delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T8">The type of the eighth parameter of the delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T9">The type of the ninth parameter of the delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T10">The type of the tenth parameter of the delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T11">The type of the eleventh parameter of the delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T12">The type of the twelfth parameter of the delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T13">The type of the thirteenth parameter of the delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T14">The type of the fourteenth parameter of the delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T15">The type of the fifteenth parameter of the delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T16">The type of the sixteenth parameter of the delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T17">The type of the seventeenth parameter of the delegate <paramref name="method"/>.</typeparam>
        /// <param name="caller">A <see cref="MethodBase"/> object representing the executing caller method.</param>
        /// <param name="method">The delegate to instrument and invoke.</param>
        /// <param name="arg1">The first parameter of the delegate <paramref name="method"/>.</param>
        /// <param name="arg2">The second parameter of the delegate <paramref name="method"/>.</param>
        /// <param name="arg3">The third parameter of the delegate <paramref name="method"/>.</param>
        /// <param name="arg4">The fourth parameter of the delegate <paramref name="method"/>.</param>
        /// <param name="arg5">The fifth parameter of the delegate <paramref name="method"/>.</param>
        /// <param name="arg6">The sixth parameter of the delegate <paramref name="method"/>.</param>
        /// <param name="arg7">The seventh parameter of the delegate <paramref name="method"/>.</param>
        /// <param name="arg8">The eighth parameter of the delegate <paramref name="method"/>.</param>
        /// <param name="arg9">The ninth parameter of the delegate <paramref name="method"/>.</param>
        /// <param name="arg10">The tenth parameter of the delegate <paramref name="method"/>.</param>
        /// <param name="arg11">The eleventh parameter of the delegate <paramref name="method"/>.</param>
        /// <param name="arg12">The twelfth parameter of the delegate <paramref name="method"/>.</param>
        /// <param name="arg13">The thirteenth parameter of the delegate <paramref name="method"/>.</param>
        /// <param name="arg14">The fourteenth parameter of the delegate <paramref name="method"/>.</param>
        /// <param name="arg15">The fifteenth parameter of the delegate <paramref name="method"/>.</param>
        /// <param name="arg16">The sixteenth parameter of the delegate <paramref name="method"/>.</param>
        /// <param name="arg17">The seventeenth parameter of the delegate <paramref name="method"/>.</param>
        /// <remarks>The instrumentation of <paramref name="method"/> is thread safe and the instrumented result is passed to <see cref="TimeMeasureCompleted"/>.</remarks>
        public static void ExecuteAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17>(MethodBase caller, Act<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, T16 arg16, T17 arg17)
        {
            Instrumentation.ExecuteAction(caller, method, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15, arg16, arg17);
        }

        /// <summary>
        /// Instrument and executes the specified delegate <paramref name="method"/>.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter of the delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of the delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T5">The type of the fifth parameter of the delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T6">The type of the sixth parameter of the delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T7">The type of the seventh parameter of the delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T8">The type of the eighth parameter of the delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T9">The type of the ninth parameter of the delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T10">The type of the tenth parameter of the delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T11">The type of the eleventh parameter of the delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T12">The type of the twelfth parameter of the delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T13">The type of the thirteenth parameter of the delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T14">The type of the fourteenth parameter of the delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T15">The type of the fifteenth parameter of the delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T16">The type of the sixteenth parameter of the delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T17">The type of the seventeenth parameter of the delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T18">The type of the eighteenth parameter of the delegate <paramref name="method"/>.</typeparam>
        /// <param name="caller">A <see cref="MethodBase"/> object representing the executing caller method.</param>
        /// <param name="method">The delegate to instrument and invoke.</param>
        /// <param name="arg1">The first parameter of the delegate <paramref name="method"/>.</param>
        /// <param name="arg2">The second parameter of the delegate <paramref name="method"/>.</param>
        /// <param name="arg3">The third parameter of the delegate <paramref name="method"/>.</param>
        /// <param name="arg4">The fourth parameter of the delegate <paramref name="method"/>.</param>
        /// <param name="arg5">The fifth parameter of the delegate <paramref name="method"/>.</param>
        /// <param name="arg6">The sixth parameter of the delegate <paramref name="method"/>.</param>
        /// <param name="arg7">The seventh parameter of the delegate <paramref name="method"/>.</param>
        /// <param name="arg8">The eighth parameter of the delegate <paramref name="method"/>.</param>
        /// <param name="arg9">The ninth parameter of the delegate <paramref name="method"/>.</param>
        /// <param name="arg10">The tenth parameter of the delegate <paramref name="method"/>.</param>
        /// <param name="arg11">The eleventh parameter of the delegate <paramref name="method"/>.</param>
        /// <param name="arg12">The twelfth parameter of the delegate <paramref name="method"/>.</param>
        /// <param name="arg13">The thirteenth parameter of the delegate <paramref name="method"/>.</param>
        /// <param name="arg14">The fourteenth parameter of the delegate <paramref name="method"/>.</param>
        /// <param name="arg15">The fifteenth parameter of the delegate <paramref name="method"/>.</param>
        /// <param name="arg16">The sixteenth parameter of the delegate <paramref name="method"/>.</param>
        /// <param name="arg17">The seventeenth parameter of the delegate <paramref name="method"/>.</param>
        /// <param name="arg18">The eighteenth parameter of the delegate <paramref name="method"/>.</param>
        /// <remarks>The instrumentation of <paramref name="method"/> is thread safe and the instrumented result is passed to <see cref="TimeMeasureCompleted"/>.</remarks>
        public static void ExecuteAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18>(MethodBase caller, Act<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, T16 arg16, T17 arg17, T18 arg18)
        {
            Instrumentation.ExecuteAction(caller, method, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15, arg16, arg17, arg18);
        }

        /// <summary>
        /// Instrument and executes the specified delegate <paramref name="method"/>.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter of the delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of the delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T5">The type of the fifth parameter of the delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T6">The type of the sixth parameter of the delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T7">The type of the seventh parameter of the delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T8">The type of the eighth parameter of the delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T9">The type of the ninth parameter of the delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T10">The type of the tenth parameter of the delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T11">The type of the eleventh parameter of the delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T12">The type of the twelfth parameter of the delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T13">The type of the thirteenth parameter of the delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T14">The type of the fourteenth parameter of the delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T15">The type of the fifteenth parameter of the delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T16">The type of the sixteenth parameter of the delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T17">The type of the seventeenth parameter of the delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T18">The type of the eighteenth parameter of the delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T19">The type of the nineteenth parameter of the delegate <paramref name="method"/>.</typeparam>
        /// <param name="caller">A <see cref="MethodBase"/> object representing the executing caller method.</param>
        /// <param name="method">The delegate to instrument and invoke.</param>
        /// <param name="arg1">The first parameter of the delegate <paramref name="method"/>.</param>
        /// <param name="arg2">The second parameter of the delegate <paramref name="method"/>.</param>
        /// <param name="arg3">The third parameter of the delegate <paramref name="method"/>.</param>
        /// <param name="arg4">The fourth parameter of the delegate <paramref name="method"/>.</param>
        /// <param name="arg5">The fifth parameter of the delegate <paramref name="method"/>.</param>
        /// <param name="arg6">The sixth parameter of the delegate <paramref name="method"/>.</param>
        /// <param name="arg7">The seventh parameter of the delegate <paramref name="method"/>.</param>
        /// <param name="arg8">The eighth parameter of the delegate <paramref name="method"/>.</param>
        /// <param name="arg9">The ninth parameter of the delegate <paramref name="method"/>.</param>
        /// <param name="arg10">The tenth parameter of the delegate <paramref name="method"/>.</param>
        /// <param name="arg11">The eleventh parameter of the delegate <paramref name="method"/>.</param>
        /// <param name="arg12">The twelfth parameter of the delegate <paramref name="method"/>.</param>
        /// <param name="arg13">The thirteenth parameter of the delegate <paramref name="method"/>.</param>
        /// <param name="arg14">The fourteenth parameter of the delegate <paramref name="method"/>.</param>
        /// <param name="arg15">The fifteenth parameter of the delegate <paramref name="method"/>.</param>
        /// <param name="arg16">The sixteenth parameter of the delegate <paramref name="method"/>.</param>
        /// <param name="arg17">The seventeenth parameter of the delegate <paramref name="method"/>.</param>
        /// <param name="arg18">The eighteenth parameter of the delegate <paramref name="method"/>.</param>
        /// <param name="arg19">The nineteenth parameter of the delegate <paramref name="method"/>.</param>
        /// <remarks>The instrumentation of <paramref name="method"/> is thread safe and the instrumented result is passed to <see cref="TimeMeasureCompleted"/>.</remarks>
        public static void ExecuteAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19>(MethodBase caller, Act<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, T16 arg16, T17 arg17, T18 arg18, T19 arg19)
        {
            Instrumentation.ExecuteAction(caller, method, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15, arg16, arg17, arg18, arg19);
        }

        /// <summary>
        /// Instrument and executes the specified delegate <paramref name="method"/>.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter of the delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of the delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T5">The type of the fifth parameter of the delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T6">The type of the sixth parameter of the delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T7">The type of the seventh parameter of the delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T8">The type of the eighth parameter of the delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T9">The type of the ninth parameter of the delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T10">The type of the tenth parameter of the delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T11">The type of the eleventh parameter of the delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T12">The type of the twelfth parameter of the delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T13">The type of the thirteenth parameter of the delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T14">The type of the fourteenth parameter of the delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T15">The type of the fifteenth parameter of the delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T16">The type of the sixteenth parameter of the delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T17">The type of the seventeenth parameter of the delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T18">The type of the eighteenth parameter of the delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T19">The type of the nineteenth parameter of the delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T20">The type of the twentieth parameter of the delegate <paramref name="method"/>.</typeparam>
        /// <param name="caller">A <see cref="MethodBase"/> object representing the executing caller method.</param>
        /// <param name="method">The delegate to instrument and invoke.</param>
        /// <param name="arg1">The first parameter of the delegate <paramref name="method"/>.</param>
        /// <param name="arg2">The second parameter of the delegate <paramref name="method"/>.</param>
        /// <param name="arg3">The third parameter of the delegate <paramref name="method"/>.</param>
        /// <param name="arg4">The fourth parameter of the delegate <paramref name="method"/>.</param>
        /// <param name="arg5">The fifth parameter of the delegate <paramref name="method"/>.</param>
        /// <param name="arg6">The sixth parameter of the delegate <paramref name="method"/>.</param>
        /// <param name="arg7">The seventh parameter of the delegate <paramref name="method"/>.</param>
        /// <param name="arg8">The eighth parameter of the delegate <paramref name="method"/>.</param>
        /// <param name="arg9">The ninth parameter of the delegate <paramref name="method"/>.</param>
        /// <param name="arg10">The tenth parameter of the delegate <paramref name="method"/>.</param>
        /// <param name="arg11">The eleventh parameter of the delegate <paramref name="method"/>.</param>
        /// <param name="arg12">The twelfth parameter of the delegate <paramref name="method"/>.</param>
        /// <param name="arg13">The thirteenth parameter of the delegate <paramref name="method"/>.</param>
        /// <param name="arg14">The fourteenth parameter of the delegate <paramref name="method"/>.</param>
        /// <param name="arg15">The fifteenth parameter of the delegate <paramref name="method"/>.</param>
        /// <param name="arg16">The sixteenth parameter of the delegate <paramref name="method"/>.</param>
        /// <param name="arg17">The seventeenth parameter of the delegate <paramref name="method"/>.</param>
        /// <param name="arg18">The eighteenth parameter of the delegate <paramref name="method"/>.</param>
        /// <param name="arg19">The nineteenth parameter of the delegate <paramref name="method"/>.</param>
        /// <param name="arg20">The twentieth parameter of the delegate <paramref name="method"/>.</param>
        /// <remarks>The instrumentation of <paramref name="method"/> is thread safe and the instrumented result is passed to <see cref="TimeMeasureCompleted"/>.</remarks>
        public static void ExecuteAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20>(MethodBase caller, Act<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, T16 arg16, T17 arg17, T18 arg18, T19 arg19, T20 arg20)
        {
            Instrumentation.ExecuteAction(caller, method, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15, arg16, arg17, arg18, arg19, arg20);
        }

        /// <summary>
        /// Instrument and executes the specified function delegate <paramref name="method"/>.
        /// </summary>
        /// <typeparam name="TResult">The type of the return value of the function delegate <paramref name="method"/>.</typeparam>
        /// <param name="caller">A <see cref="MethodBase"/> object representing the executing caller method.</param>
        /// <param name="method">The function delegate to instrument and invoke.</param>
        /// <returns>The result of the function delegate <paramref name="method"/>.</returns>
        /// <remarks>The instrumentation of <paramref name="method"/> is thread safe and the instrumented result is passed to <see cref="TimeMeasureCompleted"/>.</remarks>
        public static TResult ExecuteFunction<TResult>(MethodBase caller, Doer<TResult> method)
        {
            return Instrumentation.ExecuteFunction(caller, method);
        }

        /// <summary>
        /// Instrument and executes the specified function delegate <paramref name="method"/>.
        /// </summary>
        /// <typeparam name="T">The type of the parameter of the function delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="TResult">The type of the return value of the function delegate <paramref name="method"/>.</typeparam>
        /// <param name="caller">A <see cref="MethodBase"/> object representing the executing caller method.</param>
        /// <param name="method">The function delegate to instrument and invoke.</param>
        /// <param name="arg">The parameter of the function delegate <paramref name="method"/>.</param>
        /// <returns>The result of the function delegate <paramref name="method"/>.</returns>
        /// <remarks>The instrumentation of <paramref name="method"/> is thread safe and the instrumented result is passed to <see cref="TimeMeasureCompleted"/>.</remarks>
        public static TResult ExecuteFunction<T, TResult>(MethodBase caller, Doer<T, TResult> method, T arg)
        {
            return Instrumentation.ExecuteFunction(caller, method, arg);
        }

        /// <summary>
        /// Instrument and executes the specified function delegate <paramref name="method"/>.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter of the function delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the function delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="TResult">The type of the return value of the function delegate <paramref name="method"/>.</typeparam>
        /// <param name="caller">A <see cref="MethodBase"/> object representing the executing caller method.</param>
        /// <param name="method">The function delegate to instrument and invoke.</param>
        /// <param name="arg1">The first parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg2">The second parameter of the function delegate <paramref name="method"/>.</param>
        /// <returns>The result of the function delegate <paramref name="method"/>.</returns>
        /// <remarks>The instrumentation of <paramref name="method"/> is thread safe and the instrumented result is passed to <see cref="TimeMeasureCompleted"/>.</remarks>
        public static TResult ExecuteFunction<T1, T2, TResult>(MethodBase caller, Doer<T1, T2, TResult> method, T1 arg1, T2 arg2)
        {
            return Instrumentation.ExecuteFunction(caller, method, arg1, arg2);
        }

        /// <summary>
        /// Instrument and executes the specified function delegate <paramref name="method"/>.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter of the function delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the function delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the function delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="TResult">The type of the return value of the function delegate <paramref name="method"/>.</typeparam>
        /// <param name="caller">A <see cref="MethodBase"/> object representing the executing caller method.</param>
        /// <param name="method">The function delegate to instrument and invoke.</param>
        /// <param name="arg1">The first parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg2">The second parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg3">The third parameter of the function delegate <paramref name="method"/>.</param>
        /// <returns>The result of the function delegate <paramref name="method"/>.</returns>
        /// <remarks>The instrumentation of <paramref name="method"/> is thread safe and the instrumented result is passed to <see cref="TimeMeasureCompleted"/>.</remarks>
        public static TResult ExecuteFunction<T1, T2, T3, TResult>(MethodBase caller, Doer<T1, T2, T3, TResult> method, T1 arg1, T2 arg2, T3 arg3)
        {
            return Instrumentation.ExecuteFunction(caller, method, arg1, arg2, arg3);
        }

        /// <summary>
        /// Instrument and executes the specified function delegate <paramref name="method"/>.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter of the function delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the function delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the function delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of the function delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="TResult">The type of the return value of the function delegate <paramref name="method"/>.</typeparam>
        /// <param name="caller">A <see cref="MethodBase"/> object representing the executing caller method.</param>
        /// <param name="method">The function delegate to instrument and invoke.</param>
        /// <param name="arg1">The first parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg2">The second parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg3">The third parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg4">The fourth parameter of the function delegate <paramref name="method"/>.</param>
        /// <returns>The result of the function delegate <paramref name="method"/>.</returns>
        /// <remarks>The instrumentation of <paramref name="method"/> is thread safe and the instrumented result is passed to <see cref="TimeMeasureCompleted"/>.</remarks>
        public static TResult ExecuteFunction<T1, T2, T3, T4, TResult>(MethodBase caller, Doer<T1, T2, T3, T4, TResult> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            return Instrumentation.ExecuteFunction(caller, method, arg1, arg2, arg3, arg4);
        }

        /// <summary>
        /// Instrument and executes the specified function delegate <paramref name="method"/>.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter of the function delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the function delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the function delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of the function delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T5">The type of the fifth parameter of the function delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="TResult">The type of the return value of the function delegate <paramref name="method"/>.</typeparam>
        /// <param name="caller">A <see cref="MethodBase"/> object representing the executing caller method.</param>
        /// <param name="method">The function delegate to instrument and invoke.</param>
        /// <param name="arg1">The first parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg2">The second parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg3">The third parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg4">The fourth parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg5">The fifth parameter of the function delegate <paramref name="method"/>.</param>
        /// <returns>The result of the function delegate <paramref name="method"/>.</returns>
        /// <remarks>The instrumentation of <paramref name="method"/> is thread safe and the instrumented result is passed to <see cref="TimeMeasureCompleted"/>.</remarks>
        public static TResult ExecuteFunction<T1, T2, T3, T4, T5, TResult>(MethodBase caller, Doer<T1, T2, T3, T4, T5, TResult> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        {
            return Instrumentation.ExecuteFunction(caller, method, arg1, arg2, arg3, arg4, arg5);
        }

        /// <summary>
        /// Instrument and executes the specified function delegate <paramref name="method"/>.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter of the function delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the function delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the function delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of the function delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T5">The type of the fifth parameter of the function delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T6">The type of the sixth parameter of the function delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="TResult">The type of the return value of the function delegate <paramref name="method"/>.</typeparam>
        /// <param name="caller">A <see cref="MethodBase"/> object representing the executing caller method.</param>
        /// <param name="method">The function delegate to instrument and invoke.</param>
        /// <param name="arg1">The first parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg2">The second parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg3">The third parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg4">The fourth parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg5">The fifth parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg6">The sixth parameter of the function delegate <paramref name="method"/>.</param>
        /// <returns>The result of the function delegate <paramref name="method"/>.</returns>
        /// <remarks>The instrumentation of <paramref name="method"/> is thread safe and the instrumented result is passed to <see cref="TimeMeasureCompleted"/>.</remarks>
        public static TResult ExecuteFunction<T1, T2, T3, T4, T5, T6, TResult>(MethodBase caller, Doer<T1, T2, T3, T4, T5, T6, TResult> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6)
        {
            return Instrumentation.ExecuteFunction(caller, method, arg1, arg2, arg3, arg4, arg5, arg6);
        }

        /// <summary>
        /// Instrument and executes the specified function delegate <paramref name="method"/>.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter of the function delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the function delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the function delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of the function delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T5">The type of the fifth parameter of the function delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T6">The type of the sixth parameter of the function delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T7">The type of the seventh parameter of the function delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="TResult">The type of the return value of the function delegate <paramref name="method"/>.</typeparam>
        /// <param name="caller">A <see cref="MethodBase"/> object representing the executing caller method.</param>
        /// <param name="method">The function delegate to instrument and invoke.</param>
        /// <param name="arg1">The first parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg2">The second parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg3">The third parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg4">The fourth parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg5">The fifth parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg6">The sixth parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg7">The seventh parameter of the function delegate <paramref name="method"/>.</param>
        /// <returns>The result of the function delegate <paramref name="method"/>.</returns>
        /// <remarks>The instrumentation of <paramref name="method"/> is thread safe and the instrumented result is passed to <see cref="TimeMeasureCompleted"/>.</remarks>
        public static TResult ExecuteFunction<T1, T2, T3, T4, T5, T6, T7, TResult>(MethodBase caller, Doer<T1, T2, T3, T4, T5, T6, T7, TResult> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7)
        {
            return Instrumentation.ExecuteFunction(caller, method, arg1, arg2, arg3, arg4, arg5, arg6, arg7);
        }

        /// <summary>
        /// Instrument and executes the specified function delegate <paramref name="method"/>.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter of the function delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the function delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the function delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of the function delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T5">The type of the fifth parameter of the function delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T6">The type of the sixth parameter of the function delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T7">The type of the seventh parameter of the function delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T8">The type of the eighth parameter of the function delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="TResult">The type of the return value of the function delegate <paramref name="method"/>.</typeparam>
        /// <param name="caller">A <see cref="MethodBase"/> object representing the executing caller method.</param>
        /// <param name="method">The function delegate to instrument and invoke.</param>
        /// <param name="arg1">The first parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg2">The second parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg3">The third parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg4">The fourth parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg5">The fifth parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg6">The sixth parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg7">The seventh parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg8">The eighth parameter of the function delegate <paramref name="method"/>.</param>
        /// <returns>The result of the function delegate <paramref name="method"/>.</returns>
        /// <remarks>The instrumentation of <paramref name="method"/> is thread safe and the instrumented result is passed to <see cref="TimeMeasureCompleted"/>.</remarks>
        public static TResult ExecuteFunction<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(MethodBase caller, Doer<T1, T2, T3, T4, T5, T6, T7, T8, TResult> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8)
        {
            return Instrumentation.ExecuteFunction(caller, method, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8);
        }

        /// <summary>
        /// Instrument and executes the specified function delegate <paramref name="method"/>.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter of the function delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the function delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the function delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of the function delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T5">The type of the fifth parameter of the function delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T6">The type of the sixth parameter of the function delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T7">The type of the seventh parameter of the function delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T8">The type of the eighth parameter of the function delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T9">The type of the ninth parameter of the function delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="TResult">The type of the return value of the function delegate <paramref name="method"/>.</typeparam>
        /// <param name="caller">A <see cref="MethodBase"/> object representing the executing caller method.</param>
        /// <param name="method">The function delegate to instrument and invoke.</param>
        /// <param name="arg1">The first parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg2">The second parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg3">The third parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg4">The fourth parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg5">The fifth parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg6">The sixth parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg7">The seventh parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg8">The eighth parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg9">The ninth parameter of the function delegate <paramref name="method"/>.</param>
        /// <returns>The result of the function delegate <paramref name="method"/>.</returns>
        /// <remarks>The instrumentation of <paramref name="method"/> is thread safe and the instrumented result is passed to <see cref="TimeMeasureCompleted"/>.</remarks>
        public static TResult ExecuteFunction<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>(MethodBase caller, Doer<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9)
        {
            return Instrumentation.ExecuteFunction(caller, method, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9);
        }

        /// <summary>
        /// Instrument and executes the specified function delegate <paramref name="method"/>.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter of the function delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the function delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the function delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of the function delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T5">The type of the fifth parameter of the function delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T6">The type of the sixth parameter of the function delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T7">The type of the seventh parameter of the function delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T8">The type of the eighth parameter of the function delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T9">The type of the ninth parameter of the function delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T10">The type of the tenth parameter of the function delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="TResult">The type of the return value of the function delegate <paramref name="method"/>.</typeparam>
        /// <param name="caller">A <see cref="MethodBase"/> object representing the executing caller method.</param>
        /// <param name="method">The function delegate to instrument and invoke.</param>
        /// <param name="arg1">The first parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg2">The second parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg3">The third parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg4">The fourth parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg5">The fifth parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg6">The sixth parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg7">The seventh parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg8">The eighth parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg9">The ninth parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg10">The tenth parameter of the function delegate <paramref name="method"/>.</param>
        /// <returns>The result of the function delegate <paramref name="method"/>.</returns>
        /// <remarks>The instrumentation of <paramref name="method"/> is thread safe and the instrumented result is passed to <see cref="TimeMeasureCompleted"/>.</remarks>
        public static TResult ExecuteFunction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>(MethodBase caller, Doer<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10)
        {
            return Instrumentation.ExecuteFunction(caller, method, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10);
        }

        /// <summary>
        /// Instrument and executes the specified function delegate <paramref name="method"/>.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter of the function delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the function delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the function delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of the function delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T5">The type of the fifth parameter of the function delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T6">The type of the sixth parameter of the function delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T7">The type of the seventh parameter of the function delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T8">The type of the eighth parameter of the function delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T9">The type of the ninth parameter of the function delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T10">The type of the tenth parameter of the function delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T11">The type of the eleventh parameter of the function delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="TResult">The type of the return value of the function delegate <paramref name="method"/>.</typeparam>
        /// <param name="caller">A <see cref="MethodBase"/> object representing the executing caller method.</param>
        /// <param name="method">The function delegate to instrument and invoke.</param>
        /// <param name="arg1">The first parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg2">The second parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg3">The third parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg4">The fourth parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg5">The fifth parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg6">The sixth parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg7">The seventh parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg8">The eighth parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg9">The ninth parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg10">The tenth parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg11">The eleventh parameter of the function delegate <paramref name="method"/>.</param>
        /// <returns>The result of the function delegate <paramref name="method"/>.</returns>
        /// <remarks>The instrumentation of <paramref name="method"/> is thread safe and the instrumented result is passed to <see cref="TimeMeasureCompleted"/>.</remarks>
        public static TResult ExecuteFunction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult>(MethodBase caller, Doer<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11)
        {
            return Instrumentation.ExecuteFunction(caller, method, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11);
        }

        /// <summary>
        /// Instrument and executes the specified function delegate <paramref name="method"/>.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter of the function delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the function delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the function delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of the function delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T5">The type of the fifth parameter of the function delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T6">The type of the sixth parameter of the function delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T7">The type of the seventh parameter of the function delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T8">The type of the eighth parameter of the function delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T9">The type of the ninth parameter of the function delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T10">The type of the tenth parameter of the function delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T11">The type of the eleventh parameter of the function delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T12">The type of the twelfth parameter of the function delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="TResult">The type of the return value of the function delegate <paramref name="method"/>.</typeparam>
        /// <param name="caller">A <see cref="MethodBase"/> object representing the executing caller method.</param>
        /// <param name="method">The function delegate to instrument and invoke.</param>
        /// <param name="arg1">The first parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg2">The second parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg3">The third parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg4">The fourth parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg5">The fifth parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg6">The sixth parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg7">The seventh parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg8">The eighth parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg9">The ninth parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg10">The tenth parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg11">The eleventh parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg12">The twelfth parameter of the function delegate <paramref name="method"/>.</param>
        /// <returns>The result of the function delegate <paramref name="method"/>.</returns>
        /// <remarks>The instrumentation of <paramref name="method"/> is thread safe and the instrumented result is passed to <see cref="TimeMeasureCompleted"/>.</remarks>
        public static TResult ExecuteFunction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult>(MethodBase caller, Doer<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12)
        {
            return Instrumentation.ExecuteFunction(caller, method, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12);
        }

        /// <summary>
        /// Instrument and executes the specified function delegate <paramref name="method"/>.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter of the function delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the function delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the function delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of the function delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T5">The type of the fifth parameter of the function delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T6">The type of the sixth parameter of the function delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T7">The type of the seventh parameter of the function delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T8">The type of the eighth parameter of the function delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T9">The type of the ninth parameter of the function delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T10">The type of the tenth parameter of the function delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T11">The type of the eleventh parameter of the function delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T12">The type of the twelfth parameter of the function delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T13">The type of the thirteenth parameter of the function delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="TResult">The type of the return value of the function delegate <paramref name="method"/>.</typeparam>
        /// <param name="caller">A <see cref="MethodBase"/> object representing the executing caller method.</param>
        /// <param name="method">The function delegate to instrument and invoke.</param>
        /// <param name="arg1">The first parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg2">The second parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg3">The third parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg4">The fourth parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg5">The fifth parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg6">The sixth parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg7">The seventh parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg8">The eighth parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg9">The ninth parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg10">The tenth parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg11">The eleventh parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg12">The twelfth parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg13">The thirteenth parameter of the function delegate <paramref name="method"/>.</param>
        /// <returns>The result of the function delegate <paramref name="method"/>.</returns>
        /// <remarks>The instrumentation of <paramref name="method"/> is thread safe and the instrumented result is passed to <see cref="TimeMeasureCompleted"/>.</remarks>
        public static TResult ExecuteFunction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>(MethodBase caller, Doer<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13)
        {
            return Instrumentation.ExecuteFunction(caller, method, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13);
        }

        /// <summary>
        /// Instrument and executes the specified function delegate <paramref name="method"/>.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter of the function delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the function delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the function delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of the function delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T5">The type of the fifth parameter of the function delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T6">The type of the sixth parameter of the function delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T7">The type of the seventh parameter of the function delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T8">The type of the eighth parameter of the function delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T9">The type of the ninth parameter of the function delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T10">The type of the tenth parameter of the function delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T11">The type of the eleventh parameter of the function delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T12">The type of the twelfth parameter of the function delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T13">The type of the thirteenth parameter of the function delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T14">The type of the fourteenth parameter of the function delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="TResult">The type of the return value of the function delegate <paramref name="method"/>.</typeparam>
        /// <param name="caller">A <see cref="MethodBase"/> object representing the executing caller method.</param>
        /// <param name="method">The function delegate to instrument and invoke.</param>
        /// <param name="arg1">The first parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg2">The second parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg3">The third parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg4">The fourth parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg5">The fifth parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg6">The sixth parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg7">The seventh parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg8">The eighth parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg9">The ninth parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg10">The tenth parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg11">The eleventh parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg12">The twelfth parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg13">The thirteenth parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg14">The fourteenth parameter of the function delegate <paramref name="method"/>.</param>
        /// <returns>The result of the function delegate <paramref name="method"/>.</returns>
        /// <remarks>The instrumentation of <paramref name="method"/> is thread safe and the instrumented result is passed to <see cref="TimeMeasureCompleted"/>.</remarks>
        public static TResult ExecuteFunction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult>(MethodBase caller, Doer<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14)
        {
            return Instrumentation.ExecuteFunction(caller, method, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14);
        }

        /// <summary>
        /// Instrument and executes the specified function delegate <paramref name="method"/>.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter of the function delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the function delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the function delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of the function delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T5">The type of the fifth parameter of the function delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T6">The type of the sixth parameter of the function delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T7">The type of the seventh parameter of the function delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T8">The type of the eighth parameter of the function delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T9">The type of the ninth parameter of the function delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T10">The type of the tenth parameter of the function delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T11">The type of the eleventh parameter of the function delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T12">The type of the twelfth parameter of the function delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T13">The type of the thirteenth parameter of the function delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T14">The type of the fourteenth parameter of the function delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T15">The type of the fifteenth parameter of the function delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="TResult">The type of the return value of the function delegate <paramref name="method"/>.</typeparam>
        /// <param name="caller">A <see cref="MethodBase"/> object representing the executing caller method.</param>
        /// <param name="method">The function delegate to instrument and invoke.</param>
        /// <param name="arg1">The first parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg2">The second parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg3">The third parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg4">The fourth parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg5">The fifth parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg6">The sixth parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg7">The seventh parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg8">The eighth parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg9">The ninth parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg10">The tenth parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg11">The eleventh parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg12">The twelfth parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg13">The thirteenth parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg14">The fourteenth parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg15">The fifteenth parameter of the function delegate <paramref name="method"/>.</param>
        /// <returns>The result of the function delegate <paramref name="method"/>.</returns>
        /// <remarks>The instrumentation of <paramref name="method"/> is thread safe and the instrumented result is passed to <see cref="TimeMeasureCompleted"/>.</remarks>
        public static TResult ExecuteFunction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult>(MethodBase caller, Doer<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15)
        {
            return Instrumentation.ExecuteFunction(caller, method, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15);
        }

        /// <summary>
        /// Instrument and executes the specified function delegate <paramref name="method"/>.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter of the function delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the function delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the function delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of the function delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T5">The type of the fifth parameter of the function delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T6">The type of the sixth parameter of the function delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T7">The type of the seventh parameter of the function delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T8">The type of the eighth parameter of the function delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T9">The type of the ninth parameter of the function delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T10">The type of the tenth parameter of the function delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T11">The type of the eleventh parameter of the function delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T12">The type of the twelfth parameter of the function delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T13">The type of the thirteenth parameter of the function delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T14">The type of the fourteenth parameter of the function delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T15">The type of the fifteenth parameter of the function delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T16">The type of the sixteenth parameter of the function delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="TResult">The type of the return value of the function delegate <paramref name="method"/>.</typeparam>
        /// <param name="caller">A <see cref="MethodBase"/> object representing the executing caller method.</param>
        /// <param name="method">The function delegate to instrument and invoke.</param>
        /// <param name="arg1">The first parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg2">The second parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg3">The third parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg4">The fourth parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg5">The fifth parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg6">The sixth parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg7">The seventh parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg8">The eighth parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg9">The ninth parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg10">The tenth parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg11">The eleventh parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg12">The twelfth parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg13">The thirteenth parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg14">The fourteenth parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg15">The fifteenth parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg16">The sixteenth parameter of the function delegate <paramref name="method"/>.</param>
        /// <returns>The result of the function delegate <paramref name="method"/>.</returns>
        /// <remarks>The instrumentation of <paramref name="method"/> is thread safe and the instrumented result is passed to <see cref="TimeMeasureCompleted"/>.</remarks>
        public static TResult ExecuteFunction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult>(MethodBase caller, Doer<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, T16 arg16)
        {
            return Instrumentation.ExecuteFunction(caller, method, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15, arg16);
        }

        /// <summary>
        /// Instrument and executes the specified function delegate <paramref name="method"/>.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter of the function delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the function delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the function delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of the function delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T5">The type of the fifth parameter of the function delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T6">The type of the sixth parameter of the function delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T7">The type of the seventh parameter of the function delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T8">The type of the eighth parameter of the function delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T9">The type of the ninth parameter of the function delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T10">The type of the tenth parameter of the function delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T11">The type of the eleventh parameter of the function delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T12">The type of the twelfth parameter of the function delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T13">The type of the thirteenth parameter of the function delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T14">The type of the fourteenth parameter of the function delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T15">The type of the fifteenth parameter of the function delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T16">The type of the sixteenth parameter of the function delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T17">The type of the seventeenth parameter of the function delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="TResult">The type of the return value of the function delegate <paramref name="method"/>.</typeparam>
        /// <param name="caller">A <see cref="MethodBase"/> object representing the executing caller method.</param>
        /// <param name="method">The function delegate to instrument and invoke.</param>
        /// <param name="arg1">The first parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg2">The second parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg3">The third parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg4">The fourth parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg5">The fifth parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg6">The sixth parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg7">The seventh parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg8">The eighth parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg9">The ninth parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg10">The tenth parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg11">The eleventh parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg12">The twelfth parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg13">The thirteenth parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg14">The fourteenth parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg15">The fifteenth parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg16">The sixteenth parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg17">The seventeenth parameter of the function delegate <paramref name="method"/>.</param>
        /// <returns>The result of the function delegate <paramref name="method"/>.</returns>
        /// <remarks>The instrumentation of <paramref name="method"/> is thread safe and the instrumented result is passed to <see cref="TimeMeasureCompleted"/>.</remarks>
        public static TResult ExecuteFunction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, TResult>(MethodBase caller, Doer<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, TResult> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, T16 arg16, T17 arg17)
        {
            return Instrumentation.ExecuteFunction(caller, method, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15, arg16, arg17);
        }

        /// <summary>
        /// Instrument and executes the specified function delegate <paramref name="method"/>.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter of the function delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the function delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the function delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of the function delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T5">The type of the fifth parameter of the function delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T6">The type of the sixth parameter of the function delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T7">The type of the seventh parameter of the function delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T8">The type of the eighth parameter of the function delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T9">The type of the ninth parameter of the function delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T10">The type of the tenth parameter of the function delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T11">The type of the eleventh parameter of the function delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T12">The type of the twelfth parameter of the function delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T13">The type of the thirteenth parameter of the function delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T14">The type of the fourteenth parameter of the function delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T15">The type of the fifteenth parameter of the function delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T16">The type of the sixteenth parameter of the function delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T17">The type of the seventeenth parameter of the function delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T18">The type of the eighteenth parameter of the function delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="TResult">The type of the return value of the function delegate <paramref name="method"/>.</typeparam>
        /// <param name="caller">A <see cref="MethodBase"/> object representing the executing caller method.</param>
        /// <param name="method">The function delegate to instrument and invoke.</param>
        /// <param name="arg1">The first parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg2">The second parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg3">The third parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg4">The fourth parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg5">The fifth parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg6">The sixth parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg7">The seventh parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg8">The eighth parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg9">The ninth parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg10">The tenth parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg11">The eleventh parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg12">The twelfth parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg13">The thirteenth parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg14">The fourteenth parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg15">The fifteenth parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg16">The sixteenth parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg17">The seventeenth parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg18">The eighteenth parameter of the function delegate <paramref name="method"/>.</param>
        /// <returns>The result of the function delegate <paramref name="method"/>.</returns>
        /// <remarks>The instrumentation of <paramref name="method"/> is thread safe and the instrumented result is passed to <see cref="TimeMeasureCompleted"/>.</remarks>
        public static TResult ExecuteFunction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, TResult>(MethodBase caller, Doer<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, TResult> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, T16 arg16, T17 arg17, T18 arg18)
        {
            return Instrumentation.ExecuteFunction(caller, method, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15, arg16, arg17, arg18);
        }

        /// <summary>
        /// Instrument and executes the specified function delegate <paramref name="method"/>.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter of the function delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the function delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the function delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of the function delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T5">The type of the fifth parameter of the function delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T6">The type of the sixth parameter of the function delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T7">The type of the seventh parameter of the function delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T8">The type of the eighth parameter of the function delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T9">The type of the ninth parameter of the function delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T10">The type of the tenth parameter of the function delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T11">The type of the eleventh parameter of the function delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T12">The type of the twelfth parameter of the function delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T13">The type of the thirteenth parameter of the function delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T14">The type of the fourteenth parameter of the function delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T15">The type of the fifteenth parameter of the function delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T16">The type of the sixteenth parameter of the function delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T17">The type of the seventeenth parameter of the function delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T18">The type of the eighteenth parameter of the function delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T19">The type of the nineteenth parameter of the function delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="TResult">The type of the return value of the function delegate <paramref name="method"/>.</typeparam>
        /// <param name="caller">A <see cref="MethodBase"/> object representing the executing caller method.</param>
        /// <param name="method">The function delegate to instrument and invoke.</param>
        /// <param name="arg1">The first parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg2">The second parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg3">The third parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg4">The fourth parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg5">The fifth parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg6">The sixth parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg7">The seventh parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg8">The eighth parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg9">The ninth parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg10">The tenth parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg11">The eleventh parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg12">The twelfth parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg13">The thirteenth parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg14">The fourteenth parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg15">The fifteenth parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg16">The sixteenth parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg17">The seventeenth parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg18">The eighteenth parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg19">The nineteenth parameter of the function delegate <paramref name="method"/>.</param>
        /// <returns>The result of the function delegate <paramref name="method"/>.</returns>
        /// <remarks>The instrumentation of <paramref name="method"/> is thread safe and the instrumented result is passed to <see cref="TimeMeasureCompleted"/>.</remarks>
        public static TResult ExecuteFunction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, TResult>(MethodBase caller, Doer<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, TResult> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, T16 arg16, T17 arg17, T18 arg18, T19 arg19)
        {
            return Instrumentation.ExecuteFunction(caller, method, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15, arg16, arg17, arg18, arg19);
        }

        /// <summary>
        /// Instrument and executes the specified function delegate <paramref name="method"/>.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter of the function delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the function delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the function delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of the function delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T5">The type of the fifth parameter of the function delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T6">The type of the sixth parameter of the function delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T7">The type of the seventh parameter of the function delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T8">The type of the eighth parameter of the function delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T9">The type of the ninth parameter of the function delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T10">The type of the tenth parameter of the function delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T11">The type of the eleventh parameter of the function delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T12">The type of the twelfth parameter of the function delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T13">The type of the thirteenth parameter of the function delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T14">The type of the fourteenth parameter of the function delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T15">The type of the fifteenth parameter of the function delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T16">The type of the sixteenth parameter of the function delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T17">The type of the seventeenth parameter of the function delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T18">The type of the eighteenth parameter of the function delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T19">The type of the nineteenth parameter of the function delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T20">The type of the twentieth parameter of the function delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="TResult">The type of the return value of the function delegate <paramref name="method"/>.</typeparam>
        /// <param name="caller">A <see cref="MethodBase"/> object representing the executing caller method.</param>
        /// <param name="method">The function delegate to instrument and invoke.</param>
        /// <param name="arg1">The first parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg2">The second parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg3">The third parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg4">The fourth parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg5">The fifth parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg6">The sixth parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg7">The seventh parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg8">The eighth parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg9">The ninth parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg10">The tenth parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg11">The eleventh parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg12">The twelfth parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg13">The thirteenth parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg14">The fourteenth parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg15">The fifteenth parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg16">The sixteenth parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg17">The seventeenth parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg18">The eighteenth parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg19">The nineteenth parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg20">The twentieth parameter of the function delegate <paramref name="method"/>.</param>
        /// <returns>The result of the function delegate <paramref name="method"/>.</returns>
        /// <remarks>The instrumentation of <paramref name="method"/> is thread safe and the instrumented result is passed to <see cref="TimeMeasureCompleted"/>.</remarks>
        public static TResult ExecuteFunction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, TResult>(MethodBase caller, Doer<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, TResult> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, T16 arg16, T17 arg17, T18 arg18, T19 arg19, T20 arg20)
        {
            return Instrumentation.ExecuteFunction(caller, method, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15, arg16, arg17, arg18, arg19, arg20);
        }

        internal static void DefaultTimeMeasureCompletedCallback(string memberName, TimeSpan elapsed, IDictionary<string, object> data)
        {
            Validator.ThrowIfNullOrEmpty(memberName, "memberName");
            Validator.ThrowIfNull(data, "data");

            StringBuilder result = new StringBuilder(String.Format(CultureInfo.InvariantCulture, "{0} took {1}ms to execute.", memberName, Math.Round(elapsed.TotalMilliseconds)));
            if (data.Count > 0)
            {
                result.Append(" Parameters: { ");
                foreach (KeyValuePair<string, object> keyValuePair in data)
                {
                    result.AppendFormat(CultureInfo.InvariantCulture, "{0}={1}, ", keyValuePair.Key, ObjectUtility.ToString(keyValuePair.Value));
                }
                result.Remove(result.Length - 2, 2);
                result.Append(" }");
            }
#if DEBUG
            Debug.WriteLine(result.ToString());
#else
            Trace.WriteLine(result.ToString());
#endif
        }
    }
}