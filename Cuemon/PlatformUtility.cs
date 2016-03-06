using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using Cuemon.Collections.Generic;
using Cuemon.Management;

namespace Cuemon
{
    /// <summary>
    /// Provides information about the current platform.
    /// </summary>
    public static class PlatformUtility
    {
        private static readonly object PadLock = new object();
        private static IReadOnlyDictionary<string, object> ProcessorInfo = InitProcessorInfo();

        private static IReadOnlyDictionary<string, object> InitProcessorInfo()
        {
            lock (PadLock)
            {
                int logicalProcessorCount = Environment.ProcessorCount;
                int coreCount = logicalProcessorCount >= 4 ? logicalProcessorCount / 2 : logicalProcessorCount;
                if (TesterDoerUtility.TryExecuteFunction(ManagementUtility.GetProcessorInfo, out ProcessorInfo))
                {
                    try
                    {
                        IDictionary<string, object> compositeProcessorInfo = DictionaryConverter.FromEnumerable(ProcessorInfo);
                        IReadOnlyDictionary<string, object> computerSystem = ManagementUtility.GetComputerSystemInfo();
                        if (computerSystem.ContainsKey("NumberOfProcessors")) { compositeProcessorInfo.Add("NumberOfProcessors", computerSystem["NumberOfProcessors"]); }
                        if (!compositeProcessorInfo.ContainsKey("NumberOfCores")) { compositeProcessorInfo.Add("NumberOfCores", coreCount); }
                        if (!compositeProcessorInfo.ContainsKey("NumberOfLogicalProcessors")) { compositeProcessorInfo.Add("NumberOfLogicalProcessors", logicalProcessorCount); }
                        return new ReadOnlyDictionary<string, object>(compositeProcessorInfo);
                    }
                    catch (Exception ex)
                    {
                        Trace.WriteLine(ExceptionUtility.Refine(ex, MethodBase.GetCurrentMethod()));
                    }
                }
                IDictionary<string, object> fallback = new Dictionary<string, object>();
                fallback.Add("NumberOfCores", coreCount);
                fallback.Add("NumberOfLogicalProcessors", logicalProcessorCount);
                return new ReadOnlyDictionary<string, object>(fallback);
            }
        }

        /// <summary>
        /// Gets the processor information of the current machine.
        /// </summary>
        /// <value>An <see cref="IReadOnlyDictionary{String,Object}"/> representing various properties of the current machines processor/processors.</value>
        public static IReadOnlyDictionary<string, object> Processor
        {
            get { return ProcessorInfo; }
        }

        /// <summary>
        /// Gets the number of processors currently assigned to the currently active process.
        /// </summary>
        /// <returns>The number of processors currently assigned to the currently active process..</returns>
        /// <remarks>
        /// Code was inspired and originally authored by Jesse C. Slicer @ StackOverflow: http://stackoverflow.com/questions/188503/detecting-the-number-of-processors/189371#189371
        /// </remarks>
        public static int GetProcessorAffinityCount()
        {
            return GetProcessorAffinityCount(Process.GetCurrentProcess());
        }

        /// <summary>
        /// Gets the number of processors currently assigned to the specified <paramref name="process"/>.
        /// </summary>
        /// <param name="process">The <see cref="Process"/> to query.</param>
        /// <returns>The number of processors currently assigned to the specified <paramref name="process"/>.</returns>
        /// <remarks>
        /// Code was inspired and originally authored by Jesse C. Slicer @ StackOverflow: http://stackoverflow.com/questions/188503/detecting-the-number-of-processors/189371#189371
        /// </remarks>
        public static int GetProcessorAffinityCount(Process process)
        {
            if (process == null) { throw new ArgumentNullException(nameof(process)); }
            uint processAffinityMask = (uint)process.ProcessorAffinity;
            const uint bitsPerByte = 8;
            uint loop = bitsPerByte * sizeof(uint);
            uint result = 0;

            while (loop > 0)
            {
                --loop;
                result += (processAffinityMask & 1);
                processAffinityMask >>= 1;
            }

            return Convert.ToInt32((result != 0) ? result : 1);
        }
    }
}
