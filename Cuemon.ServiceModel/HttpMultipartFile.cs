using System.IO;
using System.Net.Mime;

namespace Cuemon.ServiceModel
{
    /// <summary>
    /// Represents a file part of an HTTP message entity-body that is formatted using multipart/form-data MIME type. This class cannot be inherited.
    /// </summary>
    public sealed class HttpMultipartFile
    {
        internal HttpMultipartFile(string fileName, ContentType mimeType, Stream data)
        {
            this.FileName = fileName;
            this.MimeType = mimeType;
            this.Data = data;
        }

        /// <summary>
        /// Gets the name of the file that this instance represents.
        /// </summary>
        /// <value>The name of the file that this instance represents.</value>
        public string FileName { get; private set; }

        /// <summary>
        /// Gets the MIME type of the file that this instance represents.
        /// </summary>
        /// <value>The MIME type of the file that this instance represents.</value>
        public ContentType MimeType { get; private set; }


        /// <summary>
        /// Gets the data of the file that this instance represents.
        /// </summary>
        /// <value>The data of the file that this instance represents.</value>
        public Stream Data { get; private set; }
    }
}
