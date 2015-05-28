using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
namespace Cuemon.Data
{
	/// <summary>
	/// This <see cref="DataDependency"/> class will monitor any changes occurred to an underlying data source while notifying subscribing objects.
	/// </summary>
	public sealed class DataDependency : Dependency
	{
		private readonly object _locker = new object();

		#region Constructors
		/// <summary>
		/// Initializes a new instance of the <see cref="DataDependency"/> class.
		/// </summary>
		/// <param name="manager">The <see cref="DataManager"/> to be used for the underlying data operations.</param>
		/// <param name="command">The <see cref="IDataCommand"/> to execute and monitor for changes.</param>
		/// <param name="parameters">An optional sequence of <see cref="IDataParameter"/> to use with the associated <paramref name="command"/>.</param>
		/// <remarks>The signaling is default delayed 15 seconds before first invoke. Signaling occurs every 2 minutes.</remarks>
		public DataDependency(DataManager manager, IDataCommand command, params IDataParameter[] parameters) : this(manager, command, TimeSpan.FromMinutes(2), parameters)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="DataDependency"/> class.
		/// </summary>
		/// <param name="manager">The <see cref="DataManager"/> to be used for the underlying data operations.</param>
		/// <param name="command">The <see cref="IDataCommand"/> to execute and monitor for changes.</param>
		/// <param name="period">The time interval between periodic signaling for changes to the specified <paramref name="command"/> by the associated <see cref="DataWatcher"/>. Specify negative one (-1) milliseconds to disable periodic signaling.</param>
		/// <param name="parameters">An optional sequence of <see cref="IDataParameter"/> to use with the associated <paramref name="command"/>.</param>
		/// <remarks>The signaling is default delayed 15 seconds before first invoke.</remarks>
		public DataDependency(DataManager manager, IDataCommand command, TimeSpan period, params IDataParameter[] parameters) : this(manager, command, TimeSpan.FromSeconds(15), period, parameters)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="DataDependency"/> class.
		/// </summary>
		/// <param name="manager">The <see cref="DataManager"/> to be used for the underlying data operations.</param>
		/// <param name="command">The <see cref="IDataCommand"/> to execute and monitor for changes.</param>
		/// <param name="dueTime">The amount of time to delay before the associated <see cref="DataWatcher"/> starts signaling. Specify negative one (-1) milliseconds to prevent the signaling from starting. Specify zero (0) to start the signaling immediately.</param>
		/// <param name="period">The time interval between periodic signaling for changes to the specified <paramref name="command"/> by the associated <see cref="DataWatcher"/>. Specify negative one (-1) milliseconds to disable periodic signaling.</param>
		/// <param name="parameters">An optional sequence of <see cref="IDataParameter"/> to use with the associated <paramref name="command"/>.</param>
		public DataDependency(DataManager manager, IDataCommand command, TimeSpan dueTime, TimeSpan period, params IDataParameter[] parameters) : this(manager, command, dueTime, period, TimeSpan.Zero, parameters)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="DataDependency"/> class.
		/// </summary>
		/// <param name="manager">The <see cref="DataManager"/> to be used for the underlying data operations.</param>
		/// <param name="command">The <see cref="IDataCommand"/> to execute and monitor for changes.</param>
		/// <param name="dueTime">The amount of time to delay before the associated <see cref="DataWatcher"/> starts signaling. Specify negative one (-1) milliseconds to prevent the signaling from starting. Specify zero (0) to start the signaling immediately.</param>
		/// <param name="period">The time interval between periodic signaling for changes to the specified <paramref name="command"/> by the associated <see cref="DataWatcher"/>. Specify negative one (-1) milliseconds to disable periodic signaling.</param>
		/// <param name="parameters">An optional sequence of <see cref="IDataParameter"/> to use with the associated <paramref name="command"/>.</param>
		/// <param name="dueTimeOnChanged">The amount of time to postpone a <see cref="Watcher.Changed"/> event. Specify zero (0) to disable postponing.</param>
		public DataDependency(DataManager manager, IDataCommand command, TimeSpan dueTime, TimeSpan period, TimeSpan dueTimeOnChanged, params IDataParameter[] parameters)
		{
		    this.Manager = manager;
		    this.Command = command;
		    this.DueTime = dueTime;
		    this.Period = period;
		    this.DueTimeOnChanged = dueTimeOnChanged;
		    this.Parameters = parameters;
		}
		#endregion

		#region Properties
        private DataManager Manager { get; set; }

        private IDataCommand Command { get; set; }

        private TimeSpan DueTime { get; set; }

        private TimeSpan Period { get; set; }

        private TimeSpan DueTimeOnChanged { get; set; }

        private IDataParameter[] Parameters { get; set; }

		private DateTime UtcCreated { get; set; }

		private IEnumerable<DataWatcher> Watchers
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
			List<DataWatcher> watchers = new List<DataWatcher>();
			DataWatcher watcher = null;
			DataWatcher tempWatcher = null;
			try
			{
				tempWatcher = new DataWatcher(this.Manager, this.Command, this.DueTime, this.Period, this.DueTimeOnChanged, this.Parameters);
				tempWatcher.Changed += new EventHandler<WatcherEventArgs>(WatcherChanged);
				watcher = tempWatcher;
				tempWatcher = null;
			}
			finally 
			{
				if (tempWatcher != null) { tempWatcher.Dispose(); }
			}
			watchers.Add(watcher);
			this.Watchers = watchers.ToArray();
			this.UtcCreated = DateTime.UtcNow;
			this.SetUtcLastModified(this.UtcCreated);
		}

		private void WatcherChanged(object sender, WatcherEventArgs args)
		{
			this.SetUtcLastModified(DateTime.UtcNow);
			if (!this.HasChanged) { return; }
			if (this.Watchers != null)
			{
				lock (_locker)
				{
					if (this.Watchers != null)
					{
                        foreach (DataWatcher watcher in this.Watchers)
                        {
                            watcher.Changed -= new EventHandler<WatcherEventArgs>(WatcherChanged);
                            watcher.Dispose();
                        }
					}
                    this.Watchers = null;
				}
			}
			this.OnDependencyChangedRaised(new DependencyEventArgs(this.UtcLastModified));
		}
		#endregion
	}
}