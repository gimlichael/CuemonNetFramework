using System.Threading;

namespace Cuemon.Threading
{
    /// <summary>
    /// Specifies a set of features to apply on the <see cref="Thread"/> object created by the <see cref="ThreadUtility.StartNew(Act)"/> method.
    /// </summary>
    public sealed class ThreadSettings
    {
        /// <summary>
        /// Gets or sets a value indicating whether a <see cref="Thread"/> is initialized as a background thread.
        /// </summary>
        /// <value><c>true</c> if a <see cref="Thread"/> is initialized as a background thread; otherwise, <c>false</c>.</value>
        public bool Background { get; set; }

        /// <summary>
        /// Gets or sets the name of a <see cref="Thread"/>.
        /// </summary>
        /// <value>The name of a <see cref="Thread"/>.</value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets a value indicating the scheduling priority of a <see cref="Thread"/>.
        /// </summary>
        /// <value>One of the <see cref="ThreadPriority"/> values. The default value is <see cref="ThreadPriority.Normal"/>.</value>
        public ThreadPriority Priority { get; set; }
    }
}