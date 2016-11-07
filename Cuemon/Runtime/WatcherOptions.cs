using System;

namespace Cuemon.Runtime
{
    /// <summary>
    /// Specifies options for the <see cref="Watcher"/>.
    /// </summary>
    public class WatcherOptions
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WatcherOptions"/> class to support the <see cref="Watcher"/> object.
        /// </summary>
        /// <remarks>
        /// The following table shows the initial property values for an instance of <see cref="WatcherOptions"/>.
        /// <list type="table">
        ///     <listheader>
        ///         <term>Property</term>
        ///         <description>Initial Value</description>
        ///     </listheader>
        ///     <item>
        ///         <term><see cref="Period"/></term>
        ///         <description><c>Every 2 minutes</c></description>
        ///     </item>
        ///     <item>
        ///         <term><see cref="DueTime"/></term>
        ///         <description><c>Delay 15 seconds</c></description>
        ///     </item>
        ///     <item>
        ///         <term><see cref="PostponeChangedEvent"/></term>
        ///         <description><c>Disabled</c></description>
        ///     </item>
        /// </list>
        /// </remarks>
        public WatcherOptions()
        {
            Period = TimeSpan.FromMinutes(2);
            DueTime = TimeSpan.FromSeconds(15);
            PostponeChangedEvent = TimeSpan.Zero;
        }

        /// <summary>
        /// Gets or sets the <see cref="TimeSpan"/> representing the amount of time to delay before the <see cref="Watcher"/> starts signaling. Specify negative one (-1) milliseconds to prevent the signaling from starting. Specify zero (0) to start the signaling immediately.
        /// </summary>
        /// <value>A <see cref="TimeSpan"/> representing the amount of time to delay before the <see cref="Watcher"/> starts signaling.</value>
        public TimeSpan DueTime { get; set; }

        /// <summary>
        /// Gets the time interval between periodic signaling. Specify negative one (-1) milliseconds to disable periodic signaling.
        /// </summary>
        /// <value>A <see cref="TimeSpan"/> representing the time interval between periodic signaling.</value>
        public TimeSpan Period { get; set; }

        /// <summary>
        /// Gets the amount of time to postpone a <see cref="Watcher.Changed" /> event. Specify zero (0) to disable postponing.
        /// </summary>
        /// <value>A <see cref="TimeSpan"/> representing the amount of time to postpone a <see cref="Watcher.Changed" /> event.</value>
        public TimeSpan PostponeChangedEvent { get; set; }

        /// <summary>
        /// Gets or sets the watcher implementation callback delegate.
        /// </summary>
        /// <value>The watcher implementation callback delegate.</value>
        public Act<object> WatcherImplementationCallback { get; set; }
    }
}