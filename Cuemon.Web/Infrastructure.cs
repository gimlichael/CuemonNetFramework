using System;
using System.Diagnostics;
using System.Globalization;
using System.IO.Compression;
using System.Security.Principal;
using System.Web;

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

        internal static void ApplyCompression(HttpApplication context, GlobalModule module, CompressionMethodScheme compression, Doer<bool> compressWhenTrue)
        {
            switch (compression)
            {
                case CompressionMethodScheme.Deflate:
                    if (compressWhenTrue()) { context.Response.Filter = new DeflateStream(context.Response.Filter, CompressionMode.Compress); }
                    module.ParseCompressionHeaders = true;
                    break;
                case CompressionMethodScheme.GZip:
                    if (compressWhenTrue()) { context.Response.Filter = new GZipStream(context.Response.Filter, CompressionMode.Compress); }
                    module.ParseCompressionHeaders = true;
                    break;
                case CompressionMethodScheme.Identity:
                case CompressionMethodScheme.Compress:
                case CompressionMethodScheme.None:
                    module.ParseCompressionHeaders = true;
                    break;
            }
        }
    }
}