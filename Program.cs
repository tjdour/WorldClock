// World Clock - Mini Project - MSSA CAD 21
// Uses Spectre.Console to provide an interactive console interface for displaying and managing world clocks.
// Clock locations are currently selected from a predefined in-memory list for the mini-project scope.
// Users can view current times, add or remove locations, and compare times between two locations.

using WorldClock;
using Spectre.Console;

LocationManager locationManager = new LocationManager();
WorldClockService clockService = new WorldClockService();

List<ClockLocation> startupLocations = locationManager.GetAvailableLocations();

locationManager.AddLocation(startupLocations.First(location => location.City == "New York"));

locationManager.AddLocation(startupLocations.First(location => location.City == "London"));

locationManager.AddLocation(startupLocations.First(location => location.City == "Tokyo"));

bool running = true;

while (running)
{
    AnsiConsole.Clear();

    string choice = AnsiConsole.Prompt(new SelectionPrompt<string>().Title("[bold]World Clock[/]").AddChoices("View Clocks", "Add Location", "Remove Location", "Compare Locations", "Exit"));

    switch (choice)
    {
        case "View Clocks":
            DisplayClocks();
            break;

        case "Add Location":
            AddLocation();
            break;

        case "Remove Location":
            RemoveLocation();
            break;

        case "Compare Locations":
            CompareLocations();
            break;

        case "Exit":
            running = false;
            break;
    }
}

void DisplayClocks()
{
    AnsiConsole.Clear();

    List<ClockLocation> locations = locationManager.GetLocations();

    if (locations.Count == 0)
    {
        AnsiConsole.MarkupLine("[yellow]There are currently no locations on your dashboard.[/]");

        Pause();
        return;
    }

    Table table = new Table();

    table.Title = new TableTitle("World Clock");

    table.AddColumn("Location");
    table.AddColumn("Current Time");
    table.AddColumn("Date");

    foreach (ClockLocation location in locations)
    {
        DateTime localTime = clockService.GetLocalTime(location);

        table.AddRow(location.City, localTime.ToString("h:mm tt"), localTime.ToString("dddd, MMMM d, yyyy"));
    }

    AnsiConsole.Write(table);

    Pause();
}

void AddLocation()
{
    AnsiConsole.Clear();

    List<ClockLocation> unselectedLocations = locationManager.GetUnselectedLocations();

    if (unselectedLocations.Count == 0)
    {
        AnsiConsole.MarkupLine("[yellow]All available locations are already on your dashboard.[/]");

        Pause();
        return;
    }

    string city = AnsiConsole.Prompt(new SelectionPrompt<string>().Title("Which location would you like to add?").AddChoices(unselectedLocations.Select(location => location.City)));

    ClockLocation selectedLocation = unselectedLocations.First(location => location.City == city);

    bool added = locationManager.AddLocation(selectedLocation);

    if (added)
    {
        AnsiConsole.MarkupLine($"[green]{city} was added.[/]");
    }
    else
    {
        AnsiConsole.MarkupLine($"[yellow]{city} is already on your dashboard.[/]");
    }

    Pause();
}
void RemoveLocation()
{
    AnsiConsole.Clear();

    List<ClockLocation> locations = locationManager.GetLocations();

    if (locations.Count == 0)
    {
        AnsiConsole.MarkupLine("[yellow]There are no locations to remove.[/]");

        Pause();
        return;
    }

    string city = AnsiConsole.Prompt(new SelectionPrompt<string>().Title("Which location would you like to remove?").AddChoices(locations.Select(location => location.City)));

    bool removed = locationManager.RemoveLocation(city);

    if (removed)
    {
        AnsiConsole.MarkupLine($"[green]{city} was removed.[/]");
    }
    else
    {
        AnsiConsole.MarkupLine($"[red]{city} could not be found.[/]");
    }

    Pause();
}

void CompareLocations()
{
    AnsiConsole.Clear();

    List<ClockLocation> locations = locationManager.GetLocations();

    if (locations.Count < 2)
    {
        AnsiConsole.MarkupLine("[yellow]You need at least two locations to compare.[/]");

        Pause();
        return;
    }


    string firstCity = AnsiConsole.Prompt( new SelectionPrompt<string>() 
        .Title("Select the first location:") .AddChoices( locations
            .Select(location => location.City) ) );


    string secondCity = AnsiConsole.Prompt(new SelectionPrompt<string>()
        .Title("Select the second location:").AddChoices(locations.Where(location => location.City != firstCity)
            .Select(location => location.City)));


    ClockLocation firstLocation = locations.First(location => location.City == firstCity);

    ClockLocation secondLocation = locations.First(location => location.City == secondCity);


    DateTime firstTime = clockService.GetLocalTime(firstLocation);

    DateTime secondTime = clockService.GetLocalTime(secondLocation);

    TimeSpan difference = clockService.GetTimeDifference(firstLocation, secondLocation);

    Table comparisonTable = new Table();

    comparisonTable.Title = new TableTitle("Location Comparison");

    comparisonTable.AddColumn("Location");
    comparisonTable.AddColumn("Current Time");
    comparisonTable.AddColumn("Date");

    comparisonTable.AddRow(firstLocation.City, firstTime.ToString("h:mm tt"), firstTime.ToString("dddd, MMMM d, yyyy"));

    comparisonTable.AddRow(secondLocation.City, secondTime.ToString("h:mm tt"), secondTime.ToString("dddd, MMMM d, yyyy"));

    AnsiConsole.Write(comparisonTable);

    AnsiConsole.WriteLine();

    double differenceHours = difference.TotalHours;

    if (differenceHours > 0)
    {
        AnsiConsole.MarkupLine($"[cyan]{secondLocation.City} is {differenceHours:0.#} hours ahead of {firstLocation.City}.[/]");
    }
    else if (differenceHours < 0)
    {
        AnsiConsole.MarkupLine($"[cyan]{secondLocation.City} is {Math.Abs(differenceHours):0.#} hours behind {firstLocation.City}.[/]");
    }
    else
    {
        AnsiConsole.MarkupLine($"[cyan]{firstLocation.City} and {secondLocation.City} are currently in the same time offset.[/]");
    }


    if (firstTime.Date != secondTime.Date)
    {
        AnsiConsole.MarkupLine("[yellow]These locations are currently on different calendar days.[/]");
    }


    Pause();
}

void Pause()
{
    AnsiConsole.WriteLine();

    AnsiConsole.MarkupLine("[grey]Press any key to return to the menu...[/]");

    Console.ReadKey(true);
}

//Spectre table
//Table table = new Table();

//table.Title = new TableTitle("World Clock");

//table.AddColumn("Location");
//table.AddColumn("Current Time");
//table.AddColumn("Date");


//foreach (ClockLocation location in locationManager.GetLocations())
//{
//    DateTime localTime = clockService.GetLocalTime(location);

//    table.AddRow(location.City, localTime.ToString("hh:mm tt"), localTime.ToString("MM/dd/yyyy"));
//    //Console.WriteLine($"Location: {location.City}");
//    //Console.WriteLine($"Current Time: {localTime}");
//    //Console.WriteLine();
//}
//AnsiConsole.Write(table);

//ClockLocation location = new ClockLocation("New York", "Eastern Standard Time");
//List<ClockLocation> locations = new List<ClockLocation>
//{
//    new ClockLocation("New York", "Eastern Standard Time"),
//    new ClockLocation("London", "GMT Standard Time"),
//    new ClockLocation("Tokyo", "Tokyo Standard Time")
//};
//WorldClockService clockService = new WorldClockService();

//WorldClockService clockService = new WorldClockService();

//DateTime localTime = clockService.GetLocalTime(location);

//Console.WriteLine($"Location: {location.City}");
//Console.WriteLine($"Current Time: {localTime}");
//foreach (ClockLocation location in locations)
//{
//    DateTime localTime = clockService.GetLocalTime(location);

//    Console.WriteLine($"Location: {location.City}");
//    Console.WriteLine($"Current Time: {localTime}");
//    Console.WriteLine();
//}







