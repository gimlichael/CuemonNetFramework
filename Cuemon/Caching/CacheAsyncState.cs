using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Cuemon.Caching
{
    internal class CacheAsyncState<TResult>
    {
        public CacheAsyncState(CacheCollection cache, string key, string group)
        {
            this.Cache = cache;
            this.Key = key;
            this.Group = group;
        }

        public CacheAsyncState<TResult> With(Doer<TResult> method)
        {
            this.Doer = new DoerFactory<TResult>(method);
            return this;
        }

        public CacheAsyncState<TResult> With<T>(Doer<T, TResult> method, T arg)
        {
            this.Doer = new DoerFactory<T, TResult>(method, arg);
            return this;
        }

        public CacheAsyncState<TResult> With<T1, T2>(Doer<T1, T2, TResult> method, T1 arg1, T2 arg2)
        {
            this.Doer = new DoerFactory<T1, T2, TResult>(method, arg1, arg2);
            return this;
        }

        public CacheAsyncState<TResult> With<T1, T2, T3>(Doer<T1, T2, T3, TResult> method, T1 arg1, T2 arg2, T3 arg3)
        {
            this.Doer = new DoerFactory<T1, T2, T3, TResult>(method, arg1, arg2, arg3);
            return this;
        }

        public CacheAsyncState<TResult> With<T1, T2, T3, T4>(Doer<T1, T2, T3, T4, TResult> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            this.Doer = new DoerFactory<T1, T2, T3, T4, TResult>(method, arg1, arg2, arg3, arg4);
            return this;
        }

        public CacheAsyncState<TResult> With<T1, T2, T3, T4, T5>(Doer<T1, T2, T3, T4, T5, TResult> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        {
            this.Doer = new DoerFactory<T1, T2, T3, T4, T5, TResult>(method, arg1, arg2, arg3, arg4, arg5);
            return this;
        }

        public CacheAsyncState<TResult> With<T1, T2, T3, T4, T5, T6>(Doer<T1, T2, T3, T4, T5, T6, TResult> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6)
        {
            this.Doer = new DoerFactory<T1, T2, T3, T4, T5, T6, TResult>(method, arg1, arg2, arg3, arg4, arg5, arg6);
            return this;
        }

        public CacheAsyncState<TResult> With<T1, T2, T3, T4, T5, T6, T7>(Doer<T1, T2, T3, T4, T5, T6, T7, TResult> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7)
        {
            this.Doer = new DoerFactory<T1, T2, T3, T4, T5, T6, T7, TResult>(method, arg1, arg2, arg3, arg4, arg5, arg6, arg7);
            return this;
        }

        public CacheAsyncState<TResult> With<T1, T2, T3, T4, T5, T6, T7, T8>(Doer<T1, T2, T3, T4, T5, T6, T7, T8, TResult> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8)
        {
            this.Doer = new DoerFactory<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(method, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8);
            return this;
        }

        public CacheAsyncState<TResult> With<T1, T2, T3, T4, T5, T6, T7, T8, T9>(Doer<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9)
        {
            this.Doer = new DoerFactory<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>(method, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9);
            return this;
        }

        public CacheAsyncState<TResult> With<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(Doer<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult> method, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10)
        {
            this.Doer = new DoerFactory<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>(method, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10);
            return this;
        }

        public string Key { get; set; }

        public string Group { get; set; }

        public CacheCollection Cache { get; set; }

        public DoerFactory<TResult> Doer { get; set; }

        public static void Callback(IAsyncResult asyncResult)
        {
            try
            {
                CacheAsyncState<TResult> asyncState = asyncResult.AsyncState as CacheAsyncState<TResult>;
                if (asyncState == null) { return; }
                lock (asyncState.Cache)
                {
                    TResult result = asyncState.Doer.EndExecuteMethod(asyncResult);
                    asyncState.Cache.Set(asyncState.Key, asyncState.Group, result);
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message);
            }
        }
    }
}
