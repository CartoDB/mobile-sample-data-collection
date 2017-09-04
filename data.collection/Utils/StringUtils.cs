using System;
namespace data.collection
{
    public static class StringUtils
    {
        public static string EscapeQuotes(this string source)
        {
            return source.Replace("'", "''").Replace("\"", "\\\"");
        }
    }
}
