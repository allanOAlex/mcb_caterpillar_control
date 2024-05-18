namespace GECA.Client.Console.Shared
{
    public static class AppExtensions
    {
        public static string ToString(this DateTime? dt, string format) => dt == null ? "n/a" : ((DateTime)dt).ToString(format);
        public static string ToString(this DateTimeOffset? dt, string format) => dt == null ? "n/a" : ((DateTimeOffset)dt).ToString(format);
    }
}
