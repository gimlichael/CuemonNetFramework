using System;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using Cuemon.Collections.Generic;
using Cuemon.Management;

namespace Cuemon.Diagnostics
{
	/// <summary>
	/// Provides a snapshot of information about an associated process.
	/// </summary>
	public sealed class ProcessSnapshot
	{
		private int _id;
		private int _handles;
		private int _threads;
		private long _workingSet;
		private long _peakWorkingSet;
		private long _privateWorkingSet;
		private long _commitSize;
		private string _userName;
		private string _name;

		#region Constructors
		/// <summary>
		/// Initializes a new instance of the <see cref="ProcessSnapshot"/> class and associates it with the process resource that is running the calling application. 
		/// </summary>
		public ProcessSnapshot() : this(Process.GetCurrentProcess())
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ProcessSnapshot"/> class.
		/// </summary>
		/// <param name="process">The process to snapshot for information.</param>
		public ProcessSnapshot(Process process)
		{
			if (process == null) { throw new ArgumentNullException("process"); }
			this.Initialize(process);
		}
		#endregion

		#region Properties
		/// <summary>
		/// Gets a snapshot of the process resource that is running the calling application. 
		/// </summary>
		/// <returns>A <see cref="ProcessSnapshot"/> associated with the process resource that is running the calling application.</returns>
		public static ProcessSnapshot GetCurrentProcess()
		{
			ProcessSnapshot process = new ProcessSnapshot();
			return process;
		}

		/// <summary>
		/// Gets the unique identifier for the associated process. This is also referred to as PID in Windows Task Manager.
		/// </summary>
		public int ProcessIdentifier
		{
			get { return _id; }
		}

		/// <summary>
		/// Gets the number of handles opened by the process.
		/// </summary>
		public int Handles
		{
			get { return _handles; }
		}

		/// <summary>
		/// Gets the number of threads in use by the process.
		/// </summary>
		public int Threads
		{
			get { return _threads; }
		}

		/// <summary>
		/// Gets the amount of physical memory allocated for the associated process.
		/// </summary>
		public long WorkingSet
		{
			get { return _workingSet; }
		}

		/// <summary>
		/// Gets the maximum amount of physical memory used by the associated process.
		/// </summary>
		public long PeakWorkingSet
		{
			get { return _peakWorkingSet; }
		}

		/// <summary>
		/// Gets the amount of memory allocated for the associated process minus shared memory or 0 if unable to retrieve the information.
		/// </summary>
		public long PrivateWorkingSet
		{
			get { return _privateWorkingSet; }
		}

		/// <summary>
		/// Gets the amount of virtual memory committed by the associated process.
		/// </summary>
		public long CommitSize
		{
			get { return _commitSize; }
		}

		/// <summary>
		/// Gets the name of the process. This is also referred to as Image Name in Windows Task Manager.
		/// </summary>
		public string Name
		{
			get { return _name; }
		}

		/// <summary>
		/// Gets the user name under which the process is running or null if unable to retrieve the information.
		/// </summary>
		public string UserName
		{
			get { return _userName; }
		}
		#endregion

		#region Methods
		private void Initialize(Process process)
		{
			using (process)
			{
				_name = process.ProcessName.Replace(".vshost", "");
				_commitSize = process.PagedMemorySize64;
				_handles = process.HandleCount;
				_threads = process.Threads.Count;
				_peakWorkingSet = process.PeakWorkingSet64;
				_workingSet = process.WorkingSet64;
				_id = process.Id;
				
				using (PerformanceCounter counter = new PerformanceCounter("Process", "Working Set - Private", process.ProcessName))
				{
					_privateWorkingSet = counter.RawValue;
				}
				
				IReadOnlyDictionary<string, object> processInfo = ManagementUtility.GetProcessInfo(process);
				if (processInfo.Count > 0)
				{
					_userName = processInfo["UserName"] as string;
				}
			}
		}

		/// <summary>
		/// Returns a <see cref="System.String"/> that represents this instance.
		/// </summary>
		/// <returns>
		/// A <see cref="System.String"/> that represents this instance.
		/// </returns>
		public override string ToString()
		{
			StringBuilder builder = new StringBuilder();
			builder.AppendFormat(CultureInfo.InvariantCulture, "Process Name: {0}", this.Name);
			builder.AppendLine();
			builder.AppendFormat(CultureInfo.InvariantCulture, "Process ID: {0}", this.ProcessIdentifier);
			builder.AppendLine();
			builder.AppendFormat(CultureInfo.InvariantCulture, "User Name: {0}", this.UserName);
			builder.AppendLine();
			builder.AppendFormat(CultureInfo.InvariantCulture, "Working Set: {0} K", NumberUtility.BytesToKilobytes(this.WorkingSet).ToString("N0", CultureInfo.InvariantCulture));
			builder.AppendLine();
			builder.AppendFormat(CultureInfo.InvariantCulture, "Peak Working Set: {0} K", NumberUtility.BytesToKilobytes(this.PeakWorkingSet).ToString("N0", CultureInfo.InvariantCulture));
			builder.AppendLine();
			builder.AppendFormat(CultureInfo.InvariantCulture, "Private Working Set: {0} K", NumberUtility.BytesToKilobytes(this.PrivateWorkingSet).ToString("N0", CultureInfo.InvariantCulture));
			builder.AppendLine();
			builder.AppendFormat(CultureInfo.InvariantCulture, "Commit Size: {0} K", NumberUtility.BytesToKilobytes(this.CommitSize).ToString("N0", CultureInfo.InvariantCulture));
			builder.AppendLine();
			builder.AppendFormat(CultureInfo.InvariantCulture, "Handles: {0}", this.Handles);
			builder.AppendLine();
			builder.AppendFormat(CultureInfo.InvariantCulture, "Threads: {0}", this.Threads);
			return builder.ToString();
		}
		#endregion
	}
}