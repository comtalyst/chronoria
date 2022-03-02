namespace Chronoria_PersistentWorkers.utils
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
            return DateTime.Now;
        }
    }
}
