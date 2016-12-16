using System;
using System.Net;
using System.Reflection;
using System.Web;
using Cuemon.Integrity;
using Cuemon.Diagnostics;
using Cuemon.Reflection;
using Cuemon.Web;

namespace Cuemon.ServiceModel
{
    public abstract partial class Endpoint
    {
        /// <summary>
        /// Executes the specified function delegate <paramref name="method"/>.
        /// </summary>
        /// <typeparam name="TResult">The type of the return value of the function delegate <paramref name="method"/>.</typeparam>
        /// <param name="caller">A <see cref="MethodBase"/> object representing the executing caller method.</param>
        /// <param name="method">The function delegate to instrument and invoke.</param>
        /// <param name="setup">The <see cref="TimeMeasureOptions"/> which need to be configured.</param>
        /// <returns>The result of the function delegate <paramref name="method"/>.</returns>
        /// <remarks>The instrumentation of <paramref name="method"/> is thread safe and the instrumented result is passed to <see cref="TimeMeasure.TimeMeasureCompletedCallback"/>.</remarks>
        public TResult ExecuteFunction<TResult>(MethodBase caller, Doer<TResult> method, Act<TimeMeasureOptions> setup = null)
        {
            var factory = DoerFactory.Create(method);
            return ExecuteFunctionCore(caller, factory, null, null, setup);
        }

        /// <summary>
        /// Executes the specified function delegate <paramref name="method"/>.
        /// </summary>
        /// <typeparam name="TResult">The type of the return value of the function delegate <paramref name="method"/>.</typeparam>
        /// <param name="caller">A <see cref="MethodBase"/> object representing the executing caller method.</param>
        /// <param name="validator">A <see cref="CacheValidator"/> object that represents the content validation of the resource.</param>
        /// <param name="method">The function delegate to instrument and invoke.</param>
        /// <param name="setup">The <see cref="TimeMeasureOptions"/> which need to be configured.</param>
        /// <returns>The result of the function delegate <paramref name="method"/>.</returns>
        /// <remarks>The instrumentation of <paramref name="method"/> is thread safe and the instrumented result is passed to <see cref="TimeMeasure.TimeMeasureCompletedCallback"/>.</remarks>
        public TResult ExecuteFunction<TResult>(MethodBase caller, CacheValidator validator, Doer<TResult> method, Act<TimeMeasureOptions> setup = null)
        {
            var factory = DoerFactory.Create(method);
            return ExecuteFunctionCore(caller, factory, validator, null, setup);
        }

        /// <summary>
        /// Executes the specified function delegate <paramref name="method"/>.
        /// </summary>
        /// <typeparam name="TResult">The type of the return value of the function delegate <paramref name="method"/>.</typeparam>
        /// <param name="caller">A <see cref="MethodBase"/> object representing the executing caller method.</param>
        /// <param name="validator">A <see cref="CacheValidator"/> object that represents the content validation of the resource.</param>
        /// <param name="cacheability">Sets the <b>Cache-Control</b> header to one of the values of <see cref="HttpCacheability"/>.</param>
        /// <param name="method">The function delegate to instrument and invoke.</param>
        /// <param name="setup">The <see cref="TimeMeasureOptions"/> which need to be configured.</param>
        /// <returns>The result of the function delegate <paramref name="method"/>.</returns>
        /// <remarks>The instrumentation of <paramref name="method"/> is thread safe and the instrumented result is passed to <see cref="TimeMeasure.TimeMeasureCompletedCallback"/>.</remarks>
        public TResult ExecuteFunction<TResult>(MethodBase caller, CacheValidator validator, HttpCacheability cacheability, Doer<TResult> method, Act<TimeMeasureOptions> setup = null)
        {
            var factory = DoerFactory.Create(method);
            return ExecuteFunctionCore(caller, factory, validator, cacheability, setup);
        }

        /// <summary>
        /// Executes the specified function delegate <paramref name="method"/>.
        /// </summary>
        /// <typeparam name="T">The type of the parameter of the function delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="TResult">The type of the return value of the function delegate <paramref name="method"/>.</typeparam>
        /// <param name="caller">A <see cref="MethodBase"/> object representing the executing caller method.</param>
        /// <param name="method">The function delegate to instrument and invoke.</param>
        /// <param name="arg">The parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="setup">The <see cref="TimeMeasureOptions"/> which need to be configured.</param>
        /// <returns>The result of the function delegate <paramref name="method"/>.</returns>
        /// <remarks>The instrumentation of <paramref name="method"/> is thread safe and the instrumented result is passed to <see cref="TimeMeasure.TimeMeasureCompletedCallback"/>.</remarks>
        public TResult ExecuteFunction<T, TResult>(MethodBase caller, Doer<T, TResult> method, T arg, Act<TimeMeasureOptions> setup = null)
        {
            var factory = DoerFactory.Create(method, arg);
            return ExecuteFunctionCore(caller, factory, null, null, setup);
        }

        /// <summary>
        /// Executes the specified function delegate <paramref name="method"/>.
        /// </summary>
        /// <typeparam name="T">The type of the parameter of the function delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="TResult">The type of the return value of the function delegate <paramref name="method"/>.</typeparam>
        /// <param name="caller">A <see cref="MethodBase"/> object representing the executing caller method.</param>
        /// <param name="validator">A <see cref="CacheValidator"/> object that represents the content validation of the resource.</param>
        /// <param name="method">The function delegate to instrument and invoke.</param>
        /// <param name="arg">The parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="setup">The <see cref="TimeMeasureOptions"/> which need to be configured.</param>
        /// <returns>The result of the function delegate <paramref name="method"/>.</returns>
        /// <remarks>The instrumentation of <paramref name="method"/> is thread safe and the instrumented result is passed to <see cref="TimeMeasure.TimeMeasureCompletedCallback"/>.</remarks>
        public TResult ExecuteFunction<T, TResult>(MethodBase caller, CacheValidator validator, Doer<T, TResult> method, T arg, Act<TimeMeasureOptions> setup = null)
        {
            var factory = DoerFactory.Create(method, arg);
            return ExecuteFunctionCore(caller, factory, validator, null, setup);
        }

        /// <summary>
        /// Executes the specified function delegate <paramref name="method"/>.
        /// </summary>
        /// <typeparam name="T">The type of the parameter of the function delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="TResult">The type of the return value of the function delegate <paramref name="method"/>.</typeparam>
        /// <param name="caller">A <see cref="MethodBase"/> object representing the executing caller method.</param>
        /// <param name="validator">A <see cref="CacheValidator"/> object that represents the content validation of the resource.</param>
        /// <param name="cacheability">Sets the <b>Cache-Control</b> header to one of the values of <see cref="HttpCacheability"/>.</param>
        /// <param name="method">The function delegate to instrument and invoke.</param>
        /// <param name="arg">The parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="setup">The <see cref="TimeMeasureOptions"/> which need to be configured.</param>
        /// <returns>The result of the function delegate <paramref name="method"/>.</returns>
        /// <remarks>The instrumentation of <paramref name="method"/> is thread safe and the instrumented result is passed to <see cref="TimeMeasure.TimeMeasureCompletedCallback"/>.</remarks>
        public TResult ExecuteFunction<T, TResult>(MethodBase caller, CacheValidator validator, HttpCacheability cacheability, Doer<T, TResult> method, T arg, Act<TimeMeasureOptions> setup = null)
        {
            var factory = DoerFactory.Create(method, arg);
            return ExecuteFunctionCore(caller, factory, validator, cacheability, setup);
        }

        /// <summary>
        /// Executes the specified function delegate <paramref name="method"/>.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter of the function delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the function delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="TResult">The type of the return value of the function delegate <paramref name="method"/>.</typeparam>
        /// <param name="caller">A <see cref="MethodBase"/> object representing the executing caller method.</param>
        /// <param name="method">The function delegate to instrument and invoke.</param>
        /// <param name="arg1">The first parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg2">The second parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="setup">The <see cref="TimeMeasureOptions"/> which need to be configured.</param>
        /// <returns>The result of the function delegate <paramref name="method"/>.</returns>
        /// <remarks>The instrumentation of <paramref name="method"/> is thread safe and the instrumented result is passed to <see cref="TimeMeasure.TimeMeasureCompletedCallback"/>.</remarks>
        public TResult ExecuteFunction<T1, T2, TResult>(MethodBase caller, Doer<T1, T2, TResult> method, T1 arg1, T2 arg2, Act<TimeMeasureOptions> setup = null)
        {
            var factory = DoerFactory.Create(method, arg1, arg2);
            return ExecuteFunctionCore(caller, factory, null, null, setup);
        }

        /// <summary>
        /// Executes the specified function delegate <paramref name="method"/>.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter of the function delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the function delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="TResult">The type of the return value of the function delegate <paramref name="method"/>.</typeparam>
        /// <param name="caller">A <see cref="MethodBase"/> object representing the executing caller method.</param>
        /// <param name="validator">A <see cref="CacheValidator"/> object that represents the content validation of the resource.</param>
        /// <param name="method">The function delegate to instrument and invoke.</param>
        /// <param name="arg1">The first parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg2">The second parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="setup">The <see cref="TimeMeasureOptions"/> which need to be configured.</param>
        /// <returns>The result of the function delegate <paramref name="method"/>.</returns>
        /// <remarks>The instrumentation of <paramref name="method"/> is thread safe and the instrumented result is passed to <see cref="TimeMeasure.TimeMeasureCompletedCallback"/>.</remarks>
        public TResult ExecuteFunction<T1, T2, TResult>(MethodBase caller, CacheValidator validator, Doer<T1, T2, TResult> method, T1 arg1, T2 arg2, Act<TimeMeasureOptions> setup = null)
        {
            var factory = DoerFactory.Create(method, arg1, arg2);
            return ExecuteFunctionCore(caller, factory, validator, null, setup);
        }

        /// <summary>
        /// Executes the specified function delegate <paramref name="method"/>.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter of the function delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the function delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="TResult">The type of the return value of the function delegate <paramref name="method"/>.</typeparam>
        /// <param name="caller">A <see cref="MethodBase"/> object representing the executing caller method.</param>
        /// <param name="validator">A <see cref="CacheValidator"/> object that represents the content validation of the resource.</param>
        /// <param name="cacheability">Sets the <b>Cache-Control</b> header to one of the values of <see cref="HttpCacheability"/>.</param>
        /// <param name="method">The function delegate to instrument and invoke.</param>
        /// <param name="arg1">The first parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg2">The second parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="setup">The <see cref="TimeMeasureOptions"/> which need to be configured.</param>
        /// <returns>The result of the function delegate <paramref name="method"/>.</returns>
        /// <remarks>The instrumentation of <paramref name="method"/> is thread safe and the instrumented result is passed to <see cref="TimeMeasure.TimeMeasureCompletedCallback"/>.</remarks>
        public TResult ExecuteFunction<T1, T2, TResult>(MethodBase caller, CacheValidator validator, HttpCacheability cacheability, Doer<T1, T2, TResult> method, T1 arg1, T2 arg2, Act<TimeMeasureOptions> setup = null)
        {
            var factory = DoerFactory.Create(method, arg1, arg2);
            return ExecuteFunctionCore(caller, factory, validator, cacheability, setup);
        }

        /// <summary>
        /// Executes the specified function delegate <paramref name="method"/>.
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
        /// <param name="setup">The <see cref="TimeMeasureOptions"/> which need to be configured.</param>
        /// <returns>The result of the function delegate <paramref name="method"/>.</returns>
        /// <remarks>The instrumentation of <paramref name="method"/> is thread safe and the instrumented result is passed to <see cref="TimeMeasure.TimeMeasureCompletedCallback"/>.</remarks>
        public TResult ExecuteFunction<T1, T2, T3, TResult>(MethodBase caller, Doer<T1, T2, T3, TResult> method, T1 arg1, T2 arg2, T3 arg3, Act<TimeMeasureOptions> setup = null)
        {
            var factory = DoerFactory.Create(method, arg1, arg2, arg3);
            return ExecuteFunctionCore(caller, factory, null, null, setup);
        }

        /// <summary>
        /// Executes the specified function delegate <paramref name="method"/>.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter of the function delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the function delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the function delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="TResult">The type of the return value of the function delegate <paramref name="method"/>.</typeparam>
        /// <param name="caller">A <see cref="MethodBase"/> object representing the executing caller method.</param>
        /// <param name="validator">A <see cref="CacheValidator"/> object that represents the content validation of the resource.</param>
        /// <param name="method">The function delegate to instrument and invoke.</param>
        /// <param name="arg1">The first parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg2">The second parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg3">The third parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="setup">The <see cref="TimeMeasureOptions"/> which need to be configured.</param>
        /// <returns>The result of the function delegate <paramref name="method"/>.</returns>
        /// <remarks>The instrumentation of <paramref name="method"/> is thread safe and the instrumented result is passed to <see cref="TimeMeasure.TimeMeasureCompletedCallback"/>.</remarks>
        public TResult ExecuteFunction<T1, T2, T3, TResult>(MethodBase caller, CacheValidator validator, Doer<T1, T2, T3, TResult> method, T1 arg1, T2 arg2, T3 arg3, Act<TimeMeasureOptions> setup = null)
        {
            var factory = DoerFactory.Create(method, arg1, arg2, arg3);
            return ExecuteFunctionCore(caller, factory, validator, null, setup);
        }

        /// <summary>
        /// Executes the specified function delegate <paramref name="method"/>.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter of the function delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the function delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the function delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="TResult">The type of the return value of the function delegate <paramref name="method"/>.</typeparam>
        /// <param name="caller">A <see cref="MethodBase"/> object representing the executing caller method.</param>
        /// <param name="validator">A <see cref="CacheValidator"/> object that represents the content validation of the resource.</param>
        /// <param name="cacheability">Sets the <b>Cache-Control</b> header to one of the values of <see cref="HttpCacheability"/>.</param>
        /// <param name="method">The function delegate to instrument and invoke.</param>
        /// <param name="arg1">The first parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg2">The second parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg3">The third parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="setup">The <see cref="TimeMeasureOptions"/> which need to be configured.</param>
        /// <returns>The result of the function delegate <paramref name="method"/>.</returns>
        /// <remarks>The instrumentation of <paramref name="method"/> is thread safe and the instrumented result is passed to <see cref="TimeMeasure.TimeMeasureCompletedCallback"/>.</remarks>
        public TResult ExecuteFunction<T1, T2, T3, TResult>(MethodBase caller, CacheValidator validator, HttpCacheability cacheability, Doer<T1, T2, T3, TResult> method, T1 arg1, T2 arg2, T3 arg3, Act<TimeMeasureOptions> setup = null)
        {
            var factory = DoerFactory.Create(method, arg1, arg2, arg3);
            return ExecuteFunctionCore(caller, factory, validator, cacheability, setup);
        }

        /// <summary>
        /// Executes the specified function delegate <paramref name="method"/>.
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
        /// <param name="setup">The <see cref="TimeMeasureOptions"/> which need to be configured.</param>
        /// <returns>The result of the function delegate <paramref name="method"/>.</returns>
        /// <remarks>The instrumentation of <paramref name="method"/> is thread safe and the instrumented result is passed to <see cref="TimeMeasure.TimeMeasureCompletedCallback"/>.</remarks>
        public TResult ExecuteFunction<T1, T2, T3, T4, TResult>(MethodBase caller, Doer<T1, T2, T3, T4, TResult> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, Act<TimeMeasureOptions> setup = null)
        {
            var factory = DoerFactory.Create(method, arg1, arg2, arg3, arg4);
            return ExecuteFunctionCore(caller, factory, null, null, setup);
        }

        /// <summary>
        /// Executes the specified function delegate <paramref name="method"/>.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter of the function delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the function delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the function delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of the function delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="TResult">The type of the return value of the function delegate <paramref name="method"/>.</typeparam>
        /// <param name="caller">A <see cref="MethodBase"/> object representing the executing caller method.</param>
        /// <param name="validator">A <see cref="CacheValidator"/> object that represents the content validation of the resource.</param>
        /// <param name="method">The function delegate to instrument and invoke.</param>
        /// <param name="arg1">The first parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg2">The second parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg3">The third parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg4">The fourth parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="setup">The <see cref="TimeMeasureOptions"/> which need to be configured.</param>
        /// <returns>The result of the function delegate <paramref name="method"/>.</returns>
        /// <remarks>The instrumentation of <paramref name="method"/> is thread safe and the instrumented result is passed to <see cref="TimeMeasure.TimeMeasureCompletedCallback"/>.</remarks>
        public TResult ExecuteFunction<T1, T2, T3, T4, TResult>(MethodBase caller, CacheValidator validator, Doer<T1, T2, T3, T4, TResult> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, Act<TimeMeasureOptions> setup = null)
        {
            var factory = DoerFactory.Create(method, arg1, arg2, arg3, arg4);
            return ExecuteFunctionCore(caller, factory, validator, null, setup);
        }

        /// <summary>
        /// Executes the specified function delegate <paramref name="method"/>.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter of the function delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the function delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the function delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of the function delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="TResult">The type of the return value of the function delegate <paramref name="method"/>.</typeparam>
        /// <param name="caller">A <see cref="MethodBase"/> object representing the executing caller method.</param>
        /// <param name="validator">A <see cref="CacheValidator"/> object that represents the content validation of the resource.</param>
        /// <param name="cacheability">Sets the <b>Cache-Control</b> header to one of the values of <see cref="HttpCacheability"/>.</param>
        /// <param name="method">The function delegate to instrument and invoke.</param>
        /// <param name="arg1">The first parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg2">The second parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg3">The third parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg4">The fourth parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="setup">The <see cref="TimeMeasureOptions"/> which need to be configured.</param>
        /// <returns>The result of the function delegate <paramref name="method"/>.</returns>
        /// <remarks>The instrumentation of <paramref name="method"/> is thread safe and the instrumented result is passed to <see cref="TimeMeasure.TimeMeasureCompletedCallback"/>.</remarks>
        public TResult ExecuteFunction<T1, T2, T3, T4, TResult>(MethodBase caller, CacheValidator validator, HttpCacheability cacheability, Doer<T1, T2, T3, T4, TResult> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, Act<TimeMeasureOptions> setup = null)
        {
            var factory = DoerFactory.Create(method, arg1, arg2, arg3, arg4);
            return ExecuteFunctionCore(caller, factory, validator, cacheability, setup);
        }

        /// <summary>
        /// Executes the specified function delegate <paramref name="method"/>.
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
        /// <param name="setup">The <see cref="TimeMeasureOptions"/> which need to be configured.</param>
        /// <returns>The result of the function delegate <paramref name="method"/>.</returns>
        /// <remarks>The instrumentation of <paramref name="method"/> is thread safe and the instrumented result is passed to <see cref="TimeMeasure.TimeMeasureCompletedCallback"/>.</remarks>
        public TResult ExecuteFunction<T1, T2, T3, T4, T5, TResult>(MethodBase caller, Doer<T1, T2, T3, T4, T5, TResult> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, Act<TimeMeasureOptions> setup = null)
        {
            var factory = DoerFactory.Create(method, arg1, arg2, arg3, arg4, arg5);
            return ExecuteFunctionCore(caller, factory, null, null, setup);
        }

        /// <summary>
        /// Executes the specified function delegate <paramref name="method"/>.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter of the function delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the function delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the function delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of the function delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T5">The type of the fifth parameter of the function delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="TResult">The type of the return value of the function delegate <paramref name="method"/>.</typeparam>
        /// <param name="caller">A <see cref="MethodBase"/> object representing the executing caller method.</param>
        /// <param name="validator">A <see cref="CacheValidator"/> object that represents the content validation of the resource.</param>
        /// <param name="method">The function delegate to instrument and invoke.</param>
        /// <param name="arg1">The first parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg2">The second parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg3">The third parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg4">The fourth parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg5">The fifth parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="setup">The <see cref="TimeMeasureOptions"/> which need to be configured.</param>
        /// <returns>The result of the function delegate <paramref name="method"/>.</returns>
        /// <remarks>The instrumentation of <paramref name="method"/> is thread safe and the instrumented result is passed to <see cref="TimeMeasure.TimeMeasureCompletedCallback"/>.</remarks>
        public TResult ExecuteFunction<T1, T2, T3, T4, T5, TResult>(MethodBase caller, CacheValidator validator, Doer<T1, T2, T3, T4, T5, TResult> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, Act<TimeMeasureOptions> setup = null)
        {
            var factory = DoerFactory.Create(method, arg1, arg2, arg3, arg4, arg5);
            return ExecuteFunctionCore(caller, factory, validator, null, setup);
        }

        /// <summary>
        /// Executes the specified function delegate <paramref name="method"/>.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter of the function delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the function delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the function delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of the function delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T5">The type of the fifth parameter of the function delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="TResult">The type of the return value of the function delegate <paramref name="method"/>.</typeparam>
        /// <param name="caller">A <see cref="MethodBase"/> object representing the executing caller method.</param>
        /// <param name="validator">A <see cref="CacheValidator"/> object that represents the content validation of the resource.</param>
        /// <param name="cacheability">Sets the <b>Cache-Control</b> header to one of the values of <see cref="HttpCacheability"/>.</param>
        /// <param name="method">The function delegate to instrument and invoke.</param>
        /// <param name="arg1">The first parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg2">The second parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg3">The third parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg4">The fourth parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg5">The fifth parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="setup">The <see cref="TimeMeasureOptions"/> which need to be configured.</param>
        /// <returns>The result of the function delegate <paramref name="method"/>.</returns>
        /// <remarks>The instrumentation of <paramref name="method"/> is thread safe and the instrumented result is passed to <see cref="TimeMeasure.TimeMeasureCompletedCallback"/>.</remarks>
        public TResult ExecuteFunction<T1, T2, T3, T4, T5, TResult>(MethodBase caller, CacheValidator validator, HttpCacheability cacheability, Doer<T1, T2, T3, T4, T5, TResult> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, Act<TimeMeasureOptions> setup = null)
        {
            var factory = DoerFactory.Create(method, arg1, arg2, arg3, arg4, arg5);
            return ExecuteFunctionCore(caller, factory, validator, cacheability, setup);
        }

        /// <summary>
        /// Executes the specified function delegate <paramref name="method"/>.
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
        /// <param name="setup">The <see cref="TimeMeasureOptions"/> which need to be configured.</param>
        /// <returns>The result of the function delegate <paramref name="method"/>.</returns>
        /// <remarks>The instrumentation of <paramref name="method"/> is thread safe and the instrumented result is passed to <see cref="TimeMeasure.TimeMeasureCompletedCallback"/>.</remarks>
        public TResult ExecuteFunction<T1, T2, T3, T4, T5, T6, TResult>(MethodBase caller, Doer<T1, T2, T3, T4, T5, T6, TResult> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, Act<TimeMeasureOptions> setup = null)
        {
            var factory = DoerFactory.Create(method, arg1, arg2, arg3, arg4, arg5, arg6);
            return ExecuteFunctionCore(caller, factory, null, null, setup);
        }

        /// <summary>
        /// Executes the specified function delegate <paramref name="method"/>.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter of the function delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the function delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the function delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of the function delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T5">The type of the fifth parameter of the function delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T6">The type of the sixth parameter of the function delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="TResult">The type of the return value of the function delegate <paramref name="method"/>.</typeparam>
        /// <param name="caller">A <see cref="MethodBase"/> object representing the executing caller method.</param>
        /// <param name="validator">A <see cref="CacheValidator"/> object that represents the content validation of the resource.</param>
        /// <param name="method">The function delegate to instrument and invoke.</param>
        /// <param name="arg1">The first parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg2">The second parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg3">The third parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg4">The fourth parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg5">The fifth parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg6">The sixth parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="setup">The <see cref="TimeMeasureOptions"/> which need to be configured.</param>
        /// <returns>The result of the function delegate <paramref name="method"/>.</returns>
        /// <remarks>The instrumentation of <paramref name="method"/> is thread safe and the instrumented result is passed to <see cref="TimeMeasure.TimeMeasureCompletedCallback"/>.</remarks>
        public TResult ExecuteFunction<T1, T2, T3, T4, T5, T6, TResult>(MethodBase caller, CacheValidator validator, Doer<T1, T2, T3, T4, T5, T6, TResult> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, Act<TimeMeasureOptions> setup = null)
        {
            var factory = DoerFactory.Create(method, arg1, arg2, arg3, arg4, arg5, arg6);
            return ExecuteFunctionCore(caller, factory, validator, null, setup);
        }

        /// <summary>
        /// Executes the specified function delegate <paramref name="method"/>.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter of the function delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the function delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the function delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of the function delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T5">The type of the fifth parameter of the function delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T6">The type of the sixth parameter of the function delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="TResult">The type of the return value of the function delegate <paramref name="method"/>.</typeparam>
        /// <param name="caller">A <see cref="MethodBase"/> object representing the executing caller method.</param>
        /// <param name="validator">A <see cref="CacheValidator"/> object that represents the content validation of the resource.</param>
        /// <param name="cacheability">Sets the <b>Cache-Control</b> header to one of the values of <see cref="HttpCacheability"/>.</param>
        /// <param name="method">The function delegate to instrument and invoke.</param>
        /// <param name="arg1">The first parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg2">The second parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg3">The third parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg4">The fourth parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg5">The fifth parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg6">The sixth parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="setup">The <see cref="TimeMeasureOptions"/> which need to be configured.</param>
        /// <returns>The result of the function delegate <paramref name="method"/>.</returns>
        /// <remarks>The instrumentation of <paramref name="method"/> is thread safe and the instrumented result is passed to <see cref="TimeMeasure.TimeMeasureCompletedCallback"/>.</remarks>
        public TResult ExecuteFunction<T1, T2, T3, T4, T5, T6, TResult>(MethodBase caller, CacheValidator validator, HttpCacheability cacheability, Doer<T1, T2, T3, T4, T5, T6, TResult> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, Act<TimeMeasureOptions> setup = null)
        {
            var factory = DoerFactory.Create(method, arg1, arg2, arg3, arg4, arg5, arg6);
            return ExecuteFunctionCore(caller, factory, validator, cacheability, setup);
        }

        /// <summary>
        /// Executes the specified function delegate <paramref name="method"/>.
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
        /// <param name="setup">The <see cref="TimeMeasureOptions"/> which need to be configured.</param>
        /// <returns>The result of the function delegate <paramref name="method"/>.</returns>
        /// <remarks>The instrumentation of <paramref name="method"/> is thread safe and the instrumented result is passed to <see cref="TimeMeasure.TimeMeasureCompletedCallback"/>.</remarks>
        public TResult ExecuteFunction<T1, T2, T3, T4, T5, T6, T7, TResult>(MethodBase caller, Doer<T1, T2, T3, T4, T5, T6, T7, TResult> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, Act<TimeMeasureOptions> setup = null)
        {
            var factory = DoerFactory.Create(method, arg1, arg2, arg3, arg4, arg5, arg6, arg7);
            return ExecuteFunctionCore(caller, factory, null, null, setup);
        }

        /// <summary>
        /// Executes the specified function delegate <paramref name="method"/>.
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
        /// <param name="validator">A <see cref="CacheValidator"/> object that represents the content validation of the resource.</param>
        /// <param name="method">The function delegate to instrument and invoke.</param>
        /// <param name="arg1">The first parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg2">The second parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg3">The third parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg4">The fourth parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg5">The fifth parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg6">The sixth parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg7">The seventh parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="setup">The <see cref="TimeMeasureOptions"/> which need to be configured.</param>
        /// <returns>The result of the function delegate <paramref name="method"/>.</returns>
        /// <remarks>The instrumentation of <paramref name="method"/> is thread safe and the instrumented result is passed to <see cref="TimeMeasure.TimeMeasureCompletedCallback"/>.</remarks>
        public TResult ExecuteFunction<T1, T2, T3, T4, T5, T6, T7, TResult>(MethodBase caller, CacheValidator validator, Doer<T1, T2, T3, T4, T5, T6, T7, TResult> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, Act<TimeMeasureOptions> setup = null)
        {
            var factory = DoerFactory.Create(method, arg1, arg2, arg3, arg4, arg5, arg6, arg7);
            return ExecuteFunctionCore(caller, factory, validator, null, setup);
        }

        /// <summary>
        /// Executes the specified function delegate <paramref name="method"/>.
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
        /// <param name="validator">A <see cref="CacheValidator"/> object that represents the content validation of the resource.</param>
        /// <param name="cacheability">Sets the <b>Cache-Control</b> header to one of the values of <see cref="HttpCacheability"/>.</param>
        /// <param name="method">The function delegate to instrument and invoke.</param>
        /// <param name="arg1">The first parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg2">The second parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg3">The third parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg4">The fourth parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg5">The fifth parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg6">The sixth parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg7">The seventh parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="setup">The <see cref="TimeMeasureOptions"/> which need to be configured.</param>
        /// <returns>The result of the function delegate <paramref name="method"/>.</returns>
        /// <remarks>The instrumentation of <paramref name="method"/> is thread safe and the instrumented result is passed to <see cref="TimeMeasure.TimeMeasureCompletedCallback"/>.</remarks>
        public TResult ExecuteFunction<T1, T2, T3, T4, T5, T6, T7, TResult>(MethodBase caller, CacheValidator validator, HttpCacheability cacheability, Doer<T1, T2, T3, T4, T5, T6, T7, TResult> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, Act<TimeMeasureOptions> setup = null)
        {
            var factory = DoerFactory.Create(method, arg1, arg2, arg3, arg4, arg5, arg6, arg7);
            return ExecuteFunctionCore(caller, factory, validator, cacheability, setup);
        }

        /// <summary>
        /// Executes the specified function delegate <paramref name="method"/>.
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
        /// <param name="setup">The <see cref="TimeMeasureOptions"/> which need to be configured.</param>
        /// <returns>The result of the function delegate <paramref name="method"/>.</returns>
        /// <remarks>The instrumentation of <paramref name="method"/> is thread safe and the instrumented result is passed to <see cref="TimeMeasure.TimeMeasureCompletedCallback"/>.</remarks>
        public TResult ExecuteFunction<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(MethodBase caller, Doer<T1, T2, T3, T4, T5, T6, T7, T8, TResult> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, Act<TimeMeasureOptions> setup = null)
        {
            var factory = DoerFactory.Create(method, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8);
            return ExecuteFunctionCore(caller, factory, null, null, setup);
        }

        /// <summary>
        /// Executes the specified function delegate <paramref name="method"/>.
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
        /// <param name="validator">A <see cref="CacheValidator"/> object that represents the content validation of the resource.</param>
        /// <param name="method">The function delegate to instrument and invoke.</param>
        /// <param name="arg1">The first parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg2">The second parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg3">The third parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg4">The fourth parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg5">The fifth parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg6">The sixth parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg7">The seventh parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg8">The eighth parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="setup">The <see cref="TimeMeasureOptions"/> which need to be configured.</param>
        /// <returns>The result of the function delegate <paramref name="method"/>.</returns>
        /// <remarks>The instrumentation of <paramref name="method"/> is thread safe and the instrumented result is passed to <see cref="TimeMeasure.TimeMeasureCompletedCallback"/>.</remarks>
        public TResult ExecuteFunction<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(MethodBase caller, CacheValidator validator, Doer<T1, T2, T3, T4, T5, T6, T7, T8, TResult> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, Act<TimeMeasureOptions> setup = null)
        {
            var factory = DoerFactory.Create(method, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8);
            return ExecuteFunctionCore(caller, factory, validator, null, setup);
        }

        /// <summary>
        /// Executes the specified function delegate <paramref name="method"/>.
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
        /// <param name="validator">A <see cref="CacheValidator"/> object that represents the content validation of the resource.</param>
        /// <param name="cacheability">Sets the <b>Cache-Control</b> header to one of the values of <see cref="HttpCacheability"/>.</param>
        /// <param name="method">The function delegate to instrument and invoke.</param>
        /// <param name="arg1">The first parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg2">The second parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg3">The third parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg4">The fourth parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg5">The fifth parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg6">The sixth parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg7">The seventh parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg8">The eighth parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="setup">The <see cref="TimeMeasureOptions"/> which need to be configured.</param>
        /// <returns>The result of the function delegate <paramref name="method"/>.</returns>
        /// <remarks>The instrumentation of <paramref name="method"/> is thread safe and the instrumented result is passed to <see cref="TimeMeasure.TimeMeasureCompletedCallback"/>.</remarks>
        public TResult ExecuteFunction<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(MethodBase caller, CacheValidator validator, HttpCacheability cacheability, Doer<T1, T2, T3, T4, T5, T6, T7, T8, TResult> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, Act<TimeMeasureOptions> setup = null)
        {
            var factory = DoerFactory.Create(method, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8);
            return ExecuteFunctionCore(caller, factory, validator, cacheability, setup);
        }

        /// <summary>
        /// Executes the specified function delegate <paramref name="method"/>.
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
        /// <param name="setup">The <see cref="TimeMeasureOptions"/> which need to be configured.</param>
        /// <returns>The result of the function delegate <paramref name="method"/>.</returns>
        /// <remarks>The instrumentation of <paramref name="method"/> is thread safe and the instrumented result is passed to <see cref="TimeMeasure.TimeMeasureCompletedCallback"/>.</remarks>
        public TResult ExecuteFunction<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>(MethodBase caller, Doer<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, Act<TimeMeasureOptions> setup = null)
        {
            var factory = DoerFactory.Create(method, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9);
            return ExecuteFunctionCore(caller, factory, null, null, setup);
        }

        /// <summary>
        /// Executes the specified function delegate <paramref name="method"/>.
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
        /// <param name="validator">A <see cref="CacheValidator"/> object that represents the content validation of the resource.</param>
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
        /// <param name="setup">The <see cref="TimeMeasureOptions"/> which need to be configured.</param>
        /// <returns>The result of the function delegate <paramref name="method"/>.</returns>
        /// <remarks>The instrumentation of <paramref name="method"/> is thread safe and the instrumented result is passed to <see cref="TimeMeasure.TimeMeasureCompletedCallback"/>.</remarks>
        public TResult ExecuteFunction<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>(MethodBase caller, CacheValidator validator, Doer<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, Act<TimeMeasureOptions> setup = null)
        {
            var factory = DoerFactory.Create(method, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9);
            return ExecuteFunctionCore(caller, factory, validator, null, setup);
        }

        /// <summary>
        /// Executes the specified function delegate <paramref name="method"/>.
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
        /// <param name="validator">A <see cref="CacheValidator"/> object that represents the content validation of the resource.</param>
        /// <param name="cacheability">Sets the <b>Cache-Control</b> header to one of the values of <see cref="HttpCacheability"/>.</param>
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
        /// <param name="setup">The <see cref="TimeMeasureOptions"/> which need to be configured.</param>
        /// <returns>The result of the function delegate <paramref name="method"/>.</returns>
        /// <remarks>The instrumentation of <paramref name="method"/> is thread safe and the instrumented result is passed to <see cref="TimeMeasure.TimeMeasureCompletedCallback"/>.</remarks>
        public TResult ExecuteFunction<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>(MethodBase caller, CacheValidator validator, HttpCacheability cacheability, Doer<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, Act<TimeMeasureOptions> setup = null)
        {
            var factory = DoerFactory.Create(method, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9);
            return ExecuteFunctionCore(caller, factory, validator, cacheability, setup);
        }

        /// <summary>
        /// Executes the specified function delegate <paramref name="method"/>.
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
        /// <param name="setup">The <see cref="TimeMeasureOptions"/> which need to be configured.</param>
        /// <returns>The result of the function delegate <paramref name="method"/>.</returns>
        /// <remarks>The instrumentation of <paramref name="method"/> is thread safe and the instrumented result is passed to <see cref="TimeMeasure.TimeMeasureCompletedCallback"/>.</remarks>
        public TResult ExecuteFunction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>(MethodBase caller, Doer<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, Act<TimeMeasureOptions> setup = null)
        {
            var factory = DoerFactory.Create(method, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10);
            return ExecuteFunctionCore(caller, factory, null, null, setup);
        }

        /// <summary>
        /// Executes the specified function delegate <paramref name="method"/>.
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
        /// <param name="validator">A <see cref="CacheValidator"/> object that represents the content validation of the resource.</param>
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
        /// <param name="setup">The <see cref="TimeMeasureOptions"/> which need to be configured.</param>
        /// <returns>The result of the function delegate <paramref name="method"/>.</returns>
        /// <remarks>The instrumentation of <paramref name="method"/> is thread safe and the instrumented result is passed to <see cref="TimeMeasure.TimeMeasureCompletedCallback"/>.</remarks>
        public TResult ExecuteFunction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>(MethodBase caller, CacheValidator validator, Doer<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, Act<TimeMeasureOptions> setup = null)
        {
            var factory = DoerFactory.Create(method, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10);
            return ExecuteFunctionCore(caller, factory, validator, null, setup);
        }

        /// <summary>
        /// Executes the specified function delegate <paramref name="method"/>.
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
        /// <param name="validator">A <see cref="CacheValidator"/> object that represents the content validation of the resource.</param>
        /// <param name="cacheability">Sets the <b>Cache-Control</b> header to one of the values of <see cref="HttpCacheability"/>.</param>
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
        /// <param name="setup">The <see cref="TimeMeasureOptions"/> which need to be configured.</param>
        /// <returns>The result of the function delegate <paramref name="method"/>.</returns>
        /// <remarks>The instrumentation of <paramref name="method"/> is thread safe and the instrumented result is passed to <see cref="TimeMeasure.TimeMeasureCompletedCallback"/>.</remarks>
        public TResult ExecuteFunction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>(MethodBase caller, CacheValidator validator, HttpCacheability cacheability, Doer<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, Act<TimeMeasureOptions> setup = null)
        {
            var factory = DoerFactory.Create(method, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10);
            return ExecuteFunctionCore(caller, factory, validator, cacheability, setup);
        }

        /// <summary>
        /// Executes the specified function delegate <paramref name="method"/>.
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
        /// <param name="setup">The <see cref="TimeMeasureOptions"/> which need to be configured.</param>
        /// <returns>The result of the function delegate <paramref name="method"/>.</returns>
        /// <remarks>The instrumentation of <paramref name="method"/> is thread safe and the instrumented result is passed to <see cref="TimeMeasure.TimeMeasureCompletedCallback"/>.</remarks>
        public TResult ExecuteFunction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult>(MethodBase caller, Doer<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, Act<TimeMeasureOptions> setup = null)
        {
            var factory = DoerFactory.Create(method, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11);
            return ExecuteFunctionCore(caller, factory, null, null, setup);
        }

        /// <summary>
        /// Executes the specified function delegate <paramref name="method"/>.
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
        /// <param name="validator">A <see cref="CacheValidator"/> object that represents the content validation of the resource.</param>
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
        /// <param name="setup">The <see cref="TimeMeasureOptions"/> which need to be configured.</param>
        /// <returns>The result of the function delegate <paramref name="method"/>.</returns>
        /// <remarks>The instrumentation of <paramref name="method"/> is thread safe and the instrumented result is passed to <see cref="TimeMeasure.TimeMeasureCompletedCallback"/>.</remarks>
        public TResult ExecuteFunction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult>(MethodBase caller, CacheValidator validator, Doer<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, Act<TimeMeasureOptions> setup = null)
        {
            var factory = DoerFactory.Create(method, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11);
            return ExecuteFunctionCore(caller, factory, validator, null, setup);
        }

        /// <summary>
        /// Executes the specified function delegate <paramref name="method"/>.
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
        /// <param name="validator">A <see cref="CacheValidator"/> object that represents the content validation of the resource.</param>
        /// <param name="cacheability">Sets the <b>Cache-Control</b> header to one of the values of <see cref="HttpCacheability"/>.</param>
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
        /// <param name="setup">The <see cref="TimeMeasureOptions"/> which need to be configured.</param>
        /// <returns>The result of the function delegate <paramref name="method"/>.</returns>
        /// <remarks>The instrumentation of <paramref name="method"/> is thread safe and the instrumented result is passed to <see cref="TimeMeasure.TimeMeasureCompletedCallback"/>.</remarks>
        public TResult ExecuteFunction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult>(MethodBase caller, CacheValidator validator, HttpCacheability cacheability, Doer<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, Act<TimeMeasureOptions> setup = null)
        {
            var factory = DoerFactory.Create(method, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11);
            return ExecuteFunctionCore(caller, factory, validator, cacheability, setup);
        }

        /// <summary>
        /// Executes the specified function delegate <paramref name="method"/>.
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
        /// <param name="setup">The <see cref="TimeMeasureOptions"/> which need to be configured.</param>
        /// <returns>The result of the function delegate <paramref name="method"/>.</returns>
        /// <remarks>The instrumentation of <paramref name="method"/> is thread safe and the instrumented result is passed to <see cref="TimeMeasure.TimeMeasureCompletedCallback"/>.</remarks>
        public TResult ExecuteFunction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult>(MethodBase caller, Doer<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, Act<TimeMeasureOptions> setup = null)
        {
            var factory = DoerFactory.Create(method, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12);
            return ExecuteFunctionCore(caller, factory, null, null, setup);
        }

        /// <summary>
        /// Executes the specified function delegate <paramref name="method"/>.
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
        /// <param name="validator">A <see cref="CacheValidator"/> object that represents the content validation of the resource.</param>
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
        /// <param name="setup">The <see cref="TimeMeasureOptions"/> which need to be configured.</param>
        /// <returns>The result of the function delegate <paramref name="method"/>.</returns>
        /// <remarks>The instrumentation of <paramref name="method"/> is thread safe and the instrumented result is passed to <see cref="TimeMeasure.TimeMeasureCompletedCallback"/>.</remarks>
        public TResult ExecuteFunction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult>(MethodBase caller, CacheValidator validator, Doer<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, Act<TimeMeasureOptions> setup = null)
        {
            var factory = DoerFactory.Create(method, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12);
            return ExecuteFunctionCore(caller, factory, validator, null, setup);
        }

        /// <summary>
        /// Executes the specified function delegate <paramref name="method"/>.
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
        /// <param name="validator">A <see cref="CacheValidator"/> object that represents the content validation of the resource.</param>
        /// <param name="cacheability">Sets the <b>Cache-Control</b> header to one of the values of <see cref="HttpCacheability"/>.</param>
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
        /// <param name="setup">The <see cref="TimeMeasureOptions"/> which need to be configured.</param>
        /// <returns>The result of the function delegate <paramref name="method"/>.</returns>
        /// <remarks>The instrumentation of <paramref name="method"/> is thread safe and the instrumented result is passed to <see cref="TimeMeasure.TimeMeasureCompletedCallback"/>.</remarks>
        public TResult ExecuteFunction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult>(MethodBase caller, CacheValidator validator, HttpCacheability cacheability, Doer<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, Act<TimeMeasureOptions> setup = null)
        {
            var factory = DoerFactory.Create(method, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12);
            return ExecuteFunctionCore(caller, factory, validator, cacheability, setup);
        }

        /// <summary>
        /// Executes the specified function delegate <paramref name="method"/>.
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
        /// <param name="setup">The <see cref="TimeMeasureOptions"/> which need to be configured.</param>
        /// <returns>The result of the function delegate <paramref name="method"/>.</returns>
        /// <remarks>The instrumentation of <paramref name="method"/> is thread safe and the instrumented result is passed to <see cref="TimeMeasure.TimeMeasureCompletedCallback"/>.</remarks>
        public TResult ExecuteFunction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>(MethodBase caller, Doer<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, Act<TimeMeasureOptions> setup = null)
        {
            var factory = DoerFactory.Create(method, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13);
            return ExecuteFunctionCore(caller, factory, null, null, setup);
        }

        /// <summary>
        /// Executes the specified function delegate <paramref name="method"/>.
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
        /// <param name="validator">A <see cref="CacheValidator"/> object that represents the content validation of the resource.</param>
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
        /// <param name="setup">The <see cref="TimeMeasureOptions"/> which need to be configured.</param>
        /// <returns>The result of the function delegate <paramref name="method"/>.</returns>
        /// <remarks>The instrumentation of <paramref name="method"/> is thread safe and the instrumented result is passed to <see cref="TimeMeasure.TimeMeasureCompletedCallback"/>.</remarks>
        public TResult ExecuteFunction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>(MethodBase caller, CacheValidator validator, Doer<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, Act<TimeMeasureOptions> setup = null)
        {
            var factory = DoerFactory.Create(method, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13);
            return ExecuteFunctionCore(caller, factory, validator, null, setup);
        }

        /// <summary>
        /// Executes the specified function delegate <paramref name="method"/>.
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
        /// <param name="validator">A <see cref="CacheValidator"/> object that represents the content validation of the resource.</param>
        /// <param name="cacheability">Sets the <b>Cache-Control</b> header to one of the values of <see cref="HttpCacheability"/>.</param>
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
        /// <param name="setup">The <see cref="TimeMeasureOptions"/> which need to be configured.</param>
        /// <returns>The result of the function delegate <paramref name="method"/>.</returns>
        /// <remarks>The instrumentation of <paramref name="method"/> is thread safe and the instrumented result is passed to <see cref="TimeMeasure.TimeMeasureCompletedCallback"/>.</remarks>
        public TResult ExecuteFunction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>(MethodBase caller, CacheValidator validator, HttpCacheability cacheability, Doer<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, Act<TimeMeasureOptions> setup = null)
        {
            var factory = DoerFactory.Create(method, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13);
            return ExecuteFunctionCore(caller, factory, validator, cacheability, setup);
        }

        /// <summary>
        /// Executes the specified function delegate <paramref name="method"/>.
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
        /// <param name="setup">The <see cref="TimeMeasureOptions"/> which need to be configured.</param>
        /// <returns>The result of the function delegate <paramref name="method"/>.</returns>
        /// <remarks>The instrumentation of <paramref name="method"/> is thread safe and the instrumented result is passed to <see cref="TimeMeasure.TimeMeasureCompletedCallback"/>.</remarks>
        public TResult ExecuteFunction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult>(MethodBase caller, Doer<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, Act<TimeMeasureOptions> setup = null)
        {
            var factory = DoerFactory.Create(method, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14);
            return ExecuteFunctionCore(caller, factory, null, null, setup);
        }

        /// <summary>
        /// Executes the specified function delegate <paramref name="method"/>.
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
        /// <param name="validator">A <see cref="CacheValidator"/> object that represents the content validation of the resource.</param>
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
        /// <param name="setup">The <see cref="TimeMeasureOptions"/> which need to be configured.</param>
        /// <returns>The result of the function delegate <paramref name="method"/>.</returns>
        /// <remarks>The instrumentation of <paramref name="method"/> is thread safe and the instrumented result is passed to <see cref="TimeMeasure.TimeMeasureCompletedCallback"/>.</remarks>
        public TResult ExecuteFunction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult>(MethodBase caller, CacheValidator validator, Doer<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, Act<TimeMeasureOptions> setup = null)
        {
            var factory = DoerFactory.Create(method, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14);
            return ExecuteFunctionCore(caller, factory, validator, null, setup);
        }

        /// <summary>
        /// Executes the specified function delegate <paramref name="method"/>.
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
        /// <param name="validator">A <see cref="CacheValidator"/> object that represents the content validation of the resource.</param>
        /// <param name="cacheability">Sets the <b>Cache-Control</b> header to one of the values of <see cref="HttpCacheability"/>.</param>
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
        /// <param name="setup">The <see cref="TimeMeasureOptions"/> which need to be configured.</param>
        /// <returns>The result of the function delegate <paramref name="method"/>.</returns>
        /// <remarks>The instrumentation of <paramref name="method"/> is thread safe and the instrumented result is passed to <see cref="TimeMeasure.TimeMeasureCompletedCallback"/>.</remarks>
        public TResult ExecuteFunction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult>(MethodBase caller, CacheValidator validator, HttpCacheability cacheability, Doer<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, Act<TimeMeasureOptions> setup = null)
        {
            var factory = DoerFactory.Create(method, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14);
            return ExecuteFunctionCore(caller, factory, validator, cacheability, setup);
        }

        /// <summary>
        /// Executes the specified function delegate <paramref name="method"/>.
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
        /// <param name="setup">The <see cref="TimeMeasureOptions"/> which need to be configured.</param>
        /// <returns>The result of the function delegate <paramref name="method"/>.</returns>
        /// <remarks>The instrumentation of <paramref name="method"/> is thread safe and the instrumented result is passed to <see cref="TimeMeasure.TimeMeasureCompletedCallback"/>.</remarks>
        public TResult ExecuteFunction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult>(MethodBase caller, Doer<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, Act<TimeMeasureOptions> setup = null)
        {
            var factory = DoerFactory.Create(method, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15);
            return ExecuteFunctionCore(caller, factory, null, null, setup);
        }

        /// <summary>
        /// Executes the specified function delegate <paramref name="method"/>.
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
        /// <param name="validator">A <see cref="CacheValidator"/> object that represents the content validation of the resource.</param>
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
        /// <param name="setup">The <see cref="TimeMeasureOptions"/> which need to be configured.</param>
        /// <returns>The result of the function delegate <paramref name="method"/>.</returns>
        /// <remarks>The instrumentation of <paramref name="method"/> is thread safe and the instrumented result is passed to <see cref="TimeMeasure.TimeMeasureCompletedCallback"/>.</remarks>
        public TResult ExecuteFunction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult>(MethodBase caller, CacheValidator validator, Doer<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, Act<TimeMeasureOptions> setup = null)
        {
            var factory = DoerFactory.Create(method, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15);
            return ExecuteFunctionCore(caller, factory, validator, null, setup);
        }

        /// <summary>
        /// Executes the specified function delegate <paramref name="method"/>.
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
        /// <param name="validator">A <see cref="CacheValidator"/> object that represents the content validation of the resource.</param>
        /// <param name="cacheability">Sets the <b>Cache-Control</b> header to one of the values of <see cref="HttpCacheability"/>.</param>
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
        /// <param name="setup">The <see cref="TimeMeasureOptions"/> which need to be configured.</param>
        /// <returns>The result of the function delegate <paramref name="method"/>.</returns>
        /// <remarks>The instrumentation of <paramref name="method"/> is thread safe and the instrumented result is passed to <see cref="TimeMeasure.TimeMeasureCompletedCallback"/>.</remarks>
        public TResult ExecuteFunction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult>(MethodBase caller, CacheValidator validator, HttpCacheability cacheability, Doer<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, Act<TimeMeasureOptions> setup = null)
        {
            var factory = DoerFactory.Create(method, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15);
            return ExecuteFunctionCore(caller, factory, validator, cacheability, setup);
        }

        /// <summary>
        /// Executes the specified function delegate <paramref name="method"/>.
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
        /// <param name="setup">The <see cref="TimeMeasureOptions"/> which need to be configured.</param>
        /// <returns>The result of the function delegate <paramref name="method"/>.</returns>
        /// <remarks>The instrumentation of <paramref name="method"/> is thread safe and the instrumented result is passed to <see cref="TimeMeasure.TimeMeasureCompletedCallback"/>.</remarks>
        public TResult ExecuteFunction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult>(MethodBase caller, Doer<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, T16 arg16, Act<TimeMeasureOptions> setup = null)
        {
            var factory = DoerFactory.Create(method, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15, arg16);
            return ExecuteFunctionCore(caller, factory, null, null, setup);
        }

        /// <summary>
        /// Executes the specified function delegate <paramref name="method"/>.
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
        /// <param name="validator">A <see cref="CacheValidator"/> object that represents the content validation of the resource.</param>
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
        /// <param name="setup">The <see cref="TimeMeasureOptions"/> which need to be configured.</param>
        /// <returns>The result of the function delegate <paramref name="method"/>.</returns>
        /// <remarks>The instrumentation of <paramref name="method"/> is thread safe and the instrumented result is passed to <see cref="TimeMeasure.TimeMeasureCompletedCallback"/>.</remarks>
        public TResult ExecuteFunction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult>(MethodBase caller, CacheValidator validator, Doer<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, T16 arg16, Act<TimeMeasureOptions> setup = null)
        {
            var factory = DoerFactory.Create(method, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15, arg16);
            return ExecuteFunctionCore(caller, factory, validator, null, setup);
        }

        /// <summary>
        /// Executes the specified function delegate <paramref name="method"/>.
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
        /// <param name="validator">A <see cref="CacheValidator"/> object that represents the content validation of the resource.</param>
        /// <param name="cacheability">Sets the <b>Cache-Control</b> header to one of the values of <see cref="HttpCacheability"/>.</param>
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
        /// <param name="setup">The <see cref="TimeMeasureOptions"/> which need to be configured.</param>
        /// <returns>The result of the function delegate <paramref name="method"/>.</returns>
        /// <remarks>The instrumentation of <paramref name="method"/> is thread safe and the instrumented result is passed to <see cref="TimeMeasure.TimeMeasureCompletedCallback"/>.</remarks>
        public TResult ExecuteFunction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult>(MethodBase caller, CacheValidator validator, HttpCacheability cacheability, Doer<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, T16 arg16, Act<TimeMeasureOptions> setup = null)
        {
            var factory = DoerFactory.Create(method, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15, arg16);
            return ExecuteFunctionCore(caller, factory, validator, cacheability, setup);
        }

        /// <summary>
        /// Executes the specified function delegate <paramref name="method"/>.
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
        /// <param name="setup">The <see cref="TimeMeasureOptions"/> which need to be configured.</param>
        /// <returns>The result of the function delegate <paramref name="method"/>.</returns>
        /// <remarks>The instrumentation of <paramref name="method"/> is thread safe and the instrumented result is passed to <see cref="TimeMeasure.TimeMeasureCompletedCallback"/>.</remarks>
        public TResult ExecuteFunction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, TResult>(MethodBase caller, Doer<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, TResult> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, T16 arg16, T17 arg17, Act<TimeMeasureOptions> setup = null)
        {
            var factory = DoerFactory.Create(method, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15, arg16, arg17);
            return ExecuteFunctionCore(caller, factory, null, null, setup);
        }

        /// <summary>
        /// Executes the specified function delegate <paramref name="method"/>.
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
        /// <param name="validator">A <see cref="CacheValidator"/> object that represents the content validation of the resource.</param>
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
        /// <param name="setup">The <see cref="TimeMeasureOptions"/> which need to be configured.</param>
        /// <returns>The result of the function delegate <paramref name="method"/>.</returns>
        /// <remarks>The instrumentation of <paramref name="method"/> is thread safe and the instrumented result is passed to <see cref="TimeMeasure.TimeMeasureCompletedCallback"/>.</remarks>
        public TResult ExecuteFunction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, TResult>(MethodBase caller, CacheValidator validator, Doer<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, TResult> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, T16 arg16, T17 arg17, Act<TimeMeasureOptions> setup = null)
        {
            var factory = DoerFactory.Create(method, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15, arg16, arg17);
            return ExecuteFunctionCore(caller, factory, validator, null, setup);
        }

        /// <summary>
        /// Executes the specified function delegate <paramref name="method"/>.
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
        /// <param name="validator">A <see cref="CacheValidator"/> object that represents the content validation of the resource.</param>
        /// <param name="cacheability">Sets the <b>Cache-Control</b> header to one of the values of <see cref="HttpCacheability"/>.</param>
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
        /// <param name="setup">The <see cref="TimeMeasureOptions"/> which need to be configured.</param>
        /// <returns>The result of the function delegate <paramref name="method"/>.</returns>
        /// <remarks>The instrumentation of <paramref name="method"/> is thread safe and the instrumented result is passed to <see cref="TimeMeasure.TimeMeasureCompletedCallback"/>.</remarks>
        public TResult ExecuteFunction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, TResult>(MethodBase caller, CacheValidator validator, HttpCacheability cacheability, Doer<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, TResult> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, T16 arg16, T17 arg17, Act<TimeMeasureOptions> setup = null)
        {
            var factory = DoerFactory.Create(method, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15, arg16, arg17);
            return ExecuteFunctionCore(caller, factory, validator, cacheability, setup);
        }

        /// <summary>
        /// Executes the specified function delegate <paramref name="method"/>.
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
        /// <param name="setup">The <see cref="TimeMeasureOptions"/> which need to be configured.</param>
        /// <returns>The result of the function delegate <paramref name="method"/>.</returns>
        /// <remarks>The instrumentation of <paramref name="method"/> is thread safe and the instrumented result is passed to <see cref="TimeMeasure.TimeMeasureCompletedCallback"/>.</remarks>
        public TResult ExecuteFunction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, TResult>(MethodBase caller, Doer<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, TResult> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, T16 arg16, T17 arg17, T18 arg18, Act<TimeMeasureOptions> setup = null)
        {
            var factory = DoerFactory.Create(method, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15, arg16, arg17, arg18);
            return ExecuteFunctionCore(caller, factory, null, null, setup);
        }

        /// <summary>
        /// Executes the specified function delegate <paramref name="method"/>.
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
        /// <param name="validator">A <see cref="CacheValidator"/> object that represents the content validation of the resource.</param>
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
        /// <param name="setup">The <see cref="TimeMeasureOptions"/> which need to be configured.</param>
        /// <returns>The result of the function delegate <paramref name="method"/>.</returns>
        /// <remarks>The instrumentation of <paramref name="method"/> is thread safe and the instrumented result is passed to <see cref="TimeMeasure.TimeMeasureCompletedCallback"/>.</remarks>
        public TResult ExecuteFunction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, TResult>(MethodBase caller, CacheValidator validator, Doer<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, TResult> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, T16 arg16, T17 arg17, T18 arg18, Act<TimeMeasureOptions> setup = null)
        {
            var factory = DoerFactory.Create(method, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15, arg16, arg17, arg18);
            return ExecuteFunctionCore(caller, factory, validator, null, setup);
        }

        /// <summary>
        /// Executes the specified function delegate <paramref name="method"/>.
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
        /// <param name="validator">A <see cref="CacheValidator"/> object that represents the content validation of the resource.</param>
        /// <param name="cacheability">Sets the <b>Cache-Control</b> header to one of the values of <see cref="HttpCacheability"/>.</param>
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
        /// <param name="setup">The <see cref="TimeMeasureOptions"/> which need to be configured.</param>
        /// <returns>The result of the function delegate <paramref name="method"/>.</returns>
        /// <remarks>The instrumentation of <paramref name="method"/> is thread safe and the instrumented result is passed to <see cref="TimeMeasure.TimeMeasureCompletedCallback"/>.</remarks>
        public TResult ExecuteFunction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, TResult>(MethodBase caller, CacheValidator validator, HttpCacheability cacheability, Doer<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, TResult> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, T16 arg16, T17 arg17, T18 arg18, Act<TimeMeasureOptions> setup = null)
        {
            var factory = DoerFactory.Create(method, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15, arg16, arg17, arg18);
            return ExecuteFunctionCore(caller, factory, validator, cacheability, setup);
        }

        /// <summary>
        /// Executes the specified function delegate <paramref name="method"/>.
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
        /// <param name="setup">The <see cref="TimeMeasureOptions"/> which need to be configured.</param>
        /// <returns>The result of the function delegate <paramref name="method"/>.</returns>
        /// <remarks>The instrumentation of <paramref name="method"/> is thread safe and the instrumented result is passed to <see cref="TimeMeasure.TimeMeasureCompletedCallback"/>.</remarks>
        public TResult ExecuteFunction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, TResult>(MethodBase caller, Doer<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, TResult> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, T16 arg16, T17 arg17, T18 arg18, T19 arg19, Act<TimeMeasureOptions> setup = null)
        {
            var factory = DoerFactory.Create(method, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15, arg16, arg17, arg18, arg19);
            return ExecuteFunctionCore(caller, factory, null, null, setup);
        }

        /// <summary>
        /// Executes the specified function delegate <paramref name="method"/>.
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
        /// <param name="validator">A <see cref="CacheValidator"/> object that represents the content validation of the resource.</param>
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
        /// <param name="setup">The <see cref="TimeMeasureOptions"/> which need to be configured.</param>
        /// <returns>The result of the function delegate <paramref name="method"/>.</returns>
        /// <remarks>The instrumentation of <paramref name="method"/> is thread safe and the instrumented result is passed to <see cref="TimeMeasure.TimeMeasureCompletedCallback"/>.</remarks>
        public TResult ExecuteFunction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, TResult>(MethodBase caller, CacheValidator validator, Doer<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, TResult> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, T16 arg16, T17 arg17, T18 arg18, T19 arg19, Act<TimeMeasureOptions> setup = null)
        {
            var factory = DoerFactory.Create(method, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15, arg16, arg17, arg18, arg19);
            return ExecuteFunctionCore(caller, factory, validator, null, setup);
        }

        /// <summary>
        /// Executes the specified function delegate <paramref name="method"/>.
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
        /// <param name="validator">A <see cref="CacheValidator"/> object that represents the content validation of the resource.</param>
        /// <param name="cacheability">Sets the <b>Cache-Control</b> header to one of the values of <see cref="HttpCacheability"/>.</param>
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
        /// <param name="setup">The <see cref="TimeMeasureOptions"/> which need to be configured.</param>
        /// <returns>The result of the function delegate <paramref name="method"/>.</returns>
        /// <remarks>The instrumentation of <paramref name="method"/> is thread safe and the instrumented result is passed to <see cref="TimeMeasure.TimeMeasureCompletedCallback"/>.</remarks>
        public TResult ExecuteFunction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, TResult>(MethodBase caller, CacheValidator validator, HttpCacheability cacheability, Doer<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, TResult> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, T16 arg16, T17 arg17, T18 arg18, T19 arg19, Act<TimeMeasureOptions> setup = null)
        {
            var factory = DoerFactory.Create(method, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15, arg16, arg17, arg18, arg19);
            return ExecuteFunctionCore(caller, factory, validator, cacheability, setup);
        }

        /// <summary>
        /// Executes the specified function delegate <paramref name="method"/>.
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
        /// <param name="setup">The <see cref="TimeMeasureOptions"/> which need to be configured.</param>
        /// <returns>The result of the function delegate <paramref name="method"/>.</returns>
        /// <remarks>The instrumentation of <paramref name="method"/> is thread safe and the instrumented result is passed to <see cref="TimeMeasure.TimeMeasureCompletedCallback"/>.</remarks>
        public TResult ExecuteFunction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, TResult>(MethodBase caller, Doer<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, TResult> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, T16 arg16, T17 arg17, T18 arg18, T19 arg19, T20 arg20, Act<TimeMeasureOptions> setup = null)
        {
            var factory = DoerFactory.Create(method, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15, arg16, arg17, arg18, arg19, arg20);
            return ExecuteFunctionCore(caller, factory, null, null, setup);
        }

        /// <summary>
        /// Executes the specified function delegate <paramref name="method"/>.
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
        /// <param name="validator">A <see cref="CacheValidator"/> object that represents the content validation of the resource.</param>
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
        /// <param name="setup">The <see cref="TimeMeasureOptions"/> which need to be configured.</param>
        /// <returns>The result of the function delegate <paramref name="method"/>.</returns>
        /// <remarks>The instrumentation of <paramref name="method"/> is thread safe and the instrumented result is passed to <see cref="TimeMeasure.TimeMeasureCompletedCallback"/>.</remarks>
        public TResult ExecuteFunction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, TResult>(MethodBase caller, CacheValidator validator, Doer<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, TResult> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, T16 arg16, T17 arg17, T18 arg18, T19 arg19, T20 arg20, Act<TimeMeasureOptions> setup = null)
        {
            var factory = DoerFactory.Create(method, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15, arg16, arg17, arg18, arg19, arg20);
            return ExecuteFunctionCore(caller, factory, validator, null, setup);
        }

        /// <summary>
        /// Executes the specified function delegate <paramref name="method"/>.
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
        /// <param name="validator">A <see cref="CacheValidator"/> object that represents the content validation of the resource.</param>
        /// <param name="cacheability">Sets the <b>Cache-Control</b> header to one of the values of <see cref="HttpCacheability"/>.</param>
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
        /// <param name="setup">The <see cref="TimeMeasureOptions"/> which need to be configured.</param>
        /// <returns>The result of the function delegate <paramref name="method"/>.</returns>
        /// <remarks>The instrumentation of <paramref name="method"/> is thread safe and the instrumented result is passed to <see cref="TimeMeasure.TimeMeasureCompletedCallback"/>.</remarks>
        public TResult ExecuteFunction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, TResult>(MethodBase caller, CacheValidator validator, HttpCacheability cacheability, Doer<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, TResult> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, T16 arg16, T17 arg17, T18 arg18, T19 arg19, T20 arg20, Act<TimeMeasureOptions> setup = null)
        {
            var factory = DoerFactory.Create(method, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15, arg16, arg17, arg18, arg19, arg20);
            return ExecuteFunctionCore(caller, factory, validator, cacheability, setup);
        }

        private TResult ExecuteFunctionCore<TTuple, TResult>(MethodBase caller, DoerFactory<TTuple, TResult> factory, CacheValidator validator, HttpCacheability? cacheability, Act<TimeMeasureOptions> setup) where TTuple : Template
        {
            var options = DelegateUtility.ConfigureAction(setup);
            var utcNow = DateTime.UtcNow;
            var expires = DateTime.MinValue;
            try
            {
                return TimeMeasure.WithFunc(() =>
                {
                    HttpCacheability resolvedCacheability = cacheability.HasValue ? cacheability.Value : HttpCacheability.NoCache;
                    HttpCachingAttribute cachingAttribute = GetCachingAttribute(caller);
                    HttpResponseAttribute responseAttribute = ReflectionUtility.GetAttribute<HttpResponseAttribute>(caller);
                    if (cachingAttribute != null)
                    {
                        if (cachingAttribute.EnableCaching)
                        {
                            if (!cacheability.HasValue) { resolvedCacheability = cachingAttribute.Cacheability; }
                            expires = utcNow.Add(TimeSpanConverter.FromDouble(cachingAttribute.Duration, cachingAttribute.DurationUnit));
                        }
                    }


                    bool suppressContent;
                    ClientSideCachingHandler(validator, expires, resolvedCacheability, out suppressContent);
                    if (suppressContent)
                    {
                        StatusCodeHandler(HttpStatusCode.NotModified);
                        return default(TResult);
                    }

                    HttpStatusCode successStatusCode = responseAttribute?.SuccessStatusCode ?? HttpStatusCode.OK;
                    string successStatusDescription = responseAttribute?.SuccessStatusDescription;
                    StatusCodeHandler(successStatusCode, successStatusDescription);

                    return factory.ExecuteMethod();
                }, o =>
                {
                    o.RuntimeParameters = factory.GenericArguments.ToArray();
                    o.MethodDescriptorCallback = () => new MethodDescriptor(caller);
                    o.TimeMeasureCompletedThreshold = options.TimeMeasureCompletedThreshold;
                }).Result;
            }
            catch (Exception ex)
            {
                ExceptionHandler(caller, ex, factory.GenericArguments.ToArray());
                return default(TResult);
            }
        }

        private HttpCachingAttribute GetCachingAttribute(MethodBase caller)
        {
            HttpCachingAttribute cachingClassAttribute = ReflectionUtility.GetAttribute<HttpCachingAttribute>(GetType(), true);
            HttpCachingAttribute cachingMethodAttribute = ReflectionUtility.GetAttribute<HttpCachingAttribute>(caller);
            if (cachingMethodAttribute != null) { return cachingMethodAttribute; }
            if (cachingClassAttribute != null) { return cachingClassAttribute; }
            return null;
        }
    }
}