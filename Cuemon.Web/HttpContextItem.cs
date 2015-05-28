using System;
using System.Collections.Generic;
using System.Web;
using System.Xml.Serialization;
using Cuemon.Xml.Serialization;
namespace Cuemon.Web
{
    /// <summary>
    /// Specifies the different (and most important) supported types of the <see cref="HttpContext"/> object.
    /// </summary>
    public enum HttpContextType
    {
        /// <summary>
        /// Specifies that items are taken from the application state object.
        /// </summary>
        Application,
        /// <summary>
        /// Specifies that items are taken from the cookies sent by the client.
        /// </summary>
        Cookies,
        /// <summary>
        /// Specifies that items are taken from the form variables.
        /// </summary>
        Form,
        /// <summary>
        /// Specifies that files are taken from the form variables.
        /// </summary>
        Files,
        /// <summary>
        /// Specifies that items are taken from the HTTP query string variables.
        /// </summary>
        QueryString,
        /// <summary>
        /// Specifies that items are taken from the session state object.
        /// </summary>
        Session,
        /// <summary>
        /// Specifies that items are taken from the Web server variables.
        /// </summary>
        ServerVariables 
    }

    /// <summary>
    /// Represents an item in the <see cref="HttpContext"/> class.
    /// </summary>
    [XmlRoot("Item")]
    [XmlSerialization(EnableAutomatedXmlSerialization = true)]
    public sealed class HttpContextItem : XmlSerialization
    {
        private readonly string _name;
        private readonly object _value;
        private readonly HttpContextType _contextType;
        private readonly DateTime? _expires;
        private readonly int? _contentLength;
        private readonly string _fileName;
        private readonly string _contentType;

        #region Constructors
        HttpContextItem()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpContextItem"/> class.
        /// </summary>
        /// <param name="name">The name of the item.</param>
        /// <param name="value">The value of the item.</param>
        /// <param name="contextType">The actual context type to mark for the item.</param>
        internal HttpContextItem(string name, object value, HttpContextType contextType)
        {
            _name = name;
            _value = value;
            _contextType = contextType;
            _expires = null;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpContextItem"/> class.
        /// </summary>
        /// <param name="name">The name of the item.</param>
        /// <param name="value">The value of the item.</param>
        /// <param name="contextType">The actual context type to mark for the item.</param>
        /// <param name="expires">The expiration date of the item. Only set from cookies.</param>
        internal HttpContextItem(string name, object value, HttpContextType contextType, DateTime expires)
        {
            _name = name;
            _value = value;
            _contextType = contextType;
            _expires = expires;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpContextItem"/> class.
        /// </summary>
        /// <param name="name">The name of the item.</param>
        /// <param name="contextType">The actual context type to mark for the item.</param>
        /// <param name="fileName">The name of the file item.</param>
        /// <param name="contentType">The MIME type of the file item.</param>
        /// <param name="contentLength">The length of the file item.</param>
        internal HttpContextItem(string name, HttpContextType contextType, string fileName, string contentType, int contentLength)
        {
            _name = name;
            _value = null;
            _fileName = fileName;
            _contextType = contextType;
            _contentLength = contentLength;
            _contentType = contentType;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the request type of the item.
        /// </summary>
        /// <value>The request type of the item.</value>
        [XmlAttribute("type")]
        public HttpContextType Type
        {
            get { return _contextType; }
        }

        /// <summary>
        /// Gets the name of the item.
        /// </summary>
        /// <value>The name of the item.</value>
        [XmlAttribute("name")]
        public string Name
        {
            get { return _name; }
        }

        /// <summary>
        /// Gets the value of the item.
        /// </summary>
        /// <value>The value of the item.</value>
        [XmlText]
        public object Value
        {
            get { return _value; }
        }

        /// <summary>
        /// Gets the expiration date of the cookie item.
        /// </summary>
        /// <value>The expiration date of the cookie item.</value>
        [XmlAttribute("expires")]
        public DateTime? Expires
        {
            get
            {
                if (this.Type == HttpContextType.Cookies)
                {
                    return _expires.Value;
                }
                return null;
            }
        }

        /// <summary>
        /// Gets the length of a file item.
        /// </summary>
        /// <value>The length of a file item.</value>
        [XmlAttribute("contentLength")]
        public int? ContentLength
        {
            get
            {
                if (this.Type == HttpContextType.Files)
                {
                    return _contentLength.Value;
                }
                return null;
            }
        }

        /// <summary>
        /// Gets the name of the file item.
        /// </summary>
        /// <value>The name of the file item.</value>
        [XmlAttribute("fileName")]
        public string FileName
        {
            get
            {
                if (this.Type == HttpContextType.Files)
                {
                    return _fileName;
                }
                return null;
            }
        }

        /// <summary>
        /// Gets the MIME type of a file item.
        /// </summary>
        /// <value>The MIME type of a file item.</value>
        [XmlAttribute("contentType")]
        public string ContentType
        {
            get
            {
                if (this.Type == HttpContextType.Files)
                {
                    return _contentType;
                }
                return null;
            }
        }
        #endregion
    }
}