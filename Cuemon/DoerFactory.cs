﻿using System;
using System.Reflection;

namespace Cuemon
{
    /// <summary>
    /// Provides access to factory methods for creating <see cref="DoerFactory{TTuple,TResult}"/> instances that encapsulate a function delegate with a variable amount of generic arguments.
    /// </summary>
    public static class DoerFactory
    {
        /// <summary>
        /// Creates a new <see cref="DoerFactory{TTuple,TResult}"/> instance encapsulating the specified <paramref name="method"/>.
        /// </summary>
        /// <typeparam name="TResult">The type of the return value of the function delegate <paramref name="method"/>.</typeparam>
        /// <param name="method">The function delegate to invoke.</param>
        /// <returns>An instance of <see cref="DoerFactory{TTuple,TResult}"/> object initialized with the specified <paramref name="method"/>.</returns>
        public static DoerFactory<Template, TResult> Create<TResult>(Doer<TResult> method)
        {
            return new DoerFactory<Template, TResult>(tuple => method(), TupleUtility.CreateZero(), method);
        }

        /// <summary>
        /// Creates a new <see cref="DoerFactory{TTuple,TResult}"/> instance encapsulating the specified <paramref name="method"/> and one generic argument.
        /// </summary>
        /// <typeparam name="T">The type of the parameter of the function delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="TResult">The type of the return value of the function delegate <paramref name="method"/>.</typeparam>
        /// <param name="method">The function delegate to invoke.</param>
        /// <param name="arg">The parameter of the function delegate <paramref name="method"/>.</param>
        /// <returns>An instance of <see cref="DoerFactory{TTuple,TResult}"/> object initialized with the specified <paramref name="method"/> and one generic argument.</returns>
        public static DoerFactory<Template<T>, TResult> Create<T, TResult>(Doer<T, TResult> method, T arg)
        {
            return new DoerFactory<Template<T>, TResult>(tuple => method(tuple.Arg1), TupleUtility.CreateOne(arg), method);
        }

        /// <summary>
        /// Creates a new <see cref="DoerFactory{TTuple,TResult}"/> instance encapsulating the specified <paramref name="method"/> and two generic arguments.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter of the function delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the function delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="TResult">The type of the return value of the function delegate <paramref name="method"/>.</typeparam>
        /// <param name="method">The function delegate to invoke.</param>
        /// <param name="arg1">The first parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg2">The second parameter of the function delegate <paramref name="method"/>.</param>
        /// <returns>An instance of <see cref="DoerFactory{TTuple,TResult}"/> object initialized with the specified <paramref name="method"/> and two generic arguments.</returns>
        public static DoerFactory<Template<T1, T2>, TResult> Create<T1, T2, TResult>(Doer<T1, T2, TResult> method, T1 arg1, T2 arg2)
        {
            return new DoerFactory<Template<T1, T2>, TResult>(tuple => method(tuple.Arg1, tuple.Arg2), TupleUtility.CreateTwo(arg1, arg2), method);
        }

        /// <summary>
        /// Creates a new <see cref="DoerFactory{TTuple,TResult}"/> instance encapsulating the specified <paramref name="method"/> and three generic arguments.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter of the function delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the function delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the function delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="TResult">The type of the return value of the function delegate <paramref name="method"/>.</typeparam>
        /// <param name="method">The function delegate to invoke.</param>
        /// <param name="arg1">The first parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg2">The second parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg3">The third parameter of the function delegate <paramref name="method"/>.</param>
        /// <returns>An instance of <see cref="DoerFactory{TTuple,TResult}"/> object initialized with the specified <paramref name="method"/> and three generic arguments.</returns>
        public static DoerFactory<Template<T1, T2, T3>, TResult> Create<T1, T2, T3, TResult>(Doer<T1, T2, T3, TResult> method, T1 arg1, T2 arg2, T3 arg3)
        {
            return new DoerFactory<Template<T1, T2, T3>, TResult>(tuple => method(tuple.Arg1, tuple.Arg2, tuple.Arg3), TupleUtility.CreateThree(arg1, arg2, arg3), method);
        }

        /// <summary>
        /// Creates a new <see cref="DoerFactory{TTuple,TResult}"/> instance encapsulating the specified <paramref name="method"/> and four generic arguments.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter of the function delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the function delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the function delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of the function delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="TResult">The type of the return value of the function delegate <paramref name="method"/>.</typeparam>
        /// <param name="method">The function delegate to invoke.</param>
        /// <param name="arg1">The first parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg2">The second parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg3">The third parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg4">The fourth parameter of the function delegate <paramref name="method"/>.</param>
        /// <returns>An instance of <see cref="DoerFactory{TTuple,TResult}"/> object initialized with the specified <paramref name="method"/> and four generic arguments.</returns>
        public static DoerFactory<Template<T1, T2, T3, T4>, TResult> Create<T1, T2, T3, T4, TResult>(Doer<T1, T2, T3, T4, TResult> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            return new DoerFactory<Template<T1, T2, T3, T4>, TResult>(tuple => method(tuple.Arg1, tuple.Arg2, tuple.Arg3, tuple.Arg4), TupleUtility.CreateFour(arg1, arg2, arg3, arg4), method);
        }

        /// <summary>
        /// Creates a new <see cref="DoerFactory{TTuple,TResult}"/> instance encapsulating the specified <paramref name="method"/> and five generic arguments.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter of the function delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the function delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the function delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of the function delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T5">The type of the fifth parameter of the function delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="TResult">The type of the return value of the function delegate <paramref name="method"/>.</typeparam>
        /// <param name="method">The function delegate to invoke.</param>
        /// <param name="arg1">The first parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg2">The second parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg3">The third parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg4">The fourth parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg5">The fifth parameter of the function delegate <paramref name="method"/>.</param>
        /// <returns>An instance of <see cref="DoerFactory{TTuple,TResult}"/> object initialized with the specified <paramref name="method"/> and five generic arguments.</returns>
        public static DoerFactory<Template<T1, T2, T3, T4, T5>, TResult> Create<T1, T2, T3, T4, T5, TResult>(Doer<T1, T2, T3, T4, T5, TResult> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        {
            return new DoerFactory<Template<T1, T2, T3, T4, T5>, TResult>(tuple => method(tuple.Arg1, tuple.Arg2, tuple.Arg3, tuple.Arg4, tuple.Arg5), TupleUtility.CreateFive(arg1, arg2, arg3, arg4, arg5), method);
        }

        /// <summary>
        /// Creates a new <see cref="DoerFactory{TTuple,TResult}"/> instance encapsulating the specified <paramref name="method"/> and six generic arguments.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter of the function delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the function delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the function delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of the function delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T5">The type of the fifth parameter of the function delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T6">The type of the sixth parameter of the function delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="TResult">The type of the return value of the function delegate <paramref name="method"/>.</typeparam>
        /// <param name="method">The function delegate to invoke.</param>
        /// <param name="arg1">The first parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg2">The second parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg3">The third parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg4">The fourth parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg5">The fifth parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg6">The sixth parameter of the function delegate <paramref name="method"/>.</param>
        /// <returns>An instance of <see cref="DoerFactory{TTuple,TResult}"/> object initialized with the specified <paramref name="method"/> and six generic arguments.</returns>
        public static DoerFactory<Template<T1, T2, T3, T4, T5, T6>, TResult> Create<T1, T2, T3, T4, T5, T6, TResult>(Doer<T1, T2, T3, T4, T5, T6, TResult> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6)
        {
            return new DoerFactory<Template<T1, T2, T3, T4, T5, T6>, TResult>(tuple => method(tuple.Arg1, tuple.Arg2, tuple.Arg3, tuple.Arg4, tuple.Arg5, tuple.Arg6), TupleUtility.CreateSix(arg1, arg2, arg3, arg4, arg5, arg6), method);
        }

        /// <summary>
        /// Creates a new <see cref="DoerFactory{TTuple,TResult}"/> instance encapsulating the specified <paramref name="method"/> and seven generic arguments.
        /// </summary>
        /// <typeparam name="T1">The type of the first parameter of the function delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the function delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the function delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of the function delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T5">The type of the fifth parameter of the function delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T6">The type of the sixth parameter of the function delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="T7">The type of the seventh parameter of the function delegate <paramref name="method"/>.</typeparam>
        /// <typeparam name="TResult">The type of the return value of the function delegate <paramref name="method"/>.</typeparam>
        /// <param name="method">The function delegate to invoke.</param>
        /// <param name="arg1">The first parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg2">The second parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg3">The third parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg4">The fourth parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg5">The fifth parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg6">The sixth parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg7">The seventh parameter of the function delegate <paramref name="method"/>.</param>
        /// <returns>An instance of <see cref="DoerFactory{TTuple,TResult}"/> object initialized with the specified <paramref name="method"/> and seven generic arguments.</returns>
        public static DoerFactory<Template<T1, T2, T3, T4, T5, T6, T7>, TResult> Create<T1, T2, T3, T4, T5, T6, T7, TResult>(Doer<T1, T2, T3, T4, T5, T6, T7, TResult> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7)
        {
            return new DoerFactory<Template<T1, T2, T3, T4, T5, T6, T7>, TResult>(tuple => method(tuple.Arg1, tuple.Arg2, tuple.Arg3, tuple.Arg4, tuple.Arg5, tuple.Arg6, tuple.Arg7), TupleUtility.CreateSeven(arg1, arg2, arg3, arg4, arg5, arg6, arg7), method);
        }

        /// <summary>
        /// Creates a new <see cref="DoerFactory{TTuple,TResult}"/> instance encapsulating the specified <paramref name="method"/> and eight generic arguments.
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
        /// <param name="method">The function delegate to invoke.</param>
        /// <param name="arg1">The first parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg2">The second parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg3">The third parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg4">The fourth parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg5">The fifth parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg6">The sixth parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg7">The seventh parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg8">The eighth parameter of the function delegate <paramref name="method"/>.</param>
        /// <returns>An instance of <see cref="DoerFactory{TTuple,TResult}"/> object initialized with the specified <paramref name="method"/> and eight generic arguments.</returns>
        public static DoerFactory<Template<T1, T2, T3, T4, T5, T6, T7, T8>, TResult> Create<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(Doer<T1, T2, T3, T4, T5, T6, T7, T8, TResult> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8)
        {
            return new DoerFactory<Template<T1, T2, T3, T4, T5, T6, T7, T8>, TResult>(tuple => method(tuple.Arg1, tuple.Arg2, tuple.Arg3, tuple.Arg4, tuple.Arg5, tuple.Arg6, tuple.Arg7, tuple.Arg8), TupleUtility.CreateEight(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8), method);
        }

        /// <summary>
        /// Creates a new <see cref="DoerFactory{TTuple,TResult}"/> instance encapsulating the specified <paramref name="method"/> and nine generic arguments.
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
        /// <param name="method">The function delegate to invoke.</param>
        /// <param name="arg1">The first parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg2">The second parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg3">The third parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg4">The fourth parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg5">The fifth parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg6">The sixth parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg7">The seventh parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg8">The eighth parameter of the function delegate <paramref name="method"/>.</param>
        /// <param name="arg9">The ninth parameter of the function delegate <paramref name="method"/>.</param>
        /// <returns>An instance of <see cref="DoerFactory{TTuple,TResult}"/> object initialized with the specified <paramref name="method"/> and nine generic arguments.</returns>
        public static DoerFactory<Template<T1, T2, T3, T4, T5, T6, T7, T8, T9>, TResult> Create<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>(Doer<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9)
        {
            return new DoerFactory<Template<T1, T2, T3, T4, T5, T6, T7, T8, T9>, TResult>(tuple => method(tuple.Arg1, tuple.Arg2, tuple.Arg3, tuple.Arg4, tuple.Arg5, tuple.Arg6, tuple.Arg7, tuple.Arg8, tuple.Arg9), TupleUtility.CreateNine(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9), method);
        }

        /// <summary>
        /// Creates a new <see cref="DoerFactory{TTuple,TResult}"/> instance encapsulating the specified <paramref name="method"/> and ten generic arguments.
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
        /// <param name="method">The function delegate to invoke.</param>
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
        /// <returns>An instance of <see cref="DoerFactory{TTuple,TResult}"/> object initialized with the specified <paramref name="method"/> and ten generic arguments.</returns>
        public static DoerFactory<Template<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>, TResult> Create<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>(Doer<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10)
        {
            return new DoerFactory<Template<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>, TResult>(tuple => method(tuple.Arg1, tuple.Arg2, tuple.Arg3, tuple.Arg4, tuple.Arg5, tuple.Arg6, tuple.Arg7, tuple.Arg8, tuple.Arg9, tuple.Arg10), TupleUtility.CreateTen(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10), method);
        }

        /// <summary>
        /// Creates a new <see cref="DoerFactory{TTuple,TResult}"/> instance encapsulating the specified <paramref name="method"/> and eleven generic arguments.
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
        /// <param name="method">The function delegate to invoke.</param>
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
        /// <returns>An instance of <see cref="DoerFactory{TTuple,TResult}"/> object initialized with the specified <paramref name="method"/> and eleven generic arguments.</returns>
        public static DoerFactory<Template<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>, TResult> Create<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult>(Doer<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11)
        {
            return new DoerFactory<Template<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>, TResult>(tuple => method(tuple.Arg1, tuple.Arg2, tuple.Arg3, tuple.Arg4, tuple.Arg5, tuple.Arg6, tuple.Arg7, tuple.Arg8, tuple.Arg9, tuple.Arg10, tuple.Arg11), TupleUtility.CreateEleven(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11), method);
        }

        /// <summary>
        /// Creates a new <see cref="DoerFactory{TTuple,TResult}"/> instance encapsulating the specified <paramref name="method"/> and twelfth generic arguments.
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
        /// <param name="method">The function delegate to invoke.</param>
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
        /// <returns>An instance of <see cref="DoerFactory{TTuple,TResult}"/> object initialized with the specified <paramref name="method"/> and twelfth generic arguments.</returns>
        public static DoerFactory<Template<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>, TResult> Create<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult>(Doer<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12)
        {
            return new DoerFactory<Template<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>, TResult>(tuple => method(tuple.Arg1, tuple.Arg2, tuple.Arg3, tuple.Arg4, tuple.Arg5, tuple.Arg6, tuple.Arg7, tuple.Arg8, tuple.Arg9, tuple.Arg10, tuple.Arg11, tuple.Arg12), TupleUtility.CreateTwelve(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12), method);
        }

        /// <summary>
        /// Creates a new <see cref="DoerFactory{TTuple,TResult}"/> instance encapsulating the specified <paramref name="method"/> and thirteen generic arguments.
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
        /// <param name="method">The function delegate to invoke.</param>
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
        /// <returns>An instance of <see cref="DoerFactory{TTuple,TResult}"/> object initialized with the specified <paramref name="method"/> and thirteen generic arguments.</returns>
        public static DoerFactory<Template<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>, TResult> Create<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>(Doer<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13)
        {
            return new DoerFactory<Template<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>, TResult>(tuple => method(tuple.Arg1, tuple.Arg2, tuple.Arg3, tuple.Arg4, tuple.Arg5, tuple.Arg6, tuple.Arg7, tuple.Arg8, tuple.Arg9, tuple.Arg10, tuple.Arg11, tuple.Arg12, tuple.Arg13), TupleUtility.CreateThirteen(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13), method);
        }

        /// <summary>
        /// Creates a new <see cref="DoerFactory{TTuple,TResult}"/> instance encapsulating the specified <paramref name="method"/> and fourteen generic arguments.
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
        /// <param name="method">The function delegate to invoke.</param>
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
        /// <returns>An instance of <see cref="DoerFactory{TTuple,TResult}"/> object initialized with the specified <paramref name="method"/> and fourteen generic arguments.</returns>
        public static DoerFactory<Template<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>, TResult> Create<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult>(Doer<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14)
        {
            return new DoerFactory<Template<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>, TResult>(tuple => method(tuple.Arg1, tuple.Arg2, tuple.Arg3, tuple.Arg4, tuple.Arg5, tuple.Arg6, tuple.Arg7, tuple.Arg8, tuple.Arg9, tuple.Arg10, tuple.Arg11, tuple.Arg12, tuple.Arg13, tuple.Arg14), TupleUtility.CreateFourteen(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14), method);
        }

        /// <summary>
        /// Creates a new <see cref="DoerFactory{TTuple,TResult}"/> instance encapsulating the specified <paramref name="method"/> and fifteen generic arguments.
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
        /// <param name="method">The function delegate to invoke.</param>
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
        /// <returns>An instance of <see cref="DoerFactory{TTuple,TResult}"/> object initialized with the specified <paramref name="method"/> and fifteen generic arguments.</returns>
        public static DoerFactory<Template<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>, TResult> Create<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult>(Doer<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15)
        {
            return new DoerFactory<Template<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>, TResult>(tuple => method(tuple.Arg1, tuple.Arg2, tuple.Arg3, tuple.Arg4, tuple.Arg5, tuple.Arg6, tuple.Arg7, tuple.Arg8, tuple.Arg9, tuple.Arg10, tuple.Arg11, tuple.Arg12, tuple.Arg13, tuple.Arg14, tuple.Arg15), TupleUtility.CreateFifteen(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15), method);
        }

        /// <summary>
        /// Creates a new <see cref="DoerFactory{TTuple,TResult}"/> instance encapsulating the specified <paramref name="method"/> and sixteen generic arguments.
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
        /// <param name="method">The function delegate to invoke.</param>
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
        /// <returns>An instance of <see cref="DoerFactory{TTuple,TResult}"/> object initialized with the specified <paramref name="method"/> and sixteen generic arguments.</returns>
        public static DoerFactory<Template<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>, TResult> Create<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult>(Doer<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, T16 arg16)
        {
            return new DoerFactory<Template<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>, TResult>(tuple => method(tuple.Arg1, tuple.Arg2, tuple.Arg3, tuple.Arg4, tuple.Arg5, tuple.Arg6, tuple.Arg7, tuple.Arg8, tuple.Arg9, tuple.Arg10, tuple.Arg11, tuple.Arg12, tuple.Arg13, tuple.Arg14, tuple.Arg15, tuple.Arg16), TupleUtility.CreateSixteen(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15, arg16), method);
        }

        /// <summary>
        /// Creates a new <see cref="DoerFactory{TTuple,TResult}"/> instance encapsulating the specified <paramref name="method"/> and seventeen generic arguments.
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
        /// <param name="method">The function delegate to invoke.</param>
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
        /// <returns>An instance of <see cref="DoerFactory{TTuple,TResult}"/> object initialized with the specified <paramref name="method"/> and seventeen generic arguments.</returns>
        public static DoerFactory<Template<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17>, TResult> Create<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, TResult>(Doer<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, TResult> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, T16 arg16, T17 arg17)
        {
            return new DoerFactory<Template<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17>, TResult>(tuple => method(tuple.Arg1, tuple.Arg2, tuple.Arg3, tuple.Arg4, tuple.Arg5, tuple.Arg6, tuple.Arg7, tuple.Arg8, tuple.Arg9, tuple.Arg10, tuple.Arg11, tuple.Arg12, tuple.Arg13, tuple.Arg14, tuple.Arg15, tuple.Arg16, tuple.Arg17), TupleUtility.CreateSeventeen(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15, arg16, arg17), method);
        }

        /// <summary>
        /// Creates a new <see cref="DoerFactory{TTuple,TResult}"/> instance encapsulating the specified <paramref name="method"/> and eighteen generic arguments.
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
        /// <param name="method">The function delegate to invoke.</param>
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
        /// <returns>An instance of <see cref="DoerFactory{TTuple,TResult}"/> object initialized with the specified <paramref name="method"/> and eighteen generic arguments.</returns>
        public static DoerFactory<Template<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18>, TResult> Create<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, TResult>(Doer<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, TResult> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, T16 arg16, T17 arg17, T18 arg18)
        {
            return new DoerFactory<Template<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18>, TResult>(tuple => method(tuple.Arg1, tuple.Arg2, tuple.Arg3, tuple.Arg4, tuple.Arg5, tuple.Arg6, tuple.Arg7, tuple.Arg8, tuple.Arg9, tuple.Arg10, tuple.Arg11, tuple.Arg12, tuple.Arg13, tuple.Arg14, tuple.Arg15, tuple.Arg16, tuple.Arg17, tuple.Arg18), TupleUtility.CreateEighteen(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15, arg16, arg17, arg18), method);
        }

        /// <summary>
        /// Creates a new <see cref="DoerFactory{TTuple,TResult}"/> instance encapsulating the specified <paramref name="method"/> and nineteen generic arguments.
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
        /// <param name="method">The function delegate to invoke.</param>
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
        /// <returns>An instance of <see cref="DoerFactory{TTuple,TResult}"/> object initialized with the specified <paramref name="method"/> and nineteen generic arguments.</returns>
        public static DoerFactory<Template<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19>, TResult> Create<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, TResult>(Doer<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, TResult> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, T16 arg16, T17 arg17, T18 arg18, T19 arg19)
        {
            return new DoerFactory<Template<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19>, TResult>(tuple => method(tuple.Arg1, tuple.Arg2, tuple.Arg3, tuple.Arg4, tuple.Arg5, tuple.Arg6, tuple.Arg7, tuple.Arg8, tuple.Arg9, tuple.Arg10, tuple.Arg11, tuple.Arg12, tuple.Arg13, tuple.Arg14, tuple.Arg15, tuple.Arg16, tuple.Arg17, tuple.Arg18, tuple.Arg19), TupleUtility.CreateNineteen(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15, arg16, arg17, arg18, arg19), method);
        }

        /// <summary>
        /// Creates a new <see cref="DoerFactory{TTuple,TResult}"/> instance encapsulating the specified <paramref name="method"/> and twenty generic arguments.
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
        /// <param name="method">The function delegate to invoke.</param>
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
        /// <returns>An instance of <see cref="DoerFactory{TTuple,TResult}"/> object initialized with the specified <paramref name="method"/> and twenty generic arguments.</returns>
        public static DoerFactory<Template<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20>, TResult> Create<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, TResult>(Doer<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, TResult> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, T16 arg16, T17 arg17, T18 arg18, T19 arg19, T20 arg20)
        {
            return new DoerFactory<Template<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20>, TResult>(tuple => method(tuple.Arg1, tuple.Arg2, tuple.Arg3, tuple.Arg4, tuple.Arg5, tuple.Arg6, tuple.Arg7, tuple.Arg8, tuple.Arg9, tuple.Arg10, tuple.Arg11, tuple.Arg12, tuple.Arg13, tuple.Arg14, tuple.Arg15, tuple.Arg16, tuple.Arg17, tuple.Arg18, tuple.Arg19, tuple.Arg20), TupleUtility.CreateTwenty(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15, arg16, arg17, arg18, arg19, arg20), method);
        }

        /// <summary>
        /// Invokes the specified delegate <paramref name="method" /> with a n-<paramref name="tuple" /> argument.
        /// </summary>
        /// <typeparam name="TTuple">The type of the n-tuple representation of a <see cref="Template" />.</typeparam>
        /// <typeparam name="TResult">The type of the return value of the function delegate <paramref name="method" />.</typeparam>
        /// <param name="method">The function delegate to invoke.</param>
        /// <param name="tuple">The n-tuple argument of <paramref name="method" />.</param>
        /// <returns>The result of the the function delegate <paramref name="method"/>.</returns>
        public static TResult Invoke<TTuple, TResult>(Doer<TTuple, TResult> method, TTuple tuple) where TTuple : Template
        {
            DoerFactory<TTuple, TResult> factory = new DoerFactory<TTuple, TResult>(method, tuple);
            return factory.ExecuteMethod();
        }
    }

    /// <summary>
    /// Provides an easy way of invoking an <see cref="Doer{TResult}" /> function delegate regardless of the amount of parameters provided.
    /// </summary>
    /// <typeparam name="TTuple">The type of the n-tuple representation of a <see cref="Template"/>.</typeparam>
    /// <typeparam name="TResult">The type of the return value of the function delegate <see cref="Method"/>.</typeparam>
    public class DoerFactory<TTuple, TResult> : TemplateFactory<TTuple> where TTuple : Template
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DoerFactory{TTuple,TResult}"/> class.
        /// </summary>
        /// <param name="method">The function delegate to invoke.</param>
        /// <param name="tuple">The n-tuple argument of <paramref name="method"/>.</param>
        public DoerFactory(Doer<TTuple, TResult> method, TTuple tuple) : this(method, tuple, method)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DoerFactory{TTuple, TResult}"/> class.
        /// </summary>
        /// <param name="method">The function delegate to invoke.</param>
        /// <param name="tuple">The n-tuple argument of <paramref name="method"/>.</param>
        /// <param name="originalDelegate">The original delegate wrapped by <paramref name="method"/>.</param>
        internal DoerFactory(Doer<TTuple, TResult> method, TTuple tuple, Delegate originalDelegate) : base(tuple, originalDelegate != null)
        {
            Method = method;
            DelegateInfo = Infrastructure.ResolveDelegateInfo(method == null ? null : method, originalDelegate);
        }

        /// <summary>
        /// Gets the function delegate to invoke.
        /// </summary>
        /// <value>The function delegate to invoke.</value>
        private Doer<TTuple, TResult> Method { get; set; }

        /// <summary>
        /// Gets a value indicating whether this instance has an assigned function delegate.
        /// </summary>
        /// <value><c>true</c> if this instance an assigned function delegate; otherwise, <c>false</c>.</value>
        public override bool HasDelegate { get { return base.HasDelegate; } }

        /// <summary>
        /// Gets the method represented by the function delegate.
        /// </summary>
        /// <value>A <see cref="MethodInfo" /> describing the method represented by the function delegate.</value>
        public override MethodInfo DelegateInfo
        {
            get { return base.DelegateInfo; }
        }

        /// <summary>
        /// Executes the function delegate associated with this instance.
        /// </summary>
        /// <returns>The result of the the function delegate associated with this instance.</returns>
        public TResult ExecuteMethod()
        {
            ThrowIfNoValidDelegate(Condition.IsNull(Method));
            return Method(GenericArguments);
        }

        /// <summary>
        /// Executes the specified delegate asynchronous operation with the specified arguments.
        /// </summary>
        /// <param name="callback">The method to be called when the operation has completed.</param>
        /// <param name="state">An optional object that contains information about the asynchronous operation.</param>
        public void BeginExecuteMethod(AsyncCallback callback, object state)
        {
            ThrowIfNoValidDelegate(Condition.IsNull(Method));
            Method.BeginInvoke(GenericArguments, callback, state);
        }

        /// <summary>
        /// Retrieves the return value of the asynchronous operation represented by the <see cref="IAsyncResult"/> passed.
        /// </summary>
        /// <param name="result">The <see cref="IAsyncResult"/> that represents a specific invoke asynchronous operation, returned when calling <see cref="BeginExecuteMethod"/>.</param>
        /// <returns>The <typeparamref name="TResult"/> generated by the asynchronous operation.</returns>
        public TResult EndExecuteMethod(IAsyncResult result)
        {
            ThrowIfNoValidDelegate(Condition.IsNull(Method));
            return Method.EndInvoke(result);
        }

        /// <summary>
        /// Creates a shallow copy of the current <see cref="DoerFactory{TTuple,TResult}"/> object.
        /// </summary>
        /// <returns>A new <see cref="DoerFactory{TTuple,TResult}"/> that is a copy of this instance.</returns>
        /// <remarks>When thread safety is required this is the method to invoke.</remarks>
        public DoerFactory<TTuple, TResult> Clone()
        {
            return new DoerFactory<TTuple, TResult>(Method, GenericArguments.Clone() as TTuple);
        }
    }
}