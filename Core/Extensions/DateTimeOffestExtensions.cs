using System;

namespace Core.Extensions
{
    public static class DateTimeOffestExtensions
    {
        public static DateTimeOffset StartOfWeek(this DateTimeOffset dt, DayOfWeek startOfWeek = DayOfWeek.Monday)
        {
            int diff = (7 + (dt.DayOfWeek - startOfWeek)) % 7;
            return dt.AddDays(-1 * diff).Date;
        }

        public static DateTimeOffset GetFirstWeekOfYear(this DateTimeOffset dt)
        {
            var diff = dt.DayOfWeek - DayOfWeek.Monday;
            var year = dt.AddDays(diff == -1 ? -6 : -diff).Year;
            return GetFirstWeekOfYear(year, dt.Offset.Hours);
        }

        public static DateTimeOffset GetFirstWeekOfYear(int year, int offset = 0)
        {
            var dt = new DateTimeOffset(year, 1, 1, 0, 0, 0, new TimeSpan(0, offset, 0, 0));

            if (dt.DayOfWeek == DayOfWeek.Monday)
            {
                return dt;
            }
            var diff = DayOfWeek.Monday - dt.DayOfWeek;
            return dt.AddDays(diff == 1 ? 1 : 7 + diff);
        }
    }
}