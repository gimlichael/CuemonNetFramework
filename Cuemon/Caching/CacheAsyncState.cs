using System;
using System.Diagnostics;

namespace Cuemon.Caching
{
    internal class CacheAsyncState<TResult>
    {
        public CacheAsyncState(CacheCollection cache, string key, string group)
        {
            Cache = cache;
            Key = key;
            Group = group;
        }

        public CacheAsyncState<TResult> With(Doer<TResult> method)
        {
            EndInvoke = method.EndInvoke;
            return this;
        }

        public CacheAsyncState<TResult> With<T>(Doer<T, TResult> method)
        {
            EndInvoke = method.EndInvoke;
            return this;
        }

        public CacheAsyncState<TResult> With<T1, T2>(Doer<T1, T2, TResult> method)
        {
            EndInvoke = method.EndInvoke;
            return this;
        }

        public CacheAsyncState<TResult> With<T1, T2, T3>(Doer<T1, T2, T3, TResult> method)
        {
            EndInvoke = method.EndInvoke;
            return this;
        }

        public CacheAsyncState<TResult> With<T1, T2, T3, T4>(Doer<T1, T2, T3, T4, TResult> method)
        {
            EndInvoke = method.EndInvoke;
            return this;
        }

        public CacheAsyncState<TResult> With<T1, T2, T3, T4, T5>(Doer<T1, T2, T3, T4, T5, TResult> method)
        {
            EndInvoke = method.EndInvoke;
            return this;
        }

        public CacheAsyncState<TResult> With<T1, T2, T3, T4, T5, T6>(Doer<T1, T2, T3, T4, T5, T6, TResult> method)
        {
            EndInvoke = method.EndInvoke;
            return this;
        }

        public CacheAsyncState<TResult> With<T1, T2, T3, T4, T5, T6, T7>(Doer<T1, T2, T3, T4, T5, T6, T7, TResult> method)
        {
            EndInvoke = method.EndInvoke;
            return this;
        }

        public CacheAsyncState<TResult> With<T1, T2, T3, T4, T5, T6, T7, T8>(Doer<T1, T2, T3, T4, T5, T6, T7, T8, TResult> method)
        {
            EndInvoke = method.EndInvoke;
            return this;
        }

        public CacheAsyncState<TResult> With<T1, T2, T3, T4, T5, T6, T7, T8, T9>(Doer<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult> method)
        {
            EndInvoke = method.EndInvoke;
            return this;
        }

        public CacheAsyncState<TResult> With<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(Doer<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult> method)
        {
            EndInvoke = method.EndInvoke;
            return this;
        }

        public Doer<IAsyncResult, TResult> EndInvoke;

        public Template Tuple { get; set; }

        public string Key { get; set; }

        public string Group { get; set; }

        public CacheCollection Cache { get; set; }

        public static void Callback(IAsyncResult asyncResult)
        {
            try
            {
                CacheAsyncState<TResult> asyncState = asyncResult.AsyncState as CacheAsyncState<TResult>;
                if (asyncState == null) { return; }
                lock (asyncState.Cache)
                {
                    TResult result = asyncState.EndInvoke(asyncResult);
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