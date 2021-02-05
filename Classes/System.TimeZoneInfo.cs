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
    /// Extension methods for the System.DateTime.TimeZoneInfo class.
    /// </summary>
    public static class TimeZoneInfoExtensions
    {
        #region GetTimeZoneString()
        /// <summary>
        /// Get the registry ID string for the given time zone.
        /// </summary>
        /// <param name="timeZone">The target time zone from Constants.TimeZone.</param>
        /// <returns>The string that can be used with 
        /// TimeZoneInfo.FindSystemTimeZoneById() for time zone convertions.</returns>
        public static string GetTimeZoneString(Constants.TimeZone timeZone)
        {
            Universal.ValidateNoNulls(timeZone);
            return Constants.TimeZones[timeZone];
        }
        #endregion GetTimeZoneString()
    }
}
