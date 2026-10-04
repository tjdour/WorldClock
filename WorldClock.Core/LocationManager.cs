using System;
using System.Collections.Generic;
using System.Linq;

namespace WorldClock.Core;

public class LocationManager
{
    private List<ClockLocation> locations = new List<ClockLocation>();

    private List<ClockLocation> availableLocations =
    new List<ClockLocation>();

    public void SetAvailableLocations(List<ClockLocation> locations)
    {
        availableLocations = locations;
    }

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
        bool alreadyExists = locations.Any(existingLocation =>
            existingLocation.City.Equals(
                location.City,
                StringComparison.OrdinalIgnoreCase));

        if (alreadyExists)
        {
            return false;
        }

        locations.Add(location);
        return true;
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

    public List<ClockLocation> GetAvailableLocations()
    {
        return availableLocations;
    }
}
