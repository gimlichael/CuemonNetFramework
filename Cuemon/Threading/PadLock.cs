using System.Threading;
using Cuemon.Reflection;

namespace Cuemon.Threading
{
    /// <summary>
    /// Provides support for thread-safe initialization of <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type of object that is being thread-safe initialized.</typeparam>
    public class PadLock<T>
    {
        private T _value;
        private volatile bool _isValueCreated;
        private Doer<T> _factory;
        private readonly object _lock;

        /// <summary>
        /// Initializes a new instance of the <see cref="PadLock{T}"/> class.
        /// </summary>
        public PadLock()
            : this(ActivatorUtility.CreateInstance<T>)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PadLock{T}"/> class.
        /// </summary>
        /// <param name="factory">The delegate to offer thread-safe initialization.</param>
        public PadLock(Doer<T> factory)
        {
            Validator.ThrowIfNull(factory, nameof(factory));
            _lock = new object();
            _factory = factory;
        }

        /// <summary>
        /// Gets the thread-safe initialized value of the current <see cref="PadLock{T}"/> instance.
        /// </summary>
        /// <value>The thread-safe initialized value of the current <see cref="PadLock{T}"/> instance.</value>
        public T Value
        {
            get
            {
                lock (_lock)
                {
                    if (!_isValueCreated)
                    {
                        T value = _factory();
                        _factory = null;
                        _value = value;
                        _isValueCreated = true;
                    }
                }
                return _value;
            }
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        public override string ToString()
        {
            return Value.ToString();
        }
    }
}