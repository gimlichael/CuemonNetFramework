using System;
using System.Collections.Generic;
using Cuemon.Collections.Generic;

namespace Cuemon.Threading
{
    /// <summary>
    /// Provides support for a generic way of specifying parallel loops while providing ways to encapsulate and re-use existing code.
    /// </summary>
    public static partial class ParallelUtility
    {
        private static TimeSpan _defaultTimeout = TimeSpan.FromSeconds(30);

        /// <summary>
        /// Gets or sets a <see cref="TimeSpan"/> that represents the default amount of time to wait for a parallel process to complete.
        /// </summary>
        /// <value>A <see cref="TimeSpan"/> that represents the default amount of time to wait for a parallel process to complete. Default is 30 seconds.</value>
        public static TimeSpan DefaultTimeout
        {
            get { return _defaultTimeout; }
            set { _defaultTimeout = value; }
        }

        /// <summary>
        /// Provides a generic way of executing a parallel for-loop while providing ways to encapsulate and re-use existing code.
        /// </summary>
        /// <typeparam name="TSource">The type of the data in the <paramref name="initial"/>.</typeparam>
        /// <param name="body">The delegate that is invoked once per iteration.</param>
        /// <param name="initial">The initial value of the <c>counter</c> in the parallel for-loop.</param>
        /// <param name="repeats">The amount of repeats to do.</param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="body"/> is null.
        /// </exception>
        public static void For<TSource>(TSource initial, TSource repeats, Act<TSource> body) where TSource : struct, IComparable<TSource>, IEquatable<TSource>, IConvertible
        {
            For(DefaultTimeout, initial, repeats, body);
        }

        /// <summary>
        /// Provides a generic way of executing a parallel for-loop while providing ways to encapsulate and re-use existing code.
        /// </summary>
        /// <typeparam name="TSource">The type of the data in the <paramref name="initial"/>.</typeparam>
        /// <typeparam name="T">The type of the parameter of the delegate <paramref name="body"/>.</typeparam>
        /// <param name="body">The delegate that is invoked once per iteration.</param>
        /// <param name="initial">The initial value of the <c>counter</c> in the parallel for-loop.</param>
        /// <param name="repeats">The amount of repeats to do.</param>
        /// <param name="arg">The parameter of the delegate <paramref name="body"/>.</param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="body"/> is null.
        /// </exception>
        public static void For<TSource, T>(TSource initial, TSource repeats, Act<TSource, T> body, T arg) where TSource : struct, IComparable<TSource>, IEquatable<TSource>, IConvertible
        {
            For(DefaultTimeout, initial, repeats, body, arg);
        }

        /// <summary>
        /// Provides a generic way of executing a parallel for-loop while providing ways to encapsulate and re-use existing code.
        /// </summary>
        /// <typeparam name="TSource">The type of the data in the <paramref name="initial"/>.</typeparam>
        /// <typeparam name="T1">The type of the first parameter of the delegate <paramref name="body"/>.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the delegate <paramref name="body"/>.</typeparam>
        /// <param name="body">The delegate that is invoked once per iteration.</param>
        /// <param name="initial">The initial value of the <c>counter</c> in the parallel for-loop.</param>
        /// <param name="repeats">The amount of repeats to do.</param>
        /// <param name="arg1">The first parameter of the delegate <paramref name="body"/>.</param>
        /// <param name="arg2">The second parameter of the delegate <paramref name="body"/>.</param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="body"/> is null.
        /// </exception>
        public static void For<TSource, T1, T2>(TSource initial, TSource repeats, Act<TSource, T1, T2> body, T1 arg1, T2 arg2) where TSource : struct, IComparable<TSource>, IEquatable<TSource>, IConvertible
        {
            For(DefaultTimeout, initial, repeats, body, arg1, arg2);
        }

        /// <summary>
        /// Provides a generic way of executing a parallel for-loop while providing ways to encapsulate and re-use existing code.
        /// </summary>
        /// <typeparam name="TSource">The type of the data in the <paramref name="initial"/>.</typeparam>
        /// <typeparam name="T1">The type of the first parameter of the delegate <paramref name="body"/>.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the delegate <paramref name="body"/>.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the delegate <paramref name="body"/>.</typeparam>
        /// <param name="body">The delegate that is invoked once per iteration.</param>
        /// <param name="initial">The initial value of the <c>counter</c> in the parallel for-loop.</param>
        /// <param name="repeats">The amount of repeats to do.</param>
        /// <param name="arg1">The first parameter of the delegate <paramref name="body"/>.</param>
        /// <param name="arg2">The second parameter of the delegate <paramref name="body"/>.</param>
        /// <param name="arg3">The third parameter of the delegate <paramref name="body"/>.</param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="body"/> is null.
        /// </exception>
        public static void For<TSource, T1, T2, T3>(TSource initial, TSource repeats, Act<TSource, T1, T2, T3> body, T1 arg1, T2 arg2, T3 arg3) where TSource : struct, IComparable<TSource>, IEquatable<TSource>, IConvertible
        {
            For(DefaultTimeout, initial, repeats, body, arg1, arg2, arg3);
        }

        /// <summary>
        /// Provides a generic way of executing a parallel for-loop while providing ways to encapsulate and re-use existing code.
        /// </summary>
        /// <typeparam name="TSource">The type of the data in the <paramref name="initial"/>.</typeparam>
        /// <typeparam name="T1">The type of the first parameter of the delegate <paramref name="body"/>.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the delegate <paramref name="body"/>.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the delegate <paramref name="body"/>.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of the delegate <paramref name="body"/>.</typeparam>
        /// <param name="body">The delegate that is invoked once per iteration.</param>
        /// <param name="initial">The initial value of the <c>counter</c> in the parallel for-loop.</param>
        /// <param name="repeats">The amount of repeats to do.</param>
        /// <param name="arg1">The first parameter of the delegate <paramref name="body"/>.</param>
        /// <param name="arg2">The second parameter of the delegate <paramref name="body"/>.</param>
        /// <param name="arg3">The third parameter of the delegate <paramref name="body"/>.</param>
        /// <param name="arg4">The fourth parameter of the delegate <paramref name="body"/>.</param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="body"/> is null.
        /// </exception>
        public static void For<TSource, T1, T2, T3, T4>(TSource initial, TSource repeats, Act<TSource, T1, T2, T3, T4> body, T1 arg1, T2 arg2, T3 arg3, T4 arg4) where TSource : struct, IComparable<TSource>, IEquatable<TSource>, IConvertible
        {
            For(DefaultTimeout, initial, repeats, body, arg1, arg2, arg3, arg4);
        }

        /// <summary>
        /// Provides a generic way of executing a parallel for-loop while providing ways to encapsulate and re-use existing code.
        /// </summary>
        /// <typeparam name="TSource">The type of the data in the <paramref name="initial"/>.</typeparam>
        /// <typeparam name="T1">The type of the first parameter of the delegate <paramref name="body"/>.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the delegate <paramref name="body"/>.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the delegate <paramref name="body"/>.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of the delegate <paramref name="body"/>.</typeparam>
        /// <typeparam name="T5">The type of the fifth parameter of the delegate <paramref name="body"/>.</typeparam>
        /// <param name="body">The delegate that is invoked once per iteration.</param>
        /// <param name="initial">The initial value of the <c>counter</c> in the parallel for-loop.</param>
        /// <param name="repeats">The amount of repeats to do.</param>
        /// <param name="arg1">The first parameter of the delegate <paramref name="body"/>.</param>
        /// <param name="arg2">The second parameter of the delegate <paramref name="body"/>.</param>
        /// <param name="arg3">The third parameter of the delegate <paramref name="body"/>.</param>
        /// <param name="arg4">The fourth parameter of the delegate <paramref name="body"/>.</param>
        /// <param name="arg5">The fifth parameter of the delegate <paramref name="body"/>.</param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="body"/> is null.
        /// </exception>
        public static void For<TSource, T1, T2, T3, T4, T5>(TSource initial, TSource repeats, Act<TSource, T1, T2, T3, T4, T5> body, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5) where TSource : struct, IComparable<TSource>, IEquatable<TSource>, IConvertible
        {
            For(DefaultTimeout, initial, repeats, body, arg1, arg2, arg3, arg4, arg5);
        }

        /// <summary>
        /// Provides a generic way of executing a parallel for-loop while providing ways to encapsulate and re-use existing code.
        /// </summary>
        /// <typeparam name="TSource">The type of the data in the <paramref name="initial"/>.</typeparam>
        /// <param name="timeout">A <see cref="TimeSpan" /> that represents the time to wait for the parallel for-loop operation to complete.</param>
        /// <param name="initial">The initial value of the <c>counter</c> in the parallel for-loop.</param>
        /// <param name="repeats">The amount of repeats to do.</param>
        /// <param name="body">The delegate that is invoked once per iteration.</param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="body"/> is null.
        /// </exception>
        public static void For<TSource>(TimeSpan timeout, TSource initial, TSource repeats, Act<TSource> body) where TSource : struct, IComparable<TSource>, IEquatable<TSource>, IConvertible
        {
            if (body == null) { throw new ArgumentNullException("body"); }
            AssignmentUtility.ValidAsNumericOperand<TSource>();
            ActFactory<TSource> factory = new ActFactory<TSource>(body, default(TSource));
            ForCore(factory, LoopUtility.Condition, initial, RelationalOperator.LessThan, repeats, LoopUtility.Iterator, AssignmentOperator.Addition, (TSource)ConvertUtility.ChangeType(1, typeof(TSource)), timeout);
        }

        /// <summary>
        /// Provides a generic way of executing a parallel for-loop while providing ways to encapsulate and re-use existing code.
        /// </summary>
        /// <typeparam name="TSource">The type of the data in the <paramref name="initial"/>.</typeparam>
        /// <typeparam name="T">The type of the parameter of the delegate <paramref name="body"/>.</typeparam>
        /// <param name="timeout">A <see cref="TimeSpan" /> that represents the time to wait for the parallel for-loop operation to complete.</param>
        /// <param name="initial">The initial value of the <c>counter</c> in the parallel for-loop.</param>
        /// <param name="repeats">The amount of repeats to do.</param>
        /// <param name="body">The delegate that is invoked once per iteration.</param>
        /// <param name="arg">The parameter of the delegate <paramref name="body"/>.</param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="body"/> is null.
        /// </exception>
        public static void For<TSource, T>(TimeSpan timeout, TSource initial, TSource repeats, Act<TSource, T> body, T arg) where TSource : struct, IComparable<TSource>, IEquatable<TSource>, IConvertible
        {
            if (body == null) { throw new ArgumentNullException("body"); }
            AssignmentUtility.ValidAsNumericOperand<TSource>();
            ActFactory<TSource, T> factory = new ActFactory<TSource, T>(body, default(TSource), arg);
            ForCore(factory, LoopUtility.Condition, initial, RelationalOperator.LessThan, repeats, LoopUtility.Iterator, AssignmentOperator.Addition, (TSource)ConvertUtility.ChangeType(1, typeof(TSource)), timeout);
        }

        /// <summary>
        /// Provides a generic way of executing a parallel for-loop while providing ways to encapsulate and re-use existing code.
        /// </summary>
        /// <typeparam name="TSource">The type of the data in the <paramref name="initial"/>.</typeparam>
        /// <typeparam name="T1">The type of the first parameter of the delegate <paramref name="body"/>.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the delegate <paramref name="body"/>.</typeparam>
        /// <param name="timeout">A <see cref="TimeSpan" /> that represents the time to wait for the parallel for-loop operation to complete.</param>
        /// <param name="initial">The initial value of the <c>counter</c> in the parallel for-loop.</param>
        /// <param name="repeats">The amount of repeats to do.</param>
        /// <param name="body">The delegate that is invoked once per iteration.</param>
        /// <param name="arg1">The first parameter of the delegate <paramref name="body"/>.</param>
        /// <param name="arg2">The second parameter of the delegate <paramref name="body"/>.</param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="body"/> is null.
        /// </exception>
        public static void For<TSource, T1, T2>(TimeSpan timeout, TSource initial, TSource repeats, Act<TSource, T1, T2> body, T1 arg1, T2 arg2) where TSource : struct, IComparable<TSource>, IEquatable<TSource>, IConvertible
        {
            if (body == null) { throw new ArgumentNullException("body"); }
            AssignmentUtility.ValidAsNumericOperand<TSource>();
            ActFactory<TSource, T1, T2> factory = new ActFactory<TSource, T1, T2>(body, default(TSource), arg1, arg2);
            ForCore(factory, LoopUtility.Condition, initial, RelationalOperator.LessThan, repeats, LoopUtility.Iterator, AssignmentOperator.Addition, (TSource)ConvertUtility.ChangeType(1, typeof(TSource)), timeout);
        }

        /// <summary>
        /// Provides a generic way of executing a parallel for-loop while providing ways to encapsulate and re-use existing code.
        /// </summary>
        /// <typeparam name="TSource">The type of the data in the <paramref name="initial"/>.</typeparam>
        /// <typeparam name="T1">The type of the first parameter of the delegate <paramref name="body"/>.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the delegate <paramref name="body"/>.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the delegate <paramref name="body"/>.</typeparam>
        /// <param name="timeout">A <see cref="TimeSpan" /> that represents the time to wait for the parallel for-loop operation to complete.</param>
        /// <param name="initial">The initial value of the <c>counter</c> in the parallel for-loop.</param>
        /// <param name="repeats">The amount of repeats to do.</param>
        /// <param name="body">The delegate that is invoked once per iteration.</param>
        /// <param name="arg1">The first parameter of the delegate <paramref name="body"/>.</param>
        /// <param name="arg2">The second parameter of the delegate <paramref name="body"/>.</param>
        /// <param name="arg3">The third parameter of the delegate <paramref name="body"/>.</param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="body"/> is null.
        /// </exception>
        public static void For<TSource, T1, T2, T3>(TimeSpan timeout, TSource initial, TSource repeats, Act<TSource, T1, T2, T3> body, T1 arg1, T2 arg2, T3 arg3) where TSource : struct, IComparable<TSource>, IEquatable<TSource>, IConvertible
        {
            if (body == null) { throw new ArgumentNullException("body"); }
            AssignmentUtility.ValidAsNumericOperand<TSource>();
            ActFactory<TSource, T1, T2, T3> factory = new ActFactory<TSource, T1, T2, T3>(body, default(TSource), arg1, arg2, arg3);
            ForCore(factory, LoopUtility.Condition, initial, RelationalOperator.LessThan, repeats, LoopUtility.Iterator, AssignmentOperator.Addition, (TSource)ConvertUtility.ChangeType(1, typeof(TSource)), timeout);
        }

        /// <summary>
        /// Provides a generic way of executing a parallel for-loop while providing ways to encapsulate and re-use existing code.
        /// </summary>
        /// <typeparam name="TSource">The type of the data in the <paramref name="initial"/>.</typeparam>
        /// <typeparam name="T1">The type of the first parameter of the delegate <paramref name="body"/>.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the delegate <paramref name="body"/>.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the delegate <paramref name="body"/>.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of the delegate <paramref name="body"/>.</typeparam>
        /// <param name="timeout">A <see cref="TimeSpan" /> that represents the time to wait for the parallel for-loop operation to complete.</param>
        /// <param name="initial">The initial value of the <c>counter</c> in the parallel for-loop.</param>
        /// <param name="repeats">The amount of repeats to do.</param>
        /// <param name="body">The delegate that is invoked once per iteration.</param>
        /// <param name="arg1">The first parameter of the delegate <paramref name="body"/>.</param>
        /// <param name="arg2">The second parameter of the delegate <paramref name="body"/>.</param>
        /// <param name="arg3">The third parameter of the delegate <paramref name="body"/>.</param>
        /// <param name="arg4">The fourth parameter of the delegate <paramref name="body"/>.</param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="body"/> is null.
        /// </exception>
        public static void For<TSource, T1, T2, T3, T4>(TimeSpan timeout, TSource initial, TSource repeats, Act<TSource, T1, T2, T3, T4> body, T1 arg1, T2 arg2, T3 arg3, T4 arg4) where TSource : struct, IComparable<TSource>, IEquatable<TSource>, IConvertible
        {
            if (body == null) { throw new ArgumentNullException("body"); }
            AssignmentUtility.ValidAsNumericOperand<TSource>();
            ActFactory<TSource, T1, T2, T3, T4> factory = new ActFactory<TSource, T1, T2, T3, T4>(body, default(TSource), arg1, arg2, arg3, arg4);
            ForCore(factory, LoopUtility.Condition, initial, RelationalOperator.LessThan, repeats, LoopUtility.Iterator, AssignmentOperator.Addition, (TSource)ConvertUtility.ChangeType(1, typeof(TSource)), timeout);
        }

        /// <summary>
        /// Provides a generic way of executing a parallel for-loop while providing ways to encapsulate and re-use existing code.
        /// </summary>
        /// <typeparam name="TSource">The type of the data in the <paramref name="initial"/>.</typeparam>
        /// <typeparam name="T1">The type of the first parameter of the delegate <paramref name="body"/>.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the delegate <paramref name="body"/>.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the delegate <paramref name="body"/>.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of the delegate <paramref name="body"/>.</typeparam>
        /// <typeparam name="T5">The type of the fifth parameter of the delegate <paramref name="body"/>.</typeparam>
        /// <param name="timeout">A <see cref="TimeSpan" /> that represents the time to wait for the parallel for-loop operation to complete.</param>
        /// <param name="initial">The initial value of the <c>counter</c> in the parallel for-loop.</param>
        /// <param name="repeats">The amount of repeats to do.</param>
        /// <param name="body">The delegate that is invoked once per iteration.</param>
        /// <param name="arg1">The first parameter of the delegate <paramref name="body"/>.</param>
        /// <param name="arg2">The second parameter of the delegate <paramref name="body"/>.</param>
        /// <param name="arg3">The third parameter of the delegate <paramref name="body"/>.</param>
        /// <param name="arg4">The fourth parameter of the delegate <paramref name="body"/>.</param>
        /// <param name="arg5">The fifth parameter of the delegate <paramref name="body"/>.</param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="body"/> is null.
        /// </exception>
        public static void For<TSource, T1, T2, T3, T4, T5>(TimeSpan timeout, TSource initial, TSource repeats, Act<TSource, T1, T2, T3, T4, T5> body, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5) where TSource : struct, IComparable<TSource>, IEquatable<TSource>, IConvertible
        {
            if (body == null) { throw new ArgumentNullException("body"); }
            AssignmentUtility.ValidAsNumericOperand<TSource>();
            ActFactory<TSource, T1, T2, T3, T4, T5> factory = new ActFactory<TSource, T1, T2, T3, T4, T5>(body, default(TSource), arg1, arg2, arg3, arg4, arg5);
            ForCore(factory, LoopUtility.Condition, initial, RelationalOperator.LessThan, repeats, LoopUtility.Iterator, AssignmentOperator.Addition, (TSource)ConvertUtility.ChangeType(1, typeof(TSource)), timeout);
        }

        /// <summary>
        /// Provides a generic way of executing a parallel for-loop while providing ways to encapsulate and re-use existing code having a return value.
        /// </summary>
        /// <typeparam name="TSource">The type of the data in the <paramref name="initial"/>.</typeparam>
        /// <typeparam name="TResult">The type of the return value of the <paramref name="body"/> that this function delegate encapsulates.</typeparam>
        /// <param name="initial">The initial value of the <c>counter</c> in the parallel for-loop.</param>
        /// <param name="repeats">The amount of repeats to do.</param>
        /// <param name="body">The function delegate that is invoked once per iteration.</param>
        /// <returns>A <see cref="IReadOnlyCollection{TResult}"/> where the result of <paramref name="body"/> has been stored in the same sequential order as <paramref name="repeats"/>.</returns>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="body"/> is null.
        /// </exception>
        public static IReadOnlyCollection<TResult> ForWithResult<TSource, TResult>(TSource initial, TSource repeats, Doer<TSource, TResult> body) where TSource : struct, IComparable<TSource>, IEquatable<TSource>, IConvertible
        {
            return ForWithResult(initial, repeats, DefaultTimeout, body);
        }

        /// <summary>
        /// Provides a generic way of executing a parallel for-loop while providing ways to encapsulate and re-use existing code having a return value.
        /// </summary>
        /// <typeparam name="TSource">The type of the data in the <paramref name="initial"/>.</typeparam>
        /// <typeparam name="TResult">The type of the return value of the <paramref name="body"/> that this function delegate encapsulates.</typeparam>
        /// <typeparam name="T">The type of the parameter of the function delegate <paramref name="body"/>.</typeparam>
        /// <param name="initial">The initial value of the <c>counter</c> in the parallel for-loop.</param>
        /// <param name="repeats">The amount of repeats to do.</param>
        /// <param name="body">The function delegate that is invoked once per iteration.</param>
        /// <param name="arg">The parameter of the function delegate <paramref name="body"/>.</param>
        /// <returns>A <see cref="IReadOnlyCollection{TResult}"/> where the result of <paramref name="body"/> has been stored in the same sequential order as <paramref name="repeats"/>.</returns>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="body"/> is null.
        /// </exception>
        public static IReadOnlyCollection<TResult> ForWithResult<TSource, T, TResult>(TSource initial, TSource repeats, Doer<TSource, T, TResult> body, T arg) where TSource : struct, IComparable<TSource>, IEquatable<TSource>, IConvertible
        {
            return ForWithResult(initial, repeats, DefaultTimeout, body, arg);
        }

        /// <summary>
        /// Provides a generic way of executing a parallel for-loop while providing ways to encapsulate and re-use existing code having a return value.
        /// </summary>
        /// <typeparam name="TSource">The type of the data in the <paramref name="initial"/>.</typeparam>
        /// <typeparam name="TResult">The type of the return value of the <paramref name="body"/> that this function delegate encapsulates.</typeparam>
        /// <typeparam name="T1">The type of the first parameter of the function delegate <paramref name="body"/>.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the function delegate <paramref name="body"/>.</typeparam>
        /// <param name="initial">The initial value of the <c>counter</c> in the parallel for-loop.</param>
        /// <param name="repeats">The amount of repeats to do.</param>
        /// <param name="body">The function delegate that is invoked once per iteration.</param>
        /// <param name="arg1">The first parameter of the function delegate <paramref name="body"/>.</param>
        /// <param name="arg2">The second parameter of the function delegate <paramref name="body"/>.</param>
        /// <returns>A <see cref="IReadOnlyCollection{TResult}"/> where the result of <paramref name="body"/> has been stored in the same sequential order as <paramref name="repeats"/>.</returns>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="body"/> is null.
        /// </exception>
        public static IReadOnlyCollection<TResult> ForWithResult<TSource, T1, T2, TResult>(TSource initial, TSource repeats, Doer<TSource, T1, T2, TResult> body, T1 arg1, T2 arg2) where TSource : struct, IComparable<TSource>, IEquatable<TSource>, IConvertible
        {
            return ForWithResult(initial, repeats, DefaultTimeout, body, arg1, arg2);
        }

        /// <summary>
        /// Provides a generic way of executing a parallel for-loop while providing ways to encapsulate and re-use existing code having a return value.
        /// </summary>
        /// <typeparam name="TSource">The type of the data in the <paramref name="initial"/>.</typeparam>
        /// <typeparam name="TResult">The type of the return value of the <paramref name="body"/> that this function delegate encapsulates.</typeparam>
        /// <typeparam name="T1">The type of the first parameter of the function delegate <paramref name="body"/>.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the function delegate <paramref name="body"/>.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the function delegate <paramref name="body"/>.</typeparam>
        /// <param name="initial">The initial value of the <c>counter</c> in the parallel for-loop.</param>
        /// <param name="repeats">The amount of repeats to do.</param>
        /// <param name="body">The function delegate that is invoked once per iteration.</param>
        /// <param name="arg1">The first parameter of the function delegate <paramref name="body"/>.</param>
        /// <param name="arg2">The second parameter of the function delegate <paramref name="body"/>.</param>
        /// <param name="arg3">The third parameter of the function delegate <paramref name="body"/>.</param>
        /// <returns>A <see cref="IReadOnlyCollection{TResult}"/> where the result of <paramref name="body"/> has been stored in the same sequential order as <paramref name="repeats"/>.</returns>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="body"/> is null.
        /// </exception>
        public static IReadOnlyCollection<TResult> ForWithResult<TSource, T1, T2, T3, TResult>(TSource initial, TSource repeats, Doer<TSource, T1, T2, T3, TResult> body, T1 arg1, T2 arg2, T3 arg3) where TSource : struct, IComparable<TSource>, IEquatable<TSource>, IConvertible
        {
            return ForWithResult(initial, repeats, DefaultTimeout, body, arg1, arg2, arg3);
        }

        /// <summary>
        /// Provides a generic way of executing a parallel for-loop while providing ways to encapsulate and re-use existing code having a return value.
        /// </summary>
        /// <typeparam name="TSource">The type of the data in the <paramref name="initial"/>.</typeparam>
        /// <typeparam name="TResult">The type of the return value of the <paramref name="body"/> that this function delegate encapsulates.</typeparam>
        /// <typeparam name="T1">The type of the first parameter of the function delegate <paramref name="body"/>.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the function delegate <paramref name="body"/>.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the function delegate <paramref name="body"/>.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of the function delegate <paramref name="body"/>.</typeparam>
        /// <param name="initial">The initial value of the <c>counter</c> in the parallel for-loop.</param>
        /// <param name="repeats">The amount of repeats to do.</param>
        /// <param name="body">The function delegate that is invoked once per iteration.</param>
        /// <param name="arg1">The first parameter of the function delegate <paramref name="body"/>.</param>
        /// <param name="arg2">The second parameter of the function delegate <paramref name="body"/>.</param>
        /// <param name="arg3">The third parameter of the function delegate <paramref name="body"/>.</param>
        /// <param name="arg4">The fourth parameter of the function delegate <paramref name="body"/>.</param>
        /// <returns>A <see cref="IReadOnlyCollection{TResult}"/> where the result of <paramref name="body"/> has been stored in the same sequential order as <paramref name="repeats"/>.</returns>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="body"/> is null.
        /// </exception>
        public static IReadOnlyCollection<TResult> ForWithResult<TSource, T1, T2, T3, T4, TResult>(TSource initial, TSource repeats, Doer<TSource, T1, T2, T3, T4, TResult> body, T1 arg1, T2 arg2, T3 arg3, T4 arg4) where TSource : struct, IComparable<TSource>, IEquatable<TSource>, IConvertible
        {
            return ForWithResult(initial, repeats, DefaultTimeout, body, arg1, arg2, arg3, arg4);
        }

        /// <summary>
        /// Provides a generic way of executing a parallel for-loop while providing ways to encapsulate and re-use existing code having a return value.
        /// </summary>
        /// <typeparam name="TSource">The type of the data in the <paramref name="initial"/>.</typeparam>
        /// <typeparam name="TResult">The type of the return value of the <paramref name="body"/> that this function delegate encapsulates.</typeparam>
        /// <typeparam name="T1">The type of the first parameter of the function delegate <paramref name="body"/>.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the function delegate <paramref name="body"/>.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the function delegate <paramref name="body"/>.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of the function delegate <paramref name="body"/>.</typeparam>
        /// <typeparam name="T5">The type of the fifth parameter of the function delegate <paramref name="body"/>.</typeparam>
        /// <param name="initial">The initial value of the <c>counter</c> in the parallel for-loop.</param>
        /// <param name="repeats">The amount of repeats to do.</param>
        /// <param name="body">The function delegate that is invoked once per iteration.</param>
        /// <param name="arg1">The first parameter of the function delegate <paramref name="body"/>.</param>
        /// <param name="arg2">The second parameter of the function delegate <paramref name="body"/>.</param>
        /// <param name="arg3">The third parameter of the function delegate <paramref name="body"/>.</param>
        /// <param name="arg4">The fourth parameter of the function delegate <paramref name="body"/>.</param>
        /// <param name="arg5">The fifth parameter of the function delegate <paramref name="body"/>.</param>
        /// <returns>A <see cref="IReadOnlyCollection{TResult}"/> where the result of <paramref name="body"/> has been stored in the same sequential order as <paramref name="repeats"/>.</returns>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="body"/> is null.
        /// </exception>
        public static IReadOnlyCollection<TResult> ForWithResult<TSource, T1, T2, T3, T4, T5, TResult>(TSource initial, TSource repeats, Doer<TSource, T1, T2, T3, T4, T5, TResult> body, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5) where TSource : struct, IComparable<TSource>, IEquatable<TSource>, IConvertible
        {
            return ForWithResult(initial, repeats, DefaultTimeout, body, arg1, arg2, arg3, arg4, arg5);
        }

        /// <summary>
        /// Provides a generic way of executing a parallel for-loop while providing ways to encapsulate and re-use existing code having a return value.
        /// </summary>
        /// <typeparam name="TSource">The type of the data in the <paramref name="initial"/>.</typeparam>
        /// <typeparam name="TResult">The type of the return value of the <paramref name="body"/> that this function delegate encapsulates.</typeparam>
        /// <param name="initial">The initial value of the <c>counter</c> in the parallel for-loop.</param>
        /// <param name="repeats">The amount of repeats to do.</param>
        /// <param name="timeout">A <see cref="TimeSpan" /> that represents the time to wait for the parallel for-loop operation to complete.</param>
        /// <param name="body">The function delegate that is invoked once per iteration.</param>
        /// <returns>A <see cref="IReadOnlyCollection{TResult}"/> where the result of <paramref name="body"/> has been stored in the same sequential order as <paramref name="repeats"/>.</returns>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="body"/> is null.
        /// </exception>
        public static IReadOnlyCollection<TResult> ForWithResult<TSource, TResult>(TSource initial, TSource repeats, TimeSpan timeout, Doer<TSource, TResult> body) where TSource : struct, IComparable<TSource>, IEquatable<TSource>, IConvertible
        {
            Validator.ThrowIfNull(body, "body");
            AssignmentUtility.ValidAsNumericOperand<TSource>();
            DoerFactory<TSource, TResult> factory = new DoerFactory<TSource, TResult>(body, default(TSource));
            return ForCore(factory, LoopUtility.Condition, initial, RelationalOperator.LessThan, repeats, LoopUtility.Iterator, AssignmentOperator.Addition, (TSource)ConvertUtility.ChangeType(1, typeof(TSource)), timeout);
        }

        /// <summary>
        /// Provides a generic way of executing a parallel for-loop while providing ways to encapsulate and re-use existing code having a return value.
        /// </summary>
        /// <typeparam name="TSource">The type of the data in the <paramref name="initial"/>.</typeparam>
        /// <typeparam name="TResult">The type of the return value of the <paramref name="body"/> that this function delegate encapsulates.</typeparam>
        /// <typeparam name="T">The type of the parameter of the function delegate <paramref name="body"/>.</typeparam>
        /// <param name="initial">The initial value of the <c>counter</c> in the parallel for-loop.</param>
        /// <param name="repeats">The amount of repeats to do.</param>
        /// <param name="timeout">A <see cref="TimeSpan" /> that represents the time to wait for the parallel for-loop operation to complete.</param>
        /// <param name="body">The function delegate that is invoked once per iteration.</param>
        /// <param name="arg">The parameter of the function delegate <paramref name="body"/>.</param>
        /// <returns>A <see cref="IReadOnlyCollection{TResult}"/> where the result of <paramref name="body"/> has been stored in the same sequential order as <paramref name="repeats"/>.</returns>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="body"/> is null.
        /// </exception>
        public static IReadOnlyCollection<TResult> ForWithResult<TSource, T, TResult>(TSource initial, TSource repeats, TimeSpan timeout, Doer<TSource, T, TResult> body, T arg) where TSource : struct, IComparable<TSource>, IEquatable<TSource>, IConvertible
        {
            Validator.ThrowIfNull(body, "body");
            AssignmentUtility.ValidAsNumericOperand<TSource>();
            DoerFactory<TSource, T, TResult> factory = new DoerFactory<TSource, T, TResult>(body, default(TSource), arg);
            return ForCore(factory, LoopUtility.Condition, initial, RelationalOperator.LessThan, repeats, LoopUtility.Iterator, AssignmentOperator.Addition, (TSource)ConvertUtility.ChangeType(1, typeof(TSource)), timeout);
        }

        /// <summary>
        /// Provides a generic way of executing a parallel for-loop while providing ways to encapsulate and re-use existing code having a return value.
        /// </summary>
        /// <typeparam name="TSource">The type of the data in the <paramref name="initial"/>.</typeparam>
        /// <typeparam name="TResult">The type of the return value of the <paramref name="body"/> that this function delegate encapsulates.</typeparam>
        /// <typeparam name="T1">The type of the first parameter of the function delegate <paramref name="body"/>.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the function delegate <paramref name="body"/>.</typeparam>
        /// <param name="initial">The initial value of the <c>counter</c> in the parallel for-loop.</param>
        /// <param name="repeats">The amount of repeats to do.</param>
        /// <param name="timeout">A <see cref="TimeSpan" /> that represents the time to wait for the parallel for-loop operation to complete.</param>
        /// <param name="body">The function delegate that is invoked once per iteration.</param>
        /// <param name="arg1">The first parameter of the function delegate <paramref name="body"/>.</param>
        /// <param name="arg2">The second parameter of the function delegate <paramref name="body"/>.</param>
        /// <returns>A <see cref="IReadOnlyCollection{TResult}"/> where the result of <paramref name="body"/> has been stored in the same sequential order as <paramref name="repeats"/>.</returns>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="body"/> is null.
        /// </exception>
        public static IReadOnlyCollection<TResult> ForWithResult<TSource, T1, T2, TResult>(TSource initial, TSource repeats, TimeSpan timeout, Doer<TSource, T1, T2, TResult> body, T1 arg1, T2 arg2) where TSource : struct, IComparable<TSource>, IEquatable<TSource>, IConvertible
        {
            Validator.ThrowIfNull(body, "body");
            AssignmentUtility.ValidAsNumericOperand<TSource>();
            DoerFactory<TSource, T1, T2, TResult> factory = new DoerFactory<TSource, T1, T2, TResult>(body, default(TSource), arg1, arg2);
            return ForCore(factory, LoopUtility.Condition, initial, RelationalOperator.LessThan, repeats, LoopUtility.Iterator, AssignmentOperator.Addition, (TSource)ConvertUtility.ChangeType(1, typeof(TSource)), timeout);
        }

        /// <summary>
        /// Provides a generic way of executing a parallel for-loop while providing ways to encapsulate and re-use existing code having a return value.
        /// </summary>
        /// <typeparam name="TSource">The type of the data in the <paramref name="initial"/>.</typeparam>
        /// <typeparam name="TResult">The type of the return value of the <paramref name="body"/> that this function delegate encapsulates.</typeparam>
        /// <typeparam name="T1">The type of the first parameter of the function delegate <paramref name="body"/>.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the function delegate <paramref name="body"/>.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the function delegate <paramref name="body"/>.</typeparam>
        /// <param name="initial">The initial value of the <c>counter</c> in the parallel for-loop.</param>
        /// <param name="repeats">The amount of repeats to do.</param>
        /// <param name="timeout">A <see cref="TimeSpan" /> that represents the time to wait for the parallel for-loop operation to complete.</param>
        /// <param name="body">The function delegate that is invoked once per iteration.</param>
        /// <param name="arg1">The first parameter of the function delegate <paramref name="body"/>.</param>
        /// <param name="arg2">The second parameter of the function delegate <paramref name="body"/>.</param>
        /// <param name="arg3">The third parameter of the function delegate <paramref name="body"/>.</param>
        /// <returns>A <see cref="IReadOnlyCollection{TResult}"/> where the result of <paramref name="body"/> has been stored in the same sequential order as <paramref name="repeats"/>.</returns>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="body"/> is null.
        /// </exception>
        public static IReadOnlyCollection<TResult> ForWithResult<TSource, T1, T2, T3, TResult>(TSource initial, TSource repeats, TimeSpan timeout, Doer<TSource, T1, T2, T3, TResult> body, T1 arg1, T2 arg2, T3 arg3) where TSource : struct, IComparable<TSource>, IEquatable<TSource>, IConvertible
        {
            Validator.ThrowIfNull(body, "body");
            AssignmentUtility.ValidAsNumericOperand<TSource>();
            DoerFactory<TSource, T1, T2, T3, TResult> factory = new DoerFactory<TSource, T1, T2, T3, TResult>(body, default(TSource), arg1, arg2, arg3);
            return ForCore(factory, LoopUtility.Condition, initial, RelationalOperator.LessThan, repeats, LoopUtility.Iterator, AssignmentOperator.Addition, (TSource)ConvertUtility.ChangeType(1, typeof(TSource)), timeout);
        }

        /// <summary>
        /// Provides a generic way of executing a parallel for-loop while providing ways to encapsulate and re-use existing code having a return value.
        /// </summary>
        /// <typeparam name="TSource">The type of the data in the <paramref name="initial"/>.</typeparam>
        /// <typeparam name="TResult">The type of the return value of the <paramref name="body"/> that this function delegate encapsulates.</typeparam>
        /// <typeparam name="T1">The type of the first parameter of the function delegate <paramref name="body"/>.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the function delegate <paramref name="body"/>.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the function delegate <paramref name="body"/>.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of the function delegate <paramref name="body"/>.</typeparam>
        /// <param name="initial">The initial value of the <c>counter</c> in the parallel for-loop.</param>
        /// <param name="repeats">The amount of repeats to do.</param>
        /// <param name="timeout">A <see cref="TimeSpan" /> that represents the time to wait for the parallel for-loop operation to complete.</param>
        /// <param name="body">The function delegate that is invoked once per iteration.</param>
        /// <param name="arg1">The first parameter of the function delegate <paramref name="body"/>.</param>
        /// <param name="arg2">The second parameter of the function delegate <paramref name="body"/>.</param>
        /// <param name="arg3">The third parameter of the function delegate <paramref name="body"/>.</param>
        /// <param name="arg4">The fourth parameter of the function delegate <paramref name="body"/>.</param>
        /// <returns>A <see cref="IReadOnlyCollection{TResult}"/> where the result of <paramref name="body"/> has been stored in the same sequential order as <paramref name="repeats"/>.</returns>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="body"/> is null.
        /// </exception>
        public static IReadOnlyCollection<TResult> ForWithResult<TSource, T1, T2, T3, T4, TResult>(TSource initial, TSource repeats, TimeSpan timeout, Doer<TSource, T1, T2, T3, T4, TResult> body, T1 arg1, T2 arg2, T3 arg3, T4 arg4) where TSource : struct, IComparable<TSource>, IEquatable<TSource>, IConvertible
        {
            Validator.ThrowIfNull(body, "body");
            AssignmentUtility.ValidAsNumericOperand<TSource>();
            DoerFactory<TSource, T1, T2, T3, T4, TResult> factory = new DoerFactory<TSource, T1, T2, T3, T4, TResult>(body, default(TSource), arg1, arg2, arg3, arg4);
            return ForCore(factory, LoopUtility.Condition, initial, RelationalOperator.LessThan, repeats, LoopUtility.Iterator, AssignmentOperator.Addition, (TSource)ConvertUtility.ChangeType(1, typeof(TSource)), timeout);
        }

        /// <summary>
        /// Provides a generic way of executing a parallel for-loop while providing ways to encapsulate and re-use existing code having a return value.
        /// </summary>
        /// <typeparam name="TSource">The type of the data in the <paramref name="initial"/>.</typeparam>
        /// <typeparam name="TResult">The type of the return value of the <paramref name="body"/> that this function delegate encapsulates.</typeparam>
        /// <typeparam name="T1">The type of the first parameter of the function delegate <paramref name="body"/>.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the function delegate <paramref name="body"/>.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the function delegate <paramref name="body"/>.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of the function delegate <paramref name="body"/>.</typeparam>
        /// <typeparam name="T5">The type of the fifth parameter of the function delegate <paramref name="body"/>.</typeparam>
        /// <param name="initial">The initial value of the <c>counter</c> in the parallel for-loop.</param>
        /// <param name="repeats">The amount of repeats to do.</param>
        /// <param name="timeout">A <see cref="TimeSpan" /> that represents the time to wait for the parallel for-loop operation to complete.</param>
        /// <param name="body">The function delegate that is invoked once per iteration.</param>
        /// <param name="arg1">The first parameter of the function delegate <paramref name="body"/>.</param>
        /// <param name="arg2">The second parameter of the function delegate <paramref name="body"/>.</param>
        /// <param name="arg3">The third parameter of the function delegate <paramref name="body"/>.</param>
        /// <param name="arg4">The fourth parameter of the function delegate <paramref name="body"/>.</param>
        /// <param name="arg5">The fifth parameter of the function delegate <paramref name="body"/>.</param>
        /// <returns>A <see cref="IReadOnlyCollection{TResult}"/> where the result of <paramref name="body"/> has been stored in the same sequential order as <paramref name="repeats"/>.</returns>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="body"/> is null.
        /// </exception>
        public static IReadOnlyCollection<TResult> ForWithResult<TSource, T1, T2, T3, T4, T5, TResult>(TSource initial, TSource repeats, TimeSpan timeout, Doer<TSource, T1, T2, T3, T4, T5, TResult> body, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5) where TSource : struct, IComparable<TSource>, IEquatable<TSource>, IConvertible
        {
            Validator.ThrowIfNull(body, "body");
            AssignmentUtility.ValidAsNumericOperand<TSource>();
            DoerFactory<TSource, T1, T2, T3, T4, T5, TResult> factory = new DoerFactory<TSource, T1, T2, T3, T4, T5, TResult>(body, default(TSource), arg1, arg2, arg3, arg4, arg5);
            return ForCore(factory, LoopUtility.Condition, initial, RelationalOperator.LessThan, repeats, LoopUtility.Iterator, AssignmentOperator.Addition, (TSource)ConvertUtility.ChangeType(1, typeof(TSource)), timeout);
        }

        /// <summary>
        /// Provides a generic way of executing a parallel foreach-loop while providing ways to encapsulate and re-use existing code having a return value.
        /// </summary>
        /// <typeparam name="TSource">The type of the data in the <paramref name="source"/>.</typeparam>
        /// <typeparam name="TResult">The type of the return value of the <paramref name="body"/> that this function delegate encapsulates.</typeparam>
        /// <param name="source">An enumerable data source.</param>
        /// <param name="body">The function delegate that is invoked once per iteration.</param>
        /// <returns>A <see cref="IReadOnlyCollection{TResult}"/> where the result of <paramref name="body"/> has been stored in the same sequential order as <paramref name="source"/>.</returns>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="body"/> is null -or- <paramref name="source"/> is null.
        /// </exception>
        public static IReadOnlyCollection<TResult> ForEachWithResult<TSource, TResult>(IEnumerable<TSource> source, Doer<TSource, TResult> body)
        {
            return ForEachWithResult(source, DefaultTimeout, body);
        }

        /// <summary>
        /// Provides a generic way of executing a parallel foreach-loop while providing ways to encapsulate and re-use existing code having a return value.
        /// </summary>
        /// <typeparam name="TSource">The type of the data in the <paramref name="source"/>.</typeparam>
        /// <typeparam name="TResult">The type of the return value of the <paramref name="body"/> that this function delegate encapsulates.</typeparam>
        /// <typeparam name="T">The type of the parameter of the function delegate <paramref name="body"/>.</typeparam>
        /// <param name="source">An enumerable data source.</param>
        /// <param name="body">The function delegate that is invoked once per iteration.</param>
        /// <param name="arg">The parameter of the function delegate <paramref name="body"/>.</param>
        /// <returns>A <see cref="IReadOnlyCollection{TResult}"/> where the result of <paramref name="body"/> has been stored in the same sequential order as <paramref name="source"/>.</returns>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="body"/> is null -or- <paramref name="source"/> is null.
        /// </exception>
        public static IReadOnlyCollection<TResult> ForEachWithResult<TSource, T, TResult>(IEnumerable<TSource> source, Doer<TSource, T, TResult> body, T arg)
        {
            return ForEachWithResult(source, DefaultTimeout, body, arg);
        }

        /// <summary>
        /// Provides a generic way of executing a parallel foreach-loop while providing ways to encapsulate and re-use existing code having a return value.
        /// </summary>
        /// <typeparam name="TSource">The type of the data in the <paramref name="source"/>.</typeparam>
        /// <typeparam name="TResult">The type of the return value of the <paramref name="body"/> that this function delegate encapsulates.</typeparam>
        /// <typeparam name="T1">The type of the first parameter of the function delegate <paramref name="body"/>.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the function delegate <paramref name="body"/>.</typeparam>
        /// <param name="source">An enumerable data source.</param>
        /// <param name="body">The function delegate that is invoked once per iteration.</param>
        /// <param name="arg1">The first parameter of the function delegate <paramref name="body"/>.</param>
        /// <param name="arg2">The second parameter of the function delegate <paramref name="body"/>.</param>
        /// <returns>A <see cref="IReadOnlyCollection{TResult}"/> where the result of <paramref name="body"/> has been stored in the same sequential order as <paramref name="source"/>.</returns>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="body"/> is null -or- <paramref name="source"/> is null.
        /// </exception>
        public static IReadOnlyCollection<TResult> ForEachWithResult<TSource, T1, T2, TResult>(IEnumerable<TSource> source, Doer<TSource, T1, T2, TResult> body, T1 arg1, T2 arg2)
        {
            return ForEachWithResult(source, DefaultTimeout, body, arg1, arg2);
        }

        /// <summary>
        /// Provides a generic way of executing a parallel foreach-loop while providing ways to encapsulate and re-use existing code having a return value.
        /// </summary>
        /// <typeparam name="TSource">The type of the data in the <paramref name="source"/>.</typeparam>
        /// <typeparam name="TResult">The type of the return value of the <paramref name="body"/> that this function delegate encapsulates.</typeparam>
        /// <typeparam name="T1">The type of the first parameter of the function delegate <paramref name="body"/>.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the function delegate <paramref name="body"/>.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the function delegate <paramref name="body"/>.</typeparam>
        /// <param name="source">An enumerable data source.</param>
        /// <param name="body">The function delegate that is invoked once per iteration.</param>
        /// <param name="arg1">The first parameter of the function delegate <paramref name="body"/>.</param>
        /// <param name="arg2">The second parameter of the function delegate <paramref name="body"/>.</param>
        /// <param name="arg3">The third parameter of the function delegate <paramref name="body"/>.</param>
        /// <returns>A <see cref="IReadOnlyCollection{TResult}"/> where the result of <paramref name="body"/> has been stored in the same sequential order as <paramref name="source"/>.</returns>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="body"/> is null -or- <paramref name="source"/> is null.
        /// </exception>
        public static IReadOnlyCollection<TResult> ForEachWithResult<TSource, T1, T2, T3, TResult>(IEnumerable<TSource> source, Doer<TSource, T1, T2, T3, TResult> body, T1 arg1, T2 arg2, T3 arg3)
        {
            return ForEachWithResult(source, DefaultTimeout, body, arg1, arg2, arg3);
        }

        /// <summary>
        /// Provides a generic way of executing a parallel foreach-loop while providing ways to encapsulate and re-use existing code having a return value.
        /// </summary>
        /// <typeparam name="TSource">The type of the data in the <paramref name="source"/>.</typeparam>
        /// <typeparam name="TResult">The type of the return value of the <paramref name="body"/> that this function delegate encapsulates.</typeparam>
        /// <typeparam name="T1">The type of the first parameter of the function delegate <paramref name="body"/>.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the function delegate <paramref name="body"/>.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the function delegate <paramref name="body"/>.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of the function delegate <paramref name="body"/>.</typeparam>
        /// <param name="source">An enumerable data source.</param>
        /// <param name="body">The function delegate that is invoked once per iteration.</param>
        /// <param name="arg1">The first parameter of the function delegate <paramref name="body"/>.</param>
        /// <param name="arg2">The second parameter of the function delegate <paramref name="body"/>.</param>
        /// <param name="arg3">The third parameter of the function delegate <paramref name="body"/>.</param>
        /// <param name="arg4">The fourth parameter of the function delegate <paramref name="body"/>.</param>
        /// <returns>A <see cref="IReadOnlyCollection{TResult}"/> where the result of <paramref name="body"/> has been stored in the same sequential order as <paramref name="source"/>.</returns>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="body"/> is null -or- <paramref name="source"/> is null.
        /// </exception>
        public static IReadOnlyCollection<TResult> ForEachWithResult<TSource, T1, T2, T3, T4, TResult>(IEnumerable<TSource> source, Doer<TSource, T1, T2, T3, T4, TResult> body, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            return ForEachWithResult(source, DefaultTimeout, body, arg1, arg2, arg3, arg4);
        }

        /// <summary>
        /// Provides a generic way of executing a parallel foreach-loop while providing ways to encapsulate and re-use existing code having a return value.
        /// </summary>
        /// <typeparam name="TSource">The type of the data in the <paramref name="source"/>.</typeparam>
        /// <typeparam name="TResult">The type of the return value of the <paramref name="body"/> that this function delegate encapsulates.</typeparam>
        /// <typeparam name="T1">The type of the first parameter of the function delegate <paramref name="body"/>.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the function delegate <paramref name="body"/>.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the function delegate <paramref name="body"/>.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of the function delegate <paramref name="body"/>.</typeparam>
        /// <typeparam name="T5">The type of the fifth parameter of the function delegate <paramref name="body"/>.</typeparam>
        /// <param name="source">An enumerable data source.</param>
        /// <param name="body">The function delegate that is invoked once per iteration.</param>
        /// <param name="arg1">The first parameter of the function delegate <paramref name="body"/>.</param>
        /// <param name="arg2">The second parameter of the function delegate <paramref name="body"/>.</param>
        /// <param name="arg3">The third parameter of the function delegate <paramref name="body"/>.</param>
        /// <param name="arg4">The fourth parameter of the function delegate <paramref name="body"/>.</param>
        /// <param name="arg5">The fifth parameter of the function delegate <paramref name="body"/>.</param>
        /// <returns>A <see cref="IReadOnlyCollection{TResult}"/> where the result of <paramref name="body"/> has been stored in the same sequential order as <paramref name="source"/>.</returns>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="body"/> is null -or- <paramref name="source"/> is null.
        /// </exception>
        public static IReadOnlyCollection<TResult> ForEachWithResult<TSource, T1, T2, T3, T4, T5, TResult>(IEnumerable<TSource> source, Doer<TSource, T1, T2, T3, T4, T5, TResult> body, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        {
            return ForEachWithResult(source, DefaultTimeout, body, arg1, arg2, arg3, arg4, arg5);
        }

        /// <summary>
        /// Provides a generic way of executing a parallel foreach-loop while providing ways to encapsulate and re-use existing code having a return value.
        /// </summary>
        /// <typeparam name="TSource">The type of the data in the <paramref name="source"/>.</typeparam>
        /// <typeparam name="TResult">The type of the return value of the <paramref name="body"/> that this function delegate encapsulates.</typeparam>
        /// <param name="source">An enumerable data source.</param>
        /// <param name="timeout">A <see cref="TimeSpan" /> that represents the time to wait for the parallel foreach-loop operation to complete.</param>
        /// <param name="body">The function delegate that is invoked once per iteration.</param>
        /// <returns>A <see cref="IReadOnlyCollection{TResult}"/> where the result of <paramref name="body"/> has been stored in the same sequential order as <paramref name="source"/>.</returns>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="body"/> is null -or- <paramref name="source"/> is null.
        /// </exception>
        public static IReadOnlyCollection<TResult> ForEachWithResult<TSource, TResult>(IEnumerable<TSource> source, TimeSpan timeout, Doer<TSource, TResult> body)
        {
            if (body == null) { throw new ArgumentNullException("body"); }
            if (source == null) { throw new ArgumentNullException("source"); }
            DoerFactory<TSource, TResult> factory = new DoerFactory<TSource, TResult>(body, default(TSource));
            return ForEachCore(factory, source, timeout);
        }

        /// <summary>
        /// Provides a generic way of executing a parallel foreach-loop while providing ways to encapsulate and re-use existing code having a return value.
        /// </summary>
        /// <typeparam name="TSource">The type of the data in the <paramref name="source"/>.</typeparam>
        /// <typeparam name="TResult">The type of the return value of the <paramref name="body"/> that this function delegate encapsulates.</typeparam>
        /// <typeparam name="T">The type of the parameter of the function delegate <paramref name="body"/>.</typeparam>
        /// <param name="source">An enumerable data source.</param>
        /// <param name="timeout">A <see cref="TimeSpan" /> that represents the time to wait for the parallel foreach-loop operation to complete.</param>
        /// <param name="body">The function delegate that is invoked once per iteration.</param>
        /// <param name="arg">The parameter of the function delegate <paramref name="body"/>.</param>
        /// <returns>A <see cref="IReadOnlyCollection{TResult}"/> where the result of <paramref name="body"/> has been stored in the same sequential order as <paramref name="source"/>.</returns>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="body"/> is null -or- <paramref name="source"/> is null.
        /// </exception>
        public static IReadOnlyCollection<TResult> ForEachWithResult<TSource, T, TResult>(IEnumerable<TSource> source, TimeSpan timeout, Doer<TSource, T, TResult> body, T arg)
        {
            if (body == null) { throw new ArgumentNullException("body"); }
            if (source == null) { throw new ArgumentNullException("source"); }
            DoerFactory<TSource, T, TResult> factory = new DoerFactory<TSource, T, TResult>(body, default(TSource), arg);
            return ForEachCore(factory, source, timeout);
        }

        /// <summary>
        /// Provides a generic way of executing a parallel foreach-loop while providing ways to encapsulate and re-use existing code having a return value.
        /// </summary>
        /// <typeparam name="TSource">The type of the data in the <paramref name="source"/>.</typeparam>
        /// <typeparam name="TResult">The type of the return value of the <paramref name="body"/> that this function delegate encapsulates.</typeparam>
        /// <typeparam name="T1">The type of the first parameter of the function delegate <paramref name="body"/>.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the function delegate <paramref name="body"/>.</typeparam>
        /// <param name="source">An enumerable data source.</param>
        /// <param name="timeout">A <see cref="TimeSpan" /> that represents the time to wait for the parallel foreach-loop operation to complete.</param>
        /// <param name="body">The function delegate that is invoked once per iteration.</param>
        /// <param name="arg1">The first parameter of the function delegate <paramref name="body"/>.</param>
        /// <param name="arg2">The second parameter of the function delegate <paramref name="body"/>.</param>
        /// <returns>A <see cref="IReadOnlyCollection{TResult}"/> where the result of <paramref name="body"/> has been stored in the same sequential order as <paramref name="source"/>.</returns>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="body"/> is null -or- <paramref name="source"/> is null.
        /// </exception>
        public static IReadOnlyCollection<TResult> ForEachWithResult<TSource, T1, T2, TResult>(IEnumerable<TSource> source, TimeSpan timeout, Doer<TSource, T1, T2, TResult> body, T1 arg1, T2 arg2)
        {
            if (body == null) { throw new ArgumentNullException("body"); }
            if (source == null) { throw new ArgumentNullException("source"); }
            DoerFactory<TSource, T1, T2, TResult> factory = new DoerFactory<TSource, T1, T2, TResult>(body, default(TSource), arg1, arg2);
            return ForEachCore(factory, source, timeout);
        }

        /// <summary>
        /// Provides a generic way of executing a parallel foreach-loop while providing ways to encapsulate and re-use existing code having a return value.
        /// </summary>
        /// <typeparam name="TSource">The type of the data in the <paramref name="source"/>.</typeparam>
        /// <typeparam name="TResult">The type of the return value of the <paramref name="body"/> that this function delegate encapsulates.</typeparam>
        /// <typeparam name="T1">The type of the first parameter of the function delegate <paramref name="body"/>.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the function delegate <paramref name="body"/>.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the function delegate <paramref name="body"/>.</typeparam>
        /// <param name="source">An enumerable data source.</param>
        /// <param name="timeout">A <see cref="TimeSpan" /> that represents the time to wait for the parallel foreach-loop operation to complete.</param>
        /// <param name="body">The function delegate that is invoked once per iteration.</param>
        /// <param name="arg1">The first parameter of the function delegate <paramref name="body"/>.</param>
        /// <param name="arg2">The second parameter of the function delegate <paramref name="body"/>.</param>
        /// <param name="arg3">The third parameter of the function delegate <paramref name="body"/>.</param>
        /// <returns>A <see cref="IReadOnlyCollection{TResult}"/> where the result of <paramref name="body"/> has been stored in the same sequential order as <paramref name="source"/>.</returns>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="body"/> is null -or- <paramref name="source"/> is null.
        /// </exception>
        public static IReadOnlyCollection<TResult> ForEachWithResult<TSource, T1, T2, T3, TResult>(IEnumerable<TSource> source, TimeSpan timeout, Doer<TSource, T1, T2, T3, TResult> body, T1 arg1, T2 arg2, T3 arg3)
        {
            if (body == null) { throw new ArgumentNullException("body"); }
            if (source == null) { throw new ArgumentNullException("source"); }
            DoerFactory<TSource, T1, T2, T3, TResult> factory = new DoerFactory<TSource, T1, T2, T3, TResult>(body, default(TSource), arg1, arg2, arg3);
            return ForEachCore(factory, source, timeout);
        }

        /// <summary>
        /// Provides a generic way of executing a parallel foreach-loop while providing ways to encapsulate and re-use existing code having a return value.
        /// </summary>
        /// <typeparam name="TSource">The type of the data in the <paramref name="source"/>.</typeparam>
        /// <typeparam name="TResult">The type of the return value of the <paramref name="body"/> that this function delegate encapsulates.</typeparam>
        /// <typeparam name="T1">The type of the first parameter of the function delegate <paramref name="body"/>.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the function delegate <paramref name="body"/>.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the function delegate <paramref name="body"/>.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of the function delegate <paramref name="body"/>.</typeparam>
        /// <param name="source">An enumerable data source.</param>
        /// <param name="timeout">A <see cref="TimeSpan" /> that represents the time to wait for the parallel foreach-loop operation to complete.</param>
        /// <param name="body">The function delegate that is invoked once per iteration.</param>
        /// <param name="arg1">The first parameter of the function delegate <paramref name="body"/>.</param>
        /// <param name="arg2">The second parameter of the function delegate <paramref name="body"/>.</param>
        /// <param name="arg3">The third parameter of the function delegate <paramref name="body"/>.</param>
        /// <param name="arg4">The fourth parameter of the function delegate <paramref name="body"/>.</param>
        /// <returns>A <see cref="IReadOnlyCollection{TResult}"/> where the result of <paramref name="body"/> has been stored in the same sequential order as <paramref name="source"/>.</returns>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="body"/> is null -or- <paramref name="source"/> is null.
        /// </exception>
        public static IReadOnlyCollection<TResult> ForEachWithResult<TSource, T1, T2, T3, T4, TResult>(IEnumerable<TSource> source, TimeSpan timeout, Doer<TSource, T1, T2, T3, T4, TResult> body, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            if (body == null) { throw new ArgumentNullException("body"); }
            if (source == null) { throw new ArgumentNullException("source"); }
            DoerFactory<TSource, T1, T2, T3, T4, TResult> factory = new DoerFactory<TSource, T1, T2, T3, T4, TResult>(body, default(TSource), arg1, arg2, arg3, arg4);
            return ForEachCore(factory, source, timeout);
        }

        /// <summary>
        /// Provides a generic way of executing a parallel foreach-loop while providing ways to encapsulate and re-use existing code having a return value.
        /// </summary>
        /// <typeparam name="TSource">The type of the data in the <paramref name="source"/>.</typeparam>
        /// <typeparam name="TResult">The type of the return value of the <paramref name="body"/> that this function delegate encapsulates.</typeparam>
        /// <typeparam name="T1">The type of the first parameter of the function delegate <paramref name="body"/>.</typeparam>
        /// <typeparam name="T2">The type of the second parameter of the function delegate <paramref name="body"/>.</typeparam>
        /// <typeparam name="T3">The type of the third parameter of the function delegate <paramref name="body"/>.</typeparam>
        /// <typeparam name="T4">The type of the fourth parameter of the function delegate <paramref name="body"/>.</typeparam>
        /// <typeparam name="T5">The type of the fifth parameter of the function delegate <paramref name="body"/>.</typeparam>
        /// <param name="source">An enumerable data source.</param>
        /// <param name="timeout">A <see cref="TimeSpan" /> that represents the time to wait for the parallel foreach-loop operation to complete.</param>
        /// <param name="body">The function delegate that is invoked once per iteration.</param>
        /// <param name="arg1">The first parameter of the function delegate <paramref name="body"/>.</param>
        /// <param name="arg2">The second parameter of the function delegate <paramref name="body"/>.</param>
        /// <param name="arg3">The third parameter of the function delegate <paramref name="body"/>.</param>
        /// <param name="arg4">The fourth parameter of the function delegate <paramref name="body"/>.</param>
        /// <param name="arg5">The fifth parameter of the function delegate <paramref name="body"/>.</param>
        /// <returns>A <see cref="IReadOnlyCollection{TResult}"/> where the result of <paramref name="body"/> has been stored in the same sequential order as <paramref name="source"/>.</returns>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="body"/> is null -or- <paramref name="source"/> is null.
        /// </exception>
        public static IReadOnlyCollection<TResult> ForEachWithResult<TSource, T1, T2, T3, T4, T5, TResult>(IEnumerable<TSource> source, TimeSpan timeout, Doer<TSource, T1, T2, T3, T4, T5, TResult> body, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        {
            if (body == null) { throw new ArgumentNullException("body"); }
            if (source == null) { throw new ArgumentNullException("source"); }
            DoerFactory<TSource, T1, T2, T3, T4, T5, TResult> factory = new DoerFactory<TSource, T1, T2, T3, T4, T5, TResult>(body, default(TSource), arg1, arg2, arg3, arg4, arg5);
            return ForEachCore(factory, source, timeout);
        }
        
        private static void ForCore<TSource>(ActFactory<TSource> factory, Doer<TSource, RelationalOperator, TSource, bool> condition, TSource initial, RelationalOperator relational, TSource repeats, Doer<TSource, AssignmentOperator, TSource, TSource> iterator, AssignmentOperator assignment, TSource step, TimeSpan timeout) where TSource : struct, IComparable<TSource>, IEquatable<TSource>, IConvertible
        {
            CountdownEventSlim sync = null;
            try
            {
                try
                {
                    sync = new CountdownEventSlim(1);
                    ActWorkItemPool pool = new ActWorkItemPool();
                    for (TSource i = initial; condition(i, relational, repeats); i = iterator(i, assignment, step))
                    {
                        sync.AddCount(1);
                        factory.Arg1 = i;
                        IActWorkItem work = ActWorkItem.Create(sync, ForStepCore, factory.Clone());
                        pool.ProcessWork(work);
                    }
                }
                finally
                {
                    sync.Signal();
                }
                sync.Wait(timeout);
            }
            finally
            {
                sync = null;
                factory = null;
            }

        }

        private static void ForStepCore(ActFactory factory)
        {
            try
            {
                factory.ExecuteMethod();
            }
            finally
            {
                factory = null;
            }    
        }

        private static IReadOnlyCollection<TResult> ForCore<TSource, TResult>(DoerFactory<TSource, TResult> factory, Doer<TSource, RelationalOperator, TSource, bool> condition, TSource initial, RelationalOperator relational, TSource repeats, Doer<TSource, AssignmentOperator, TSource, TSource> iterator, AssignmentOperator assignment, TSource step, TimeSpan timeout) where TSource : struct, IComparable<TSource>, IEquatable<TSource>, IConvertible
        {
            CountdownEventSlim sync = null;
            SortedDoerWorkItemPool<TSource, TResult> pool = null;
            try
            {
                try
                {
                    sync = new CountdownEventSlim(1);
                    pool = new SortedDoerWorkItemPool<TSource, TResult>();
                    for (TSource i = initial; condition(i, relational, repeats); i = iterator(i, assignment, step))
                    {
                        sync.AddCount(1);
                        factory.Arg1 = i;
                        ISortedDoerWorkItem<TSource, TResult> work = SortedDoerWorkItem.Create(i, sync, ForStepCore, factory.Clone());
                        pool.ProcessWork(work);
                    }
                }
                finally
                {
                    sync.Signal();
                }
                sync.Wait(timeout);
            }
            finally
            {
                sync = null;
                factory = null;
            }
            return pool.Result;
        }

        private static TResult ForStepCore<TResult>(DoerFactory<TResult> factory)
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

        private static IReadOnlyCollection<TResult> ForEachCore<TSource, TResult>(DoerFactory<TSource, TResult> factory, IEnumerable<TSource> elements, TimeSpan timeout)
        {
            CountdownEventSlim sync = null;
            SortedDoerWorkItemPool<long, TResult> pool = null;
            try
            {
                try
                {
                    PartitionCollection<TSource> partition = new PartitionCollection<TSource>(elements);
                    pool = new SortedDoerWorkItemPool<long, TResult>();
                    sync = new CountdownEventSlim(1);
                    long sorter = 0;
                    while (partition.HasPartitions)
                    {
                        foreach (TSource element in partition)
                        {
                            sync.AddCount(1);
                            factory.Arg1 = element;
                            ISortedDoerWorkItem<long, TResult> work = SortedDoerWorkItem.Create(sorter, sync, ForEachElementCore, factory.Clone());
                            pool.ProcessWork(work);
                            sorter++;
                        }
                    }
                }
                finally
                {
                    sync.Signal();
                }
                sync.Wait(timeout);
            }
            finally
            {
                sync = null;
                factory = null;
            }
            return pool.Result;
        }

        private static TResult ForEachElementCore<TResult>(DoerFactory<TResult> factory)
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