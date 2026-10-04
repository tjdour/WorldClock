using WorldClock.Core;
using WorldClock.Maui.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace WorldClock.Maui;

public partial class MainPage : ContentPage
{
    private readonly LocationManager locationManager;
    private readonly WorldClockService clockService;

    //clock refreshes every second, so we need to keep track of the items to update them instead of creating new ones each time
    private List<ClockDisplayItem> clockItems = new List<ClockDisplayItem>();
    private bool locationsLoaded = false;

    public MainPage()
    {
        InitializeComponent();

        locationManager = new LocationManager();
        clockService = new WorldClockService();

        //RefreshLocationPicker();
        //DisplayTimes();
        
        StartClockTimer();
    }

    private void StartClockTimer()
    {
        Dispatcher.StartTimer(
            TimeSpan.FromSeconds(1),
            () =>
            {
                UpdateTimes();
                return true;
            });
    }

    private void OnAddLocationClicked(object? sender, EventArgs e)
    {
        if (LocationPicker.SelectedItem == null)
        {
            return;
        }

        string selectedCity =
            LocationPicker.SelectedItem.ToString()!;

        ClockLocation selectedLocation =
            locationManager
                .GetAvailableLocations()
                .First(location => location.City == selectedCity);

        locationManager.AddLocation(selectedLocation);
        SaveLocationToDatabase(selectedLocation);

        DisplayTimes();
        RefreshLocationPicker();
        
    }

    private void OnRemoveLocationClicked(object? sender, EventArgs e)
    {
        if (sender is not Button button)
        {
            return;
        }

        string? city = button.CommandParameter?.ToString();

        if (string.IsNullOrWhiteSpace(city))
        {
            return;
        }

        locationManager.RemoveLocation(city);
        RemoveLocationFromDatabase(city);

        DisplayTimes();
        RefreshLocationPicker();
        
    }



    private void DisplayTimes()
    {
        List<ClockLocation> selectedLocations =
            locationManager.GetLocations();

        clockItems = new List<ClockDisplayItem>();

        foreach (ClockLocation location in selectedLocations)
        {
            DateTime localTime =
                clockService.GetLocalTime(location);

            clockItems.Add(new ClockDisplayItem
            {
                City = location.City,
                Time = localTime.ToString("hh:mm:ss tt")
            });
        }

        ClockCollectionView.ItemsSource = clockItems;
    }

    private void UpdateTimes()
    {
        foreach (ClockDisplayItem item in clockItems)
        {
            ClockLocation? location =
                locationManager
                    .GetLocations()
                    .FirstOrDefault(location =>
                        location.City == item.City);

            if (location == null)
            {
                continue;
            }

            DateTime localTime =
                clockService.GetLocalTime(location);

            item.Time = localTime.ToString("hh:mm:ss tt");
        }
    }


    //locations not already selected will be displayed in the picker
    private void RefreshLocationPicker()
    {
        LocationPicker.ItemsSource =
            locationManager
                .GetUnselectedLocations()
                .Select(location => location.City)
                .ToList();

        LocationPicker.SelectedItem = null;
    }

  
    


    // Database operations
    private void SaveLocationToDatabase(ClockLocation location)
    {
        using IServiceScope scope =
            Handler.MauiContext!.Services.CreateScope();

        WorldClockDbContext dbContext =
            scope.ServiceProvider
                .GetRequiredService<WorldClockDbContext>();

        bool alreadySaved =
            dbContext.Locations.Any(saved =>
                saved.City == location.City);

        if (!alreadySaved)
        {
            dbContext.Locations.Add(
                new ClockLocation(
                    location.City,
                    location.TimeZoneId));

            dbContext.SaveChanges();
        }
    }


    private void RemoveLocationFromDatabase(string city)
    {
        using IServiceScope scope =
            Handler.MauiContext!.Services.CreateScope();

        WorldClockDbContext dbContext =
            scope.ServiceProvider
                .GetRequiredService<WorldClockDbContext>();

        ClockLocation? savedLocation =
            dbContext.Locations
                .FirstOrDefault(location =>
                    location.City == city);

        if (savedLocation != null)
        {
            dbContext.Locations.Remove(savedLocation);
            dbContext.SaveChanges();
        }
    }

    //load locations from database on startup
    private void LoadLocationsFromDatabase()
    {
        using IServiceScope scope =
            Handler.MauiContext!.Services.CreateScope();

        WorldClockDbContext dbContext =
            scope.ServiceProvider
                .GetRequiredService<WorldClockDbContext>();

        List<ClockLocation> savedLocations =
            dbContext.Locations.ToList();

        if (savedLocations.Count == 0)
        {
            List<ClockLocation> availableLocations =
                locationManager.GetAvailableLocations();

            ClockLocation newYork =
                availableLocations.First(location =>
                    location.City == "New York City");

            ClockLocation london =
                availableLocations.First(location =>
                    location.City == "London");

            ClockLocation tokyo =
                availableLocations.First(location =>
                    location.City == "Tokyo");

            locationManager.AddLocation(newYork);
            locationManager.AddLocation(london);
            locationManager.AddLocation(tokyo);

            SaveLocationToDatabase(newYork);
            SaveLocationToDatabase(london);
            SaveLocationToDatabase(tokyo);
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
        
    }

    protected override async void OnHandlerChanged()
    {
        base.OnHandlerChanged();

        if (Handler == null || locationsLoaded)
        {
            return;
        }

        await LoadCityCatalogAsync();
        LoadLocationsFromDatabase();

        locationsLoaded = true;
    }

    //City list from json file
    private async Task LoadCityCatalogAsync()
    {
        var cities = await CityDataLoader.LoadCitiesAsync();

        HashSet<string> duplicateNames =
            cities
                .GroupBy(city => city.Name, StringComparer.OrdinalIgnoreCase)
                .Where(group => group.Count() > 1)
                .Select(group => group.Key)
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

        List<ClockLocation> locations =
            cities
                .Select(city =>
                {
                    string displayName = city.Name;

                    if (duplicateNames.Contains(city.Name))
                    {
                        displayName = string.IsNullOrWhiteSpace(city.Region)
                            ? $"{city.Name} ({city.Country})"
                            : $"{city.Name} ({city.Region}, {city.Country})";
                    }

                    return new ClockLocation(
                        displayName,
                        city.TimeZone);
                })
                .OrderBy(location => location.City)
                .ToList();

        locationManager.SetAvailableLocations(locations);
    }
}