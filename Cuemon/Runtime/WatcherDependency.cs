using System;
using System.Collections.Generic;

namespace Cuemon.Runtime
{
    /// <summary>
    /// Provides the natural coupling between a <see cref="Dependency"/> and a <see cref="Watcher"/> object.
    /// </summary>
    /// <seealso cref="Dependency" />.
    public class WatcherDependency : Dependency
    {
        private readonly object _locker = new object();

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="WatcherDependency"/> class.
        /// </summary>
        /// <param name="watchersCallback">The callback function delegate that will retrieve a sequence of <see cref="Watcher"/> objects.</param>
        public WatcherDependency(Doer<IEnumerable<Watcher>> watchersCallback)
        {
            Validator.ThrowIfNull(watchersCallback, nameof(watchersCallback));
            WatchersCallback = watchersCallback;
        }

        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the watchers associated with this <see cref="Dependency"/>.
        /// </summary>
        /// <value>The watchers of this <see cref="Dependency"/>.</value>
        protected IEnumerable<Watcher> Watchers { get; set; }

        /// <summary>
        /// Gets or sets the timestamp in Coordinated Universal Time (UTC) (also known as Greenwich Mean Time) of this <see cref="Dependency"/>.
        /// </summary>
        /// <value>The UTC timestamp of when this <see cref="Dependency"/> was started.</value>
        protected DateTime UtcTimestamp { get; set; }

        /// <summary>
        /// Gets a value indicating whether the <see cref="Dependency"/> object has changed.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if the <see cref="Dependency"/> object has changed; otherwise, <c>false</c>.
        /// </value>
        public override bool HasChanged
        {
            get { return UtcLastModified > UtcTimestamp; }
        }
        #endregion

        #region Methods
        private Doer<IEnumerable<Watcher>> WatchersCallback { get; set; }

        /// <summary>
        /// Starts and performs the necessary dependency tasks of this instance.
        /// </summary>
        public override void Start()
        {
            var temp = WatchersCallback?.Invoke();
            if (temp != null)
            {
                var watchers = new List<Watcher>(temp);
                foreach (var watcher in watchers)
                {
                    watcher.Changed += WatcherChanged;
                }
                Watchers = watchers;
            }
            UtcTimestamp = DateTime.UtcNow;
            SetUtcLastModified(UtcTimestamp);
        }

        /// <summary>
        /// The method that is invoked when one or more <see cref="Watchers"/> has changed.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args">The <see cref="WatcherEventArgs"/> instance containing the event data.</param>
        protected void WatcherChanged(object sender, WatcherEventArgs args)
        {
            SetUtcLastModified(DateTime.UtcNow);
            if (!HasChanged) { return; }
            if (Watchers != null)
            {
                lock (_locker)
                {
                    if (Watchers != null)
                    {
                        foreach (var watcher in Watchers)
                        {
                            watcher.Changed -= WatcherChanged;
                            watcher.Dispose();
                        }
                    }
                    Watchers = null;
                }
            }
            OnDependencyChangedRaised(new DependencyEventArgs(UtcLastModified, args.ToString()));
        }
        #endregion
    }
}