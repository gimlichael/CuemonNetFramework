using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Cuemon.Collections.Generic;
using Cuemon.Security.Cryptography;

namespace Cuemon.IO
{
    /// <summary>
    /// Represents the mapping information that is needed to associate a given file extension with a content-type and vice-versa.
    /// </summary>
    public class FileMapping
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FileMapping"/> class.
        /// </summary>
        /// <param name="contentType">The content-type to associate with file <paramref name="extensions"/>.</param>
        /// <param name="extensions">A sequence of file extensions matching the specified <paramref name="contentType"/>.</param>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="contentType"/> is null - or  <paramref name="extensions"/> is null.
        /// </exception>
        /// <exception cref="Cuemon.ArgumentEmptyException">
        /// <paramref name="contentType"/> is empty.
        /// </exception>
        /// <exception cref="System.ArgumentException">
        /// <paramref name="contentType"/> has zero or more than one forward slash (/).
        /// </exception>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// <paramref name="extensions"/> was not specified.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="extensions"/> has at least one invalid entry.
        /// </exception>
        public FileMapping(string contentType, params string[] extensions)
        {
            if (contentType == null) { throw new ArgumentNullException("contentType"); }
            if (contentType.Length == 0) { throw new ArgumentEmptyException("contentType"); }
            if (StringUtility.Count(contentType, '/') != 1) { throw new ArgumentException("A content-type must have one forward slash character (/) as it is composed of a top-level media type followed by a subtype identifier, eg. text/plain.", "contentType"); }
            if (extensions == null) { throw new ArgumentNullException("extensions"); }
            if (extensions.Length == 0) { throw new ArgumentOutOfRangeException("extensions", "At least one extension must be specified."); }
            if (!HasExtension(extensions)) { throw new ArgumentOutOfRangeException("extensions", "At least one file name extension seems to be invalid."); }

            this.ContentType = contentType;
            this.Extensions = new ReadOnlyCollection<string>(extensions);
        }

        /// <summary>
        /// Gets the file extensions of this instance.
        /// </summary>
        /// <value>The file extensions of this instance.</value>
        public IReadOnlyCollection<string> Extensions { get; private set; }

        /// <summary>
        /// Gets the content-type of this instance.
        /// </summary>
        /// <value>The content-type of this instance.</value>
        public string ContentType { get; private set; }

        private static bool HasExtension(IEnumerable<string> extensions)
        {
            foreach (string extension in extensions)
            {
                if (!Path.HasExtension(extension)) { return false; }
            }
            return true;
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.</returns>
        public override int GetHashCode()
        {
            int result = this.ContentType.ToLowerInvariant().GetHashCode();
            foreach (string ext in this.Extensions)
            {
                result ^= ext.ToLowerInvariant().GetHashCode();
            }
            return result;
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object" /> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="T:System.Object" /> to compare with the current <see cref="T:System.Object" />.</param>
        /// <returns><c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.</returns>
        public override bool Equals(object obj)
        {
            FileMapping mapping = obj as FileMapping;
            if (mapping == null) { return false; }
            return this.Equals(mapping);
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns><c>true</c> if the current object is equal to the other parameter; otherwise, <c>false</c>. </returns>
        public bool Equals(FileMapping other)
        {
            if (other == null) { return false; }
            return (this.GetHashCode() == other.GetHashCode());
        }
    }
}
