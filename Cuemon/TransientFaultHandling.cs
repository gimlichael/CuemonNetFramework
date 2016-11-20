﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using Cuemon.Threading;

namespace Cuemon
{
    /// <summary>
    /// Provides developers ways to make their applications more resilient by adding robust transient fault handling logic ideal for temporary condition such as network connectivity issues or service unavailability.
    /// </summary>
    public static class TransientFaultHandling
    {
        /// <summary>
        /// Repetitively executes the specified <paramref name="faultSensitiveMethod"/> until the operation is successful, the amount of retry attempts has been reached, or a failed operation is not considered related to transient fault condition.
        /// </summary>
        /// <typeparam name="TResult">The type of the return value of <paramref name="faultSensitiveMethod"/>.</typeparam>
        /// <param name="faultSensitiveMethod">The fault sensitive function delegate that is invoked until an operation is successful, the amount of retry attempts has been reached, or a failed operation is not considered related to transient fault condition.</param>
        /// <param name="setup">The <see cref="TransientFaultHandlingOptions"/> which need to be configured.</param>
        /// <returns>The result from the <paramref name="faultSensitiveMethod"/>.</returns>
        public static TResult WithFunc<TResult>(Doer<TResult> faultSensitiveMethod, Act<TransientFaultHandlingOptions> setup = null)
        {
            var factory = DoerFactory.Create(faultSensitiveMethod);
            return ExecuteFunctionCore(factory, setup);
        }

        /// <summary>
        /// Repetitively executes the specified <paramref name="faultSensitiveMethod"/> until the operation is successful, the amount of retry attempts has been reached, or a failed operation is not considered related to transient fault condition.
        /// </summary>
        /// <typeparam name="T">The type of the parameter of the function delegate <paramref name="faultSensitiveMethod"/>.</typeparam>
        /// <typeparam name="TResult">The type of the return value of the function delegate <paramref name="faultSensitiveMethod"/>.</typeparam>
        /// <param name="faultSensitiveMethod">The fault sensitive function delegate that is invoked until an operation is successful, the amount of retry attempts has been reached, or a failed operation is not considered related to transient fault condition.</param>
        /// <param name="arg">The parameter of the function delegate <paramref name="faultSensitiveMethod"/>.</param>
        /// <param name="setup">The <see cref="TransientFaultHandlingOptions"/> which need to be configured.</param>
        /// <returns>The result from the <paramref name="faultSensitiveMethod"/>.</returns>
        public static TResult WithFunc<T, TResult>(Doer<T, TResult> faultSensitiveMethod, T arg, Act<TransientFaultHandlingOptions> setup = null)
        {
            var factory = DoerFactory.Create(faultSensitiveMethod, arg);
            return ExecuteFunctionCore(factory, setup);
        }

        /// <summary>
        /// Repetitively executes the specified <paramref name="faultSensitiveMethod"/> until the operation is successful, the amount of retry attempts has been reached, or a failed operation is not considered related to transient fault condition.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter of the function delegate <paramref name="faultSensitiveMethod"/>.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the function delegate <paramref name="faultSensitiveMethod"/>.</typeparam>
        /// <typeparam name="TResult">The type of the return value of the function delegate <paramref name="faultSensitiveMethod"/>.</typeparam>
        /// <param name="faultSensitiveMethod">The fault sensitive function delegate that is invoked until an operation is successful, the amount of retry attempts has been reached, or a failed operation is not considered related to transient fault condition.</param>
        /// <param name="arg1">The first parameter of the function delegate <paramref name="faultSensitiveMethod"/>.</param>
        /// <param name="arg2">The second parameter of the function delegate <paramref name="faultSensitiveMethod"/>.</param>
        /// <param name="setup">The <see cref="TransientFaultHandlingOptions"/> which need to be configured.</param>
        /// <returns>The result from the <paramref name="faultSensitiveMethod"/>.</returns>
        public static TResult WithFunc<T1, T2, TResult>(Doer<T1, T2, TResult> faultSensitiveMethod, T1 arg1, T2 arg2, Act<TransientFaultHandlingOptions> setup = null)
        {
            var factory = DoerFactory.Create(faultSensitiveMethod, arg1, arg2);
            return ExecuteFunctionCore(factory, setup);
        }

        /// <summary>
        /// Repetitively executes the specified <paramref name="faultSensitiveMethod"/> until the operation is successful, the amount of retry attempts has been reached, or a failed operation is not considered related to transient fault condition.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter of the function delegate <paramref name="faultSensitiveMethod"/>.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the function delegate <paramref name="faultSensitiveMethod"/>.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the function delegate <paramref name="faultSensitiveMethod"/>.</typeparam>
        /// <typeparam name="TResult">The type of the return value of the function delegate <paramref name="faultSensitiveMethod"/>.</typeparam>
        /// <param name="faultSensitiveMethod">The fault sensitive function delegate that is invoked until an operation is successful, the amount of retry attempts has been reached, or a failed operation is not considered related to transient fault condition.</param>
        /// <param name="arg1">The first parameter of the function delegate <paramref name="faultSensitiveMethod"/>.</param>
        /// <param name="arg2">The second parameter of the function delegate <paramref name="faultSensitiveMethod"/>.</param>
        /// <param name="arg3">The third parameter of the function delegate <paramref name="faultSensitiveMethod"/>.</param>
        /// <param name="setup">The <see cref="TransientFaultHandlingOptions"/> which need to be configured.</param>
        /// <returns>The result from the <paramref name="faultSensitiveMethod"/>.</returns>
        public static TResult WithFunc<T1, T2, T3, TResult>(Doer<T1, T2, T3, TResult> faultSensitiveMethod, T1 arg1, T2 arg2, T3 arg3, Act<TransientFaultHandlingOptions> setup = null)
        {
            var factory = DoerFactory.Create(faultSensitiveMethod, arg1, arg2, arg3);
            return ExecuteFunctionCore(factory, setup);
        }

        /// <summary>
        /// Repetitively executes the specified <paramref name="faultSensitiveMethod"/> until the operation is successful, the amount of retry attempts has been reached, or a failed operation is not considered related to transient fault condition.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter of the function delegate <paramref name="faultSensitiveMethod"/>.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the function delegate <paramref name="faultSensitiveMethod"/>.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the function delegate <paramref name="faultSensitiveMethod"/>.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of the function delegate <paramref name="faultSensitiveMethod"/>.</typeparam>
        /// <typeparam name="TResult">The type of the return value of the function delegate <paramref name="faultSensitiveMethod"/>.</typeparam>
        /// <param name="faultSensitiveMethod">The fault sensitive function delegate that is invoked until an operation is successful, the amount of retry attempts has been reached, or a failed operation is not considered related to transient fault condition.</param>
        /// <param name="arg1">The first parameter of the function delegate <paramref name="faultSensitiveMethod"/>.</param>
        /// <param name="arg2">The second parameter of the function delegate <paramref name="faultSensitiveMethod"/>.</param>
        /// <param name="arg3">The third parameter of the function delegate <paramref name="faultSensitiveMethod"/>.</param>
        /// <param name="arg4">The fourth parameter of the function delegate <paramref name="faultSensitiveMethod"/>.</param>
        /// <param name="setup">The <see cref="TransientFaultHandlingOptions"/> which need to be configured.</param>
        /// <returns>The result from the <paramref name="faultSensitiveMethod"/>.</returns>
        public static TResult WithFunc<T1, T2, T3, T4, TResult>(Doer<T1, T2, T3, T4, TResult> faultSensitiveMethod, T1 arg1, T2 arg2, T3 arg3, T4 arg4, Act<TransientFaultHandlingOptions> setup = null)
        {
            var factory = DoerFactory.Create(faultSensitiveMethod, arg1, arg2, arg3, arg4);
            return ExecuteFunctionCore(factory, setup);
        }

        /// <summary>
        /// Repetitively executes the specified <paramref name="faultSensitiveMethod"/> until the operation is successful, the amount of retry attempts has been reached, or a failed operation is not considered related to transient fault condition.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter of the function delegate <paramref name="faultSensitiveMethod"/>.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the function delegate <paramref name="faultSensitiveMethod"/>.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the function delegate <paramref name="faultSensitiveMethod"/>.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of the function delegate <paramref name="faultSensitiveMethod"/>.</typeparam>
        /// <typeparam name="T5">The type of the fifth parameter of the function delegate <paramref name="faultSensitiveMethod"/>.</typeparam>
        /// <typeparam name="TResult">The type of the return value of the function delegate <paramref name="faultSensitiveMethod"/>.</typeparam>
        /// <param name="faultSensitiveMethod">The fault sensitive function delegate that is invoked until an operation is successful, the amount of retry attempts has been reached, or a failed operation is not considered related to transient fault condition.</param>
        /// <param name="arg1">The first parameter of the function delegate <paramref name="faultSensitiveMethod"/>.</param>
        /// <param name="arg2">The second parameter of the function delegate <paramref name="faultSensitiveMethod"/>.</param>
        /// <param name="arg3">The third parameter of the function delegate <paramref name="faultSensitiveMethod"/>.</param>
        /// <param name="arg4">The fourth parameter of the function delegate <paramref name="faultSensitiveMethod"/>.</param>
        /// <param name="arg5">The fifth parameter of the function delegate <paramref name="faultSensitiveMethod"/>.</param>
        /// <param name="setup">The <see cref="TransientFaultHandlingOptions"/> which need to be configured.</param>
        /// <returns>The result from the <paramref name="faultSensitiveMethod"/>.</returns>
        public static TResult WithFunc<T1, T2, T3, T4, T5, TResult>(Doer<T1, T2, T3, T4, T5, TResult> faultSensitiveMethod, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, Act<TransientFaultHandlingOptions> setup = null)
        {
            var factory = DoerFactory.Create(faultSensitiveMethod, arg1, arg2, arg3, arg4, arg5);
            return ExecuteFunctionCore(factory, setup);
        }

        /// <summary>
        /// Repetitively executes the specified <paramref name="faultSensitiveMethod"/> until the operation is successful, the amount of retry attempts has been reached, or a failed operation is not considered related to transient fault condition.
        /// </summary>
        /// <typeparam name="TResult">The type of the out result value of the function delegate encapsulates <paramref name="faultSensitiveMethod"/>.</typeparam>
        /// <typeparam name="TSuccess">The type of the return value that indicates success of the function delegate <paramref name="faultSensitiveMethod"/>.</typeparam>
        /// <param name="faultSensitiveMethod">The fault sensitive function delegate that is invoked until an operation is successful, the amount of retry attempts has been reached, or a failed operation is not considered related to transient fault condition.</param>
        /// <param name="result">The result of the function delegate <paramref name="faultSensitiveMethod"/>.</param>
        /// <param name="setup">The <see cref="TransientFaultHandlingOptions"/> which need to be configured.</param>
        /// <returns>The return value that indicates success of the function delegate <paramref name="faultSensitiveMethod"/>.</returns>
        public static TSuccess TryWithFunc<TResult, TSuccess>(TesterDoer<TResult, TSuccess> faultSensitiveMethod, out TResult result, Act<TransientFaultHandlingOptions> setup = null)
        {
            var factory = TesterDoerFactory.Create(faultSensitiveMethod);
            return TryExecuteFunctionCore(factory, out result, setup);
        }

        /// <summary>
        /// Repetitively executes the specified <paramref name="faultSensitiveMethod"/> until the operation is successful, the amount of retry attempts has been reached, or a failed operation is not considered related to transient fault condition.
        /// </summary>
        /// <typeparam name="T">The type of the parameter of the function delegate <paramref name="faultSensitiveMethod"/>.</typeparam>
        /// <typeparam name="TResult">The type of the out result value of the function delegate encapsulates <paramref name="faultSensitiveMethod"/>.</typeparam>
        /// <typeparam name="TSuccess">The type of the return value that indicates success of the function delegate <paramref name="faultSensitiveMethod"/>.</typeparam>
        /// <param name="faultSensitiveMethod">The fault sensitive function delegate that is invoked until an operation is successful, the amount of retry attempts has been reached, or a failed operation is not considered related to transient fault condition.</param>
        /// <param name="arg">The parameter of the function delegate <paramref name="faultSensitiveMethod"/>.</param>
        /// <param name="result">The result of the function delegate <paramref name="faultSensitiveMethod"/>.</param>
        /// <param name="setup">The <see cref="TransientFaultHandlingOptions"/> which need to be configured.</param>
        /// <returns>The return value that indicates success of the function delegate <paramref name="faultSensitiveMethod"/>.</returns>
        public static TSuccess TryWithFunc<T, TResult, TSuccess>(TesterDoer<T, TResult, TSuccess> faultSensitiveMethod, T arg, out TResult result, Act<TransientFaultHandlingOptions> setup = null)
        {
            var factory = TesterDoerFactory.Create(faultSensitiveMethod, arg);
            return TryExecuteFunctionCore(factory, out result, setup);
        }

        /// <summary>
        /// Repetitively executes the specified <paramref name="faultSensitiveMethod"/> until the operation is successful, the amount of retry attempts has been reached, or a failed operation is not considered related to transient fault condition.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter of the function delegate <paramref name="faultSensitiveMethod"/>.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the function delegate <paramref name="faultSensitiveMethod"/>.</typeparam>
        /// <typeparam name="TResult">The type of the out result value of the function delegate encapsulates <paramref name="faultSensitiveMethod"/>.</typeparam>
        /// <typeparam name="TSuccess">The type of the return value that indicates success of the function delegate <paramref name="faultSensitiveMethod"/>.</typeparam>
        /// <param name="faultSensitiveMethod">The fault sensitive function delegate that is invoked until an operation is successful, the amount of retry attempts has been reached, or a failed operation is not considered related to transient fault condition.</param>
        /// <param name="arg1">The first parameter of the function delegate <paramref name="faultSensitiveMethod"/>.</param>
        /// <param name="arg2">The second parameter of the function delegate <paramref name="faultSensitiveMethod"/>.</param>
        /// <param name="result">The result of the function delegate <paramref name="faultSensitiveMethod"/>.</param>
        /// <param name="setup">The <see cref="TransientFaultHandlingOptions"/> which need to be configured.</param>
        /// <returns>The return value that indicates success of the function delegate <paramref name="faultSensitiveMethod"/>.</returns>
        public static TSuccess TryWithFunc<T1, T2, TResult, TSuccess>(TesterDoer<T1, T2, TResult, TSuccess> faultSensitiveMethod, T1 arg1, T2 arg2, out TResult result, Act<TransientFaultHandlingOptions> setup = null)
        {
            var factory = TesterDoerFactory.Create(faultSensitiveMethod, arg1, arg2);
            return TryExecuteFunctionCore(factory, out result, setup);
        }

        /// <summary>
        /// Repetitively executes the specified <paramref name="faultSensitiveMethod"/> until the operation is successful, the amount of retry attempts has been reached, or a failed operation is not considered related to transient fault condition.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter of the function delegate <paramref name="faultSensitiveMethod"/>.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the function delegate <paramref name="faultSensitiveMethod"/>.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the function delegate <paramref name="faultSensitiveMethod"/>.</typeparam>
        /// <typeparam name="TResult">The type of the out result value of the function delegate encapsulates <paramref name="faultSensitiveMethod"/>.</typeparam>
        /// <typeparam name="TSuccess">The type of the return value that indicates success of the function delegate <paramref name="faultSensitiveMethod"/>.</typeparam>
        /// <param name="faultSensitiveMethod">The fault sensitive function delegate that is invoked until an operation is successful, the amount of retry attempts has been reached, or a failed operation is not considered related to transient fault condition.</param>
        /// <param name="arg1">The first parameter of the function delegate <paramref name="faultSensitiveMethod"/>.</param>
        /// <param name="arg2">The second parameter of the function delegate <paramref name="faultSensitiveMethod"/>.</param>
        /// <param name="arg3">The third parameter of the function delegate <paramref name="faultSensitiveMethod"/>.</param>
        /// <param name="result">The result of the function delegate <paramref name="faultSensitiveMethod"/>.</param>
        /// <param name="setup">The <see cref="TransientFaultHandlingOptions"/> which need to be configured.</param>
        /// <returns>The return value that indicates success of the function delegate <paramref name="faultSensitiveMethod"/>.</returns>
        public static TSuccess TryWithFunc<T1, T2, T3, TResult, TSuccess>(TesterDoer<T1, T2, T3, TResult, TSuccess> faultSensitiveMethod, T1 arg1, T2 arg2, T3 arg3, out TResult result, Act<TransientFaultHandlingOptions> setup = null)
        {
            var factory = TesterDoerFactory.Create(faultSensitiveMethod, arg1, arg2, arg3);
            return TryExecuteFunctionCore(factory, out result, setup);
        }

        /// <summary>
        /// Repetitively executes the specified <paramref name="faultSensitiveMethod"/> until the operation is successful, the amount of retry attempts has been reached, or a failed operation is not considered related to transient fault condition.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter of the function delegate <paramref name="faultSensitiveMethod"/>.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the function delegate <paramref name="faultSensitiveMethod"/>.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the function delegate <paramref name="faultSensitiveMethod"/>.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of the function delegate <paramref name="faultSensitiveMethod"/>.</typeparam>
        /// <typeparam name="TResult">The type of the out result value of the function delegate encapsulates <paramref name="faultSensitiveMethod"/>.</typeparam>
        /// <typeparam name="TSuccess">The type of the return value that indicates success of the function delegate <paramref name="faultSensitiveMethod"/>.</typeparam>
        /// <param name="faultSensitiveMethod">The fault sensitive function delegate that is invoked until an operation is successful, the amount of retry attempts has been reached, or a failed operation is not considered related to transient fault condition.</param>
        /// <param name="arg1">The first parameter of the function delegate <paramref name="faultSensitiveMethod"/>.</param>
        /// <param name="arg2">The second parameter of the function delegate <paramref name="faultSensitiveMethod"/>.</param>
        /// <param name="arg3">The third parameter of the function delegate <paramref name="faultSensitiveMethod"/>.</param>
        /// <param name="arg4">The fourth parameter of the function delegate <paramref name="faultSensitiveMethod"/>.</param>
        /// <param name="result">The result of the function delegate <paramref name="faultSensitiveMethod"/>.</param>
        /// <param name="setup">The <see cref="TransientFaultHandlingOptions"/> which need to be configured.</param>
        /// <returns>The return value that indicates success of the function delegate <paramref name="faultSensitiveMethod"/>.</returns>
        public static TSuccess TryWithFunc<T1, T2, T3, T4, TResult, TSuccess>(TesterDoer<T1, T2, T3, T4, TResult, TSuccess> faultSensitiveMethod, T1 arg1, T2 arg2, T3 arg3, T4 arg4, out TResult result, Act<TransientFaultHandlingOptions> setup = null)
        {
            var factory = TesterDoerFactory.Create(faultSensitiveMethod, arg1, arg2, arg3, arg4);
            return TryExecuteFunctionCore(factory, out result, setup);
        }

        /// <summary>
        /// Repetitively executes the specified <paramref name="faultSensitiveMethod"/> until the operation is successful, the amount of retry attempts has been reached, or a failed operation is not considered related to transient fault condition.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter of the function delegate <paramref name="faultSensitiveMethod"/>.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the function delegate <paramref name="faultSensitiveMethod"/>.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the function delegate <paramref name="faultSensitiveMethod"/>.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of the function delegate <paramref name="faultSensitiveMethod"/>.</typeparam>
        /// <typeparam name="T5">The type of the fifth parameter of the function delegate <paramref name="faultSensitiveMethod"/>.</typeparam>
        /// <typeparam name="TResult">The type of the out result value of the function delegate encapsulates <paramref name="faultSensitiveMethod"/>.</typeparam>
        /// <typeparam name="TSuccess">The type of the return value that indicates success of the function delegate <paramref name="faultSensitiveMethod"/>.</typeparam>
        /// <param name="faultSensitiveMethod">The fault sensitive function delegate that is invoked until an operation is successful, the amount of retry attempts has been reached, or a failed operation is not considered related to transient fault condition.</param>
        /// <param name="arg1">The first parameter of the function delegate <paramref name="faultSensitiveMethod"/>.</param>
        /// <param name="arg2">The second parameter of the function delegate <paramref name="faultSensitiveMethod"/>.</param>
        /// <param name="arg3">The third parameter of the function delegate <paramref name="faultSensitiveMethod"/>.</param>
        /// <param name="arg4">The fourth parameter of the function delegate <paramref name="faultSensitiveMethod"/>.</param>
        /// <param name="arg5">The fifth parameter of the function delegate <paramref name="faultSensitiveMethod"/>.</param>
        /// <param name="result">The result of the function delegate <paramref name="faultSensitiveMethod"/>.</param>
        /// <param name="setup">The <see cref="TransientFaultHandlingOptions"/> which need to be configured.</param>
        /// <returns>The return value that indicates success of the function delegate <paramref name="faultSensitiveMethod"/>.</returns>
        public static TSuccess TryWithFunc<T1, T2, T3, T4, T5, TResult, TSuccess>(TesterDoer<T1, T2, T3, T4, T5, TResult, TSuccess> faultSensitiveMethod, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, out TResult result, Act<TransientFaultHandlingOptions> setup = null)
        {
            var factory = TesterDoerFactory.Create(faultSensitiveMethod, arg1, arg2, arg3, arg4, arg5);
            return TryExecuteFunctionCore(factory, out result, setup);
        }

        /// <summary>
        /// Repetitively executes the specified <paramref name="faultSensitiveMethod"/> until the operation is successful, the amount of retry attempts has been reached, or a failed operation is not considered related to transient fault condition.
        /// </summary>
        /// <param name="faultSensitiveMethod">The fault sensitive delegate that is invoked until an operation is successful, the amount of retry attempts has been reached, or a failed operation is not considered related to transient fault condition.</param>
        /// <param name="setup">The <see cref="TransientFaultHandlingOptions"/> which need to be configured.</param>
        public static void WithAction(Act faultSensitiveMethod, Act<TransientFaultHandlingOptions> setup = null)
        {
            var factory = ActFactory.Create(faultSensitiveMethod);
            ExecuteActionCore(factory, setup);
        }

        /// <summary>
        /// Repetitively executes the specified <paramref name="faultSensitiveMethod"/> until the operation is successful, the amount of retry attempts has been reached, or a failed operation is not considered related to transient fault condition.
        /// </summary>
        /// <typeparam name="T">The type of the parameter of the delegate <paramref name="faultSensitiveMethod"/>.</typeparam>
        /// <param name="faultSensitiveMethod">The fault sensitive delegate that is invoked until an operation is successful, the amount of retry attempts has been reached, or a failed operation is not considered related to transient fault condition.</param>
        /// <param name="arg">The parameter of the delegate <paramref name="faultSensitiveMethod"/>.</param>
        /// <param name="setup">The <see cref="TransientFaultHandlingOptions"/> which need to be configured.</param>
        public static void WithAction<T>(Act<T> faultSensitiveMethod, T arg, Act<TransientFaultHandlingOptions> setup = null)
        {
            var factory = ActFactory.Create(faultSensitiveMethod, arg);
            ExecuteActionCore(factory, setup);
        }

        /// <summary>
        /// Repetitively executes the specified <paramref name="faultSensitiveMethod"/> until the operation is successful, the amount of retry attempts has been reached, or a failed operation is not considered related to transient fault condition.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter of the delegate <paramref name="faultSensitiveMethod"/>.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the delegate <paramref name="faultSensitiveMethod"/>.</typeparam>
        /// <param name="faultSensitiveMethod">The fault sensitive delegate that is invoked until an operation is successful, the amount of retry attempts has been reached, or a failed operation is not considered related to transient fault condition.</param>
        /// <param name="arg1">The first parameter of the delegate <paramref name="faultSensitiveMethod"/>.</param>
        /// <param name="arg2">The second parameter of the delegate <paramref name="faultSensitiveMethod"/>.</param>
        /// <param name="setup">The <see cref="TransientFaultHandlingOptions"/> which need to be configured.</param>
        public static void WithAction<T1, T2>(Act<T1, T2> faultSensitiveMethod, T1 arg1, T2 arg2, Act<TransientFaultHandlingOptions> setup = null)
        {
            var factory = ActFactory.Create(faultSensitiveMethod, arg1, arg2);
            ExecuteActionCore(factory, setup);
        }

        /// <summary>
        /// Repetitively executes the specified <paramref name="faultSensitiveMethod"/> until the operation is successful, the amount of retry attempts has been reached, or a failed operation is not considered related to transient fault condition.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter of the delegate <paramref name="faultSensitiveMethod"/>.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the delegate <paramref name="faultSensitiveMethod"/>.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the delegate <paramref name="faultSensitiveMethod"/>.</typeparam>
        /// <param name="faultSensitiveMethod">The fault sensitive delegate that is invoked until an operation is successful, the amount of retry attempts has been reached, or a failed operation is not considered related to transient fault condition.</param>
        /// <param name="arg1">The first parameter of the delegate <paramref name="faultSensitiveMethod"/>.</param>
        /// <param name="arg2">The second parameter of the delegate <paramref name="faultSensitiveMethod"/>.</param>
        /// <param name="arg3">The third parameter of the delegate <paramref name="faultSensitiveMethod"/>.</param>
        /// <param name="setup">The <see cref="TransientFaultHandlingOptions"/> which need to be configured.</param>
        public static void WithAction<T1, T2, T3>(Act<T1, T2, T3> faultSensitiveMethod, T1 arg1, T2 arg2, T3 arg3, Act<TransientFaultHandlingOptions> setup = null)
        {
            var factory = ActFactory.Create(faultSensitiveMethod, arg1, arg2, arg3);
            ExecuteActionCore(factory, setup);
        }

        /// <summary>
        /// Repetitively executes the specified <paramref name="faultSensitiveMethod"/> until the operation is successful, the amount of retry attempts has been reached, or a failed operation is not considered related to transient fault condition.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter of the delegate <paramref name="faultSensitiveMethod"/>.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the delegate <paramref name="faultSensitiveMethod"/>.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the delegate <paramref name="faultSensitiveMethod"/>.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of the delegate <paramref name="faultSensitiveMethod"/>.</typeparam>
        /// <param name="faultSensitiveMethod">The fault sensitive delegate that is invoked until an operation is successful, the amount of retry attempts has been reached, or a failed operation is not considered related to transient fault condition.</param>
        /// <param name="arg1">The first parameter of the delegate <paramref name="faultSensitiveMethod"/>.</param>
        /// <param name="arg2">The second parameter of the delegate <paramref name="faultSensitiveMethod"/>.</param>
        /// <param name="arg3">The third parameter of the delegate <paramref name="faultSensitiveMethod"/>.</param>
        /// <param name="arg4">The fourth parameter of the delegate <paramref name="faultSensitiveMethod"/>.</param>
        /// <param name="setup">The <see cref="TransientFaultHandlingOptions"/> which need to be configured.</param>
        public static void WithAction<T1, T2, T3, T4>(Act<T1, T2, T3, T4> faultSensitiveMethod, T1 arg1, T2 arg2, T3 arg3, T4 arg4, Act<TransientFaultHandlingOptions> setup = null)
        {
            var factory = ActFactory.Create(faultSensitiveMethod, arg1, arg2, arg3, arg4);
            ExecuteActionCore(factory, setup);
        }

        /// <summary>
        /// Repetitively executes the specified <paramref name="faultSensitiveMethod"/> until the operation is successful, the amount of retry attempts has been reached, or a failed operation is not considered related to transient fault condition.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter of the delegate <paramref name="faultSensitiveMethod"/>.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the delegate <paramref name="faultSensitiveMethod"/>.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the delegate <paramref name="faultSensitiveMethod"/>.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of the delegate <paramref name="faultSensitiveMethod"/>.</typeparam>
        /// <typeparam name="T5">The type of the fifth parameter of the delegate <paramref name="faultSensitiveMethod"/>.</typeparam>
        /// <param name="faultSensitiveMethod">The fault sensitive delegate that is invoked until an operation is successful, the amount of retry attempts has been reached, or a failed operation is not considered related to transient fault condition.</param>
        /// <param name="arg1">The first parameter of the delegate <paramref name="faultSensitiveMethod"/>.</param>
        /// <param name="arg2">The second parameter of the delegate <paramref name="faultSensitiveMethod"/>.</param>
        /// <param name="arg3">The third parameter of the delegate <paramref name="faultSensitiveMethod"/>.</param>
        /// <param name="arg4">The fourth parameter of the delegate <paramref name="faultSensitiveMethod"/>.</param>
        /// <param name="arg5">The fifth parameter of the delegate <paramref name="faultSensitiveMethod"/>.</param>
        /// <param name="setup">The <see cref="TransientFaultHandlingOptions"/> which need to be configured.</param>
        public static void WithAction<T1, T2, T3, T4, T5>(Act<T1, T2, T3, T4, T5> faultSensitiveMethod, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, Act<TransientFaultHandlingOptions> setup = null)
        {
            var factory = ActFactory.Create(faultSensitiveMethod, arg1, arg2, arg3, arg4, arg5);
            ExecuteActionCore(factory, setup);
        }

        private static void ExecuteActionCore<TTuple>(ActFactory<TTuple> factory, Act<TransientFaultHandlingOptions> setup) where TTuple : Template
        {
            var options = DelegateUtility.ConfigureAction(setup);
            if (!options.EnableTransientFaultRecovery)
            {
                factory.ExecuteMethod();
                return;
            }
            TimeSpan totalWaitTime = TimeSpan.Zero;
            TimeSpan lastWaitTime = TimeSpan.Zero;
            bool isTransientFault = false;
            bool throwExceptions;
            List<Exception> aggregatedExceptions = new List<Exception>();
            for (int attempts = 0; ;)
            {
                TimeSpan waitTime = options.RecoveryWaitTimeCallback(attempts);
                try
                {
                    factory.ExecuteMethod();
                    return;
                }
                catch (Exception ex)
                {
                    try
                    {
                        lock (aggregatedExceptions) { aggregatedExceptions.Insert(0, ex); }
                        isTransientFault = options.TransientFaultParserCallback(ex);
                        if (attempts >= options.RetryAttempts) { throw; }
                        if (!isTransientFault) { throw; }
                        lastWaitTime = waitTime;
                        totalWaitTime = totalWaitTime.Add(waitTime);
                        attempts++;
                        Thread.Sleep(waitTime);
                    }
                    catch (Exception)
                    {
                        throwExceptions = true;
                        if (isTransientFault) { InsertTransientFaultException(aggregatedExceptions, attempts, options.RetryAttempts, lastWaitTime, totalWaitTime); }
                        break;
                    }
                }
            }
            if (throwExceptions) { throw new ThreadException(aggregatedExceptions); }
        }

        private static TResult ExecuteFunctionCore<TTuple, TResult>(DoerFactory<TTuple, TResult> factory, Act<TransientFaultHandlingOptions> setup) where TTuple : Template
        {
            var options = DelegateUtility.ConfigureAction(setup);
            if (!options.EnableTransientFaultRecovery) { return factory.ExecuteMethod(); }
            TimeSpan totalWaitTime = TimeSpan.Zero;
            TimeSpan lastWaitTime = TimeSpan.Zero;
            bool isTransientFault = false;
            bool throwExceptions;
            List<Exception> aggregatedExceptions = new List<Exception>();
            TResult result = default(TResult);
            for (int attempts = 0; ;)
            {
                bool exceptionThrown = false;
                TimeSpan waitTime = options.RecoveryWaitTimeCallback(attempts);
                try
                {
                    return factory.ExecuteMethod();
                }
                catch (Exception ex)
                {
                    try
                    {
                        lock (aggregatedExceptions) { aggregatedExceptions.Insert(0, ex); }
                        isTransientFault = options.TransientFaultParserCallback(ex);
                        if (attempts >= options.RetryAttempts) { throw; }
                        if (!isTransientFault) { throw; }
                        lastWaitTime = waitTime;
                        totalWaitTime = totalWaitTime.Add(waitTime);
                        attempts++;
                        Thread.Sleep(waitTime);
                    }
                    catch (Exception)
                    {
                        throwExceptions = true;
                        exceptionThrown = true;
                        if (isTransientFault) { InsertTransientFaultException(aggregatedExceptions, attempts, options.RetryAttempts, lastWaitTime, totalWaitTime); }
                        break;
                    }
                }
                finally
                {
                    if (exceptionThrown)
                    {
                        IDisposable disposable = result as IDisposable;
                        disposable?.Dispose();
                    }
                }
            }
            if (throwExceptions) { throw new ThreadException(aggregatedExceptions); }
            return result;
        }

        private static TSuccess TryExecuteFunctionCore<TTuple, TSuccess, TResult>(TesterDoerFactory<TTuple, TResult, TSuccess> factory, out TResult result, Act<TransientFaultHandlingOptions> setup) where TTuple : Template
        {
            result = default(TResult);
            var options = DelegateUtility.ConfigureAction(setup);
            if (!options.EnableTransientFaultRecovery) { return factory.ExecuteMethod(out result); }
            TimeSpan totalWaitTime = TimeSpan.Zero;
            TimeSpan lastWaitTime = TimeSpan.Zero;
            bool throwExceptions;
            bool isTransientFault = false;
            List<Exception> aggregatedExceptions = new List<Exception>();
            for (int attempts = 0; ;)
            {
                bool exceptionThrown = false;
                TimeSpan waitTime = options.RecoveryWaitTimeCallback(attempts);
                try
                {
                    return factory.ExecuteMethod(out result);
                }
                catch (Exception ex)
                {
                    try
                    {
                        lock (aggregatedExceptions) { aggregatedExceptions.Insert(0, ex); }
                        isTransientFault = options.TransientFaultParserCallback(ex);
                        if (attempts >= options.RetryAttempts) { throw; }
                        if (!isTransientFault) { throw; }
                        lastWaitTime = waitTime;
                        totalWaitTime = totalWaitTime.Add(waitTime);
                        attempts++;
                        Thread.Sleep(waitTime);
                    }
                    catch (Exception)
                    {
                        throwExceptions = true;
                        exceptionThrown = true;
                        if (isTransientFault) { InsertTransientFaultException(aggregatedExceptions, attempts, options.RetryAttempts, lastWaitTime, totalWaitTime); }
                        break;
                    }
                }
                finally
                {
                    if (exceptionThrown)
                    {
                        IDisposable disposable = result as IDisposable;
                        disposable?.Dispose();
                    }
                }
            }
            if (throwExceptions) { throw new ThreadException(aggregatedExceptions); }
            return default(TSuccess);
        }

        private static void InsertTransientFaultException(IList<Exception> aggregatedExceptions, int attempts, int retryAttempts, TimeSpan lastWaitTime, TimeSpan totalWaitTime)
        {
            TransientFaultException transientException = new TransientFaultException(attempts >= retryAttempts ? "The amount of retry attempts has been reached." : "An unhandled exception occurred during the execution of the current operation.");
            transientException.Data.Add("Attempts", (attempts).ToString(CultureInfo.InvariantCulture));
            transientException.Data.Add("RecoveryWaitTimeInSeconds", lastWaitTime.TotalSeconds.ToString(CultureInfo.InvariantCulture));
            transientException.Data.Add("TotalRecoveryWaitTimeInSeconds", totalWaitTime.TotalSeconds.ToString(CultureInfo.InvariantCulture));
            lock (aggregatedExceptions) { aggregatedExceptions.Insert(0, transientException); }
        }
    }
}