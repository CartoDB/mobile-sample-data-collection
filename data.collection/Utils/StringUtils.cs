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
    }
}
