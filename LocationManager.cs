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

        public bool RemoveLocation(string city)
        {
            for (int i = 0; i < locations.Count; i++)
            {
                if (locations[i].City.Equals(
                    city,
                    StringComparison.OrdinalIgnoreCase))
                {
                    locations.RemoveAt(i);
                    return true;
                }
            }

            return false;
        }




        public List<ClockLocation> GetLocations()
        {
            return locations;
        }
    }
}
