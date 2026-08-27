using System;
using System.Collections.Generic;
using System.Text;

namespace WorldClock
{
    internal class LocationManager
    {
        private List<ClockLocation> locations = new List<ClockLocation>();

        private List<ClockLocation> availableLocations = new List<ClockLocation>
        {
            new ClockLocation("New York", "Eastern Standard Time"),
            new ClockLocation("Los Angeles", "Pacific Standard Time"),
            new ClockLocation("Chicago", "Central Standard Time"),
            new ClockLocation("Denver", "Mountain Standard Time"),
            new ClockLocation("London", "GMT Standard Time"),
            new ClockLocation("Paris", "Romance Standard Time"),
            new ClockLocation("Cairo", "Egypt Standard Time"),
            new ClockLocation("Nairobi", "E. Africa Standard Time"),
            new ClockLocation("Addis Ababa", "E. Africa Standard Time"),
            new ClockLocation("Dubai", "Arabian Standard Time"),
            new ClockLocation("New Delhi", "India Standard Time"),
            new ClockLocation("Tokyo", "Tokyo Standard Time"),
            new ClockLocation("Singapore", "Singapore Standard Time"),
            new ClockLocation("Sydney", "AUS Eastern Standard Time")
        };

        public List<ClockLocation> GetUnselectedLocations()
        {
            return availableLocations
                .Where(availableLocation =>
                    !locations.Any(selectedLocation =>
                        selectedLocation.City.Equals(
                            availableLocation.City,
                            StringComparison.OrdinalIgnoreCase)))
                .ToList();
        }

        public bool AddLocation(ClockLocation location)
        {
            foreach (ClockLocation existingLocation in locations)
            {
                if (existingLocation.City.Equals(
                    location.City,
                    StringComparison.OrdinalIgnoreCase))
                {
                    return false;
                }
            }

            locations.Add(location);

            return true;
        }


        //public void AddLocation(ClockLocation location)
        //{
        //    locations.Add(location);
        //}

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

        public List<ClockLocation> GetAvailableLocations()
        {
            return availableLocations;
        }
    }
}
