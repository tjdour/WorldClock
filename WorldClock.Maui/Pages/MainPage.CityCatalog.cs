using System.Text.Json;
using WorldClock.Core;
using WorldClock.Maui.Data;
using WorldClock.Maui.Models;

namespace WorldClock.Maui;

public partial class MainPage
{
    private Dictionary<string, CityData> cityDataLookup =
        new Dictionary<string, CityData>();

    private Dictionary<string, ClockLocation> locationPickerLookup =
        new Dictionary<string, ClockLocation>();

    // Locations not already selected will be displayed in the picker
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

    // City list from JSON file
    private async Task<bool> LoadCityCatalogAsync()
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
                await DisplayAlert(
                    "City Data Error",
                    "No valid city data was found.",
                    "OK");

                return false;
            }

            HashSet<string> duplicateNames =
                cities
                    .GroupBy(
                        city => city.Name,
                        StringComparer.OrdinalIgnoreCase)
                    .Where(group => group.Count() > 1)
                    .Select(group => group.Key)
                    .ToHashSet(
                        StringComparer.OrdinalIgnoreCase);

            List<ClockLocation> locations =
                new List<ClockLocation>();

            cityDataLookup.Clear();

            foreach (CityData city in cities)
            {
                string locationName = city.Name;

                if (duplicateNames.Contains(city.Name))
                {
                    locationName =
                        string.IsNullOrWhiteSpace(city.Region)
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

            return true;
        }
        catch (FileNotFoundException)
        {
            await DisplayAlert(
                "City Data Error",
                "The cities.json file could not be found.",
                "OK");

            return false;
        }
        catch (JsonException)
        {
            await DisplayAlert(
                "City Data Error",
                "The cities.json file could not be read because its format is invalid.",
                "OK");

            return false;
        }
        catch (IOException)
        {
            await DisplayAlert(
                "City Data Error",
                "The city data file could not be opened.",
                "OK");

            return false;
        }
    }
}