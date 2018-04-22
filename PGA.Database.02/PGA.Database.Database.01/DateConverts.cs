#region

using System;
using System.Globalization;

#endregion

namespace PGA.Database
{
    /// <summary>
    ///     Class DateConverts.
    /// </summary>
    public static class DateConverts
    {
        /// <summary>
        ///     Converts the date time to ticks.
        /// </summary>
        /// <param name="dtInput">The dt input.</param>
        /// <returns>System.Int64.</returns>
        public static long ConvertDateTimeToTicks(DateTime dtInput)
        {
            return dtInput.Ticks;
        }

        /// <summary>
        ///     Converts the ticks to date time.
        /// </summary>
        /// <param name="lticks">The lticks.</param>
        /// <returns>DateTime.</returns>
        public static DateTime ConvertTicksToDateTime(long lticks)
        {
            var dtresult = new DateTime(lticks);
            return dtresult;
        }


        /// <summary>
        ///     Cons the date time to string for database.
        /// </summary>
        /// <param name="time">The time.</param>
        /// <returns>System.String.</returns>
        public static string DateTimeToStringForDatabase(DateTime time)
        {
            const string FMT = "O";
            var strDate = time.ToString(FMT);
            return strDate;
        }

        /// <summary>
        ///     Cons the date time to string for database.
        /// </summary>
        /// <param name="time">The time.</param>
        /// <returns>DateTime.</returns>
        public static DateTime StringToDateTimeForDatabase(string time)
        {
            const string FMT = "O";
            var now = DateTime.ParseExact(time, FMT, CultureInfo.InvariantCulture);
            return now;
        }

        public static string GetDateTimeNowString()
        {
            const string FMT = "O";
            var time = DateTime.Now;
            var strDate = time.ToString(FMT);
            return strDate;
        }

        /// <summary>
        ///     Gets the date time now. No need to override.
        ///     DateTime.Now works.
        /// </summary>
        /// <returns>DateTime.</returns>
        public static DateTime GetDateTimeNow()
        {
            return DateTime.Now;
        }

        public static bool DateTimesAreEqual(DateTime d1, DateTime d2)
        {
            const string FMT = "O";

            var str1 = d1.ToString(FMT);
            var str2 = d2.ToString(FMT);

            var now1 = DateTime.ParseExact(str1, FMT, CultureInfo.InvariantCulture);
            var now2 = DateTime.ParseExact(str2, FMT, CultureInfo.InvariantCulture);
            if (now1.ToBinary() == now2.ToBinary())
                return true;
            return false;
        }

        public static string DateTimeToStringFileSafe(DateTime value)
        {
            return value.ToString("yyyyMMdd-HHmmss");
        }
    }
}