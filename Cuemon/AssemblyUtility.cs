using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using Cuemon.Caching;
using Cuemon.Collections.Generic;
using Cuemon.IO;
using Cuemon.Reflection;

namespace Cuemon
{
    /// <summary>
    /// This utility class is designed to make common <see cref="Assembly"/> related operations easier to work with.
    /// </summary>
    public static class AssemblyUtility
    {
        /// <summary>
        /// Returns a <see cref="CacheValidator"/> from the specified <paramref name="assembly"/>.
        /// </summary>
        /// <param name="assembly">The assembly to resolve a <see cref="CacheValidator"/> from.</param>
        /// <returns>A <see cref="CacheValidator"/> that fully represents the integrity of the specified <paramref name="assembly"/>.</returns>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="assembly"/> is null.
        /// </exception>
        public static CacheValidator GetCacheValidator(Assembly assembly)
        {
            if (assembly == null) { throw new ArgumentNullException("assembly"); }
            if (assembly.ManifestModule is ModuleBuilder) { return CacheValidator.Default; }
            return FileUtility.GetCacheValidator(assembly.Location);
        }

        /// <summary>
        /// Returns a <see cref="Version"/> that represents the file version number of the specified <paramref name="assembly"/>.
        /// </summary>
        /// <param name="assembly">The assembly to resolve a <see cref="Version"/> from.</param>
        /// <returns>A <see cref="Version"/> that represents the file version number of the specified <paramref name="assembly"/>.</returns>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="assembly"/> is null.
        /// </exception>
        public static Version GetFileVersion(Assembly assembly)
        {
            if (assembly == null) { throw new ArgumentNullException("assembly"); }
            if (assembly.ManifestModule is ModuleBuilder) { return new Version(0, 0, 0, 0); }
            return FileUtility.GetFileVersion(assembly.Location);
        }

        /// <summary>
        /// Returns a <see cref="Version"/> that represents the version of the product this <paramref name="assembly"/> is distributed with.
        /// </summary>
        /// <param name="assembly">The assembly to resolve a <see cref="Version"/> from.</param>
        /// <returns>A <see cref="Version"/> that represents the version of the product this <paramref name="assembly"/> is distributed with.</returns>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="assembly"/> is null.
        /// </exception>
        public static Version GetProductVersion(Assembly assembly)
        {
            if (assembly == null) { throw new ArgumentNullException("assembly"); }
            if (assembly.ManifestModule is ModuleBuilder) { return new Version(0, 0, 0, 0); }
            return FileUtility.GetProductVersion(assembly.Location);
        }

        /// <summary>
        /// Determines whether the specified <paramref name="assembly"/> is a debug build.
        /// </summary>
        /// <param name="assembly">The assembly to parse and determine whether it is a debug build or not.</param>
        /// <returns><c>true</c> if the specified <paramref name="assembly"/> is a debug build; otherwise, <c>false</c>.</returns>
        public static bool IsDebugBuild(Assembly assembly)
        {
            Doer<Assembly, bool> hasDebuggableAttribute = CachingManager.Cache.Memoize<Assembly, bool>(HasDebuggableAttributeCore);
            return hasDebuggableAttribute(assembly);
        }

        private static bool HasDebuggableAttributeCore(Assembly assembly)
        {
            DebuggableAttribute debug = EnumerableUtility.FirstOrDefault(assembly.GetCustomAttributes(typeof(DebuggableAttribute), false)) as DebuggableAttribute;
            if (debug != null)
            {
                bool isDebug = EnumUtility.HasFlag(debug.DebuggingFlags, DebuggableAttribute.DebuggingModes.Default) ||
                               EnumUtility.HasFlag(debug.DebuggingFlags, DebuggableAttribute.DebuggingModes.DisableOptimizations) ||
                               EnumUtility.HasFlag(debug.DebuggingFlags, DebuggableAttribute.DebuggingModes.EnableEditAndContinue);
                return isDebug || debug.IsJITTrackingEnabled;
            }
            return false;
        }
    }
}