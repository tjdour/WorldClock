using WorldClock.Core;

namespace WorldClock.Maui;

public partial class MainPage : ContentPage
{
    private readonly LocationManager locationManager;
    private readonly WorldClockService clockService;

    //clock refreshes every second, so we need to keep track of the items to update them instead of creating new ones each time
    private List<ClockDisplayItem> clockItems = new List<ClockDisplayItem>();

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
        RefreshComparePickers();
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

        DisplayTimes();
        RefreshLocationPicker();
        RefreshComparePickers();
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
        RefreshComparePickers();
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

    //time comparisonfeature
    private void OnCompareClicked(object? sender, EventArgs e)
    {
        if (CompareLocationOnePicker.SelectedItem == null ||
            CompareLocationTwoPicker.SelectedItem == null)
        {
            CompareResultLabel.Text =
                "Select two locations to compare.";
            return;
        }

        string firstCity =
            CompareLocationOnePicker.SelectedItem.ToString()!;

        string secondCity =
            CompareLocationTwoPicker.SelectedItem.ToString()!;

        ClockLocation firstLocation =
            locationManager
                .GetLocations()
                .First(location => location.City == firstCity);

        ClockLocation secondLocation =
            locationManager
                .GetLocations()
                .First(location => location.City == secondCity);

        DateTime firstTime =
            clockService.GetLocalTime(firstLocation);

        DateTime secondTime =
            clockService.GetLocalTime(secondLocation);

        double difference =
            (secondTime - firstTime).TotalHours;

        if (difference == 0)
        {
            CompareResultLabel.Text =
                $"{firstCity} and {secondCity} are at the same local time.";
        }
        else if (difference > 0)
        {
            CompareResultLabel.Text =
                $"{secondCity} is {difference:0.#} hours ahead of {firstCity}.";
        }
        else
        {
            CompareResultLabel.Text =
                $"{secondCity} is {Math.Abs(difference):0.#} hours behind {firstCity}.";
        }
    }

    private void RefreshComparePickers()
    {
        List<string> selectedCities =
            locationManager
                .GetLocations()
                .Select(location => location.City)
                .ToList();

        CompareLocationOnePicker.ItemsSource = selectedCities;
        CompareLocationTwoPicker.ItemsSource = selectedCities;
    }
}