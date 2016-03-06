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
            Data = ByteConverter.FromStream(data, true);
            Length = length;
            Boundary = boundary;
            Encoding = encoding;
            LineLength = -1;
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
            return Position >= Length || LastBoundaryFound;
        }

        private bool GetNextLine()
        {
            LineStart = -1;
            int index = Position;
            while (index < Length)
            {
                if (Data[index] == 10)
                {
                    LineStart = Position;
                    LineLength = index - Position;
                    Position = index + 1;
                    if ((LineLength > 0) && (Data[index - 1] == 13)) { --LineLength; }
                    break;
                }

                if (++index == Length)
                {
                    LineStart = Position;
                    LineLength = index - Position;
                    Position = Length;
                }
            }
            return LineStart >= 0;
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
            PartName = null;
            PartFileName = null;
            PartContentType = null;
            while (GetNextLine())
            {
                if (LineLength == 0) { return; }
                byte[] buffer = new byte[LineLength];
                Array.Copy(Data, LineStart, buffer, 0, LineLength);
                string currentLine = Encoding.GetString(buffer);
                int index = currentLine.IndexOf(':');
                if (index >= 0)
                {
                    string partial = currentLine.Substring(0, index);
                    if (partial.Equals("Content-Disposition", StringComparison.OrdinalIgnoreCase))
                    {
                        PartName = ExtractValueFromContentDispositionHeader(currentLine, index + 1, "name");
                        PartFileName = ExtractValueFromContentDispositionHeader(currentLine, index + 1, "filename");
                    }
                    else if (partial.Equals("Content-Type", StringComparison.OrdinalIgnoreCase))
                    {
                        PartContentType = new ContentType(currentLine.Substring(index + 1).Trim());
                    }
                }
            }
        }

        private bool AtBoundaryLine()
        {
            int length = Boundary.Length;
            if ((LineLength != length) && (LineLength != (length + 2))) { return false; }

            for (int i = 0; i < length; i++)
            {
                if (Data[LineStart + i] != Boundary[i]) { return false; }
            }

            if (LineLength != length)
            {
                if ((Data[LineStart + length] != 0x2d) || (Data[(LineStart + length) + 1] != 0x2d)) { return false; }
                LastBoundaryFound = true;
            }
            return true;
        }

        private void ParsePartData()
        {
            PartDataStart = Position;
            PartDataLength = -1;
            while (GetNextLine())
            {
                if (AtBoundaryLine())
                {
                    int previousLineNo = LineStart - 1;
                    if (Data[previousLineNo] == 10) { previousLineNo--; }
                    if (Data[previousLineNo] == 13) { previousLineNo--; }
                    PartDataLength = (previousLineNo - PartDataStart) + 1;
                    return;
                }
            }
        }

        internal IEnumerable<HttpMultipartContent> ToEnumerable()
        {
            while (GetNextLine())
            {
                if (AtBoundaryLine()) { break; }
            }

            if (AtEndOfData())
            {
                yield return null;
            }

            Parse:
            ParsePartHeaders();
            if (!AtEndOfData())
            {
                ParsePartData();
                if (PartDataLength != -1)
                {
                    if (PartName != null)
                    {
                        yield return new HttpMultipartContent(PartName, PartFileName, PartContentType, new MemoryStream(Data, PartDataStart, PartDataLength));
                    }

                    if (!AtEndOfData()) { goto Parse; }
                }
            }
        }
    }
}
