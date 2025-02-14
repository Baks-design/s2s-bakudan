using System;
using System.Collections.Generic;
using System.Text;

namespace Game.Runtime.Utilities.Extensions
{
    public static class StringExtensions
    {
        /// <summary>Checks if a string is Null or white space.</summary>
        public static bool IsNullOrWhiteSpace(this string val) => string.IsNullOrWhiteSpace(val);

        /// <summary>Checks if a string is Null or empty.</summary>
        public static bool IsNullOrEmpty(this string value) => string.IsNullOrEmpty(value);

        /// <summary>Checks if a string is null, empty, or white space.</summary>
        public static bool IsBlank(this string val) => string.IsNullOrWhiteSpace(val);

        /// <summary>Returns the string or an empty string if it is null.</summary>
        public static string OrEmpty(this string val) => val ?? string.Empty;

        /// <summary>
        /// Shortens a string to the specified maximum length. If the string's length
        /// is less than the maxLength, the original string is returned.
        /// </summary>
        public static string Shorten(this string val, int maxLength)
        {
            if (val.IsBlank()) return val;
            return val.Length <= maxLength ? val : val[..maxLength];
        }

        /// <summary>Slices a string from the start index to the end index.</summary>
        /// <exception cref="ArgumentNullException">Thrown if the input string is null or empty.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if the start or end index is invalid.</exception>
        public static string Slice(this string val, int startIndex, int endIndex)
        {
            if (val.IsBlank())
                throw new ArgumentNullException(nameof(val), "Value cannot be null or empty.");

            if (startIndex < 0 || startIndex >= val.Length)
                throw new ArgumentOutOfRangeException(nameof(startIndex));

            // If the end index is negative, it will be counted from the end of the string.
            endIndex = endIndex < 0 ? val.Length + endIndex : endIndex;

            if (endIndex < 0 || endIndex < startIndex || endIndex > val.Length)
                throw new ArgumentOutOfRangeException(nameof(endIndex));

            return val[startIndex..endIndex];
        }

        /// <summary>
        /// Converts the input string to an alphanumeric string, optionally allowing periods.
        /// </summary>
        /// <param name="input">The input string to be converted.</param>
        /// <param name="allowPeriods">A boolean flag indicating whether periods should be allowed in the output string.</param>
        /// <returns>
        /// A new string containing only alphanumeric characters, underscores, and optionally periods.
        /// If the input string is null or empty, an empty string is returned.
        /// </returns>
        public static string ConvertToAlphanumeric(this string input, bool allowPeriods = false)
        {
            if (string.IsNullOrEmpty(input))
                return string.Empty;

            var filteredChars = new List<char>(input.Length);
            var lastValidIndex = -1;

            foreach (var character in input)
            {
                if (char.IsLetterOrDigit(character) || character == '_' || (allowPeriods && character == '.'))
                {
                    // Skip leading digits or periods
                    if (filteredChars.Count == 0 && (char.IsDigit(character) || character == '.'))
                        continue;

                    filteredChars.Add(character);
                    lastValidIndex = filteredChars.Count - 1;
                }
            }

            // Remove trailing periods
            while (lastValidIndex >= 0 && filteredChars[lastValidIndex] == '.')
                lastValidIndex--;

            return lastValidIndex >= 0
                ? new string(filteredChars.ToArray(), 0, lastValidIndex + 1)
                : string.Empty;
        }

        // Rich text formatting, for Unity UI elements that support rich text.
        private static readonly StringBuilder _richTextBuilder = new();

        public static string RichColor(this string text, string color) =>
            _richTextBuilder.Clear().Append("<color=").Append(color).Append(">").Append(text).Append("</color>").ToString();

        public static string RichSize(this string text, int size) =>
            _richTextBuilder.Clear().Append("<size=").Append(size).Append(">").Append(text).Append("</size>").ToString();

        public static string RichBold(this string text) =>
            _richTextBuilder.Clear().Append("<b>").Append(text).Append("</b>").ToString();

        public static string RichItalic(this string text) =>
            _richTextBuilder.Clear().Append("<i>").Append(text).Append("</i>").ToString();

        public static string RichUnderline(this string text) =>
            _richTextBuilder.Clear().Append("<u>").Append(text).Append("</u>").ToString();

        public static string RichStrikethrough(this string text) =>
            _richTextBuilder.Clear().Append("<s>").Append(text).Append("</s>").ToString();

        public static string RichFont(this string text, string font) =>
            _richTextBuilder.Clear().Append("<font=").Append(font).Append(">").Append(text).Append("</font>").ToString();

        public static string RichAlign(this string text, string align) =>
            _richTextBuilder.Clear().Append("<align=").Append(align).Append(">").Append(text).Append("</align>").ToString();

        public static string RichGradient(this string text, string color1, string color2) =>
            _richTextBuilder.Clear().Append("<gradient=").Append(color1).Append(",").Append(color2).Append(">").Append(text).Append("</gradient>").ToString();

        public static string RichRotation(this string text, float angle) =>
            _richTextBuilder.Clear().Append("<rotate=").Append(angle).Append(">").Append(text).Append("</rotate>").ToString();

        public static string RichSpace(this string text, float space) =>
            _richTextBuilder.Clear().Append("<space=").Append(space).Append(">").Append(text).Append("</space>").ToString();
    }
}