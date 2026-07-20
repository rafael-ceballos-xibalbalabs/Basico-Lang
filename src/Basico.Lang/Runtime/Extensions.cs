using System.Globalization;

namespace Basico.Runtime
{
    public static class Extensions
    {
        public static string ToDelimitedString<T>(this IEnumerable<T> values)
        {
            return values.ToDelimitedString(x => x.ToString(),
            CultureInfo.CurrentCulture.TextInfo.ListSeparator);
        }
        public static string ToDelimitedString<T>(
        this IEnumerable<T> source, Func<T, string> converter)
        {
            return source.ToDelimitedString(converter,
                CultureInfo.CurrentCulture.TextInfo.ListSeparator);
        }


        public static string ToDelimitedString<T>(this IEnumerable<T> source,
            Func<T, string> converter, string separator)
        {
            return string.Join(separator, source.Select(converter).ToArray());
        }


    }
}
