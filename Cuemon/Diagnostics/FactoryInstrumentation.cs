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
            ActFactory factory = new ActFactory(method);
            ExecuteActionCore(factory, caller);
        }

        public void ExecuteAction<T>(MethodBase caller, Act<T> method, T arg)
        {
            ActFactory<T> factory = new ActFactory<T>(method, arg);
            ExecuteActionCore(factory, caller);
        }

        public void ExecuteAction<T1, T2>(MethodBase caller, Act<T1, T2> method, T1 arg1, T2 arg2)
        {
            ActFactory<T1, T2> factory = new ActFactory<T1, T2>(method, arg1, arg2);
            ExecuteActionCore(factory, caller);
        }

        public void ExecuteAction<T1, T2, T3>(MethodBase caller, Act<T1, T2, T3> method, T1 arg1, T2 arg2, T3 arg3)
        {
            ActFactory<T1, T2, T3> factory = new ActFactory<T1, T2, T3>(method, arg1, arg2, arg3);
            ExecuteActionCore(factory, caller);
        }

        public void ExecuteAction<T1, T2, T3, T4>(MethodBase caller, Act<T1, T2, T3, T4> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            ActFactory<T1, T2, T3, T4> factory = new ActFactory<T1, T2, T3, T4>(method, arg1, arg2, arg3, arg4);
            ExecuteActionCore(factory, caller);
        }

        public void ExecuteAction<T1, T2, T3, T4, T5>(MethodBase caller, Act<T1, T2, T3, T4, T5> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        {
            ActFactory<T1, T2, T3, T4, T5> factory = new ActFactory<T1, T2, T3, T4, T5>(method, arg1, arg2, arg3, arg4, arg5);
            ExecuteActionCore(factory, caller);
        }

        public void ExecuteAction<T1, T2, T3, T4, T5, T6>(MethodBase caller, Act<T1, T2, T3, T4, T5, T6> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6)
        {
            ActFactory<T1, T2, T3, T4, T5, T6> factory = new ActFactory<T1, T2, T3, T4, T5, T6>(method, arg1, arg2, arg3, arg4, arg5, arg6);
            ExecuteActionCore(factory, caller);
        }

        public void ExecuteAction<T1, T2, T3, T4, T5, T6, T7>(MethodBase caller, Act<T1, T2, T3, T4, T5, T6, T7> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7)
        {
            ActFactory<T1, T2, T3, T4, T5, T6, T7> factory = new ActFactory<T1, T2, T3, T4, T5, T6, T7>(method, arg1, arg2, arg3, arg4, arg5, arg6, arg7);
            ExecuteActionCore(factory, caller);
        }

        public void ExecuteAction<T1, T2, T3, T4, T5, T6, T7, T8>(MethodBase caller, Act<T1, T2, T3, T4, T5, T6, T7, T8> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8)
        {
            ActFactory<T1, T2, T3, T4, T5, T6, T7, T8> factory = new ActFactory<T1, T2, T3, T4, T5, T6, T7, T8>(method, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8);
            ExecuteActionCore(factory, caller);
        }

        public void ExecuteAction<T1, T2, T3, T4, T5, T6, T7, T8, T9>(MethodBase caller, Act<T1, T2, T3, T4, T5, T6, T7, T8, T9> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9)
        {
            ActFactory<T1, T2, T3, T4, T5, T6, T7, T8, T9> factory = new ActFactory<T1, T2, T3, T4, T5, T6, T7, T8, T9>(method, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9);
            ExecuteActionCore(factory, caller);
        }

        public void ExecuteAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(MethodBase caller, Act<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10)
        {
            ActFactory<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> factory = new ActFactory<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(method, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10);
            ExecuteActionCore(factory, caller);
        }

        public void ExecuteAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(MethodBase caller, Act<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11)
        {
            ActFactory<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> factory = new ActFactory<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(method, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11);
            ExecuteActionCore(factory, caller);
        }

        public void ExecuteAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(MethodBase caller, Act<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12)
        {
            ActFactory<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> factory = new ActFactory<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(method, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12);
            ExecuteActionCore(factory, caller);
        }

        public void ExecuteAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(MethodBase caller, Act<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13)
        {
            ActFactory<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> factory = new ActFactory<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(method, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13);
            ExecuteActionCore(factory, caller);
        }

        public void ExecuteAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(MethodBase caller, Act<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14)
        {
            ActFactory<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> factory = new ActFactory<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(method, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14);
            ExecuteActionCore(factory, caller);
        }

        public void ExecuteAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(MethodBase caller, Act<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15)
        {
            ActFactory<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> factory = new ActFactory<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(method, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15);
            ExecuteActionCore(factory, caller);
        }

        public void ExecuteAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(MethodBase caller, Act<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, T16 arg16)
        {
            ActFactory<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16> factory = new ActFactory<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(method, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15, arg16);
            ExecuteActionCore(factory, caller);
        }

        public void ExecuteAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17>(MethodBase caller, Act<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, T16 arg16, T17 arg17)
        {
            ActFactory<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17> factory = new ActFactory<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17>(method, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15, arg16, arg17);
            ExecuteActionCore(factory, caller);
        }

        public void ExecuteAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18>(MethodBase caller, Act<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, T16 arg16, T17 arg17, T18 arg18)
        {
            ActFactory<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18> factory = new ActFactory<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18>(method, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15, arg16, arg17, arg18);
            ExecuteActionCore(factory, caller);
        }

        public void ExecuteAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19>(MethodBase caller, Act<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, T16 arg16, T17 arg17, T18 arg18, T19 arg19)
        {
            ActFactory<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19> factory = new ActFactory<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19>(method, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15, arg16, arg17, arg18, arg19);
            ExecuteActionCore(factory, caller);
        }

        public void ExecuteAction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20>(MethodBase caller, Act<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, T16 arg16, T17 arg17, T18 arg18, T19 arg19, T20 arg20)
        {
            ActFactory<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20> factory = new ActFactory<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20>(method, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15, arg16, arg17, arg18, arg19, arg20);
            ExecuteActionCore(factory, caller);
        }

        public TResult ExecuteFunction<TResult>(MethodBase caller, Doer<TResult> method)
        {
            DoerFactory<TResult> factory = new DoerFactory<TResult>(method);
            return ExecuteFunctionCore(factory, caller);
        }

        public TResult ExecuteFunction<T, TResult>(MethodBase caller, Doer<T, TResult> method, T arg)
        {
            DoerFactory<T, TResult> factory = new DoerFactory<T, TResult>(method, arg);
            return ExecuteFunctionCore(factory, caller);
        }

        public TResult ExecuteFunction<T1, T2, TResult>(MethodBase caller, Doer<T1, T2, TResult> method, T1 arg1, T2 arg2)
        {
            DoerFactory<T1, T2, TResult> factory = new DoerFactory<T1, T2, TResult>(method, arg1, arg2);
            return ExecuteFunctionCore(factory, caller);
        }

        public TResult ExecuteFunction<T1, T2, T3, TResult>(MethodBase caller, Doer<T1, T2, T3, TResult> method, T1 arg1, T2 arg2, T3 arg3)
        {
            DoerFactory<T1, T2, T3, TResult> factory = new DoerFactory<T1, T2, T3, TResult>(method, arg1, arg2, arg3);
            return ExecuteFunctionCore(factory, caller);
        }

        public TResult ExecuteFunction<T1, T2, T3, T4, TResult>(MethodBase caller, Doer<T1, T2, T3, T4, TResult> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            DoerFactory<T1, T2, T3, T4, TResult> factory = new DoerFactory<T1, T2, T3, T4, TResult>(method, arg1, arg2, arg3, arg4);
            return ExecuteFunctionCore(factory, caller);
        }

        public TResult ExecuteFunction<T1, T2, T3, T4, T5, TResult>(MethodBase caller, Doer<T1, T2, T3, T4, T5, TResult> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        {
            DoerFactory<T1, T2, T3, T4, T5, TResult> factory = new DoerFactory<T1, T2, T3, T4, T5, TResult>(method, arg1, arg2, arg3, arg4, arg5);
            return ExecuteFunctionCore(factory, caller);
        }

        public TResult ExecuteFunction<T1, T2, T3, T4, T5, T6, TResult>(MethodBase caller, Doer<T1, T2, T3, T4, T5, T6, TResult> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6)
        {
            DoerFactory<T1, T2, T3, T4, T5, T6, TResult> factory = new DoerFactory<T1, T2, T3, T4, T5, T6, TResult>(method, arg1, arg2, arg3, arg4, arg5, arg6);
            return ExecuteFunctionCore(factory, caller);
        }

        public TResult ExecuteFunction<T1, T2, T3, T4, T5, T6, T7, TResult>(MethodBase caller, Doer<T1, T2, T3, T4, T5, T6, T7, TResult> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7)
        {
            DoerFactory<T1, T2, T3, T4, T5, T6, T7, TResult> factory = new DoerFactory<T1, T2, T3, T4, T5, T6, T7, TResult>(method, arg1, arg2, arg3, arg4, arg5, arg6, arg7);
            return ExecuteFunctionCore(factory, caller);
        }

        public TResult ExecuteFunction<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(MethodBase caller, Doer<T1, T2, T3, T4, T5, T6, T7, T8, TResult> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8)
        {
            DoerFactory<T1, T2, T3, T4, T5, T6, T7, T8, TResult> factory = new DoerFactory<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(method, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8);
            return ExecuteFunctionCore(factory, caller);
        }

        public TResult ExecuteFunction<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>(MethodBase caller, Doer<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9)
        {
            DoerFactory<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult> factory = new DoerFactory<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>(method, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9);
            return ExecuteFunctionCore(factory, caller);
        }

        public TResult ExecuteFunction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>(MethodBase caller, Doer<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10)
        {
            DoerFactory<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult> factory = new DoerFactory<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>(method, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10);
            return ExecuteFunctionCore(factory, caller);
        }

        public TResult ExecuteFunction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult>(MethodBase caller, Doer<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11)
        {
            DoerFactory<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult> factory = new DoerFactory<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TResult>(method, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11);
            return ExecuteFunctionCore(factory, caller);
        }

        public TResult ExecuteFunction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult>(MethodBase caller, Doer<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12)
        {
            DoerFactory<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult> factory = new DoerFactory<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TResult>(method, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12);
            return ExecuteFunctionCore(factory, caller);
        }

        public TResult ExecuteFunction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>(MethodBase caller, Doer<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13)
        {
            DoerFactory<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult> factory = new DoerFactory<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TResult>(method, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13);
            return ExecuteFunctionCore(factory, caller);
        }

        public TResult ExecuteFunction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult>(MethodBase caller, Doer<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14)
        {
            DoerFactory<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult> factory = new DoerFactory<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TResult>(method, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14);
            return ExecuteFunctionCore(factory, caller);
        }

        public TResult ExecuteFunction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult>(MethodBase caller, Doer<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15)
        {
            DoerFactory<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult> factory = new DoerFactory<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TResult>(method, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15);
            return ExecuteFunctionCore(factory, caller);
        }

        public TResult ExecuteFunction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult>(MethodBase caller, Doer<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, T16 arg16)
        {
            DoerFactory<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult> factory = new DoerFactory<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TResult>(method, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15, arg16);
            return ExecuteFunctionCore(factory, caller);
        }

        public TResult ExecuteFunction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, TResult>(MethodBase caller, Doer<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, TResult> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, T16 arg16, T17 arg17)
        {
            DoerFactory<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, TResult> factory = new DoerFactory<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, TResult>(method, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15, arg16, arg17);
            return ExecuteFunctionCore(factory, caller);
        }

        public TResult ExecuteFunction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, TResult>(MethodBase caller, Doer<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, TResult> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, T16 arg16, T17 arg17, T18 arg18)
        {
            DoerFactory<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, TResult> factory = new DoerFactory<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, TResult>(method, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15, arg16, arg17, arg18);
            return ExecuteFunctionCore(factory, caller);
        }

        public TResult ExecuteFunction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, TResult>(MethodBase caller, Doer<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, TResult> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, T16 arg16, T17 arg17, T18 arg18, T19 arg19)
        {
            DoerFactory<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, TResult> factory = new DoerFactory<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, TResult>(method, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15, arg16, arg17, arg18, arg19);
            return ExecuteFunctionCore(factory, caller);
        }

        public TResult ExecuteFunction<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, TResult>(MethodBase caller, Doer<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, TResult> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10, T11 arg11, T12 arg12, T13 arg13, T14 arg14, T15 arg15, T16 arg16, T17 arg17, T18 arg18, T19 arg19, T20 arg20)
        {
            DoerFactory<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, TResult> factory = new DoerFactory<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19, T20, TResult>(method, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10, arg11, arg12, arg13, arg14, arg15, arg16, arg17, arg18, arg19, arg20);
            return ExecuteFunctionCore(factory, caller);
        }

        protected override void TimeMeasureCompletedHandling(string memberName, TimeSpan elapsed, IDictionary<string, object> data)
        {
            InstrumentationUtility.TimeMeasureCompleted(memberName, elapsed, data);
        }

        private void ExecuteActionCore(ActFactory factory, MethodBase caller)
        {
            Guid threadSafeId = Guid.NewGuid();
            try
            {
                this.OnMethodEntered(caller, threadSafeId);
                factory.ExecuteMethod();
            }
            catch (Exception ex)
            {
                throw ExceptionUtility.Refine(ex, caller, factory.GetGenericArguments());
            }
            finally
            {
                this.OnMethodExited(caller, threadSafeId, factory.GetGenericArguments());
            }
        }

        private TResult ExecuteFunctionCore<TResult>(DoerFactory<TResult> factory, MethodBase caller)
        {
            Guid threadSafeId = Guid.NewGuid();
            try
            {
                this.OnMethodEntered(caller, threadSafeId);
                return factory.ExecuteMethod();
            }
            catch (Exception ex)
            {
                throw ExceptionUtility.Refine(ex, caller, factory.GetGenericArguments());
            }
            finally
            {
                this.OnMethodExited(caller, threadSafeId, factory.GetGenericArguments());
            }
        }
    }
}
