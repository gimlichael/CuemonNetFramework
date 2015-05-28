using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Cuemon.Collections.Generic;

namespace Cuemon.Messaging
{
    public static class InvokerQueue
    {
        private static readonly IDictionary<string, object> QueuedValues = new Dictionary<string, object>();

        #region Constructors
        #endregion

        #region Properties
        #endregion

        #region Methods
        public static void Add<T>(string key, T value, Action<T> method)
        {
            InvokerQueue.Add(key, value, method, 256);
        }

        public static void Add<T>(string key, T value, Action<T> method, int maxQueueSize)
        {
            InvokerQueue.Add(key, value, method, maxQueueSize, 128);
        }

        public static void Add<T>(string key, T value, Action<T> method, int maxQueueSize, int chunkSize)
        {
            if (key == null) { throw new ArgumentNullException("key"); }
            if (value == null) { throw new ArgumentNullException("value"); }
            if (!QueuedValues.ContainsKey(key))
            {
                lock (QueuedValues)
                {
                    if (!QueuedValues.ContainsKey(key))
                    {
                        QueuedValues.Add(key, new QueueList<T>(method, maxQueueSize, chunkSize));
                    }
                    InvokerQueue.HandleQueuedValues<T>(key);
                    ((QueueList<T>)QueuedValues[key]).Add(value);
                }
            }
        }

        private static void HandleQueuedValues<T>(string key)
        {
            QueueList<T> queuedValues = ((QueueList<T>)QueuedValues[key]);
            if (queuedValues.HasReachedQueueLimit)
            {
                IEnumerable<T> enumerableQueuedValues = queuedValues;
                while (EnumerableUtility.Any(enumerableQueuedValues))
                {
                    IEnumerator<T> chunkedQueuedValues = EnumerableUtility.Chunk(ref enumerableQueuedValues, queuedValues.ChunkSize).GetEnumerator();
                    while (chunkedQueuedValues.MoveNext())
                    {
                        queuedValues.Method(chunkedQueuedValues.Current);
                    }
                    InvokerQueue.Clear<T>(key);
                }
            }
        }

        public static bool Remove<T>(string key)
        {
            if (key == null) { throw new ArgumentNullException("key"); }
            lock (QueuedValues)
            {
                if (QueuedValues.ContainsKey(key))
                {
                    QueueList<T> values = QueuedValues[key] as QueueList<T>;
                    if (values != null)
                    {
                        if (values.Count > 0) { InvokerQueue.HandleQueuedValues<T>(key); }
                    }
                }
                return QueuedValues.Remove(key);
            }
        }

        public static void Clear()
        {
            QueuedValues.Clear();
        }

        public static void Clear<T>(string key)
        {
            if (key == null) { throw new ArgumentNullException("key"); }
            if (QueuedValues.ContainsKey(key))
            {
                lock (QueuedValues)
                {
                    if (QueuedValues.ContainsKey(key))
                    {
                        ((IList)QueuedValues[key]).Clear();
                    }
                }
            }
        }

        public static IList<T> Get<T>(string key)
        {
            if (key == null) { throw new ArgumentNullException("key"); }
            lock (QueuedValues)
            {
                if (QueuedValues.ContainsKey(key)) { return QueuedValues[key] as IList<T>; }
                return new List<T>();
            }
        }

        public static bool TryGet<T>(string key, out IEnumerable<T> queuedValues)
        {
            if (key == null) { throw new ArgumentNullException("key"); }
            lock (QueuedValues)
            {
                object tryQueuedValues;
                if (QueuedValues.TryGetValue(key, out tryQueuedValues))
                {
                    queuedValues = tryQueuedValues as IList<T>;
                    if (queuedValues == null)  { goto empty; }
                    return true;
                }
            }
            empty:
            queuedValues = new List<T>();
            return false;
        }
        #endregion
    }
}