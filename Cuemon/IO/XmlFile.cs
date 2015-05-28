using System;
using Cuemon.Diagnostics;
namespace Cuemon.IO
{
    /// <summary>
    /// Exposes an interface for reading common properties of a XML file.
    /// </summary>
    public sealed class XmlFile : FileBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="XmlFile"/> class.
        /// </summary>
        /// <param name="fileLocation">The file location.</param>
        public XmlFile(string fileLocation)
            : base(fileLocation)
        {
        }
    }
}