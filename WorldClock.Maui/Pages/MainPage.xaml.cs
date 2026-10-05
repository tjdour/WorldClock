using WorldClock.Core;
using System.Text.Json;

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

    private async void OnAddLocationClicked(
    object? sender,
    EventArgs e)
    {
        if (LocationPicker.SelectedItem == null)
        {
            return;
        }

        string selectedText = LocationPicker.SelectedItem.ToString()!;

        if (!locationPickerLookup.TryGetValue(
            selectedText,
            out ClockLocation? selectedLocation))
        {
            return;
        }

        bool added = locationManager.AddLocation(selectedLocation);

        if (!added)
        {
            return;
        }

        bool saved = SaveLocationToDatabase(selectedLocation);

        if (!saved)
        {
            // Undo the in-memory change so the app
            // stays consistent with the database.
            locationManager.RemoveLocation(
                selectedLocation.City);

            await DisplayAlertAsync(
                 "Database Error",
                "The location could not be saved.",
                "OK");

            return;
        }

        DisplayTimes();
        RefreshLocationPicker();
    }

    private async void OnRemoveLocationClicked(object? sender, EventArgs e)
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

        ClockLocation? location =
            locationManager
                .GetLocations()
                .FirstOrDefault(location =>
                    location.City.Equals(
                        city,
                        StringComparison.OrdinalIgnoreCase));

        if (location == null)
        {
            return;
        }

        bool removed = locationManager.RemoveLocation(city);

        if (!removed)
        {
            return;
        }

        bool databaseRemoved = RemoveLocationFromDatabase(city);

        if (!databaseRemoved)
        {
            // Restore the location if the DB operation failed.
            locationManager.AddLocation(location);

            await DisplayAlertAsync(
                "Database Error",
                "The location could not be removed.",
                "OK");

            return;
        }

        DisplayTimes();
        RefreshLocationPicker();
    }

    // Try to get the local time for a given location, returning false if the time zone is invalid or not found
    private bool TryGetLocalTime(ClockLocation location, out DateTime localTime)
    {
        localTime = default;

        if (string.IsNullOrWhiteSpace(location.TimeZoneId))
        {
            return false;
        }

        try
        {
            localTime = clockService.GetLocalTime(location);

            return true;
        }
        catch (TimeZoneNotFoundException)
        {
            return false;
        }
        catch (InvalidTimeZoneException)
        {
            return false;
        }
    }
    private void DisplayTimes()
    {
        List<ClockLocation> selectedLocations =
            locationManager.GetLocations();

        clockItems = new List<ClockDisplayItem>();

        foreach (ClockLocation location in selectedLocations)
        {
            string timeText;

            if (TryGetLocalTime(location, out DateTime localTime))
            {
                timeText = localTime.ToString("hh:mm:ss tt");
            }
            else
            {
                timeText = "Unavailable";
            }

            clockItems.Add(new ClockDisplayItem
            {
                City = location.City,
                Time = timeText
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

            if (TryGetLocalTime(location, out DateTime localTime))
            {
                item.Time = localTime.ToString("hh:mm:ss tt");
            }
            else
            {
                item.Time = "Unavailable";
            }
        }
    }

    protected override async void OnHandlerChanged()
    {
        base.OnHandlerChanged();

        if (Handler == null || locationsLoaded)
        {
            return;
        }

        // Load city catalog from JSON file handling
        bool catalogLoaded = await LoadCityCatalogAsync();

        if (!catalogLoaded)
        {
            locationsLoaded = true;
            return;
        }

        await LoadLocationsFromDatabase();

        locationsLoaded = true;
    }

    
    
}