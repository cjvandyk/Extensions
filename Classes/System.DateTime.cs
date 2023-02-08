#pragma warning disable CS1587, CS1591, CS1998, IDE0059, IDE0028

/// <summary>
/// Author: Cornelius J. van Dyk blog.cjvandyk.com @cjvandyk
/// This code is provided under GNU GPL 3.0 and is a copyrighted work of the
/// author and contributors.  Please see:
/// https://github.com/cjvandyk/Extensions/blob/main/LICENSE
/// </summary>

using System;
using static Extensions.Universal;

namespace Extensions
{
    /// <summary>
    /// Extensions for the System.DateTime class.
    /// </summary>
    [Serializable]
    public static partial class DateTimeExtensions
    {
        #region Ago
        private static DateTime Ago(this DateTime time,
                                   int number)
        {
            return default;
        }
        #endregion Ago

        #region ToTimeZone()
        public static System.DateTime ToTimeZone(this System.DateTime dateTime,
                                          Constants.TimeZone fromZone = 
                                              Constants.TimeZone.CentralStandardTime,
                                          Constants.TimeZone toZone = 
                                              Constants.TimeZone.EasternStandardTime)
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
#pragma warning restore CS1587, CS1591, CS1998, IDE0059, IDE0028
