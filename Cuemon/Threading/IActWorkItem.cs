namespace Cuemon.Threading
{
	/// <summary>
    /// Defines a way to control work operations on a per thread basis.
	/// </summary>
	public interface IActWorkItem : IData
	{
		/// <summary>
        /// Gets an instance of the <see cref="ISynchronization"/> object used for threaded synchronization signaling.
		/// </summary>
		ISynchronization Synchronization { get; }

		/// <summary>
        /// The work to be processed by a pool of threads.
		/// </summary>
		void ProcessWork();
	}
}