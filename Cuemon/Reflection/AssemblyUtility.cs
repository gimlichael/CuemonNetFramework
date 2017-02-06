using System;
using System.Diagnostics;
using System.Reflection;
using System.Reflection.Emit;
using Cuemon.Integrity;
using Cuemon.Collections.Generic;
using Cuemon.IO;
using Cuemon.Runtime.Caching;

namespace Cuemon.Reflection
{
    /// <summary>
    /// This utility class is designed to make common <see cref="Assembly"/> related operations easier to work with.
    /// </summary>
    public static class AssemblyUtility
    {
        /// <summary>
        /// Returns a <see cref="CacheValidator"/> from the specified <paramref name="assembly" />.
        /// </summary>
        /// <param name="assembly">The assembly to resolve a <see cref="CacheValidator" /> from.</param>
        /// <param name="readByteForByteChecksum"><c>true</c> to read the <paramref name="assembly"/> byte-for-byte to promote a strong integrity checksum; <c>false</c> to read common properties of the <paramref name="assembly"/> for a weak (but reliable) integrity checksum.</param>
        /// <param name="setup">The <see cref="CacheValidatorOptions" /> which need to be configured.</param>
        /// <returns>A <see cref="CacheValidator" /> that fully represents the integrity of the specified <paramref name="assembly" />.</returns>
        public static CacheValidator GetCacheValidator(Assembly assembly, bool readByteForByteChecksum = false, Act<CacheValidatorOptions> setup = null)
        {
            if ((assembly == null) || (assembly.ManifestModule is ModuleBuilder)) { return CacheValidator.Default; }
            var assemblyHashCode64 = StructUtility.GetHashCode64(assembly.FullName);
            var assemblyLocation = assembly.Location;
            return string.IsNullOrEmpty(assemblyLocation) ? new CacheValidator(DateTime.MinValue, DateTime.MaxValue, assemblyHashCode64, setup) : FileUtility.GetCacheValidator(assemblyLocation, readByteForByteChecksum ? int.MaxValue : 0, setup).CombineWith(assemblyHashCode64);
        }

        /// <summary>
        /// Returns a <see cref="Version"/> that represents the version number of the specified <paramref name="assembly"/>.
        /// </summary>
        /// <param name="assembly">The assembly to resolve a <see cref="Version"/> from.</param>
        /// <returns>A <see cref="Version"/> that represents the version number of the specified <paramref name="assembly"/>.</returns>
        public static Version GetAssemblyVersion(Assembly assembly)
        {
            if ((assembly == null) || (assembly.ManifestModule is ModuleBuilder)) { return VersionUtility.MinValue; }
            return assembly.GetName().Version;
        }

        /// <summary>
        /// Returns a <see cref="Version"/> that represents the file version number of the specified <paramref name="assembly"/>.
        /// </summary>
        /// <param name="assembly">The assembly to resolve a <see cref="Version"/> from.</param>
        /// <returns>A <see cref="Version"/> that represents the file version number of the specified <paramref name="assembly"/>.</returns>
        public static Version GetFileVersion(Assembly assembly)
        {
            if ((assembly == null) || (assembly.ManifestModule is ModuleBuilder)) { return VersionUtility.MinValue; }
            var fileVersion = GetFileVersionCore(assembly);
            if (fileVersion != VersionUtility.MaxValue) { return fileVersion; }
            return GetAssemblyVersion(assembly);
        }

        /// <summary>
        /// Returns a <see cref="Version"/> that represents the version of the product this <paramref name="assembly"/> is distributed with.
        /// </summary>
        /// <param name="assembly">The assembly to resolve a <see cref="Version"/> from.</param>
        /// <returns>A <see cref="Version"/> that represents the version of the product this <paramref name="assembly"/> is distributed with.</returns>
        public static Version GetProductVersion(Assembly assembly)
        {
            if ((assembly == null) || (assembly.ManifestModule is ModuleBuilder)) { return VersionUtility.MinValue; }
            var productVersion = GetProductVersionCore(assembly);
            if (productVersion != VersionUtility.MaxValue) { return productVersion; }
            return GetFileVersion(assembly);
        }

        private static Version GetProductVersionCore(Assembly assembly)
        {
            var version = ReflectionUtility.GetAttribute<AssemblyInformationalVersionAttribute>(assembly);
            return version == null ? VersionUtility.MaxValue : new Version(version.InformationalVersion);
        }

        private static Version GetFileVersionCore(Assembly assembly)
        {
            var version = ReflectionUtility.GetAttribute<AssemblyFileVersionAttribute>(assembly);
            return version == null ? VersionUtility.MaxValue : new Version(version.Version);
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
            if (assembly == null) { return false; }
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