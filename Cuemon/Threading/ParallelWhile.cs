using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Cuemon.Threading
{
    internal class ParallelWhile<TSource>
    {
        public static int NumberOfParallelTasks;

        static ParallelWhile()
        {
            NumberOfParallelTasks = Environment.ProcessorCount;
        }

        public static void While<T>(IEnumerator<T> enumerator, Action<T> action)
        {
            var syncRoot = new object();
            InvokeAsync<T> del = InvokeAction;

            var seedItemArray = new T[NumberOfParallelTasks];
            var resultList = new List<IAsyncResult>(NumberOfParallelTasks);

            for (int i = 0; i < NumberOfParallelTasks; i++)
            {
                bool moveNext;

                lock (syncRoot)
                {
                    moveNext = enumerator.MoveNext();
                    seedItemArray[i] = enumerator.Current;
                }

                if (moveNext)
                {
                    var iAsyncResult = del.BeginInvoke(enumerator, action, seedItemArray[i], syncRoot, i, null, null);
                    resultList.Add(iAsyncResult);
                }
            }

            foreach (var iAsyncResult in resultList)
            {
                del.EndInvoke(iAsyncResult);
                iAsyncResult.AsyncWaitHandle.Close();
            }
        }

        public static void ForEach<T>(IEnumerable<T> enumerable, Action<T> action)
        {
            if (enumerable == null) return;

            IEnumerator<T> enumerator = enumerable.GetEnumerator();
            While(enumerator, action);

        }

        delegate void InvokeAsync<T>(IEnumerator<T> enumerator, Action<T> achtion, T item, object syncRoot, int i);

        static void InvokeAction<T>(IEnumerator<T> enumerator, Action<T> action, T item, object syncRoot, int i)
        {
            if (String.IsNullOrEmpty(Thread.CurrentThread.Name))
                Thread.CurrentThread.Name =
            String.Format("Parallel.ForEach Worker Thread No:{0}", i);

            bool moveNext = true;

            while (moveNext)
            {
                action.Invoke(item);

                lock (syncRoot)
                {
                    moveNext = enumerator.MoveNext();
                    item = enumerator.Current;
                }
            }
        }
    }
}