using System;
using System.Diagnostics;
using System.Globalization;
using System.Security.Principal;

namespace Cuemon.Web
{
    internal static class Infrastructure
    {
        internal static void TraceWriteLifecycleEvent(string lifecycleEvent, TimeSpan elapsed)
        {
            Trace.WriteLine(string.Format(CultureInfo.InvariantCulture, "{0} milliseconds elapsed: {1}.", elapsed.TotalMilliseconds, lifecycleEvent));
        }

        internal static bool TryGetWindowsIdentity(Doer<WindowsIdentity> resolver, out WindowsIdentity identity)
        {
            identity = resolver != null ? resolver() : null;
            return (identity != null);
        }

        internal static void InvokeWitImpersonationContextOrDefault<T>(Doer<WindowsIdentity> identityResolver, Act<T> body, T arg)
        {
            WindowsIdentity identity;
            if (identityResolver != null && TryGetWindowsIdentity(identityResolver, out identity) && !identity.IsAnonymous)
            {
                try
                {
                    using (WindowsImpersonationContext wic = identity.Impersonate())
                    {
                        try
                        {
                            body(arg);
                        }
                        finally
                        {
                            wic.Undo();
                        }
                    }
                }
                finally
                {
                    identity.Dispose();
                }
            }
            else
            {
                body(arg);
            }
        }

        internal static TResult InvokeWitImpersonationContextOrDefault<T, TResult>(Doer<WindowsIdentity> identityResolver, Doer<T, TResult> body, T arg)
        {
            TResult result;
            WindowsIdentity identity;
            if (identityResolver != null && TryGetWindowsIdentity(identityResolver, out identity))
            {
                try
                {
                    using (WindowsImpersonationContext wic = identity.Impersonate())
                    {
                        try
                        {
                            result = body(arg);
                        }
                        finally
                        {
                            wic.Undo();
                        }
                    }
                }
                finally
                {
                    if (identity != null) { identity.Dispose(); }
                }
            }
            else
            {
                result = body(arg);
            }
            return result;
        }
    }
}