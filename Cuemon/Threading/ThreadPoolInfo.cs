using System.Globalization;
using System.Threading;

namespace Cuemon.Threading
{
    /// <summary>
    /// Provides information about available threads in the <see cref="ThreadPool"/>.
    /// </summary>
    public sealed class ThreadPoolInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ThreadPoolInfo"/> class.
        /// </summary>
        internal ThreadPoolInfo()
        {
            int workerThreads, completionPortThreads;
            ThreadPool.GetAvailableThreads(out workerThreads, out completionPortThreads);
            
            ThreadPoolSettings settings = new ThreadPoolSettings();
            this.WorkerThreads = settings.MaximumWorkerThreads - workerThreads;
            this.AsynchronousCompletionThreads = settings.MaximumAsynchronousCompletionThreads - completionPortThreads;
        }

        /// <summary>
        /// Gets the number of available worker threads.
        /// </summary>
        /// <value>The number of available worker threads.</value>
        public int WorkerThreads { get; private set; }

        /// <summary>
        /// Gets the number of available asynchronous I/O threads.
        /// </summary>
        /// <value>The number of available asynchronous I/O threads.</value>
        public int AsynchronousCompletionThreads { get; private set; }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "WorkerThreads: {0}, AsynchronousCompletionThreads: {1}.", this.WorkerThreads, this.AsynchronousCompletionThreads);
        }
    }
}
