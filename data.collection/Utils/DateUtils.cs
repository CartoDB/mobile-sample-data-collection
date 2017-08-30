using System;
namespace data.collection
{
    public class DateUtils
    {
        public static double CurrentMilliseconds
        {
            get { return DateTime.Now.ToUnixTimestamp(); }
        }
    }

    public static class DateExtensions
    {
		public static double ToUnixTimestamp(this DateTime date)
		{
			DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
			TimeSpan diff = date.ToUniversalTime() - origin;
			return Math.Floor(diff.TotalSeconds);
		}

	}
}
