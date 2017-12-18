using System.Globalization;
using System.Text;

namespace Inshapardaz.Domain.Helpers
{
    public static class StringExtentions
    {
        public static string TrimSpecialCharacters(this string input)
        {
            return input.Trim(' ', '\'', '"', '[', ']', '(', ')', ',', '۔', '.', '‌', '،' );
        }

        public static string RemoveMovements(this string input)
        {
            var result = new StringBuilder();
            foreach (var c in input)
            {
                if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                {
                    result.Append(c);
                }
            }
            return result.ToString();
        }

        public static string[] SplitIntoSentences(this string input)
        {
            return input.Split('?', '۔', '.', '\n');
        }
    }
}
