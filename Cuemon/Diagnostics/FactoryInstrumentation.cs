using System;
using System.Collections.Generic;
using System.Reflection;

namespace Cuemon.Diagnostics
{
    /// <summary>
    /// A factory implementation of the <see cref="Instrumentation"/> class that favors diagnostics, monitoring and measuring performance through <see cref="Act"/> and <see cref="Doer{TResult}"/> overloads.
    /// </summary>
    internal class FactoryInstrumentation : Instrumentation
    {
        internal FactoryInstrumentation() : base(true, false)
        {
        }

        public void ExecuteAction(MethodBase caller, Act method)
        {
            var factory = ActFactory.Create(method);
            ExecuteActionCore(factory, caller);
        }

        public void ExecuteAction<T>(MethodBase caller, Act<T> method, T arg)
        {
            var factory = ActFactory.Create(method, arg);
            ExecuteActionCore(factory, caller);
        }

        public void ExecuteAction<T1, T2>(MethodBase caller, Act<T1, T2> method, T1 arg1, T2 arg2)
        {
            var factory = ActFactory.Create(method, arg1, arg2);
            ExecuteActionCore(factory, caller);
        }

        public void ExecuteAction<T1, T2, T3>(MethodBase caller, Act<T1, T2, T3> method, T1 arg1, T2 arg2, T3 arg3)
        {
            var factory = ActFactory.Create(method, arg1, arg2, arg3);
            ExecuteActionCore(factory, caller);
        }

        public void ExecuteAction<T1, T2, T3, T4>(MethodBase caller, Act<T1, T2, T3, T4> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            var factory = ActFactory.Create(method, arg1, arg2, arg3, arg4);
            ExecuteActionCore(factory, caller);
        }

        public void ExecuteAction<T1, T2, T3, T4, T5>(MethodBase caller, Act<T1, T2, T3, T4, T5> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        {
            var factory = ActFactory.Create(method, arg1, arg2, arg3, arg4, arg5);
            ExecuteActionCore(factory, caller);
        }

        public void ExecuteAction<T1, T2, T3, T4, T5, T6>(MethodBase caller, Act<T1, T2, T3, T4, T5, T6> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6)
        {
            var factory = ActFactory.Create(method, arg1, arg2, arg3, arg4, arg5, arg6);
            ExecuteActionCore(factory, caller);
        }

        public void ExecuteAction<T1, T2, T3, T4, T5, T6, T7>(MethodBase caller, Act<T1, T2, T3, T4, T5, T6, T7> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7)
        {
            var factory = ActFactory.Create(method, arg1, arg2, arg3, arg4, arg5, arg6, arg7);
            ExecuteActionCore(factory, caller);
        }

        public void ExecuteAction<T1, T2, T3, T4, T5, T6, T7, T8>(MethodBase caller, Act<T1, T2, T3, T4, T5, T6, T7, T8> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8)
        {
            var factory = ActFactory.Create(method, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8);
            ExecuteActionCore(factory, caller);
        }

        public void ExecuteAction<T1, T2, T3, T4, T5, T6, T7, T8, T9>(MethodBase caller, Act<T1, T2, T3, T4, T5, T6, T7, T8, T9> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9)
        {
            var factory = ActFactory.Create(method, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9);
            ExecuteActionCore(factory, caller);
        }

        public void ExecuteAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(MethodBase caller, Act<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10)
        {
            var factory = ActFactory.Create(method, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10);
            ExecuteActionCore(factory, caller);
        }

        public void ExecuteAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(MethodBase caller, Act<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11)
        {
            var factory = ActFactory.Create(method, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11);
            ExecuteActionCore(factory, caller);
        }

        public void ExecuteAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(MethodBase caller, Act<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12)
        {
            var factory = ActFactory.Create(method, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12);
            ExecuteActionCore(factory, caller);
        }

        public void ExecuteAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(MethodBase caller, Act<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13)
        {
            var factory = ActFactory.Create(method, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13);
            ExecuteActionCore(factory, caller);
        }

        public void ExecuteAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(MethodBase caller, Act<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14)
        {
            var factory = ActFactory.Create(method, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14);
            ExecuteActionCore(factory, caller);
        }

        public void ExecuteAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(MethodBase caller, Act<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15)
        {
            var factory = ActFactory.Create(method, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15);
            ExecuteActionCore(factory, caller);
        }

        public void ExecuteAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(MethodBase caller, Act<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, T16 arg16)
        {
            var factory = ActFactory.Create(method, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15, arg16);
            ExecuteActionCore(factory, caller);
        }

        public void ExecuteAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17>(MethodBase caller, Act<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, T16 arg16, T17 arg17)
        {
            var factory = ActFactory.Create(method, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15, arg16, arg17);
            ExecuteActionCore(factory, caller);
        }

        public void ExecuteAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18>(MethodBase caller, Act<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, T16 arg16, T17 arg17, T18 arg18)
        {
            var factory = ActFactory.Create(method, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15, arg16, arg17, arg18);
            ExecuteActionCore(factory, caller);
        }

        public void ExecuteAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19>(MethodBase caller, Act<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, T16 arg16, T17 arg17, T18 arg18, T19 arg19)
        {
            var factory = ActFactory.Create(method, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15, arg16, arg17, arg18, arg19);
            ExecuteActionCore(factory, caller);
        }

        public void ExecuteAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20>(MethodBase caller, Act<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, T16 arg16, T17 arg17, T18 arg18, T19 arg19, T20 arg20)
        {
            var factory = ActFactory.Create(method, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15, arg16, arg17, arg18, arg19, arg20);
            ExecuteActionCore(factory, caller);
        }

        public TResult ExecuteFunction<TResult>(MethodBase caller, Doer<TResult> method)
        {
            var factory = DoerFactory.Create(method);
            return ExecuteFunctionCore(factory, caller);
        }

        public TResult ExecuteFunction<T, TResult>(MethodBase caller, Doer<T, TResult> method, T arg)
        {
            var factory = DoerFactory.Create(method, arg);
            return ExecuteFunctionCore(factory, caller);
        }

        public TResult ExecuteFunction<T1, T2, TResult>(MethodBase caller, Doer<T1, T2, TResult> method, T1 arg1, T2 arg2)
        {
            var factory = DoerFactory.Create(method, arg1, arg2);
            return ExecuteFunctionCore(factory, caller);
        }

        public TResult ExecuteFunction<T1, T2, T3, TResult>(MethodBase caller, Doer<T1, T2, T3, TResult> method, T1 arg1, T2 arg2, T3 arg3)
        {
            var factory = DoerFactory.Create(method, arg1, arg2, arg3);
            return ExecuteFunctionCore(factory, caller);
        }

        public TResult ExecuteFunction<T1, T2, T3, T4, TResult>(MethodBase caller, Doer<T1, T2, T3, T4, TResult> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            var factory = DoerFactory.Create(method, arg1, arg2, arg3, arg4);
            return ExecuteFunctionCore(factory, caller);
        }

        public TResult ExecuteFunction<T1, T2, T3, T4, T5, TResult>(MethodBase caller, Doer<T1, T2, T3, T4, T5, TResult> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        {
            var factory = DoerFactory.Create(method, arg1, arg2, arg3, arg4, arg5);
            return ExecuteFunctionCore(factory, caller);
        }

        public TResult ExecuteFunction<T1, T2, T3, T4, T5, T6, TResult>(MethodBase caller, Doer<T1, T2, T3, T4, T5, T6, TResult> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6)
        {
            var factory = DoerFactory.Create(method, arg1, arg2, arg3, arg4, arg5, arg6);
            return ExecuteFunctionCore(factory, caller);
        }

        public TResult ExecuteFunction<T1, T2, T3, T4, T5, T6, T7, TResult>(MethodBase caller, Doer<T1, T2, T3, T4, T5, T6, T7, TResult> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7)
        {
            var factory = DoerFactory.Create(method, arg1, arg2, arg3, arg4, arg5, arg6, arg7);
            return ExecuteFunctionCore(factory, caller);
        }

        public TResult ExecuteFunction<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(MethodBase caller, Doer<T1, T2, T3, T4, T5, T6, T7, T8, TResult> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8)
        {
            var factory = DoerFactory.Create(method, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8);
            return ExecuteFunctionCore(factory, caller);
        }

        public TResult ExecuteFunction<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>(MethodBase caller, Doer<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9)
        {
            var factory = DoerFactory.Create(method, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9);
            return ExecuteFunctionCore(factory, caller);
        }

        public TResult ExecuteFunction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>(MethodBase caller, Doer<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10)
        {
            var factory = DoerFactory.Create(method, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10);
            return ExecuteFunctionCore(factory, caller);
        }

        public TResult ExecuteFunction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult>(MethodBase caller, Doer<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11)
        {
            var factory = DoerFactory.Create(method, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11);
            return ExecuteFunctionCore(factory, caller);
        }

        public TResult ExecuteFunction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult>(MethodBase caller, Doer<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12)
        {
            var factory = DoerFactory.Create(method, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12);
            return ExecuteFunctionCore(factory, caller);
        }

        public TResult ExecuteFunction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>(MethodBase caller, Doer<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13)
        {
            var factory = DoerFactory.Create(method, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13);
            return ExecuteFunctionCore(factory, caller);
        }

        public TResult ExecuteFunction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult>(MethodBase caller, Doer<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14)
        {
            var factory = DoerFactory.Create(method, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14);
            return ExecuteFunctionCore(factory, caller);
        }

        public TResult ExecuteFunction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult>(MethodBase caller, Doer<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15)
        {
            var factory = DoerFactory.Create(method, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15);
            return ExecuteFunctionCore(factory, caller);
        }

        public TResult ExecuteFunction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult>(MethodBase caller, Doer<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, T16 arg16)
        {
            var factory = DoerFactory.Create(method, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15, arg16);
            return ExecuteFunctionCore(factory, caller);
        }

        public TResult ExecuteFunction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, TResult>(MethodBase caller, Doer<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, TResult> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, T16 arg16, T17 arg17)
        {
            var factory = DoerFactory.Create(method, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15, arg16, arg17);
            return ExecuteFunctionCore(factory, caller);
        }

        public TResult ExecuteFunction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, TResult>(MethodBase caller, Doer<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, TResult> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, T16 arg16, T17 arg17, T18 arg18)
        {
            var factory = DoerFactory.Create(method, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15, arg16, arg17, arg18);
            return ExecuteFunctionCore(factory, caller);
        }

        public TResult ExecuteFunction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, TResult>(MethodBase caller, Doer<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, TResult> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, T16 arg16, T17 arg17, T18 arg18, T19 arg19)
        {
            var factory = DoerFactory.Create(method, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15, arg16, arg17, arg18, arg19);
            return ExecuteFunctionCore(factory, caller);
        }

        public TResult ExecuteFunction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, TResult>(MethodBase caller, Doer<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, TResult> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, T16 arg16, T17 arg17, T18 arg18, T19 arg19, T20 arg20)
        {
            var factory = DoerFactory.Create(method, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15, arg16, arg17, arg18, arg19, arg20);
            return ExecuteFunctionCore(factory, caller);
        }

        protected override void TimeMeasureCompletedHandling(string memberName, TimeSpan elapsed, IDictionary<string, object> data)
        {
            InstrumentationUtility.TimeMeasureCompleted(memberName, elapsed, data);
        }

        private void ExecuteActionCore<TTuple>(ActFactory<TTuple> factory, MethodBase caller) where TTuple : Template
        {
            Guid threadSafeId = Guid.NewGuid();
            try
            {
                OnMethodEntered(caller, threadSafeId);
                factory.ExecuteMethod();
            }
            catch (Exception ex)
            {
                throw ExceptionUtility.Refine(ex, caller, factory.GenericArguments.ToArray());
            }
            finally
            {
                OnMethodExited(caller, threadSafeId, factory.GenericArguments.ToArray());
            }
        }

        private TResult ExecuteFunctionCore<TTuple, TResult>(DoerFactory<TTuple, TResult> factory, MethodBase caller) where TTuple : Template
        {
            Guid threadSafeId = Guid.NewGuid();
            try
            {
                OnMethodEntered(caller, threadSafeId);
                return factory.ExecuteMethod();
            }
            catch (Exception ex)
            {
                throw ExceptionUtility.Refine(ex, caller, factory.GenericArguments.ToArray());
            }
            finally
            {
                OnMethodExited(caller, threadSafeId, factory.GenericArguments.ToArray());
            }
        }
    }
}