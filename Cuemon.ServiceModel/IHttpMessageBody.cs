using System.Collections.Generic;
using System.Net.Mime;
using System.Text;

namespace Cuemon.ServiceModel
{
    /// <summary>
    /// Represents the entity-body of an HTTP message.
    /// </summary>
    public interface IHttpMessageBody
    {
        /// <summary>
        /// Gets the <see cref="ContentType"/> applied to this instance of an HTTP message entity-body.
        /// </summary>
        /// <value>The <see cref="ContentType"/> applied to this instance of an HTTP message entity-body.</value>
        ContentType MimeType { get; }

        /// <summary>
        /// Gets a value indicating whether <see cref="MimeType"/> of this instance of an HTTP message entity-body has a positive match in <see cref="SupportedMimeTypes"/>.
        /// </summary>
        /// <value><c>true</c> if <see cref="MimeType"/> of this instance of an HTTP message entity-body has a positive match in <see cref="SupportedMimeTypes"/>; otherwise, <c>false</c>.</value>
        bool HasSupportedMimeType { get; }

        /// <summary>
        /// Gets a sequence of supported <see cref="ContentType"/> objects of this instance.
        /// </summary>
        /// <value>A sequence of supported <see cref="ContentType"/> objects of this instance.</value>
        IEnumerable<ContentType> SupportedMimeTypes { get; }

        /// <summary>
        /// Gets the <see cref="Encoding"/> applied to this instance of an HTTP message entity-body.
        /// </summary>
        /// <value>The <see cref="Encoding"/> applied to this instance of an HTTP message entity-body.</value>
        Encoding Encoding { get; }
    }
}