//Global Clock - Mini Project - MSSA CAD 21

using GlobalClock;


//ClockLocation location = new ClockLocation("New York", "Eastern Standard Time");
List<ClockLocation> locations = new List<ClockLocation>
{
    new ClockLocation("New York", "Eastern Standard Time"),
    new ClockLocation("London", "GMT Standard Time"),
    new ClockLocation("Tokyo", "Tokyo Standard Time")
};
//WorldClockService clockService = new WorldClockService();
WorldClockService clockService = new WorldClockService();

//DateTime localTime = clockService.GetLocalTime(location);

//Console.WriteLine($"Location: {location.City}");
//Console.WriteLine($"Current Time: {localTime}");
foreach (ClockLocation location in locations)
{
    DateTime localTime = clockService.GetLocalTime(location);

    Console.WriteLine($"Location: {location.City}");
    Console.WriteLine($"Current Time: {localTime}");
    Console.WriteLine();
}







