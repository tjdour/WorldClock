using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using WorldClock.Core;
using WorldClock.Maui.Data;
using WorldClock.Maui.Models;

namespace WorldClock.Maui;

public partial class MainPage : ContentPage
{
    private readonly LocationManager locationManager;
    private readonly WorldClockService clockService;

    //clock refreshes every second, so we need to keep track of the items to update them instead of creating new ones each time
    private List<ClockDisplayItem> clockItems = new List<ClockDisplayItem>();
    private bool locationsLoaded = false;

    private Dictionary<string, CityData> cityDataLookup =
    new Dictionary<string, CityData>();

    private Dictionary<string, ClockLocation> locationPickerLookup =
        new Dictionary<string, ClockLocation>();

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

        string selectedText =
            LocationPicker.SelectedItem.ToString()!;

        if (!locationPickerLookup.TryGetValue(
            selectedText,
            out ClockLocation? selectedLocation))
        {
            return;
        }

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
        locationPickerLookup.Clear();

        foreach (ClockLocation location
            in locationManager.GetUnselectedLocations())
        {
            string pickerText =
                GetLocationPickerText(location);

            locationPickerLookup[pickerText] =
                location;
        }

        LocationPicker.ItemsSource =
            locationPickerLookup.Keys
                .OrderBy(text => text)
                .ToList();

        LocationPicker.SelectedItem = null;
    }

    private string GetLocationPickerText(
    ClockLocation location)
    {
        if (!cityDataLookup.TryGetValue(
            location.City,
            out CityData? city))
        {
            return location.City;
        }

        if (city.Country == "US")
        {
            string state =
                GetUsStateAbbreviation(city.Region);

            if (!string.IsNullOrWhiteSpace(state))
            {
                return $"{city.Name}, {state}, USA";
            }

            return $"{city.Name}, USA";
        }

        if (!string.IsNullOrWhiteSpace(city.Region) &&
            cityDataLookup.Values.Count(other =>
                other.Name.Equals(
                    city.Name,
                    StringComparison.OrdinalIgnoreCase)) > 1)
        {
            return $"{city.Name}, {city.Region}, {city.Country}";
        }

        return $"{city.Name}, {city.Country}";
    }

    private string GetUsStateAbbreviation(string? state)
    {
        return state switch
        {
            "Alabama" => "AL",
            "Alaska" => "AK",
            "Arizona" => "AZ",
            "Arkansas" => "AR",
            "California" => "CA",
            "Colorado" => "CO",
            "Connecticut" => "CT",
            "Delaware" => "DE",
            "Florida" => "FL",
            "Georgia" => "GA",
            "Hawaii" => "HI",
            "Idaho" => "ID",
            "Illinois" => "IL",
            "Indiana" => "IN",
            "Iowa" => "IA",
            "Kansas" => "KS",
            "Kentucky" => "KY",
            "Louisiana" => "LA",
            "Maine" => "ME",
            "Maryland" => "MD",
            "Massachusetts" => "MA",
            "Michigan" => "MI",
            "Minnesota" => "MN",
            "Mississippi" => "MS",
            "Missouri" => "MO",
            "Montana" => "MT",
            "Nebraska" => "NE",
            "Nevada" => "NV",
            "New Hampshire" => "NH",
            "New Jersey" => "NJ",
            "New Mexico" => "NM",
            "New York" => "NY",
            "North Carolina" => "NC",
            "North Dakota" => "ND",
            "Ohio" => "OH",
            "Oklahoma" => "OK",
            "Oregon" => "OR",
            "Pennsylvania" => "PA",
            "Rhode Island" => "RI",
            "South Carolina" => "SC",
            "South Dakota" => "SD",
            "Tennessee" => "TN",
            "Texas" => "TX",
            "Utah" => "UT",
            "Vermont" => "VT",
            "Virginia" => "VA",
            "Washington" => "WA",
            "West Virginia" => "WV",
            "Wisconsin" => "WI",
            "Wyoming" => "WY",
            "District of Columbia" => "DC",
            _ => state ?? string.Empty
        };
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
        List<CityData> cities =
            await CityDataLoader.LoadCitiesAsync();

        HashSet<string> duplicateNames =
            cities
                .GroupBy(city => city.Name, StringComparer.OrdinalIgnoreCase)
                .Where(group => group.Count() > 1)
                .Select(group => group.Key)
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

        List<ClockLocation> locations =
            new List<ClockLocation>();

        cityDataLookup.Clear();

        foreach (CityData city in cities)
        {
            string locationName = city.Name;

            if (duplicateNames.Contains(city.Name))
            {
                locationName = string.IsNullOrWhiteSpace(city.Region)
                    ? $"{city.Name} ({city.Country})"
                    : $"{city.Name} ({city.Region}, {city.Country})";
            }

            ClockLocation location =
                new ClockLocation(
                    locationName,
                    city.TimeZone);

            locations.Add(location);

            cityDataLookup[locationName] = city;
        }

        locations = locations
            .OrderBy(location => location.City)
            .ToList();

        locationManager.SetAvailableLocations(locations);
    }
}