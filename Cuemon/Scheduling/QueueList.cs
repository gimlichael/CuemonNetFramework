using System;
using System.Collections.Generic;
using System.Text;

namespace Cuemon.Messaging
{
    internal sealed class QueueList<T> : List<T>
    {
        private readonly Action<T> _method;
        private readonly int _maxQueueSize;
        private readonly int _chunkSize;

        #region Contructors
        internal QueueList(Action<T> method) : this(method, 256)
        {
        }

        internal QueueList(Action<T> method, int maxQueueSize) : this(method, maxQueueSize, int.MaxValue)
        {
            _maxQueueSize = maxQueueSize;
        }

        internal QueueList(Action<T> method, int maxQueueSize, int chunkSize)
        {
            _method = method;
            _maxQueueSize = maxQueueSize;
            _chunkSize = chunkSize;
        }
        #endregion

        #region Properties
        public bool HasReachedQueueLimit
        {
            get { return this.Count >= this.MaxQueueSize; }
        }

        public int MaxQueueSize
        {
            get { return _maxQueueSize; }
        }

        public int ChunkSize
        {
            get { return _chunkSize; }
        }

        public Action<T> Method
        {
            get { return _method; }
        }
        #endregion
    }
}
