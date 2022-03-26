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
        public static string TimeSpanToString(long ms1, long ms2)
        {
            var dt1 = EpochMsToDateTime(ms1);
            var dt2 = EpochMsToDateTime(ms2);

            if(dt1.AddYears(1) > dt2)
            {
                var ts = TimeSpan.FromMilliseconds(ms2 - ms1);
                return ts.TotalDays + " Days";
            }
            else
            {
                var minYearDiff = dt2.Year - dt1.Year - 1;
                dt1 = dt1.AddYears(minYearDiff);
                var yearDiff = minYearDiff;
                while(dt1.AddYears(1) <= dt2)               // constant runtime
                {
                    dt1 = dt1.AddYears(1);
                    yearDiff += 1;
                }
                var ts = dt2 - dt1;
                return yearDiff + " Years and " + ts.TotalDays + " Days";
            }
        }
    }
}
