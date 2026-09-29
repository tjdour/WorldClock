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

        LocationPicker.ItemsSource =
        availableLocations
            .Select(location => location.City)
            .ToList();

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
}