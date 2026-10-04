using System;
// This class provides functionality to get the local time for a given clock location.
namespace WorldClock.Core;

public class WorldClockService
{
    public DateTime GetLocalTime(ClockLocation location)
    {
        DateTime utcNow = DateTime.UtcNow;

        TimeZoneInfo timeZone = TimeZoneInfo.FindSystemTimeZoneById(location.TimeZoneId);

        DateTime localTime = TimeZoneInfo.ConvertTimeFromUtc(utcNow, timeZone);

        return localTime;
    }

    //converts the time from one location to another local to utc then utc to target location lcoal
    public DateTime ConvertTime(ClockLocation sourceLocation, ClockLocation destinationLocation, DateTime sourceLocalTime)
    {
        TimeZoneInfo sourceTimeZone =
            TimeZoneInfo.FindSystemTimeZoneById(
                sourceLocation.TimeZoneId);

        TimeZoneInfo destinationTimeZone =
            TimeZoneInfo.FindSystemTimeZoneById(
                destinationLocation.TimeZoneId);

        DateTime unspecifiedSourceTime =
            DateTime.SpecifyKind(
                sourceLocalTime,
                DateTimeKind.Unspecified);

        DateTime utcTime =
            TimeZoneInfo.ConvertTimeToUtc(
                unspecifiedSourceTime,
                sourceTimeZone);

        return TimeZoneInfo.ConvertTimeFromUtc(
            utcTime,
            destinationTimeZone);
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
