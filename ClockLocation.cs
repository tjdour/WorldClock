using System;
using System.Collections.Generic;
using System.Text;

namespace GlobalClock
{
    internal class ClockLocation
    {
        public string City { get; set; }
        public string TimeZoneId { get; set; }

        public ClockLocation(string city, string timeZoneId)
        {
            City = city;
            TimeZoneId = timeZoneId;
        }
    }
}
