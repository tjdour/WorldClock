//Global Clock - Mini Project - MSSA CAD 21

using GlobalClock;
using Spectre.Console;

LocationManager locationManager = new LocationManager();
WorldClockService clockService = new WorldClockService();

locationManager.AddLocation( new ClockLocation("New York", "Eastern Standard Time"));
locationManager.AddLocation(new ClockLocation("London", "GMT Standard Time"));
locationManager.AddLocation(new ClockLocation("Tokyo", "Tokyo Standard Time"));

bool running = true;

while (running)
{
    AnsiConsole.Clear();

    string choice = AnsiConsole.Prompt(new SelectionPrompt<string>().Title("[bold]Global Clock[/]").AddChoices("View Clocks","Remove Location","Exit"));

    switch (choice)
    {
        case "View Clocks":
            DisplayClocks();
            break;

        case "Remove Location":
            RemoveLocation();
            break;

        case "Exit":
            running = false;
            break;
    }
}

void DisplayClocks()
{
    AnsiConsole.Clear();

    Table table = new Table();

    table.Title = new TableTitle("World Clock");

    table.AddColumn("Location");
    table.AddColumn("Current Time");
    table.AddColumn("Date");

    foreach (ClockLocation location in locationManager.GetLocations())
    {
        DateTime localTime = clockService.GetLocalTime(location);

        table.AddRow(location.City, localTime.ToString("h:mm tt"), localTime.ToString("dddd, MMMM d, yyyy"));
    }

    AnsiConsole.Write(table);

    AnsiConsole.WriteLine();
    AnsiConsole.MarkupLine("[grey]Press any key to return to the menu...[/]");
    Console.ReadKey(true);
}

void RemoveLocation()
{
    AnsiConsole.Clear();

    List<ClockLocation> locations = locationManager.GetLocations();

    if (locations.Count == 0)
    {
        AnsiConsole.MarkupLine("[yellow]There are no locations to remove.[/]");
        Console.ReadKey(true);
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







