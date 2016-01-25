using System;
using System.Diagnostics;
using System.Globalization;

namespace Cuemon.Web
{
    internal static class Infrastructure
    {
        internal static void TraceWriteLifecycleEvent(string lifecycleEvent, TimeSpan elapsed)
        {
            Trace.WriteLine(string.Format(CultureInfo.InvariantCulture, "{0} milliseconds elapsed: {1}.", elapsed.TotalMilliseconds, lifecycleEvent));
        }
    }
}