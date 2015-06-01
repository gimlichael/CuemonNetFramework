using System.Collections.Generic;
using System.IO;
using System.Net.Mime;
using System.Text;
using System.Xml.Serialization;
using Cuemon.Xml;

namespace Cuemon.ServiceModel
{
    /// <summary>
    /// Represents a entity-body of an HTTP message that is XML formatted.
    /// </summary>
    /// <typeparam name="TBody">The type of the deserialized entity-body of an HTTP message.</typeparam>
    public class XmlMessageBody<TBody> : HttpMessageBody<TBody> where TBody : IXmlSerializable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="XmlMessageBody{TBody}"/> class.
        /// </summary>
        /// <param name="mimeType">The <see cref="ContentType"/> associated with the entity-body of an HTTP message.</param>
        /// <param name="encoding">The <see cref="Encoding"/> associated with the entity-body of an HTTP message.</param>
        public XmlMessageBody(ContentType mimeType, Encoding encoding) : this(mimeType, encoding, XmlDeserializer)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="XmlMessageBody{T}"/> class.
        /// </summary>
        /// <param name="mimeType">The <see cref="ContentType"/> associated with the entity-body of an HTTP message.</param>
        /// <param name="encoding">The <see cref="Encoding"/> associated with the entity-body of an HTTP message.</param>
        /// <param name="deserializer">The function delegate that converts the entity-body of an HTTP message to an object of type <typeparamref name="TBody"/>.</param>
        public XmlMessageBody(ContentType mimeType, Encoding encoding, Doer<Stream, ContentType, Encoding, TBody> deserializer) : base(mimeType, encoding, deserializer)
        {
        }

        /// <summary>
        /// Gets a sequence of supported <see cref="ContentType"/> objects of this instance.
        /// </summary>
        /// <value>A sequence of supported <see cref="ContentType"/> objects of this instance.</value>
        public override IEnumerable<ContentType> SupportedMimeTypes
        {
            get
            {
                yield return new ContentType("application/xml");
                yield return new ContentType("text/xml");
            }
        }

        /// <summary>
        /// Deserializes the specified entity body to an object of <typeparamref name="TBody"/>.
        /// </summary>
        /// <param name="entityBody">The entity-body to deserialize.</param>
        /// <returns>An object of <typeparamref name="TBody"/> that is equivalent to <paramref name="entityBody"/>.</returns>
        public override TBody Deserialize(Stream entityBody)
        {
            return base.Deserialize(entityBody);
        }

        private static TBody XmlDeserializer(Stream entityBody, ContentType mimeType, Encoding encoding)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(TBody));
            Stream encodedStream = XmlUtility.ConvertEncoding(entityBody, encoding);
            return (TBody)serializer.Deserialize(encodedStream);
        }
    }
}
