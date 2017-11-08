using System;
namespace data.collection
{
    public static class StringUtils
    {
        public static string EscapeQuotes(this string source)
        {
            // Escape single apostrophes, replace quotation marks
            return source.Replace("'", "''").Replace("\"", "");
        }

        public static string EscapeNewLine(this string source)
        {
            return source.Replace(Environment.NewLine, "");
        }
    }
}
