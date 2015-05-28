using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net.Mime;
using System.Text;

namespace Cuemon.ServiceModel
{
    internal sealed class HttpMultipartContentParser
    {
        internal HttpMultipartContentParser(Stream data, int length, string boundary, Encoding encoding) : this(data, length, encoding.GetBytes(string.Concat("--", boundary)), encoding)
        {
        }

        internal HttpMultipartContentParser(Stream data, int length, byte[] boundary, Encoding encoding)
        {
            this.Data = ConvertUtility.ToByteArray(data, true);
            this.Length = length;
            this.Boundary = boundary;
            this.Encoding = encoding;
            this.LineLength = -1;
        }

        private byte[] Data { get; set; }

        private int Length { get; set; }

        private byte[] Boundary { get; set; }

        private Encoding Encoding { get; set; }

        private int LineStart { get; set; }

        private int LineLength { get; set; }

        private int Position { get; set; }

        private bool LastBoundaryFound { get; set; }

        private string PartName { get; set; }

        private string PartFileName { get; set; }

        private ContentType PartContentType { get; set; }

        private int PartDataStart { get; set; }
        
        private int PartDataLength { get; set; }

        private bool AtEndOfData()
        {
            return this.Position >= this.Length || this.LastBoundaryFound;
        }

        private bool GetNextLine()
        {
            this.LineStart = -1;
            int index = this.Position;
            while (index < this.Length)
            {
                if (this.Data[index] == 10)
                {
                    this.LineStart = this.Position;
                    this.LineLength = index - this.Position;
                    this.Position = index + 1;
                    if ((this.LineLength > 0) && (this.Data[index - 1] == 13)) { --this.LineLength; }
                    break;
                }

                if (++index == this.Length)
                {
                    this.LineStart = this.Position;
                    this.LineLength = index - this.Position;
                    this.Position = this.Length;
                }
            }
            return this.LineStart >= 0;
        }

        private static string ExtractValueFromContentDispositionHeader(string currentLine, int position, string name)
        {
            string pattern = string.Format(CultureInfo.InvariantCulture, "{0}=\"", name);
            int startIndex = CultureInfo.InvariantCulture.CompareInfo.IndexOf(currentLine, pattern, position, CompareOptions.IgnoreCase);
            if (startIndex < 0) { return null; }
            
            startIndex += pattern.Length;
            int index = currentLine.IndexOf('"', startIndex);
            if (index < 0) { return null; }

            if (index == startIndex) { return string.Empty; }

            return currentLine.Substring(startIndex, index - startIndex);
        }

        private void ParsePartHeaders()
        {
            this.PartName = null;
            this.PartFileName = null;
            this.PartContentType = null;
            while (this.GetNextLine())
            {
                if (this.LineLength == 0) { return; }
                byte[] buffer = new byte[this.LineLength];
                Array.Copy(this.Data, this.LineStart, buffer, 0, this.LineLength);
                string currentLine = this.Encoding.GetString(buffer);
                int index = currentLine.IndexOf(':');
                if (index >= 0)
                {
                    string partial = currentLine.Substring(0, index);
                    if (partial.Equals("Content-Disposition", StringComparison.OrdinalIgnoreCase))
                    {
                        this.PartName = ExtractValueFromContentDispositionHeader(currentLine, index + 1, "name");
                        this.PartFileName = ExtractValueFromContentDispositionHeader(currentLine, index + 1, "filename");
                    }
                    else if (partial.Equals("Content-Type", StringComparison.OrdinalIgnoreCase))
                    {
                        this.PartContentType = new ContentType(currentLine.Substring(index + 1).Trim());
                    }
                }
            }
        }

        private bool AtBoundaryLine()
        {
            int length = this.Boundary.Length;
            if ((this.LineLength != length) && (this.LineLength != (length + 2))) { return false; }

            for (int i = 0; i < length; i++)
            {
                if (this.Data[this.LineStart + i] != this.Boundary[i]) { return false; }
            }

            if (this.LineLength != length)
            {
                if ((this.Data[this.LineStart + length] != 0x2d) || (this.Data[(this.LineStart + length) + 1] != 0x2d)) { return false; }
                this.LastBoundaryFound = true;
            }
            return true;
        }

        private void ParsePartData()
        {
            this.PartDataStart = this.Position;
            this.PartDataLength = -1;
            while (this.GetNextLine())
            {
                if (this.AtBoundaryLine())
                {
                    int previousLineNo = this.LineStart - 1;
                    if (this.Data[previousLineNo] == 10) { previousLineNo--; }
                    if (this.Data[previousLineNo] == 13) { previousLineNo--; }
                    this.PartDataLength = (previousLineNo - this.PartDataStart) + 1;
                    return;
                }
            }
        }

        internal IEnumerable<HttpMultipartContent> ToEnumerable()
        {
            while (this.GetNextLine())
            {
                if (this.AtBoundaryLine()) { break; }
            }

            if (this.AtEndOfData())
            {
                yield return null;
            }
        
            Parse:
            this.ParsePartHeaders();
            if (!this.AtEndOfData())
            {
                this.ParsePartData();
                if (this.PartDataLength != -1)
                {
                    if (this.PartName != null)
                    {
                        yield return new HttpMultipartContent(this.PartName, this.PartFileName, this.PartContentType, new MemoryStream(this.Data, this.PartDataStart, this.PartDataLength));
                    }

                    if (!this.AtEndOfData()) { goto Parse; }
                }
            }
        }
    }
}
