using System;
using System.Collections.Generic;
using System.Text;

namespace GlobalClock
{
    internal class LocationManager
    {
        private List<ClockLocation> locations = new List<ClockLocation>();

        public void AddLocation(ClockLocation location)
        {
            locations.Add(location);
        }



        public List<ClockLocation> GetLocations()
        {
            return locations;
        }
    }
}
