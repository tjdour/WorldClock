using System.Text.Json;
using WorldClock.Maui.Models;

namespace WorldClock.Maui.Data;

public static class CityDataLoader
{
    public static async Task<List<CityData>> LoadCitiesAsync()
    {
        using Stream stream =
            await FileSystem.OpenAppPackageFileAsync("cities.json");

        List<CityData>? cities =
            await JsonSerializer.DeserializeAsync<List<CityData>>(stream);

        return cities ?? new List<CityData>();
    }
}