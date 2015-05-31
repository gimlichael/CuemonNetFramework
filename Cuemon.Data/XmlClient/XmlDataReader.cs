using System;
using System.Collections.Specialized;
using System.Xml;

namespace Cuemon.Data.XmlClient
{
    /// <summary>
    /// Provides a way of reading a forward-only stream of rows from an XML based data source. This class cannot be inherited.
    /// </summary>
    public sealed class XmlDataReader : StringDataReader
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="XmlDataReader" /> class.
        /// </summary>
        /// <param name="reader">The <see cref="XmlReader"/> object that contains the XML data.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="reader"/> is null.
        /// </exception>
        public XmlDataReader(XmlReader reader)
            : this(reader, ConvertUtility.ChangeType)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="XmlDataReader"/> class.
        /// </summary>
        /// <param name="reader">The <see cref="XmlReader"/> object that contains the XML data.</param>
        /// <param name="parser">The function delegate that returns a primitive object whose value is equivalent to the provided <see cref="String"/> value.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="reader"/> is null -or- <paramref name="parser"/> is null.
        /// </exception>
        /// <remarks>The default implementation uses <see cref="ConvertUtility.ChangeType(System.String)"/> as <paramref name="parser"/>.</remarks>
        public XmlDataReader(XmlReader reader, Doer<string, object> parser)
            : base(parser)
        {
            Validator.ThrowIfNull(reader, "reader");
            Validator.ThrowIfNull(parser, "parser");
            this.Reader = reader;
        }
        #endregion

        #region Properties
        private XmlReader Reader { get; set; }

        /// <summary>
        /// Gets a value indicating the depth of nesting for the current element.
        /// </summary>
        /// <value>The level of nesting.</value>
        public override int Depth
        {
            get { return this.CurrentDepth; }
        }

        private int CurrentDepth { get; set; }

        #endregion

        #region Methods
        /// <summary>
        /// Advances this instance to the next element of the XML data source.
        /// </summary>
        /// <returns><c>true</c> if there are more elements; otherwise, <c>false</c>.</returns>
        protected override bool ReadNext()
        {
            return ReadNextCore(this.Reader);
        }

        private bool ReadNextCore(XmlReader reader)
        {
            OrderedDictionary fields = new OrderedDictionary(StringComparer.OrdinalIgnoreCase);
            string elementName = null;
            while (reader.Read())
            {
                this.CurrentDepth = reader.Depth;
                switch (reader.NodeType)
                {
                    case XmlNodeType.Attribute:
                        this.Parse(reader, ref fields);
                        while (reader.MoveToNextAttribute())
                        {
                            this.Parse(reader, ref fields);
                        }
                        if (reader.MoveToElement()) { goto addFields; }
                        break;
                    case XmlNodeType.Element:
                        elementName = reader.LocalName;
                        if (reader.HasAttributes)
                        {
                            if (reader.MoveToFirstAttribute())
                            {
                                goto case XmlNodeType.Attribute;
                            }
                        }
                        break;
                    case XmlNodeType.CDATA:
                    case XmlNodeType.Text:
                        if (elementName != null) { this.Parse(elementName, reader, ref fields); }
                        elementName = null;
                        break;
                    case XmlNodeType.EndElement:
                        break;
                    default:
                        if (fields.Count > 0 && reader.Depth == 1) { goto addFields; }
                        break;
                }
            }
        addFields:
            this.SetFields(fields);
            return (fields.Count > 0);
        }

        private void Parse(XmlReader reader, ref OrderedDictionary values)
        {
            Parse(reader.LocalName, reader, ref values);
        }

        private void Parse(string localName, XmlReader reader, ref OrderedDictionary values)
        {
            if (values.Contains(localName))
            {
                values[localName] = StringParser(reader.Value);
            }
            else
            {
                values.Add(localName, StringParser(reader.Value));
            }
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
            if (this.IsDisposed) { return; }
            if (disposing)
            {
                if (this.Reader != null) { this.Reader.Close(); }
            }
            base.Dispose(disposing);
        }
        #endregion
    }
}
