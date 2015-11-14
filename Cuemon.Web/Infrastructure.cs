using System;
using System.Diagnostics;
using System.Globalization;

namespace Cuemon.Web
{
    internal static class Infrastructure
    {
        internal static void TraceWriteLifecycleEvent(string lifecycleEvent, TimeSpan elapsed)
        {
            Trace.WriteLine(string.Format(CultureInfo.InvariantCulture, "{0} milliseconds have elapsed up till {1} event.", elapsed.TotalMilliseconds, lifecycleEvent));
        }
    }
}