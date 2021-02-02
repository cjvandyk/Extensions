using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Extensions
{
    public static class TimeZoneInfo
    {
        /// <summary>
        /// Get the registry ID string for the given time zone.
        /// </summary>
        /// <param name="timeZone">The target time zone from Constants.TimeZone.</param>
        /// <returns>The string that can be used with 
        /// TimeZoneInfo.FindSystemTimeZoneById() for time zone convertions.</returns>
        public static string GetTimeZoneString(Constants.TimeZone timeZone)
        {
            return Constants.TimeZones[timeZone];
        }
    }
}
