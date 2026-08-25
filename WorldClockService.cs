using System;
using System.Collections.Generic;
using System.Text;
// This class provides functionality to get the local time for a given clock location.
namespace GlobalClock
{
    internal class WorldClockService
    {
        public DateTime GetLocalTime(ClockLocation location)
        {
            DateTime utcNow = DateTime.UtcNow;

            TimeZoneInfo timeZone = TimeZoneInfo.FindSystemTimeZoneById(location.TimeZoneId);

            DateTime localTime = TimeZoneInfo.ConvertTimeFromUtc(utcNow, timeZone);

            return localTime;
        }
    }
}
