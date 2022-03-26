namespace BaiduPanCompareTools.utils
{
    internal class DateTimeUtil
    {
        private static DateTime SYSTEM_START_TIME = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1, 0, 0, 0));
        private const string LONG_TIME_FORMAT = "yyyy-MM-dd HH:mm:ss";

        public static int GetCurrentTimestampSecond()
        {
            return Convert.ToInt32((DateTime.Now - SYSTEM_START_TIME).TotalSeconds);
        }

        public static int DateTimeToTimestampSecond(DateTime dt)
        {
            return Convert.ToInt32((dt - SYSTEM_START_TIME).TotalSeconds);
        }

        public static string TimestampSecondToLongDateString(int timestampSecond)
        {
            return DateTimeToLongDateString(TimestampSecondToDateTime(timestampSecond));
        }

        public static string DateTimeToLongDateString(DateTime dt)
        {
            return dt.ToString(LONG_TIME_FORMAT);
        }

        public static DateTime TimestampSecondToDateTime(int timestampSecond)
        {
            return SYSTEM_START_TIME.AddSeconds(timestampSecond);
        }
    }
}
