/// <summary>
/// Author: Cornelius J. van Dyk blog.cjvandyk.com @cjvandyk
/// This code is provided under GNU GPL 3.0 and is a copyrighted work of the
/// author and contributors.  Please see:
/// https://github.com/cjvandyk/Extensions/blob/main/LICENSE
/// </summary>

using System;
using static Extensions.Core;

namespace Extensions
{
    /// <summary>
    /// Extensions for the System.DateTime class.
    /// </summary>
    [Serializable]
    public static partial class DateTimeExtensions
    {
        #region AddWeeks
        /// <summary>
        /// A method for calculating additions of Weeks time units.
        /// </summary>
        /// <param name="dateTime">The current specified date/time.</param>
        /// <param name="value">The number of weeks to add.</param>
        /// <returns>The date/time after adding the specified number of
        /// weeks.</returns>
        public static DateTime AddWeeks(this DateTime dateTime, double value)
        {
            return dateTime.AddDays(value * 7);
        }
        #endregion AddWeeks

        #region AddDecades
        /// <summary>
        /// A method for calculating additions of Decades time units.
        /// </summary>
        /// <param name="dateTime">The current specified date/time.</param>
        /// <param name="value">The number of decades to add.</param>
        /// <returns>The date/time after adding the specified number of
        /// decades.</returns>
        public static DateTime AddDecades(this DateTime dateTime, int value)
        {
            return dateTime.AddYears(value * 10);
        }
        #endregion AddWeeks

        #region AddCenturies
        /// <summary>
        /// A method for calculating additions of Centuries time units.
        /// </summary>
        /// <param name="dateTime">The current specified date/time.</param>
        /// <param name="value">The number of centuries to add.</param>
        /// <returns>The date/time after adding the specified number of
        /// centuries.</returns>
        public static DateTime AddCenturies(this DateTime dateTime, int value)
        {
            return dateTime.AddYears(value * 100);
        }
        #endregion AddCenturies

        #region AddMillennia
        /// <summary>
        /// A method for calculating additions of Millennia time units.
        /// </summary>
        /// <param name="dateTime">The current specified date/time.</param>
        /// <param name="value">The number of millennia to add.</param>
        /// <returns>The date/time after adding the specified number of
        /// millennia.</returns>
        public static DateTime AddMillennia(this DateTime dateTime, int value)
        {
            return dateTime.AddYears(value * 1000);
        }
        #endregion AddMillennia

        #region Ago
        /// <summary>
        /// An enum specifying the type of time span.
        /// </summary>
        public enum AgoSpan
        {
            Seconds,
            Minutes,
            Hours,
            Days,
            Weeks,
            Months,
            Years,
            Decades,
            Centuries,
            Millennia
        }

        /// <summary>
        /// Method to calculate the date/time a specified number of time
        /// units ago.
        /// </summary>
        /// <param name="dateTime">The current specified date/time.</param>
        /// <param name="value">The number of time units to subtract.</param>
        /// <param name="timeSpan">The type of time units to subtract.</param>
        /// <returns>The date/time after specified time units have been
        /// subtracted.</returns>
        public static DateTime Ago(this DateTime dateTime,
                                   double value,
                                   AgoSpan timeSpan)
        {
            ValidateNoNulls(dateTime, value, timeSpan);
            switch (timeSpan)
            {            
                case AgoSpan.Seconds:
                    return dateTime.AddSeconds(value * -1);
                    break;
                case AgoSpan.Minutes:
                    return dateTime.AddMinutes(value * -1);
                    break;
                case AgoSpan.Hours:
                    return dateTime.AddHours(value * -1);
                    break;
                case AgoSpan.Days:
                    return dateTime.AddDays(value * -1);
                    break;
                case AgoSpan.Weeks:
                    return dateTime.AddWeeks(value * -1);
                    break;
                case AgoSpan.Months:
                    return dateTime.AddMonths((int)value * -1);
                    break;
                case AgoSpan.Years:
                    return dateTime.AddYears((int)value * -1);
                    break;
                case AgoSpan.Decades:
                    return dateTime.AddDecades((int)value * -1);
                    break;
                case AgoSpan.Centuries:
                    return dateTime.AddCenturies((int)value * -1);
                    break;
                case AgoSpan.Millennia:
                    return dateTime.AddMillennia((int)value * -1);
                    break;
            }
            return dateTime;
        }
        #endregion Ago

        #region Quarter
        /// <summary>
        /// A method that returns the quarter number based on the specified 
        /// date.
        /// </summary>
        /// <param name="dateTime">The specified date.</param>
        /// <returns>Numbers 1 through 4 representing the current 
        /// quarter.</returns>
        public static int Quarter(this DateTime dateTime)
        {
            ValidateNoNulls(dateTime);
            if (dateTime.Month <= 3)
            {
                return 1;
            }
            if (dateTime.Month <= 6)
            {
                return 2;
            }
            if (dateTime.Month <= 9)
            {
                return 3;
            }
            return 4;
        }
        #endregion Quarter

        #region ToTimeZone()
        /// <summary>
        /// Convert the current time between different specified time zones.
        /// </summary>
        /// <param name="dateTime">The current date/time.</param>
        /// <param name="fromZone">The specified time zone of the current
        /// date/time.</param>
        /// <param name="toZone">The target time zone to which the current
        /// date/time is converted.</param>
        /// <returns>The date/time in the target time zone.</returns>
        public static System.DateTime ToTimeZone(
            this System.DateTime dateTime,
            Constants.TimeZone fromZone = Constants.TimeZone.CentralStandardTime,
            Constants.TimeZone toZone = Constants.TimeZone.EasternStandardTime)
        {
            ValidateNoNulls(dateTime, fromZone, toZone);
            System.DateTime now = System.TimeZoneInfo.ConvertTimeToUtc(
                new System.DateTime(dateTime.Ticks, DateTimeKind.Unspecified),
                System.TimeZoneInfo.FindSystemTimeZoneById(
                    Extensions.TimeZoneInfoExtensions.GetTimeZoneString(fromZone)));
            return System.TimeZoneInfo.ConvertTimeFromUtc(
                now,
                System.TimeZoneInfo.FindSystemTimeZoneById(
                    Extensions.TimeZoneInfoExtensions.GetTimeZoneString(toZone)));
        }
        #endregion ToTimeZone()
    }
}
