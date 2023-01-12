#pragma warning disable CS1587

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
    /// Extension methods for the GCCH set of classes.
    /// </summary>
    public static partial class GCCH
    {
        #region GCCH()
        /// <summary>
        /// Get the registry ID string for the given time zone.
        /// </summary>
        /// <param name="timeZone">The target time zone from Constants.TimeZone.</param>
        /// <returns>The string that can be used with 
        /// TimeZoneInfo.FindSystemTimeZoneById() for time zone convertions.</returns>
        public static string GetTimeZoneString(Constants.TimeZone timeZone)
        {
            ValidateNoNulls(timeZone);
            return Constants.TimeZones[timeZone];
        }
        #endregion GetTimeZoneString()
    }
}
#pragma warning restore CS1587
