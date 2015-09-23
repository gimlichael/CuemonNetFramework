using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;
using Cuemon.Diagnostics;
using Cuemon.Reflection;
using Cuemon.Web;

namespace Cuemon.ServiceModel
{
    public abstract partial class Endpoint
    {
        /// <summary>
        /// Executes the specified delegate <paramref name="method"/>.
        /// </summary>
        /// <param name="caller">A <see cref="MethodBase"/> object representing the executing caller method.</param>
        /// <param name="method">The delegate to instrument and invoke.</param>
        /// <remarks>The instrumentation of <paramref name="method"/> is thread safe and the instrumented result is passed to <see cref="Instrumentation.TimeMeasureCompletedHandling(string,TimeSpan,IDictionary{string,object})"/>.</remarks>
        public void ExecuteAction(MethodBase caller, Act method)
        {
            var factory = ActFactory.Create(method);
            ExecuteActionCore(caller, factory);
        }

        /// <summary>
        /// Executes the specified delegate <paramref name="method"/>.
        /// </summary>
        /// <typeparam name="T">The type of the parameter of the delegate <paramref name="method"/>.</typeparam>
        /// <param name="caller">A <see cref="MethodBase"/> object representing the executing caller method.</param>
        /// <param name="method">The delegate to instrument and invoke.</param>
        /// <param name="arg">The parameter of the delegate <paramref name="method"/>.</param>
        /// <remarks>The instrumentation of <paramref name="method"/> is thread safe and the instrumented result is passed to <see cref="Instrumentation.TimeMeasureCompletedHandling(string,TimeSpan,IDictionary{string, object})"/>.</remarks>
        public void ExecuteAction<T>(MethodBase caller, Act<T> method, T arg)
        {
            var factory = ActFactory.Create(method, arg);
            ExecuteActionCore(caller, factory);
        }

        /// <summary>
        /// Executes the specified delegate <paramref name="method"/>.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter of the delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the delegate <paramref name="method"/>.</typeparam>
        /// <param name="caller">A <see cref="MethodBase"/> object representing the executing caller method.</param>
        /// <param name="method">The delegate to instrument and invoke.</param>
        /// <param name="arg1">The first parameter of the delegate <paramref name="method"/>.</param>
        /// <param name="arg2">The second parameter of the delegate <paramref name="method"/>.</param>
        /// <remarks>The instrumentation of <paramref name="method"/> is thread safe and the instrumented result is passed to <see cref="Instrumentation.TimeMeasureCompletedHandling(string,TimeSpan,IDictionary{string, object})"/>.</remarks>
        public void ExecuteAction<T1, T2>(MethodBase caller, Act<T1, T2> method, T1 arg1, T2 arg2)
        {
            var factory = ActFactory.Create(method, arg1, arg2);
            ExecuteActionCore(caller, factory);
        }

        /// <summary>
        /// Executes the specified delegate <paramref name="method"/>.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter of the delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the delegate <paramref name="method"/>.</typeparam>
        /// <param name="caller">A <see cref="MethodBase"/> object representing the executing caller method.</param>
        /// <param name="method">The delegate to instrument and invoke.</param>
        /// <param name="arg1">The first parameter of the delegate <paramref name="method"/>.</param>
        /// <param name="arg2">The second parameter of the delegate <paramref name="method"/>.</param>
        /// <param name="arg3">The third parameter of the delegate <paramref name="method"/>.</param>
        /// <remarks>The instrumentation of <paramref name="method"/> is thread safe and the instrumented result is passed to <see cref="Instrumentation.TimeMeasureCompletedHandling(string,TimeSpan,IDictionary{string, object})"/>.</remarks>
        public void ExecuteAction<T1, T2, T3>(MethodBase caller, Act<T1, T2, T3> method, T1 arg1, T2 arg2, T3 arg3)
        {
            var factory = ActFactory.Create(method, arg1, arg2, arg3);
            ExecuteActionCore(caller, factory);
        }

        /// <summary>
        /// Executes the specified delegate <paramref name="method"/>.
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
        /// <remarks>The instrumentation of <paramref name="method"/> is thread safe and the instrumented result is passed to <see cref="Instrumentation.TimeMeasureCompletedHandling(string,TimeSpan,IDictionary{string, object})"/>.</remarks>
        public void ExecuteAction<T1, T2, T3, T4>(MethodBase caller, Act<T1, T2, T3, T4> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            var factory = ActFactory.Create(method, arg1, arg2, arg3, arg4);
            ExecuteActionCore(caller, factory);
        }

        /// <summary>
        /// Executes the specified delegate <paramref name="method"/>.
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
        /// <remarks>The instrumentation of <paramref name="method"/> is thread safe and the instrumented result is passed to <see cref="Instrumentation.TimeMeasureCompletedHandling(string,TimeSpan,IDictionary{string, object})"/>.</remarks>
        public void ExecuteAction<T1, T2, T3, T4, T5>(MethodBase caller, Act<T1, T2, T3, T4, T5> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        {
            var factory = ActFactory.Create(method, arg1, arg2, arg3, arg4, arg5);
            ExecuteActionCore(caller, factory);
        }

        /// <summary>
        /// Executes the specified delegate <paramref name="method"/>.
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
        /// <remarks>The instrumentation of <paramref name="method"/> is thread safe and the instrumented result is passed to <see cref="Instrumentation.TimeMeasureCompletedHandling(string,TimeSpan,IDictionary{string, object})"/>.</remarks>
        public void ExecuteAction<T1, T2, T3, T4, T5, T6>(MethodBase caller, Act<T1, T2, T3, T4, T5, T6> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6)
        {
            var factory = ActFactory.Create(method, arg1, arg2, arg3, arg4, arg5, arg6);
            ExecuteActionCore(caller, factory);
        }

        /// <summary>
        /// Executes the specified delegate <paramref name="method"/>.
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
        /// <remarks>The instrumentation of <paramref name="method"/> is thread safe and the instrumented result is passed to <see cref="Instrumentation.TimeMeasureCompletedHandling(string,TimeSpan,IDictionary{string, object})"/>.</remarks>
        public void ExecuteAction<T1, T2, T3, T4, T5, T6, T7>(MethodBase caller, Act<T1, T2, T3, T4, T5, T6, T7> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7)
        {
            var factory = ActFactory.Create(method, arg1, arg2, arg3, arg4, arg5, arg6, arg7);
            ExecuteActionCore(caller, factory);
        }

        /// <summary>
        /// Executes the specified delegate <paramref name="method"/>.
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
        /// <remarks>The instrumentation of <paramref name="method"/> is thread safe and the instrumented result is passed to <see cref="Instrumentation.TimeMeasureCompletedHandling(string,TimeSpan,IDictionary{string, object})"/>.</remarks>
        public void ExecuteAction<T1, T2, T3, T4, T5, T6, T7, T8>(MethodBase caller, Act<T1, T2, T3, T4, T5, T6, T7, T8> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8)
        {
            var factory = ActFactory.Create(method, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8);
            ExecuteActionCore(caller, factory);
        }

        /// <summary>
        /// Executes the specified delegate <paramref name="method"/>.
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
        /// <remarks>The instrumentation of <paramref name="method"/> is thread safe and the instrumented result is passed to <see cref="Instrumentation.TimeMeasureCompletedHandling(string,TimeSpan,IDictionary{string, object})"/>.</remarks>
        public void ExecuteAction<T1, T2, T3, T4, T5, T6, T7, T8, T9>(MethodBase caller, Act<T1, T2, T3, T4, T5, T6, T7, T8, T9> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9)
        {
            var factory = ActFactory.Create(method, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9);
            ExecuteActionCore(caller, factory);
        }

        /// <summary>
        /// Executes the specified delegate <paramref name="method"/>.
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
        /// <remarks>The instrumentation of <paramref name="method"/> is thread safe and the instrumented result is passed to <see cref="Instrumentation.TimeMeasureCompletedHandling(string,TimeSpan,IDictionary{string, object})"/>.</remarks>
        public void ExecuteAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(MethodBase caller, Act<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10)
        {
            var factory = ActFactory.Create(method, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10);
            ExecuteActionCore(caller, factory);
        }

        /// <summary>
        /// Executes the specified delegate <paramref name="method"/>.
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
        /// <remarks>The instrumentation of <paramref name="method"/> is thread safe and the instrumented result is passed to <see cref="Instrumentation.TimeMeasureCompletedHandling(string,TimeSpan,IDictionary{string, object})"/>.</remarks>
        public void ExecuteAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(MethodBase caller, Act<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11)
        {
            var factory = ActFactory.Create(method, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11);
            ExecuteActionCore(caller, factory);
        }

        /// <summary>
        /// Executes the specified delegate <paramref name="method"/>.
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
        /// <remarks>The instrumentation of <paramref name="method"/> is thread safe and the instrumented result is passed to <see cref="Instrumentation.TimeMeasureCompletedHandling(string,TimeSpan,IDictionary{string, object})"/>.</remarks>
        public void ExecuteAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(MethodBase caller, Act<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12)
        {
            var factory = ActFactory.Create(method, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12);
            ExecuteActionCore(caller, factory);
        }

        /// <summary>
        /// Executes the specified delegate <paramref name="method"/>.
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
        /// <remarks>The instrumentation of <paramref name="method"/> is thread safe and the instrumented result is passed to <see cref="Instrumentation.TimeMeasureCompletedHandling(string,TimeSpan,IDictionary{string, object})"/>.</remarks>
        public void ExecuteAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(MethodBase caller, Act<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13)
        {
            var factory = ActFactory.Create(method, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13);
            ExecuteActionCore(caller, factory);
        }

        /// <summary>
        /// Executes the specified delegate <paramref name="method"/>.
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
        /// <remarks>The instrumentation of <paramref name="method"/> is thread safe and the instrumented result is passed to <see cref="Instrumentation.TimeMeasureCompletedHandling(string,TimeSpan,IDictionary{string, object})"/>.</remarks>
        public void ExecuteAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(MethodBase caller, Act<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14)
        {
            var factory = ActFactory.Create(method, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14);
            ExecuteActionCore(caller, factory);
        }

        /// <summary>
        /// Executes the specified delegate <paramref name="method"/>.
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
        /// <remarks>The instrumentation of <paramref name="method"/> is thread safe and the instrumented result is passed to <see cref="Instrumentation.TimeMeasureCompletedHandling(string,TimeSpan,IDictionary{string, object})"/>.</remarks>
        public void ExecuteAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(MethodBase caller, Act<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15)
        {
            var factory = ActFactory.Create(method, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15);
            ExecuteActionCore(caller, factory);
        }

        /// <summary>
        /// Executes the specified delegate <paramref name="method"/>.
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
        /// <remarks>The instrumentation of <paramref name="method"/> is thread safe and the instrumented result is passed to <see cref="Instrumentation.TimeMeasureCompletedHandling(string,TimeSpan,IDictionary{string, object})"/>.</remarks>
        public void ExecuteAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(MethodBase caller, Act<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, T16 arg16)
        {
            var factory = ActFactory.Create(method, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15, arg16);
            ExecuteActionCore(caller, factory);
        }

        /// <summary>
        /// Executes the specified delegate <paramref name="method"/>.
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
        /// <remarks>The instrumentation of <paramref name="method"/> is thread safe and the instrumented result is passed to <see cref="Instrumentation.TimeMeasureCompletedHandling(string,TimeSpan,IDictionary{string, object})"/>.</remarks>
        public void ExecuteAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17>(MethodBase caller, Act<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, T16 arg16, T17 arg17)
        {
            var factory = ActFactory.Create(method, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15, arg16, arg17);
            ExecuteActionCore(caller, factory);
        }

        /// <summary>
        /// Executes the specified delegate <paramref name="method"/>.
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
        /// <remarks>The instrumentation of <paramref name="method"/> is thread safe and the instrumented result is passed to <see cref="Instrumentation.TimeMeasureCompletedHandling(string,TimeSpan,IDictionary{string, object})"/>.</remarks>
        public void ExecuteAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18>(MethodBase caller, Act<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, T16 arg16, T17 arg17, T18 arg18)
        {
            var factory = ActFactory.Create(method, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15, arg16, arg17, arg18);
            ExecuteActionCore(caller, factory);
        }

        /// <summary>
        /// Executes the specified delegate <paramref name="method"/>.
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
        /// <remarks>The instrumentation of <paramref name="method"/> is thread safe and the instrumented result is passed to <see cref="Instrumentation.TimeMeasureCompletedHandling(string,TimeSpan,IDictionary{string, object})"/>.</remarks>
        public void ExecuteAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19>(MethodBase caller, Act<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, T16 arg16, T17 arg17, T18 arg18, T19 arg19)
        {
            var factory = ActFactory.Create(method, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15, arg16, arg17, arg18, arg19);
            ExecuteActionCore(caller, factory);
        }

        /// <summary>
        /// Executes the specified delegate <paramref name="method"/>.
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
        /// <remarks>The instrumentation of <paramref name="method"/> is thread safe and the instrumented result is passed to <see cref="Instrumentation.TimeMeasureCompletedHandling(string,TimeSpan,IDictionary{string, object})"/>.</remarks>
        public void ExecuteAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20>(MethodBase caller, Act<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, T16 arg16, T17 arg17, T18 arg18, T19 arg19, T20 arg20)
        {
            var factory = ActFactory.Create(method, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15, arg16, arg17, arg18, arg19, arg20);
            ExecuteActionCore(caller, factory);
        }

        private void ExecuteActionCore<TTuple>(MethodBase caller, ActFactory<TTuple> factory) where TTuple : Template
        {
            Guid threadSafeId = Guid.NewGuid();
            try
            {
                OnMethodEntered(caller, threadSafeId);
                HttpResponseAttribute responseAttribute = ReflectionUtility.GetAttribute<HttpResponseAttribute>(caller);
                HttpStatusCode successStatusCode = responseAttribute == null ? HttpStatusCode.OK : responseAttribute.SuccessStatusCode;
                string successStatusDescription = responseAttribute == null ? null : responseAttribute.SuccessStatusDescription;
                StatusCodeHandler(successStatusCode, successStatusDescription);

                factory.ExecuteMethod();
            }
            catch (Exception ex)
            {
                ExceptionHandler(caller, ex, factory.GenericArguments.ToArray());
            }
            finally
            {
                OnMethodExited(caller, threadSafeId, factory.GenericArguments.ToArray());
            }
        }
    }
}