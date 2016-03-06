using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using Cuemon.Integrity;
using Cuemon.Collections.Generic;
using Cuemon.Runtime;
using Cuemon.Runtime.Caching;

namespace Cuemon.IO
{
    /// <summary>
    /// This utility class is designed to make various file related operations easier to work with.
    /// </summary>
    public static class FileUtility
    {
        /// <summary>
        /// Returns a <see cref="Version"/> from the specified <paramref name="fileName"/>.
        /// </summary>
        /// <param name="fileName">The fully qualified name of the new file. Do not end the path with the directory separator character.</param>
        /// <returns>A <see cref="Version"/> that represents the file version number of the specified <paramref name="fileName"/>.</returns>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="fileName"/> is null.
        /// </exception>
        /// <remarks>Should the specified <paramref name="fileName"/> not contain any file version number, a <see cref="Version"/> initialized to 0.0.0.0 is returned.</remarks>
        public static Version GetFileVersion(string fileName)
        {
            if (fileName == null) { throw new ArgumentNullException(nameof(fileName)); }
            Doer<string, FileVersionInfo> getVersionInfo = CachingManager.Cache.Memoize<string, FileVersionInfo>(GetVersionInfoCore, FileVersionDependencies);
            FileVersionInfo fileVersion = getVersionInfo(fileName);
            return new Version(string.IsNullOrEmpty(fileVersion.FileVersion) ? "0.0.0.0" : fileVersion.FileVersion);
        }


        /// <summary>
        /// Returns a <see cref="Version"/> from the specified <paramref name="fileName"/>.
        /// </summary>
        /// <param name="fileName">The fully qualified name of the new file. Do not end the path with the directory separator character.</param>
        /// <returns>A <see cref="Version"/> that represents the version of the product this file is distributed with of the specified <paramref name="fileName"/>.</returns>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="fileName"/> is null.
        /// </exception>
        /// <remarks>Should the specified <paramref name="fileName"/> not contain any version of the product this file is distributed with, a <see cref="Version"/> initialized to 0.0.0.0 is returned.</remarks>
        public static Version GetProductVersion(string fileName)
        {
            if (fileName == null) { throw new ArgumentNullException(nameof(fileName)); }
            Doer<string, FileVersionInfo> getVersionInfo = CachingManager.Cache.Memoize<string, FileVersionInfo>(GetVersionInfoCore, FileVersionDependencies);
            FileVersionInfo fileVersion = getVersionInfo(fileName);
            return new Version(string.IsNullOrEmpty(fileVersion.ProductVersion) ? "0.0.0.0" : fileVersion.ProductVersion);
        }

        private static FileVersionInfo GetVersionInfoCore(string fileName)
        {
            return FileVersionInfo.GetVersionInfo(fileName);
        }

        /// <summary>
        /// Returns a <see cref="CacheValidator"/> from the specified <paramref name="fileName"/>.
        /// </summary>
        /// <param name="fileName">The fully qualified name of the new file. Do not end the path with the directory separator character.</param>
        /// <returns>A <see cref="CacheValidator"/> that represents a weak integrity check of the specified <paramref name="fileName"/>.</returns>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="fileName"/> is null.
        /// </exception>
        /// <remarks>Should the specified <paramref name="fileName"/> trigger any sort of exception, a <see cref="CacheValidator.Default"/> is returned.</remarks>
        public static CacheValidator GetCacheValidator(string fileName)
        {
            return GetCacheValidator(fileName, 0);
        }

        /// <summary>
        /// Returns a <see cref="CacheValidator"/> from the specified <paramref name="fileName"/>.
        /// </summary>
        /// <param name="fileName">The fully qualified name of the new file. Do not end the path with the directory separator character.</param>
        /// <param name="bytesToRead">The maximum size of a byte-for-byte that promotes a medium/strong integrity check of the specified <paramref name="fileName"/>. A value of 0 (or less) leaves the integrity check at weak.</param>
        /// <returns>A <see cref="CacheValidator"/> that represents either a weak, medium or strong integrity check of the specified <paramref name="fileName"/>.</returns>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="fileName"/> is null.
        /// </exception>
        /// <remarks>Should the specified <paramref name="fileName"/> trigger any sort of exception, a <see cref="CacheValidator.Default"/> is returned.</remarks>
        public static CacheValidator GetCacheValidator(string fileName, int bytesToRead)
        {
            if (fileName == null) { throw new ArgumentNullException(nameof(fileName)); }
            Doer<string, int, CacheValidator> getCacheValidator = CachingManager.Cache.Memoize<string, int, CacheValidator>(GetCacheValidatorCore, FileVersionDependencies);
            return getCacheValidator(fileName.ToUpperInvariant(), bytesToRead).Clone();
        }

        private static IEnumerable<IDependency> FileVersionDependencies(string fileName, int bytesToRead)
        {
            return FileVersionDependencies(fileName);
        }

        private static IEnumerable<IDependency> FileVersionDependencies(string fileName)
        {
            string directory = Path.GetDirectoryName(fileName);
            string filter = Path.GetFileName(fileName);
            yield return new FileDependency(directory, filter);
        }

        private static CacheValidator GetCacheValidatorCore(string fileName, int bytesToRead)
        {
            try
            {
                FileInfo fi = new FileInfo(fileName);
                if (bytesToRead > 0)
                {
                    long buffer = bytesToRead;
                    if (fi.Length < buffer) { buffer = fi.Length; }

                    byte[] checksumBytes = new byte[buffer];
                    using (FileStream openFile = fi.OpenRead())
                    {
                        openFile.Read(checksumBytes, 0, (int)buffer);
                    }
                    return new CacheValidator(fi.CreationTimeUtc, fi.LastWriteTimeUtc, StructUtility.GetHashCode32(checksumBytes));
                }
                return new CacheValidator(fi.CreationTimeUtc, fi.LastWriteTimeUtc, ChecksumMethod.Combined);
            }
            catch (Exception)
            {
                return CacheValidator.Default;
            }
        }

        /// <summary>
        /// Parses and removes invalid characters from the specified <paramref name="fileName"/>.
        /// </summary>
        /// <param name="fileName">A relative or absolute path for the file to parse.</param>
        /// <returns>A <see cref="String"/> cleansed for possible invalid characters in regards to a file location.</returns>
        public static string ParseFileName(string fileName)
        {
            if (fileName == null) throw new ArgumentNullException(nameof(fileName));
            StringBuilder parsedFileName = new StringBuilder(fileName.Length);
            char[] invalidCharacters = EnumerableConverter.ToArray(EnumerableUtility.Distinct(EnumerableUtility.Concat(Path.GetInvalidFileNameChars(), Path.GetInvalidPathChars())));
            foreach (char character in fileName)
            {
                bool isValid = true;
                foreach (char invalidCharacter in invalidCharacters)
                {
                    if (character.Equals(invalidCharacter))
                    {
                        isValid = false;
                        break;
                    }
                }
                if (!isValid) { continue; }
                parsedFileName.Append(character);
            }
            return parsedFileName.ToString();
        }

        /// <summary>
        /// Determines whether a file can be accessed by the specified <see cref="FileAccess"/> method.
        /// </summary>
        /// <param name="path">The file wanted for access.</param>
        /// <param name="access">The method to access the file by.</param>
        /// <returns>
        /// 	<c>true</c> if a file can be accessed by the specified <see cref="FileAccess"/> method; otherwise, <c>false</c>.
        /// </returns>
        public static bool CanAccess(string path, FileAccess access)
        {
            if (path == null) { throw new ArgumentNullException(nameof(path)); }
            if (path.Length == 0) { throw new ArgumentException("The path of the file cannot be empty.", nameof(path)); }
            switch (access)
            {
                case FileAccess.Read:
                case FileAccess.Write:
                case FileAccess.ReadWrite:
                    return CanAccessCore(path, access);
                default:
                    throw new ArgumentOutOfRangeException(nameof(access));
            }
        }

        private static bool CanAccessCore(string path, FileAccess access)
        {
            try
            {
                using (FileStream stream = new FileStream(path, FileMode.Open, access))
                {
                    switch (access)
                    {
                        case FileAccess.Read:
                            return stream.CanRead;
                        case FileAccess.Write:
                        case FileAccess.ReadWrite:
                            return stream.CanWrite;
                        default:
                            return false;
                    }
                }
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}