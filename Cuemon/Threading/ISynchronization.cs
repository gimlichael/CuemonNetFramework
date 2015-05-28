namespace Cuemon.Threading
{
    /// <summary>
    /// Provides asynchronous communication between objects about events.
    /// </summary>
    public interface ISynchronization
    {
        /// <summary>
        /// Registers a signal on the object implementing the <see cref="ISynchronization"/> interface.
        /// </summary>
        void Signal();

        /// <summary>
        /// Registers multiple signals on the object implementing the <see cref="ISynchronization"/> interface, decrementing the value by the specified <paramref name="count"/> amount.
        /// </summary>
        void Signal(int count);
    }
}
