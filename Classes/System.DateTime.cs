#pragma warning disable IDE1006, IDE0017, CS0162, IDE0060, IDE0079 // Naming Styles, Simplify declaration (FQCN used), break after return, Remove unused (string[] args, Remove unnecessary suppression)

/// <summary>
/// Author: Cornelius J. van Dyk blog.cjvandyk.com @cjvandyk
/// This code is provided under GNU GPL 3.0 and is a copyrighted work of the
/// author and contributors.  Please see:
/// https://github.com/cjvandyk/Quix/blob/master/LICENSE
/// </summary>

using System;

namespace Extensions
{
    /// <summary>
    /// Extensions for the System.DateTime class.
    /// </summary>
    public static class DateTimeExtensions
    {
        #region ToTimeZone()
        public static System.DateTime ToTimeZone(this System.DateTime dateTime,
                                          Constants.TimeZone fromZone = 
                                              Constants.TimeZone.CentralStandardTime,
                                          Constants.TimeZone toZone = 
                                              Constants.TimeZone.EasternStandardTime)
        {
            Universal.ValidateNoNulls(System.Reflection.MethodInfo.GetCurrentMethod().GetParameters());
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
