using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Extensions
{
    /// <summary>
    /// Extensions for the System.DateTime class.
    /// </summary>
    public static class DateTime
    {
        public static System.DateTime ToTimeZone(this System.DateTime dateTime,
                                          Constants.TimeZone fromZone = 
                                              Constants.TimeZone.CentralStandardTime,
                                          Constants.TimeZone toZone = 
                                              Constants.TimeZone.EasternStandardTime)
        {
            System.DateTime now = System.TimeZoneInfo.ConvertTimeToUtc(
                new System.DateTime(dateTime.Ticks, DateTimeKind.Unspecified),
                System.TimeZoneInfo.FindSystemTimeZoneById(
                    Extensions.TimeZoneInfo.GetTimeZoneString(fromZone)));
            return System.TimeZoneInfo.ConvertTimeFromUtc(
                now,
                System.TimeZoneInfo.FindSystemTimeZoneById(
                    Extensions.TimeZoneInfo.GetTimeZoneString(toZone)));
        }
    }
}
