﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace Cuemon
{
    internal class StringReplaceEngine
    {
        internal StringReplaceEngine(string value, IEnumerable<StringReplacePair> replacePairs, StringComparison comparison)
        {
            Value = value;
            ReplacePairs = replacePairs;
            Comparison = comparison;
            LastStartIndex = -1;
            LastLength = -1;
            ReplaceCoordinates = new List<StringReplaceCoordinate>();
        }

        private static RegexOptions ToRegExOptions(StringComparison comparison)
        {
            RegexOptions options = RegexOptions.None;
            switch (comparison)
            {
                case StringComparison.CurrentCulture:
                    break;
                case StringComparison.InvariantCulture:
                case StringComparison.Ordinal:
                    options = RegexOptions.CultureInvariant;
                    break;
                case StringComparison.CurrentCultureIgnoreCase:
                case StringComparison.InvariantCultureIgnoreCase:
                case StringComparison.OrdinalIgnoreCase:
                    options = RegexOptions.IgnoreCase;
                    break;
            }
            return options;
        }

        private static string ToRegExPattern(IEnumerable<StringReplacePair> replacePairs, out IDictionary<string, string> lookupTable)
        {
            lookupTable = new Dictionary<string, string>();
            StringBuilder pattern = new StringBuilder();
            foreach (StringReplacePair replacePair in replacePairs)
            {
                char[] characters = replacePair.OldValue.ToCharArray();
                foreach (char character in characters)
                {
                    pattern.AppendFormat(CultureInfo.InvariantCulture, @"\u{0:x4}", (uint)character);
                }
                pattern.Append("|");
                lookupTable.Add(replacePair.OldValue.ToUpperInvariant(), replacePair.NewValue);
            }
            pattern.Remove(pattern.Length - 1, 1);
            return pattern.ToString();
        }

        private string RenderReplacement()
        {
            IDictionary<string, string> lookupTable;
            Regex regex = new Regex(ToRegExPattern(ReplacePairs, out lookupTable), ToRegExOptions(Comparison));
            MatchCollection matches = regex.Matches(Value);
            foreach (Match match in matches)
            {
                ReplaceCoordinates.Add(new StringReplaceCoordinate(match.Index, match.Length, lookupTable[match.Value.ToUpperInvariant()]));
            }

            int startIndex = 0;
            if (ReplaceCoordinates.Count == 0) { return Value; }
            StringBuilder builder = new StringBuilder();
            foreach (StringReplaceCoordinate replaceCoordinate in ReplaceCoordinates)
            {
                var currentIndex = replaceCoordinate.StartIndex;
                var currentLength = replaceCoordinate.Length;

                if (LastStartIndex == -1)
                {
                    builder.Append(Value.Substring(startIndex, currentIndex));
                }
                else
                {
                    if (currentIndex > LastStartIndex)
                    {
                        int lastPosition = LastStartIndex + LastLength;
                        builder.Append(Value.Substring(lastPosition, currentIndex - lastPosition));
                    }
                }
                builder.Append(replaceCoordinate.Value);
                LastLength = currentLength;
                LastStartIndex = currentIndex;
            }

            startIndex = LastStartIndex + LastLength;
            if (startIndex < Value.Length)
            {
                builder.Append(Value.Substring(LastStartIndex + LastLength));
            }

            return builder.ToString();
        }

        private StringComparison Comparison { get; set; }

        private IEnumerable<StringReplacePair> ReplacePairs { get; set; }

        private List<StringReplaceCoordinate> ReplaceCoordinates { get; set; }

        private int LastStartIndex { get; set; }

        private int LastLength { get; set; }

        private string Value { get; set; }

        public override string ToString()
        {
            return RenderReplacement();
        }
    }
}