﻿using System;
using System.Xml.XPath;
using Cuemon.IO;
using Cuemon.Security.Cryptography;

namespace Cuemon.Xml
{
	/// <summary>
	/// Represents a JSON instance from an XML data source.
	/// </summary>
	public sealed class XmlJsonInstance : JsonInstance
	{
		private XPathNodeType _nodeType;

		/// <summary>
		/// Initializes a new instance of the <see cref="XmlJsonInstance"/> class.
		/// </summary>
		/// <param name="name">The name of the JSON object.</param>
		/// <param name="value">The value of the JSON object.</param>
		/// <param name="nodeNumber">The logical node number of the JSON object placement in the originating structural data source.</param>
		public XmlJsonInstance(string name, object value, int nodeNumber) : this(name, value, nodeNumber, XPathNodeType.Text)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="XmlJsonInstance"/> class.
		/// </summary>
		/// <param name="name">The name of the JSON object.</param>
		/// <param name="value">The value of the JSON object.</param>
		/// <param name="nodeNumber">The logical node number of the JSON object placement in the originating structural data source.</param>
		/// <param name="nodeType">The node type of the XML document to convert into a JSON representation.</param>
		public XmlJsonInstance(string name, object value, int nodeNumber, XPathNodeType nodeType) : base(name, value, nodeNumber)
		{
			_nodeType = nodeType;
		}

		/// <summary>
		/// Gets the originating node type of the XML document.
		/// </summary>
		/// <value>The originating node type of the XML document.</value>
		public XPathNodeType NodeType
		{
			get { return _nodeType; }
		}

		/// <summary>
		/// Computes and returns a MD5 signature of the following properties: <see cref="P:Cuemon.IO.JsonInstance.Name"/>, <see cref="P:Cuemon.IO.JsonInstance.Value"/>, <see cref="P:Cuemon.IO.JsonInstance.NodeNumber"/> and <see cref="NodeType"/>.
		/// </summary>
		/// <returns>
		/// A MD5 signature of the following properties: <see cref="P:Cuemon.IO.JsonInstance.Name"/>, <see cref="P:Cuemon.IO.JsonInstance.Value"/>, <see cref="P:Cuemon.IO.JsonInstance.NodeNumber"/> and <see cref="NodeType"/>.
		/// </returns>
		public override string GetSignature()
		{
			return HashUtility.ComputeHash(string.Concat(this.Name, this.Value, this.NodeNumber, Enum.GetName(typeof(XPathNodeType), this.NodeType)));
		}
	}
}