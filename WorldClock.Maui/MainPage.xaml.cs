using WorldClock.Core;

namespace WorldClock.Maui;

public partial class MainPage : ContentPage
{
    private readonly LocationManager locationManager;
    private readonly WorldClockService clockService;

    public MainPage()
    {
        InitializeComponent();

        locationManager = new LocationManager();
        clockService = new WorldClockService();

        List<ClockLocation> availableLocations =
            locationManager.GetAvailableLocations();

        locationManager.AddLocation(
            availableLocations.First(location => location.City == "New York"));

        locationManager.AddLocation(
            availableLocations.First(location => location.City == "London"));

        locationManager.AddLocation(
            availableLocations.First(location => location.City == "Tokyo"));

        RefreshLocationPicker();
        DisplayTimes();
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

        DisplayTimes();
        RefreshLocationPicker();
    }





    private void OnRefreshClicked(object? sender, EventArgs e)
    {
        DisplayTimes();
    }

    private void DisplayTimes()
    {
        List<ClockLocation> selectedLocations =
            locationManager.GetLocations();

        List<ClockDisplayItem> clockItems =
            new List<ClockDisplayItem>();

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
}