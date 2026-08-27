using System;
using System.Collections.Generic;
using System.Text;
// This class provides functionality to get the local time for a given clock location.
namespace WorldClock
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

        public TimeSpan GetTimeDifference(ClockLocation firstLocation,ClockLocation secondLocation)
        {
            DateTime utcNow = DateTime.UtcNow;

            TimeZoneInfo firstTimeZone =
                TimeZoneInfo.FindSystemTimeZoneById(firstLocation.TimeZoneId);

            TimeZoneInfo secondTimeZone =
                TimeZoneInfo.FindSystemTimeZoneById(secondLocation.TimeZoneId);

            TimeSpan firstOffset =
                firstTimeZone.GetUtcOffset(utcNow);

            TimeSpan secondOffset =
                secondTimeZone.GetUtcOffset(utcNow);

            return secondOffset - firstOffset;
        }
    }
}
