using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Permissions;
using System.Text;
namespace Cuemon
{
    [SuppressUnmanagedCodeSecurity]
    internal static class SafeNativeMethods
    {
        [DllImport("kernel32.dll")]
        [HostProtection(SecurityAction.LinkDemand, Synchronization = true, ExternalThreading = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool SwitchToThread();
    }
}