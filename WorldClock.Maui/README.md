# World Clock

**World Clock** is a C#/.NET MAUI desktop application for viewing current times in multiple cities and converting a selected date and time between time zones. It was developed as a Microsoft Software & Systems Academy (MSSA) Cloud Application Development project.

The project addresses a practical problem: planning calls, meetings, and travel across time zones without relying on mental arithmetic or fixed UTC offsets. It uses .NET's `TimeZoneInfo` APIs to account for time-zone rules, including daylight saving time (DST).

## Features

- **Live multi-city dashboard:** Display clocks for selected cities, updated every second.
- **City selection:** Add or remove locations from a packaged catalog of approximately 562 cities; distinguish cities with duplicate names using their region or country.
- **Persistent selections:** Save the dashboard's locations locally with Entity Framework Core and SQLite so they remain available after restarting the app.
- **Time-zone comparison:** Choose a source city, destination city, date, and time to calculate the corresponding destination time; swap the selected cities when needed.
- **DST-aware calculations:** Convert through UTC using .NET's system time-zone rules rather than hard-coded hour offsets.
- **Offline operation:** The city catalog and saved selections are stored locally; routine clock and conversion features do not require an online API.

## Tech Stack

| Component | Technology |
| --- | --- |
| Language | C# |
| Desktop UI | .NET MAUI, XAML, data binding |
| Core application logic | .NET class library, `TimeZoneInfo` |
| Data persistence | Entity Framework Core, SQLite |
| City catalog | JSON, `System.Text.Json` |
| Original mini-project UI | Spectre.Console |

## Project Structure

```text
WorldClock.slnx
├── WorldClock/                  # Original console mini-project (Spectre.Console)
├── WorldClock.Core/             # Shared, UI-independent application logic
│   ├── ClockLocation.cs         # City and time-zone identifier
│   ├── LocationManager.cs       # Available and selected locations
│   └── WorldClockService.cs     # Current times and time-zone conversions
└── WorldClock.Maui/             # MAUI desktop application
    ├── MainPage.xaml            # Live clock dashboard
    ├── MainPage.xaml.cs         # UI handlers and live refresh
    ├── MainPage.CityCatalog.cs  # City lookup and picker population
    ├── MainPage.Persistence.cs  # Save/load/remove saved locations
    ├── ComparePage.xaml         # Time comparison UI
    ├── ComparePage.xaml.cs      # Input validation and conversion action
    ├── Models/CityData.cs
    ├── Data/CityDataLoader.cs
    ├── Data/WorldClockDbContext.cs
    └── Resources/Raw/cities.json
```

The solution retains the original console application while the MAUI desktop application reuses `WorldClock.Core`. The current MAUI implementation uses XAML and code-behind, including partial `MainPage` files. `WorldClockService` owns time calculations; `LocationManager` owns selected/available location state; MAUI pages coordinate interaction, catalog loading, and database operations.

### Time Conversion Flow

```text
User selects source city, destination city, date, and time
    → ComparePage validates the selections
    → WorldClockService.ConvertTime(...)
    → Source local time converted to UTC
    → UTC converted to destination local time
    → Result displayed in ComparePage
```

`TimeZoneInfo` resolves the appropriate offset for the specified date, including applicable DST rules. For the dashboard, `WorldClockService.GetLocalTime(...)` starts from the current UTC time and converts it for each selected location.

## Getting Started

### Prerequisites

- Windows with **Visual Studio 2022** and the **.NET MAUI development** workload installed.
- The .NET SDK required by the target framework specified in the project's `.csproj` files.

### Build and Run

1. Clone or download this repository.
2. Open `WorldClock.slnx` in Visual Studio.
3. Restore NuGet packages if Visual Studio has not restored them automatically.
4. Set **WorldClock.Maui** as the startup project and select **Windows Machine** as the run target.
5. Build and run the application (F5).

The app uses a packaged `cities.json` file for available locations. EF Core configures a local SQLite database (`worldclock.db`) in the application's data directory, creating it when necessary. On an empty saved-location list, the dashboard initializes with New York City, London, and Tokyo.

## Usage

1. **View clocks:** Open the dashboard to see current local times for your selected cities.
2. **Add a city:** Choose an available location from the picker and add it to the dashboard.
3. **Remove a city:** Remove a saved clock; the selection is also updated in the local database.
4. **Compare times:** Open the comparison page, select two cities and a date/time, then convert or swap the cities.

## Future Improvements (Not Yet Implemented)

- Refactor the MAUI presentation layer to MVVM with ViewModels and commands.
- Add natural-language time-zone queries using an LLM with tool calls to the existing C# conversion logic.
- Improve city discovery with text search/autocomplete and add automated tests for time-zone edge cases.

## Data Attribution

The bundled city catalog is adapted from the **[city-slugs](https://github.com/mirkokiefer/city-slugs)** project's `city-slugs-1m.json` dataset (cities with populations of at least one million), itself derived from **[GeoNames](https://www.geonames.org/)** data. That dataset is licensed under **[Creative Commons Attribution 4.0 International](https://creativecommons.org/licenses/by/4.0/)**. The application uses a local copy for city names, regions, countries, and time-zone identifiers.

## Project Context

Built for the **Microsoft Software & Systems Academy (MSSA)** Cloud Application Development curriculum to practice C#, object-oriented design, collection management, MAUI desktop development, JSON processing, data persistence, and time-zone conversion.
