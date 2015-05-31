using System;
using System.Collections.Generic;
using System.IO;

namespace Cuemon.IO
{
    /// <summary>
    /// This <see cref="FileDependency"/> class will monitor any changes occurred to files or directories while notifying subscribing objects.
    /// </summary>
    public sealed class FileDependency : Dependency
    {
        private readonly object _locker = new object();

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="FileDependency" /> class.
        /// </summary>
        /// <param name="path">The directory that this <see cref="FileDependency" /> will monitor. When the directory changes, this <see cref="FileDependency" /> will notify any subscribing objects of the change.</param>
        public FileDependency(string path)
            : this(path, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileDependency" /> class.
        /// </summary>
        /// <param name="path">The directory that this <see cref="FileDependency" /> will monitor. When the directory changes, this <see cref="FileDependency" /> will notify any subscribing objects of the change.</param>
        /// <param name="filter">The type of files to watch. For example, "*.xslt" watches for changes to all XSLT files.</param>
        public FileDependency(string path, string filter)
            : this(ConvertUtility.ToArray<string>(path), filter)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileDependency"/> class.
        /// </summary>
        /// <param name="directories">An <see cref="IEnumerable{T}"/> of directories that this <see cref="FileDependency"/> will monitor. When any of these directories changes, this <see cref="FileDependency"/> will notify any subscribing objects of the change.</param>
        public FileDependency(IEnumerable<string> directories)
            : this (directories, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileDependency"/> class.
        /// </summary>
        /// <param name="directories">An <see cref="IEnumerable{T}"/> of directories that this <see cref="FileDependency"/> will monitor. When any of these directories changes, this <see cref="FileDependency"/> will notify any subscribing objects of the change.</param>
        /// <param name="filter">The type of files to watch. For example, "*.xslt" watches for changes to all XSLT files.</param>
        public FileDependency(IEnumerable<string> directories, string filter)
        {
            if (directories == null) { throw new ArgumentNullException("directories"); }
            this.Directories = directories;
            this.Filter = filter;
        }
        #endregion

        #region Properties
        private IEnumerable<string> Directories { get; set; }

        private string Filter { get; set; }

        private IEnumerable<FileSystemWatcher> Watchers
        {
            get; set;
        }

        private DateTime UtcCreated
        {
            get; set;
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="Dependency"/> object has changed.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if the <see cref="Dependency"/> object has changed; otherwise, <c>false</c>.
        /// </value>
        public override bool HasChanged
        {
            get { return (this.UtcLastModified > this.UtcCreated); }
        }
        #endregion

        #region Methods

        /// <summary>
        /// Starts and performs the necessary dependency tasks of this instance.
        /// </summary>
        public override void Start()
        {
            List<FileSystemWatcher> watchers = new List<FileSystemWatcher>();
            foreach (string directory in this.Directories)
            {
                if (!string.IsNullOrEmpty(directory))
                {
                    FileSystemWatcher tempWatcher = null;
                    FileSystemWatcher watcher = null;
                    try
                    {
                        tempWatcher = new FileSystemWatcher(directory);
                        if (!string.IsNullOrEmpty(this.Filter)) { tempWatcher.Filter = this.Filter; }
                        tempWatcher.Created += new FileSystemEventHandler(WatcherChanged);
                        tempWatcher.Changed += new FileSystemEventHandler(WatcherChanged);
                        tempWatcher.Deleted += new FileSystemEventHandler(WatcherChanged);
                        tempWatcher.Renamed += new RenamedEventHandler(WatcherChanged);
                        tempWatcher.EnableRaisingEvents = true;
                        watcher = tempWatcher;
                        tempWatcher = null;
                    }
                    finally
                    {
                        if (tempWatcher != null) { tempWatcher.Dispose(); }
                    }
                    watchers.Add(watcher);
                }
            }
            this.Watchers = watchers.ToArray();
            this.UtcCreated = DateTime.UtcNow;
            this.SetUtcLastModified(this.UtcCreated);
        }

        private void WatcherChanged(object sender, FileSystemEventArgs args)
        {
            this.SetUtcLastModified(DateTime.UtcNow);
            if (!this.HasChanged) { return; }
            if (this.Watchers != null)
            {
                lock (_locker)
                {
                    foreach (FileSystemWatcher watcher in this.Watchers)
                    {
                        watcher.Created -= new FileSystemEventHandler(WatcherChanged);
                        watcher.Changed -= new FileSystemEventHandler(WatcherChanged);
                        watcher.Deleted -= new FileSystemEventHandler(WatcherChanged);
                        watcher.Renamed -= new RenamedEventHandler(WatcherChanged);
                        watcher.Dispose();
                    }
                    this.Watchers = null;
                }
            }
            this.OnDependencyChangedRaised(new DependencyEventArgs(this.UtcLastModified));
        }
        #endregion
    }
}