using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Mime;
using System.Text;

namespace Cuemon.ServiceModel
{
    /// <summary>
    /// Represents a name/value field of an encoded multipart entity-body of an HTTP message. This class cannot be inherited.
    /// </summary>
    public sealed class HttpMultipartContent
    {
        internal HttpMultipartContent(string name, string fileName, ContentType mimeType, Stream data)
        {
            this.Name = name;
            this.FileName = fileName;
            this.MimeType = mimeType;
            this.Data = ConvertUtility.ToByteArray(data);
        }

        /// <summary>
        /// Gets the name of the file associated with this <see cref="HttpMultipartContent"/>.
        /// </summary>
        /// <value>The name of the file associated with this <see cref="HttpMultipartContent"/>.</value>
        public string FileName { get; private set; }

        /// <summary>
        /// Gets the name of this <see cref="HttpMultipartContent"/>.
        /// </summary>
        /// <value>The name of this <see cref="HttpMultipartContent"/>.</value>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the MIME type of this <see cref="HttpMultipartContent"/>.
        /// </summary>
        /// <value>The MIME type of this <see cref="HttpMultipartContent"/>.</value>
        public ContentType MimeType { get; private set; }

        /// <summary>
        /// Gets the data associated with this <see cref="HttpMultipartContent"/>.
        /// </summary>
        /// <value>The data associated with this <see cref="HttpMultipartContent"/>.</value>
        public byte[] Data { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this <see cref="HttpMultipartContent"/> suggest a file.
        /// </summary>
        /// <value><c>true</c> if this <see cref="HttpMultipartContent"/> suggest a file; otherwise, <c>false</c>.</value>
        public bool IsFile
        {
            get { return !this.IsFormItem; }
        }

        /// <summary>
        /// Gets a value indicating whether this <see cref="HttpMultipartContent"/> suggest a form item.
        /// </summary>
        /// <value><c>true</c> if this <see cref="HttpMultipartContent"/> suggest a form item; otherwise, <c>false</c>.</value>
        public bool IsFormItem
        {
            get { return string.IsNullOrEmpty(this.FileName); }
        }

        /// <summary>
        /// Gets the value of this <see cref="HttpMultipartContent"/> as a <see cref="HttpMultipartFile"/>.
        /// </summary>
        /// <returns>An instance of a <see cref="HttpMultipartFile"/> object.</returns>
        public HttpMultipartFile GetPartAsFile()
        {
            return new HttpMultipartFile(this.FileName, this.MimeType, new MemoryStream(this.Data));
        }

        /// <summary>
        /// Gets the value of this <see cref="HttpMultipartContent"/> as a <see cref="string"/>.
        /// </summary>
        /// <param name="encoding">The encoding to use when getting the form item value of this <see cref="HttpMultipartContent"/> instance.</param>
        /// <returns>A <see cref="string"/> representing a form item value.</returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="encoding"/> is null.
        /// </exception>
        public string GetPartAsString(Encoding encoding)
        {
            if (encoding == null) { throw new ArgumentNullException("encoding"); }
            return this.Data.Length > 0 ? encoding.GetString(this.Data) : string.Empty;
        }

        /// <summary>
        /// Creates and returns a sequence of <see cref="HttpMultipartContent"/> objects from the specified <paramref name="entityBody"/> of an HTTP message.
        /// </summary>
        /// <param name="entityBody">The entity-body to parse for <see cref="HttpMultipartContent"/> objects.</param>
        /// <param name="length">The length of the <paramref name="entityBody"/>.</param>
        /// <param name="boundary">The boundary parameter of the Content-Type header represented by the <paramref name="entityBody"/>.</param>
        /// <param name="encoding">The encoding associated with the <paramref name="entityBody"/>.</param>
        /// <returns>A <see cref="IEnumerable{HttpMultipartContent}"/> representing a sequence of multipart/form-data objects that is equivalent to <paramref name="entityBody"/>.</returns>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="entityBody"/> -or- <paramref name="boundary"/> -or- <paramref name="encoding"/> is null.
        /// </exception>
        public static IEnumerable<HttpMultipartContent> Parse(Stream entityBody, int length, string boundary, Encoding encoding)
        {
            if (entityBody == null) { throw new ArgumentNullException("entityBody"); }
            if (boundary == null) { throw new ArgumentNullException("boundary"); }
            if (encoding == null) { throw new ArgumentNullException("encoding"); }

            HttpMultipartContentParser parser = new HttpMultipartContentParser(entityBody, length, boundary, encoding);
            return parser.ToEnumerable();
        }

        /// <summary>
        /// Creates and returns a sequence of <see cref="HttpMultipartContent"/> objects from the specified <paramref name="entityBody"/> of an HTTP message.
        /// </summary>
        /// <param name="entityBody">The entity-body to parse for <see cref="HttpMultipartContent"/> objects.</param>
        /// <param name="length">The length of the <paramref name="entityBody"/>.</param>
        /// <param name="boundary">The boundary parameter of the Content-Type header represented by the <paramref name="entityBody"/>.</param>
        /// <param name="encoding">The encoding associated with the <paramref name="entityBody"/>.</param>
        /// <returns>A <see cref="IEnumerable{HttpMultipartContent}"/> representing a sequence of multipart/form-data objects that is equivalent to <paramref name="entityBody"/>.</returns>
        /// <exception cref="System.ArgumentNullException">
        /// <paramref name="entityBody"/> -or- <paramref name="boundary"/> -or- <paramref name="encoding"/> is null.
        /// </exception>
        public static IEnumerable<HttpMultipartContent> Parse(Stream entityBody, int length, byte[] boundary, Encoding encoding)
        {
            if (entityBody == null) { throw new ArgumentNullException("entityBody"); }
            if (boundary == null) { throw new ArgumentNullException("boundary"); }
            if (encoding == null) { throw new ArgumentNullException("encoding"); }

            HttpMultipartContentParser parser = new HttpMultipartContentParser(entityBody, length, boundary, encoding);
            return parser.ToEnumerable();
        }
    }
}
