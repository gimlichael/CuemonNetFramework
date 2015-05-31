using System.Collections.Generic;
using System.Text;

namespace Cuemon
{
    internal class StringReplaceEngine
    {
        internal StringReplaceEngine(string value) : base()
        {
            this.Value = value;
            this.LastStartIndex = -1;
            this.LastLength = -1;
            this.ReplaceCoordinates = new List<StringReplaceCoordinate>();
        }

        internal string RenderReplacement()
        {
            int startIndex = 0;
            int currentIndex;
            int currentLength;
            if (this.ReplaceCoordinates.Count == 0) { return this.Value; }
            StringBuilder builder = new StringBuilder();
            foreach (StringReplaceCoordinate replaceCoordinate in this.ReplaceCoordinates)
            {
                currentIndex = replaceCoordinate.StartIndex;
                currentLength = replaceCoordinate.Length;

                if (this.LastStartIndex == -1)
                {
                    builder.Append(this.Value.Substring(startIndex, currentIndex));
                }
                else
                {
                    if (currentIndex > this.LastStartIndex)
                    {
                        int lastPosition = this.LastStartIndex + this.LastLength;
                        builder.Append(this.Value.Substring(lastPosition, currentIndex - lastPosition));
                    }
                }
                builder.Append(replaceCoordinate.Value);
                this.LastLength = currentLength;
                this.LastStartIndex = currentIndex;
            }

            if (builder.Length < this.Value.Length)
            {
                builder.Append(this.Value.Substring(this.LastStartIndex + this.LastLength));
            }

            return builder.ToString();
        }

        internal List<StringReplaceCoordinate> ReplaceCoordinates { get; set; }
        internal int LastStartIndex { get; set; }
        internal int LastLength { get; set; }
        internal string Value { get; set; }
    }
}