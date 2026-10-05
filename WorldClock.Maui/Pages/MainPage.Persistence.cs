using Microsoft.Extensions.DependencyInjection;
using WorldClock.Core;
using WorldClock.Maui.Data;

namespace WorldClock.Maui;

public partial class MainPage
{
    private bool SaveLocationToDatabase(ClockLocation location)
    {
        try
        {
            using IServiceScope scope = Handler.MauiContext!.Services.CreateScope();

            WorldClockDbContext dbContext = scope.ServiceProvider.GetRequiredService<WorldClockDbContext>();

            bool alreadySaved = dbContext.Locations.Any(saved => saved.City == location.City);

            if (!alreadySaved)
            {
                dbContext.Locations.Add(
                    new ClockLocation(
                        location.City,
                        location.TimeZoneId));

                dbContext.SaveChanges();
            }

            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    private bool RemoveLocationFromDatabase(string city)
    {
        try
        {
            using IServiceScope scope = Handler.MauiContext!.Services.CreateScope();

            WorldClockDbContext dbContext = scope.ServiceProvider.GetRequiredService<WorldClockDbContext>();

            ClockLocation? savedLocation =
                dbContext.Locations
                    .FirstOrDefault(location =>
                        location.City == city);

            if (savedLocation != null)
            {
                dbContext.Locations.Remove(savedLocation);
                dbContext.SaveChanges();
            }

            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    // Load locations from database on startup
    private async Task<bool> LoadLocationsFromDatabase()
    {
        try
        {
            using IServiceScope scope = Handler.MauiContext!.Services.CreateScope();

            WorldClockDbContext dbContext = scope.ServiceProvider.GetRequiredService<WorldClockDbContext>();

            List<ClockLocation> savedLocations = dbContext.Locations.ToList();

            if (savedLocations.Count == 0)
            {
                List<ClockLocation> availableLocations = locationManager.GetAvailableLocations();

                string[] defaultCities =
                {
                    "New York City",
                    "London",
                    "Tokyo"
                };

                foreach (string cityName in defaultCities)
                {
                    ClockLocation? location =
                        availableLocations.FirstOrDefault(location =>
                            location.City.Equals(
                                cityName,
                                StringComparison.OrdinalIgnoreCase));

                    if (location == null)
                    {
                        continue;
                    }

                    bool added = locationManager.AddLocation(location);

                    if (!added)
                    {
                        continue;
                    }

                    bool saved = SaveLocationToDatabase(location);

                    if (!saved)
                    {
                        locationManager.RemoveLocation(location.City);
                    }
                }
            }
            else
            {
                foreach (ClockLocation location in savedLocations)
                {
                    locationManager.AddLocation(location);
                }
            }

            DisplayTimes();
            RefreshLocationPicker();

            return true;
        }
        catch (Exception)
        {
            await DisplayAlertAsync(
                "Database Error",
                "Saved locations could not be loaded.",
                "OK");

            DisplayTimes();
            RefreshLocationPicker();

            return false;
        }
    }
}