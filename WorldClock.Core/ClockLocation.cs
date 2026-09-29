using System;
// This class represents a clock location with a city name and a time zone identifier.
namespace WorldClock.Core;

public class ClockLocation
{
    public string City { get; set; }
    public string TimeZoneId { get; set; }

    public ClockLocation(string city, string timeZoneId)
    {
        City = city;
        TimeZoneId = timeZoneId;
    }
}
