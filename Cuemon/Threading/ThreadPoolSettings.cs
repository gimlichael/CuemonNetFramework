using System.Globalization;
using System.Threading;

namespace Cuemon.Threading
{
    /// <summary>
    /// Specifies a set of features to apply directly on the <see cref="ThreadPool"/> object.
    /// </summary>
    public sealed class ThreadPoolSettings
    {
        private readonly ThreadPoolSettings Original = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="ThreadPoolSettings"/> class.
        /// </summary>
        public ThreadPoolSettings()
        {
            int maxWorkerThreads, minWorkerThreads, maxCompletionPortThreads, minCompletionPortThreads;
            
            ThreadPool.GetMaxThreads(out maxWorkerThreads, out maxCompletionPortThreads);
            ThreadPool.GetMinThreads(out minWorkerThreads, out minCompletionPortThreads);

            this.MaximumAsynchronousCompletionThreads = maxCompletionPortThreads;
            this.MaximumWorkerThreads = maxWorkerThreads;
            this.MinimumAsynchronousCompletionThreads = minCompletionPortThreads;
            this.MinimumWorkerThreads = minWorkerThreads;

            this.Original = new ThreadPoolSettings(maxWorkerThreads, minWorkerThreads, maxCompletionPortThreads, minCompletionPortThreads);
        }

        private ThreadPoolSettings(int maxWorkerThreads, int minWorkerThreads, int maxCompletionPortThreads, int minCompletionPortThreads)
        {
            this.MaximumAsynchronousCompletionThreads = maxCompletionPortThreads;
            this.MaximumWorkerThreads = maxWorkerThreads;
            this.MinimumAsynchronousCompletionThreads = minCompletionPortThreads;
            this.MinimumWorkerThreads = minWorkerThreads;
        }

        private bool RollbackWasCalled { get; set; }

        /// <summary>
        /// Gets or sets the maximum number of worker threads in the thread pool.
        /// </summary>
        /// <value>The maximum number of worker threads in the thread pool.</value>
        public int MaximumWorkerThreads { get; set; }

        /// <summary>
        /// Gets or sets the maximum number of asynchronous I/O threads in the thread pool.
        /// </summary>
        /// <value>The maximum number of asynchronous I/O threads in the thread pool.</value>
        public int MaximumAsynchronousCompletionThreads { get; set; }

        /// <summary>
        /// Gets or sets the minimum number of worker threads that the thread pool creates on demand.
        /// </summary>
        /// <value>The minimum number of worker threads that the thread pool creates on demand.</value>
        public int MinimumWorkerThreads { get; set; }

        /// <summary>
        /// Gets or sets the minimum number of asynchronous I/O threads that the thread pool creates on demand.
        /// </summary>
        /// <value>The minimum number of asynchronous I/O threads that the thread pool creates on demand.</value>
        public int MinimumAsynchronousCompletionThreads { get; set; }

        /// <summary>
        /// Commit the changes on this instance directly to the <see cref="ThreadPool"/>.
        /// </summary>
        public void Commit()
        {
            ThreadPool.SetMaxThreads(this.MaximumWorkerThreads, this.MaximumAsynchronousCompletionThreads);
            ThreadPool.SetMinThreads(this.MinimumWorkerThreads, this.MinimumAsynchronousCompletionThreads);
        }

        /// <summary>
        /// Restore the original values of this instance directly to the <see cref="ThreadPool"/>.
        /// </summary>
        public void Rollback()
        {
            if (this.RollbackWasCalled) { return; }
            ThreadPool.SetMaxThreads(Original.MaximumWorkerThreads, Original.MaximumAsynchronousCompletionThreads);
            ThreadPool.SetMinThreads(Original.MinimumWorkerThreads, Original.MinimumAsynchronousCompletionThreads);
            this.RollbackWasCalled = true;
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "MaximumAsynchronousCompletionThreads: {0}, MaximumWorkerThreads: {1}, MinimumAsynchronousCompletionThreads: {2}, MinimumWorkerThreads: {3}.", this.MaximumAsynchronousCompletionThreads, this.MaximumWorkerThreads, this.MinimumAsynchronousCompletionThreads, this.MinimumWorkerThreads);
        }
    }
}
