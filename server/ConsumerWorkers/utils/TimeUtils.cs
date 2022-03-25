namespace Chronoria_ConsumerWorkers.utils
{
    public class TimeUtils
    {
        public static long DateTimeToEpochMs(DateTime dt)
        {
            return new DateTimeOffset(dt).ToUnixTimeMilliseconds();
        }
        public static DateTime EpochMsToDateTime(long ms)
        {
            return DateTimeOffset.FromUnixTimeMilliseconds(ms).DateTime;
        }
        public static DateTime now()
        {
            return DateTime.Now.ToUniversalTime();
        }
        public static string DateTimeToString(DateTime dt)
        {
            return dt.ToString("R");
        }
        public static string DateTimeToString(long ms)
        {
            return EpochMsToDateTime(ms).ToString("R");
        }
    }
}
