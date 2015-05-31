using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Management;
using System.Security.Permissions;
using System.ServiceProcess;
using Cuemon.Caching;
using Cuemon.Collections.Generic;
using Cuemon.Diagnostics;
using Cuemon.Security.Cryptography;
using Cuemon.Threading;

namespace Cuemon.Management
{
	/// <summary>
	/// This utility class is designed to make common management operations instrumented to the Windows Management Instrumentation (WMI) infrastructure easier to work with.
	/// </summary>
	public static class ManagementUtility
	{
		private static readonly object SyncRoot = new object();

		/// <summary>
		/// Determines whether the Windows Management Instrumentation service is running.
		/// </summary>
		/// <returns>
		///   <c>true</c> if the Windows Management Instrumentation service is running; otherwise, <c>false</c>.
		/// </returns>
		public static bool IsWindowsManagementInstrumentationRunning()
		{
			string key = HashUtility.ComputeHash("IsWindowsManagementInstrumentationRunning()");
			if (!CachingManager.Cache.ContainsKey(key))
			{
				lock (SyncRoot)
				{
					if (!CachingManager.Cache.ContainsKey(key))
					{
						bool isRunning = false;
						try
						{
							using (ServiceController controller = new ServiceController("Winmgmt"))
							{
								if (controller.Status == ServiceControllerStatus.Running) { isRunning = true; }
							}
						}
						catch (ArgumentException)
						{
						}
						CachingManager.Cache.Add(key, isRunning, TimeSpan.FromMinutes(15));
					}
				}
			}
			return CachingManager.Cache.Get<bool>(key);
		}

		/// <summary>
		/// Creates and returns a read-only representation of the Win32_ComputerSystem WMI class.
		/// </summary>
		/// <returns>An <see cref="IReadOnlyDictionary{String,Object}"/> representing the properties of the Win32_ComputerSystem WMI class.</returns>
		/// <remarks>
		/// For more information about the Win32_ComputerSystem WMI class, do visit this address: http://msdn.microsoft.com/en-us/library/windows/desktop/aa394102(v=vs.85).aspx.
		/// </remarks>
		public static IReadOnlyDictionary<string, object> GetComputerSystemInfo()
		{
			CheckIfWindowsManagementInstrumentationIsRunning();

			IDictionary<string, object> result = new Dictionary<string, object>();
			using (ManagementObjectCollection wmiObjects = ParseWin32("Win32_ComputerSystem"))
			{
				foreach (ManagementObject wmiObject in wmiObjects)
				{
					using (wmiObject)
					{
						AddPropertyData(wmiObject, result);
					}
				}
			}
			return new ReadOnlyDictionary<string, object>(result);
		}

		/// <summary>
		/// Creates and returns a read-only representation of the Win32_OperatingSystem WMI class.
		/// </summary>
		/// <returns>An <see cref="IReadOnlyDictionary{String,Object}"/> representing the properties of the Win32_OperatingSystem WMI class.</returns>
		/// <remarks>
		/// For more information about the Win32_OperatingSystem WMI class, do visit this address: http://msdn.microsoft.com/en-us/library/windows/desktop/aa394239(v=vs.85).aspx.
		/// </remarks>
		public static IReadOnlyDictionary<string, object> GetOperatingSystemInfo()
		{
			CheckIfWindowsManagementInstrumentationIsRunning();

			IDictionary<string, object> result = new Dictionary<string, object>();           
			using (ManagementObjectCollection wmiObjects = ParseWin32("Win32_OperatingSystem"))
			{
				foreach (ManagementObject wmiObject in wmiObjects)
				{
					using (wmiObject)
					{
						AddPropertyData(wmiObject, result);
					}
				}
			}
			return new ReadOnlyDictionary<string, object>(result);
		}

		/// <summary>
		/// Creates and returns a read-only representation of the Win32_Process WMI class for the currently active process.
		/// </summary>
		/// <returns>An <see cref="IReadOnlyDictionary{String,Object}"/> representing the properties (and owner information) of the Win32_Process WMI class.</returns>
		/// <remarks>
		/// For more information about the Win32_Process WMI class, do visit this address: http://msdn.microsoft.com/en-us/library/windows/desktop/aa394372(v=vs.85).aspx.
		/// </remarks>
		public static IReadOnlyDictionary<string, object> GetProcessInfo()
		{
			return GetProcessInfo(Process.GetCurrentProcess());
		}

		/// <summary>
		/// Creates and returns a read-only representation of the Win32_Process WMI class.
		/// </summary>
		/// <param name="process">The <see cref="Process"/> to query.</param>
		/// <returns>An <see cref="IReadOnlyDictionary{String,Object}"/> representing the properties (and owner information) of the Win32_Process WMI class.</returns>
		/// <remarks>
		/// For more information about the Win32_Process WMI class, do visit this address: http://msdn.microsoft.com/en-us/library/windows/desktop/aa394372(v=vs.85).aspx.
		/// </remarks>
		public static IReadOnlyDictionary<string, object> GetProcessInfo(Process process)
		{
			if (process == null) { throw new ArgumentNullException("process"); }
			CheckIfWindowsManagementInstrumentationIsRunning();

			IDictionary<string, object> result = new Dictionary<string, object>();
			using (ManagementObjectCollection wmiObjects = ParseWin32("Win32_Process", "ProcessID = {0}", process.Id))
			{
				foreach (ManagementObject wmiObject in wmiObjects)
				{
					using (wmiObject)
					{
						AddPropertyData(wmiObject, result);

						string[] ownerInfo = new string[2];
						uint code = (uint)wmiObject.InvokeMethod("GetOwner", ownerInfo);
						if (code == 0)
						{
							string userName = string.IsNullOrEmpty(ownerInfo[1]) ? ownerInfo[0] : string.Concat(ownerInfo[1], "\\", ownerInfo[0]);
							result.Add("UserName", userName);
						}
					}
				}
			}
			return new ReadOnlyDictionary<string, object>(result);
		}

        /// <summary>
        /// Creates and returns a read-only representation of the Win32_Processor WMI class.
        /// </summary>
        /// <returns>An <see cref="IReadOnlyDictionary{String,Object}"/> representing the properties of the Win32_Processor WMI class.</returns>
        /// <remarks>
        /// For more information about the Win32_Processor WMI class, do visit this address: http://msdn.microsoft.com/en-us/library/windows/desktop/aa394373(v=vs.85).aspx.
        /// </remarks>
        public static IReadOnlyDictionary<string, object> GetProcessorInfo()
        {
            CheckIfWindowsManagementInstrumentationIsRunning();

            IDictionary<string, object> result = new Dictionary<string, object>();
            
            using (ManagementObjectCollection wmiObjects = ParseWin32("Win32_Processor"))
            {
                foreach (ManagementObject wmiObject in wmiObjects)
                {
                    using (wmiObject)
                    {
                        AddPropertyData(wmiObject, result);
                    }
                }
            }
            return new ReadOnlyDictionary<string, object>(result);
        }

        /// <summary>
        /// Creates and returns a lightweight, read-only implementation of a Windows NT performance counter similar to Windows Performance Monitor. Includes all instance names in the result.
        /// </summary>
        /// <param name="categoryName">The name of the performance monitor object.</param>
        /// <returns>An <see cref="IReadOnlyCollection{PerformanceMonitorCounter}"/> representing a lightweight, read-only implementation of a Windows NT performance counter similar to Windows Performance Monitor.</returns>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="categoryName"/> is null.
        /// </exception>
        /// <exception cref="Cuemon.ArgumentEmptyException">
        /// <paramref name="categoryName"/> is empty.
        /// </exception>
        /// <exception cref="System.ArgumentException">
        /// <paramref name="categoryName"/> is misspelled or does not exists.
        /// </exception>
        public static IReadOnlyCollection<PerformanceMonitorCounter> GetPerformanceMonitorCounters(string categoryName)
        {
            return ManagementUtility.GetPerformanceMonitorCounters(categoryName, TimeSpan.FromMilliseconds(64), ConvertUtility.ToArray<string>("*"));
        }

        /// <summary>
        /// Creates and returns a lightweight, read-only implementation of a Windows NT performance counter similar to Windows Performance Monitor. Includes all instance names in the result.
        /// </summary>
        /// <param name="categoryName">The name of the performance monitor object.</param>
        /// <param name="sampleDelay">The delay to use when calculating the sample value. Default is 64 milliseconds. Recommended delay is 1 second.</param>
        /// <returns>An <see cref="IReadOnlyCollection{PerformanceMonitorCounter}"/> representing a lightweight, read-only implementation of a Windows NT performance counter similar to Windows Performance Monitor.</returns>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="categoryName"/> is null.
        /// </exception>
        /// <exception cref="Cuemon.ArgumentEmptyException">
        /// <paramref name="categoryName"/> is empty.
        /// </exception>
        /// <exception cref="System.ArgumentException">
        /// <paramref name="categoryName"/> is misspelled or does not exists.
        /// </exception>
        /// <remarks>If the calculated sample value of a counter depends on two counter reads, a delay has to be used before the result is accurate. The recommended delay time is one second to allow the counter to perform the next incremental read, but is default 64 milliseconds for performance considerations.</remarks>
        public static IReadOnlyCollection<PerformanceMonitorCounter> GetPerformanceMonitorCounters(string categoryName, TimeSpan sampleDelay)
        {
            return ManagementUtility.GetPerformanceMonitorCounters(categoryName, sampleDelay, ConvertUtility.ToArray<string>("*"));
        }

        /// <summary>
        /// Creates and returns a lightweight, read-only implementation of a Windows NT performance counter similar to Windows Performance Monitor.
        /// </summary>
        /// <param name="categoryName">The name of the performance monitor object.</param>
        /// <param name="instanceNames">The instance names to sample for the associated <paramref name="categoryName"/>. Use asterisk (*) to include all instance names.</param>
        /// <returns>An <see cref="IReadOnlyCollection{PerformanceMonitorCounter}"/> representing a lightweight, read-only implementation of a Windows NT performance counter similar to Windows Performance Monitor.</returns>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="categoryName"/> is null or <br/>
        /// <paramref name="instanceNames"/> is null.
        /// </exception>
        /// <exception cref="Cuemon.ArgumentEmptyException">
        /// <paramref name="categoryName"/> is empty.
        /// </exception>
        /// <exception cref="System.ArgumentException">
        /// <paramref name="categoryName"/> is misspelled or does not exists.
        /// </exception>
        public static IReadOnlyCollection<PerformanceMonitorCounter> GetPerformanceMonitorCounters(string categoryName, params string[] instanceNames)
        {
            return GetPerformanceMonitorCounters(categoryName, TimeSpan.FromMilliseconds(64), instanceNames);
        }

        /// <summary>
        /// Creates and returns a lightweight, read-only implementation of a Windows NT performance counter similar to Windows Performance Monitor.
        /// </summary>
        /// <param name="categoryName">The name of the performance monitor object.</param>
        /// <param name="sampleDelay">The delay to use when calculating the sample value. Default is 64 milliseconds. Recommended delay is 1 second.</param>
        /// <param name="instanceNames">The instance names to sample for the associated <paramref name="categoryName"/>. Use asterisk (*) to include all instance names.</param>
        /// <returns>An <see cref="IReadOnlyCollection{PerformanceMonitorCounter}"/> representing a lightweight, read-only implementation of a Windows NT performance counter similar to Windows Performance Monitor.</returns>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="categoryName"/> is null or <br/>
        /// <paramref name="instanceNames"/> is null.
        /// </exception>
        /// <exception cref="Cuemon.ArgumentEmptyException">
        /// <paramref name="categoryName"/> is empty.
        /// </exception>
        /// <exception cref="System.ArgumentException">
        /// <paramref name="categoryName"/> is misspelled or does not exists.
        /// </exception>
        /// <remarks>If the calculated sample value of a counter depends on two counter reads, a delay has to be used before the result is accurate. The recommended delay time is one second to allow the counter to perform the next incremental read, but is default 64 milliseconds for performance considerations.</remarks>
        public static IReadOnlyCollection<PerformanceMonitorCounter> GetPerformanceMonitorCounters(string categoryName, TimeSpan sampleDelay, params string[] instanceNames)
        {
            return GetPerformanceMonitorCounters(categoryName, sampleDelay, instanceNames as IEnumerable<string>);
        }

        /// <summary>
        /// Creates and returns a lightweight, read-only implementation of a Windows NT performance counter similar to Windows Performance Monitor.
        /// </summary>
        /// <param name="categoryName">The name of the performance monitor object.</param>
        /// <param name="instanceNames">The instance names to sample for the associated <paramref name="categoryName"/>. Use asterisk (*) to include all instance names.</param>
        /// <returns>An <see cref="IReadOnlyCollection{PerformanceMonitorCounter}"/> representing a lightweight, read-only implementation of a Windows NT performance counter similar to Windows Performance Monitor.</returns>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="categoryName"/> is null or <br/>
        /// <paramref name="instanceNames"/> is null.
        /// </exception>
        /// <exception cref="Cuemon.ArgumentEmptyException">
        /// <paramref name="categoryName"/> is empty.
        /// </exception>
        /// <exception cref="System.ArgumentException">
        /// <paramref name="categoryName"/> is misspelled or does not exists.
        /// </exception>
        public static IReadOnlyCollection<PerformanceMonitorCounter> GetPerformanceMonitorCounters(string categoryName, IEnumerable<string> instanceNames)
        {
            return GetPerformanceMonitorCounters(categoryName, TimeSpan.FromMilliseconds(64), instanceNames);
        }

        /// <summary>
        /// Creates and returns a lightweight, read-only implementation of a Windows NT performance counter similar to Windows Performance Monitor.
        /// </summary>
        /// <param name="categoryName">The name of the performance monitor object.</param>
        /// <param name="sampleDelay">The delay to use when calculating the sample value. Default is 64 milliseconds. Recommended delay is 1 second.</param>
        /// <param name="instanceNames">The instance names to sample for the associated <paramref name="categoryName"/>. Use asterisk (*) to include all instance names.</param>
        /// <returns>An <see cref="IReadOnlyCollection{PerformanceMonitorCounter}"/> representing a lightweight, read-only implementation of a Windows NT performance counter similar to Windows Performance Monitor.</returns>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="categoryName"/> is null or <br/>
        /// <paramref name="instanceNames"/> is null.
        /// </exception>
        /// <exception cref="Cuemon.ArgumentEmptyException">
        /// <paramref name="categoryName"/> is empty.
        /// </exception>
        /// <exception cref="System.ArgumentException">
        /// <paramref name="categoryName"/> is misspelled or does not exists.
        /// </exception>
        /// <remarks>If the calculated sample value of a counter depends on two counter reads, a delay has to be used before the result is accurate. The recommended delay time is one second to allow the counter to perform the next incremental read, but is default 64 milliseconds for performance considerations.</remarks>
        public static IReadOnlyCollection<PerformanceMonitorCounter> GetPerformanceMonitorCounters(string categoryName, TimeSpan sampleDelay, IEnumerable<string> instanceNames)
        {
            if (categoryName == null) { throw new ArgumentNullException("categoryName"); }
            if (categoryName.Length == 0) { throw new ArgumentEmptyException("categoryName"); }
            if (instanceNames == null) { throw new ArgumentNullException("instanceNames"); }
            if (!PerformanceCounterCategory.Exists(categoryName)) { throw new ArgumentException("No performance counter category exists by that name.", "categoryName"); }

            IList<string> safeInstanceNames = instanceNames as IList<string> ?? new List<string>(instanceNames);
            IList<PerformanceMonitorCounter> result = new List<PerformanceMonitorCounter>();
            PerformanceCounterCategory category = new PerformanceCounterCategory(categoryName);
            instanceNames = EnumerableUtility.FirstOrDefault(safeInstanceNames) == "*" ? category.GetInstanceNames() as IEnumerable<string> : safeInstanceNames;
            int instanceNamesCount = EnumerableUtility.Count(instanceNames);
            DoerWorkItemPool<PerformanceMonitorCounter> poolForInstances = new DoerWorkItemPool<PerformanceMonitorCounter>();
            CountdownEvent syncForInstances = null;
            try
            {
                syncForInstances = new CountdownEvent(instanceNamesCount);
                foreach (string instanceName in instanceNames)
                {
                    poolForInstances.ProcessWork(ActWorkItem.Create(syncForInstances, DoWorkInstance, category, instanceName, sampleDelay, poolForInstances, syncForInstances));
                }
                syncForInstances.Wait(sampleDelay.Add(TimeSpan.FromMinutes(1)));
            }
            finally
            {
                if (syncForInstances != null) { syncForInstances.Dispose(); }
            }

            foreach (PerformanceMonitorCounter counter in poolForInstances.Result)
            {
                if (counter == null) { continue; }
                result.Add(counter);
            }
            return new ReadOnlyCollection<PerformanceMonitorCounter>(result);
        }

        /// <summary>
        /// Creates and returns a instance name compatible with the GetPerformanceMonitorCounters methods of the currently active process.
        /// </summary>
        /// <returns>A instance name compatible with the GetPerformanceMonitorCounters methods of the currently active process.</returns>
        public static string GetInstanceName()
        {
            return GetInstanceName(Process.GetCurrentProcess());
        }

        /// <summary>
        /// Creates and returns a instance name compatible with the GetPerformanceMonitorCounters methods of the specified <paramref name="process"/>.
        /// </summary>
        /// <param name="process">The <see cref="Process"/> to extract an instance name from.</param>
        /// <returns>A instance name compatible with the GetPerformanceMonitorCounters methods of the specified <paramref name="process"/>.</returns>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="process"/> is null.
        /// </exception>
	    public static string GetInstanceName(Process process)
	    {
            if (process == null) { throw new ArgumentNullException("process"); }
            ProcessSnapshot snapshot = new ProcessSnapshot(process);
            return string.Format(CultureInfo.InvariantCulture, "{0}[{1}]", snapshot.Name.ToLowerInvariant(), snapshot.ProcessIdentifier);
	    }

        private static void DoWorkInstance(PerformanceCounterCategory category, string instanceName, TimeSpan sampleDelay, DoerWorkItemPool<PerformanceMonitorCounter> pool, CountdownEvent sync)
        {
            PerformanceCounter[] counters = category.GetCounters(instanceName);
            sync.AddCount(counters.Length);
            foreach (PerformanceCounter counter in counters)
            {
                pool.ProcessWork(DoerWorkItem.Create(sync, DoWorkCounter, counter, sampleDelay));
            }
        }

        private static PerformanceMonitorCounter DoWorkCounter(PerformanceCounter counter, TimeSpan sampleDelay)
        {
            return new PerformanceMonitorCounter(counter, sampleDelay);
        }

        /// <summary>
		/// Parses and returns a sequence of Win32 object equivalent to the specified <paramref name="className"/>.
		/// </summary>
		/// <param name="className">The name of the Win32 Class to query an instance of.</param>
		/// <returns>A <see cref="ManagementObjectCollection"/> containing the objects that match the specified query.</returns>
		/// <remarks>
		/// For more information about Win32 Classes, do visit this address: http://msdn.microsoft.com/en-us/library/windows/desktop/aa394084(v=vs.85).aspx.
		/// </remarks>
		[PermissionSet(SecurityAction.LinkDemand, Unrestricted = true)]
		public static ManagementObjectCollection ParseWin32(string className)
		{
			return ParseWin32(className, null);
		}

		/// <summary>
		/// Parses and returns a sequence of Win32 object equivalent to the specified <paramref name="className"/>.
		/// </summary>
		/// <param name="className">The name of the Win32 Class to query an instance of.</param>
		/// <param name="conditionFormat">The condition to be applied when querying the Win32 class as a composite format string (see Remarks).</param>
		/// <param name="args">An object array that contains zero or more objects to format.</param>
		/// <returns>A <see cref="ManagementObjectCollection"/> containing the objects that match the specified query.</returns>
		/// <remarks>
		/// For more information about Win32 Classes, do visit this address: http://msdn.microsoft.com/en-us/library/windows/desktop/aa394084(v=vs.85).aspx.
		/// <br/>
		/// For more information regarding the <paramref name="conditionFormat"/>, have a look here: http://msdn.microsoft.com/en-us/library/txafckwd(v=vs.80).aspx.
		/// </remarks>
        [PermissionSet(SecurityAction.LinkDemand, Unrestricted = true)]
		public static ManagementObjectCollection ParseWin32(string className, string conditionFormat, params object[] args)
		{
			if (className == null) { throw new ArgumentNullException("className"); }
			if (className.Length == 0) { throw new ArgumentEmptyException("className"); }

			SelectQuery query = new SelectQuery(className, conditionFormat != null ? string.Format(CultureInfo.InvariantCulture, conditionFormat, args) : null);
			using (ManagementObjectSearcher searcher = new ManagementObjectSearcher(query))
			{
				return searcher.Get();
			}
		}

		/// <summary>
		/// Parses and returns a <see cref="Type"/> equivalent of <paramref name="cimType"/>.
		/// </summary>
		/// <param name="cimType">The <see cref="CimType"/> to parse.</param>
		/// <returns>A <see cref="Type"/> equivalent of <paramref name="cimType"/>.</returns>
		public static Type ParseCimType(CimType cimType)
		{
			switch (cimType)
			{
				case CimType.SInt8:
					return typeof(SByte);
				case CimType.UInt8:
					return typeof(Byte);
				case CimType.Boolean:
					return typeof(Boolean);
				case CimType.SInt16:
				case CimType.Reference:
					return typeof(Int16);
				case CimType.SInt32:
					return typeof(Int32);
				case CimType.SInt64:
					return typeof(Int64);
				case CimType.UInt64:
					return typeof(UInt64);
				case CimType.UInt32:
					return typeof(UInt32);
				case CimType.UInt16:
					return typeof(UInt16);
				case CimType.Real32:
					return typeof(Single);
				case CimType.Real64:
					return typeof(Double);
				case CimType.DateTime:
					return typeof(DateTime);
				case CimType.Object:
					return typeof(object);
				case CimType.String:
					return typeof(string);
				case CimType.Char16:
					return typeof(Char);
				case CimType.None:
					return null;
			}
			throw new ArgumentOutOfRangeException("cimType", string.Format(CultureInfo.InvariantCulture, "Type, '{0}', is unsupported.", cimType));
		}

		private static void CheckIfWindowsManagementInstrumentationIsRunning()
		{
			if (!IsWindowsManagementInstrumentationRunning()) { throw new PlatformNotSupportedException("Unable to establish contact with the Windows Management Instrumentation service."); }
		}

		private static void AddPropertyData(ManagementObject management, IDictionary<string, object> preResult)
		{
		    string key = null;
		    object value = null;
		    try
		    {
                foreach (PropertyData property in management.Properties)
                {
                    Type valueType = ParseCimType(property.Type);
                    value = property.Value ?? "null";
                    if (valueType == typeof(DateTime) & property.Value != null) { value = ManagementDateTimeConverter.ToDateTime((string)value); }
                    if (property.IsArray & property.Value != null)
                    {
                        object[] originalValue = value as object[];
                        dynamic[] array = new dynamic[originalValue.Length];
                        for (int i = 0; i < originalValue.Length; i++)
                        {
                            array[i] = ConvertUtility.ChangeType(originalValue[i], valueType);
                        }
                        value = array;
                    }
                    else
                    {
                        if (property.Value != null)
                        {
                            value = ConvertUtility.ChangeType(value, valueType);
                        }
                    }

                    key = property.Name;

                    object currentValue;
                    if (preResult.TryGetValue(key, out currentValue))
                    {
                        preResult[key] = string.Concat(currentValue, ",", value);
                    }
                    else
                    {
                        preResult.Add(key, value);    
                    }
                }
		    }
		    catch (ArgumentException ex)
		    {
		        ex.Data.Add("propertyDataName", key);
                ex.Data.Add("propertyDataValue", value);
		        throw;
		    }
		}
	}
}