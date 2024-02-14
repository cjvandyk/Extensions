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
    /// Extension methods for the System.DateTime.TimeZoneInfo class.
    /// </summary>
    [Serializable]
    public static partial class TimeZoneInfoExtensions
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
            ValidateNoNulls(timeZone);
            return Constants.TimeZones[timeZone];
        }
        #endregion GetTimeZoneString()
    }
}
