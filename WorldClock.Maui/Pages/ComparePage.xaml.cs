using WorldClock.Core;
using WorldClock.Maui.Data;
using System.Text.Json;
using WorldClock.Maui.Models;

namespace WorldClock.Maui;

public partial class ComparePage : ContentPage
{
    private readonly WorldClockService clockService;

    private List<ClockLocation> compareLocations =
        new List<ClockLocation>();

    public ComparePage()
    {
        InitializeComponent();

        clockService = new WorldClockService();

        DateTime now = DateTime.Now;

        SourceDatePicker.Date = now.Date;
        SourceTimePicker.Time = now.TimeOfDay;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        if (compareLocations.Count > 0)
        {
            return;
        }

        await LoadLocationsAsync();
    }

    private async Task LoadLocationsAsync()
    {
        try
        {
            List<CityData> cities =
                await CityDataLoader.LoadCitiesAsync();

            cities = cities
                .Where(city =>
                    !string.IsNullOrWhiteSpace(city.Name) &&
                    !string.IsNullOrWhiteSpace(city.TimeZone))
                .ToList();

            if (cities.Count == 0)
            {
                DifferenceResultLabel.Text =
                    "No valid city data was found.";
                return;
            }

            compareLocations =
                cities
                    .Select(city =>
                        new ClockLocation(
                            $"{city.Name} ({city.Country})",
                            city.TimeZone))
                    .OrderBy(location => location.City)
                    .ToList();

            List<string> cityNames =
                compareLocations
                    .Select(location => location.City)
                    .ToList();

            SourceLocationPicker.ItemsSource =
                cityNames;

            DestinationLocationPicker.ItemsSource =
                cityNames;
        }
        catch (FileNotFoundException)
        {
            DifferenceResultLabel.Text =
                "The city data file could not be found.";
        }
        catch (JsonException)
        {
            DifferenceResultLabel.Text =
                "The city data file is invalid.";
        }
        catch (IOException)
        {
            DifferenceResultLabel.Text =
                "The city data file could not be opened.";
        }
    }

    private void OnNowClicked(object? sender, EventArgs e)
    {
        DateTime now = DateTime.Now;

        SourceDatePicker.Date = now.Date;
        SourceTimePicker.Time = now.TimeOfDay;
    }

    private void OnConvertClicked(object? sender, EventArgs e)
    {
        if (SourceLocationPicker.SelectedItem == null ||
            DestinationLocationPicker.SelectedItem == null)
        {
            DifferenceResultLabel.Text =
                "Select two cities.";
            return;
        }

        DateTime? selectedDate =
            SourceDatePicker.Date;

        TimeSpan? selectedTime =
            SourceTimePicker.Time;

        if (selectedDate == null ||
            selectedTime == null)
        {
            DifferenceResultLabel.Text =
                "Select a date and time.";
            return;
        }

        string sourceCity =
            SourceLocationPicker.SelectedItem.ToString()!;

        string destinationCity =
            DestinationLocationPicker.SelectedItem.ToString()!;

        ClockLocation? sourceLocation =
            compareLocations.FirstOrDefault(location =>
                location.City == sourceCity);

        ClockLocation? destinationLocation =
            compareLocations.FirstOrDefault(location =>
                location.City == destinationCity);

        if (sourceLocation == null ||
            destinationLocation == null)
        {
            DifferenceResultLabel.Text =
                "Selected city could not be found.";
            return;
        }

        DateTime sourceDateTime =
            selectedDate.Value.Date +
            selectedTime.Value;

        try
        {
            DateTime destinationDateTime =
                clockService.ConvertTime(
                    sourceLocation,
                    destinationLocation,
                    sourceDateTime);

            DestinationTimeLabel.Text =
                destinationDateTime.ToString("h:mm tt");

            DestinationDateLabel.Text =
                destinationDateTime.ToString(
                    "dddd, MMMM d, yyyy");

            double difference =
                (destinationDateTime - sourceDateTime)
                    .TotalHours;

            if (Math.Abs(difference) < 0.1)
            {
                DifferenceResultLabel.Text =
                    "Same local time";
            }
            else if (difference > 0)
            {
                DifferenceResultLabel.Text =
                    $"{destinationLocation.City} is " +
                    $"{difference:0.#} hours ahead of " +
                    $"{sourceLocation.City}";
            }
            else
            {
                DifferenceResultLabel.Text =
                    $"{destinationLocation.City} is " +
                    $"{Math.Abs(difference):0.#} hours behind " +
                    $"{sourceLocation.City}";
            }
        }
        catch (TimeZoneNotFoundException)
        {
            DifferenceResultLabel.Text =
                "Time zone information is unavailable.";
        }
        catch (InvalidTimeZoneException)
        {
            DifferenceResultLabel.Text =
                "Time zone information is invalid.";
        }
    }

    private void OnSwapClicked(object? sender, EventArgs e)
    {
        object? sourceSelection =
            SourceLocationPicker.SelectedItem;

        object? destinationSelection =
            DestinationLocationPicker.SelectedItem;

        SourceLocationPicker.SelectedItem =
            destinationSelection;

        DestinationLocationPicker.SelectedItem =
            sourceSelection;

        DifferenceResultLabel.Text = string.Empty;
        DestinationTimeLabel.Text = "--:--";
        DestinationDateLabel.Text =
            "Select a date and time";
    }

}