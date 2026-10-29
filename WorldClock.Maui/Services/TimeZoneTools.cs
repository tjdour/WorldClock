using System.Globalization;
using System.Text.Json;
using System.ComponentModel;
using Microsoft.Extensions.AI;
using WorldClock.Core;
using WorldClock.Maui.Data;
using WorldClock.Maui.Models;

namespace WorldClock.Maui.Services;

public sealed class TimeZoneTools
{
    private readonly WorldClockService clockService;
    private readonly Lazy<Task<IReadOnlyList<CatalogLocation>>> locations;

    public TimeZoneTools(WorldClockService clockService)
    {
        this.clockService = clockService;
        locations = new Lazy<Task<IReadOnlyList<CatalogLocation>>>(
            LoadCatalogAsync);
    }

    [Description("Get the current local time and date for a city in the packaged city catalog.")]
    public async Task<string> GetCurrentTime(
        string city,
        CancellationToken cancellationToken = default)
    {
        CityResolution resolution = await ResolveCityAsync(city, cancellationToken);
        if (resolution.Location is not { } location)
        {
            return resolution.Error!;
        }

        try
        {
            DateTime localTime = clockService.GetLocalTime(location.Location);
            return $"{location.DisplayName}: {localTime.ToString("dddd, MMMM d, yyyy h:mm:ss tt", CultureInfo.InvariantCulture)} ({location.Location.TimeZoneId}).";
        }
        catch (TimeZoneNotFoundException)
        {
            return $"TIME_ZONE_UNAVAILABLE: Time-zone data for {location.DisplayName} is unavailable on this device.";
        }
        catch (InvalidTimeZoneException)
        {
            return $"TIME_ZONE_INVALID: Time-zone data for {location.DisplayName} is invalid on this device.";
        }
    }

    [Description("Convert an ISO local date/time in a source city to the destination city's local time. Use yyyy-MM-ddTHH:mm or yyyy-MM-ddTHH:mm:ss. If the time is invalid or repeated because of daylight saving time, return the clarification request to the user.")]
    public async Task<string> ConvertTime(
        string sourceCity,
        string destinationCity,
        string dateTime,
        CancellationToken cancellationToken = default)
    {
        CityResolution sourceResolution = await ResolveCityAsync(sourceCity, cancellationToken);
        if (sourceResolution.Location is not { } source)
        {
            return sourceResolution.Error!;
        }

        CityResolution destinationResolution = await ResolveCityAsync(destinationCity, cancellationToken);
        if (destinationResolution.Location is not { } destination)
        {
            return destinationResolution.Error!;
        }

        if (!DateTime.TryParseExact(
                dateTime,
                ["yyyy-MM-dd'T'HH:mm", "yyyy-MM-dd'T'HH:mm:ss"],
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out DateTime sourceLocalTime))
        {
            return "INVALID_DATE_TIME: Ask the user for a specific date and time, including the year, then retry using ISO format.";
        }

        try
        {
            TimeZoneInfo sourceTimeZone =
                TimeZoneInfo.FindSystemTimeZoneById(source.Location.TimeZoneId);
            DateTime unspecifiedTime = DateTime.SpecifyKind(sourceLocalTime, DateTimeKind.Unspecified);

            if (sourceTimeZone.IsInvalidTime(unspecifiedTime))
            {
                return $"NONEXISTENT_LOCAL_TIME: {source.DisplayName} has no local time {sourceLocalTime:yyyy-MM-dd HH:mm} because of a daylight-saving transition. Ask the user for a different time.";
            }

            if (sourceTimeZone.IsAmbiguousTime(unspecifiedTime))
            {
                string offsets = string.Join(
                    " or ",
                    sourceTimeZone.GetAmbiguousTimeOffsets(unspecifiedTime)
                        .Select(FormatUtcOffset));

                return $"AMBIGUOUS_LOCAL_TIME: {sourceLocalTime:yyyy-MM-dd HH:mm} occurs twice in {source.DisplayName} during a daylight-saving transition (possible UTC offsets: {offsets}). Ask which occurrence the user means.";
            }

            DateTime destinationTime =
                clockService.ConvertTime(source.Location, destination.Location, sourceLocalTime);

            return $"{sourceLocalTime:yyyy-MM-dd HH:mm} in {source.DisplayName} is {destinationTime.ToString("dddd, MMMM d, yyyy h:mm tt", CultureInfo.InvariantCulture)} in {destination.DisplayName} ({destination.Location.TimeZoneId}).";
        }
        catch (TimeZoneNotFoundException)
        {
            return "TIME_ZONE_UNAVAILABLE: Time-zone data for one of the selected cities is unavailable on this device.";
        }
        catch (InvalidTimeZoneException)
        {
            return "TIME_ZONE_INVALID: Time-zone data for one of the selected cities is invalid on this device.";
        }
    }

    [Description("Compare the current UTC offsets of two cities and report how far the second is ahead of or behind the first.")]
    public async Task<string> GetTimeDifference(
        string cityA,
        string cityB,
        CancellationToken cancellationToken = default)
    {
        CityResolution firstResolution = await ResolveCityAsync(cityA, cancellationToken);
        if (firstResolution.Location is not { } first)
        {
            return firstResolution.Error!;
        }

        CityResolution secondResolution = await ResolveCityAsync(cityB, cancellationToken);
        if (secondResolution.Location is not { } second)
        {
            return secondResolution.Error!;
        }

        try
        {
            TimeSpan difference =
                clockService.GetTimeDifference(first.Location, second.Location);

            string description = difference == TimeSpan.Zero
                ? "the same UTC offset as"
                : difference > TimeSpan.Zero
                    ? $"{FormatHours(difference)} hours ahead of"
                    : $"{Math.Abs(difference.TotalHours).ToString("0.#", CultureInfo.InvariantCulture)} hours behind";

            return $"At the current instant, {second.DisplayName} is {description} {first.DisplayName}. Difference: {difference.TotalHours.ToString("+0.#;-0.#;0", CultureInfo.InvariantCulture)} hours.";
        }
        catch (TimeZoneNotFoundException)
        {
            return "TIME_ZONE_UNAVAILABLE: Time-zone data for one of the selected cities is unavailable on this device.";
        }
        catch (InvalidTimeZoneException)
        {
            return "TIME_ZONE_INVALID: Time-zone data for one of the selected cities is invalid on this device.";
        }
    }

    private async Task<CityResolution> ResolveCityAsync(
        string city,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(city))
        {
            return new(null, "UNSUPPORTED_CITY: A city name is required. Ask the user to specify a supported city.");
        }

        IReadOnlyList<CatalogLocation> catalog;
        try
        {
            catalog = await locations.Value.WaitAsync(cancellationToken);
        }
        catch (Exception exception) when (
            exception is FileNotFoundException or JsonException or IOException)
        {
            return new(null, "CITY_CATALOG_UNAVAILABLE: The packaged city catalog could not be loaded.");
        }

        string normalizedInput = Normalize(city);

        List<CatalogLocation> matches = catalog
            .Where(item => item.SearchTerms.Any(term => Normalize(term) == normalizedInput))
            .DistinctBy(item => item.Location.TimeZoneId + "|" + item.DisplayName)
            .ToList();

        if (matches.Count == 1)
        {
            return new(matches[0], null);
        }

        if (matches.Count > 1)
        {
            return new(null,
                $"AMBIGUOUS_CITY: '{city}' matches {string.Join(", ", matches.Select(match => match.DisplayName))}. Ask the user to choose one.");
        }

        return new(null,
            $"UNSUPPORTED_CITY: '{city}' is not in the packaged city catalog. Ask the user to choose a supported city.");
    }

    private async Task<IReadOnlyList<CatalogLocation>> LoadCatalogAsync()
    {
        List<CityData> cities = await CityDataLoader.LoadCitiesAsync();
        cities = cities
            .Where(city => !string.IsNullOrWhiteSpace(city.Name) && !string.IsNullOrWhiteSpace(city.TimeZone))
            .ToList();

        HashSet<string> duplicateNames = cities
            .GroupBy(city => city.Name, StringComparer.OrdinalIgnoreCase)
            .Where(group => group.Count() > 1)
            .Select(group => group.Key)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        return cities.Select(city =>
        {
            string displayName = city.Name;
            if (duplicateNames.Contains(city.Name))
            {
                displayName = string.IsNullOrWhiteSpace(city.Region)
                    ? $"{city.Name} ({city.Country})"
                    : $"{city.Name} ({city.Region}, {city.Country})";
            }

            return new CatalogLocation(
                new ClockLocation(displayName, city.TimeZone),
                displayName,
                [city.Id, city.Name, displayName, $"{city.Name} {city.Country}", $"{city.Name} {city.Region} {city.Country}"]);
        }).ToList();
    }

    private static string Normalize(string value) =>
        string.Concat(value.Where(char.IsLetterOrDigit)).ToUpperInvariant();

    private static string FormatHours(TimeSpan value) =>
        value.TotalHours.ToString("0.#", CultureInfo.InvariantCulture);

    private static string FormatUtcOffset(TimeSpan offset)
    {
        int totalMinutes = (int)Math.Abs(offset.TotalMinutes);
        string sign = offset < TimeSpan.Zero ? "-" : "+";
        return $"UTC{sign}{totalMinutes / 60:00}:{totalMinutes % 60:00}";
    }

    private sealed record CatalogLocation(
        ClockLocation Location,
        string DisplayName,
        IReadOnlyList<string> SearchTerms);

    private sealed record CityResolution(CatalogLocation? Location, string? Error);
}
