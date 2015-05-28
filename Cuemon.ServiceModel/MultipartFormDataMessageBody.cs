using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.Net.Mime;
using System.Runtime.CompilerServices;
using System.Text;
using System.Web;
using Cuemon.Web;

namespace Cuemon.ServiceModel
{
    /// <summary>
    /// Represents a entity-body of an HTTP message that is formatted using multipart/form-data MIME type. This class cannot be inherited.
    /// </summary>
    public sealed class MultipartFormDataMessageBody : HttpMessageBody<IEnumerable<HttpMultipartContent>>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FormUrlEncodedMessageBody"/> class.
        /// </summary>
        /// <param name="mimeType">The <see cref="ContentType"/> associated with the entity-body of an HTTP message.</param>
        /// <param name="encoding">The <see cref="Encoding"/> associated with the entity-body of an HTTP message.</param>
        public MultipartFormDataMessageBody(ContentType mimeType, Encoding encoding) : base(mimeType, encoding, MultipartFormDataDeserializer)
        {
        }

        /// <summary>
        /// Gets a sequence of supported <see cref="ContentType"/> objects of this instance.
        /// </summary>
        /// <value>A sequence of supported <see cref="ContentType"/> objects of this instance.</value>
        public override IEnumerable<ContentType> SupportedMimeTypes
        {
            get { yield return new ContentType("multipart/form-data"); }
        }

        /// <summary>
        /// Deserializes the specified entity body to a sequence of <see cref="HttpMultipartContent"/> objects.
        /// </summary>
        /// <param name="entityBody">The entity-body to deserialize.</param>
        /// <returns>A <see cref="IEnumerable{HttpMultipartContent}"/> representing a sequence of multipart/form-data objects that is equivalent to <paramref name="entityBody"/>.</returns>
        public override IEnumerable<HttpMultipartContent> Deserialize(Stream entityBody)
        {
            return base.Deserialize(entityBody);
        }

        private static IEnumerable<HttpMultipartContent> MultipartFormDataDeserializer(Stream entityBody, ContentType mimeType, Encoding encoding)
        {
            return HttpMultipartContent.Parse(entityBody, (int)entityBody.Length, mimeType.Boundary, encoding);
        }
    }
}
