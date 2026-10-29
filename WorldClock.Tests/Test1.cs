using WorldClock.Core;

namespace WorldClock.Tests;

[TestClass]
public sealed class WorldClockServiceTests
{
    [TestMethod]
    public void ConvertTime_SeattleAcrossSpringDaylightSavingBoundary_UsesCorrectOffsets()
    {
        WorldClockService service = new();
        ClockLocation seattle = new("Seattle", "America/Los_Angeles");
        ClockLocation utc = new("UTC", "UTC");

        DateTime beforeTransition = service.ConvertTime(
            seattle,
            utc,
            new DateTime(2026, 3, 8, 1, 30, 0));
        DateTime afterTransition = service.ConvertTime(
            seattle,
            utc,
            new DateTime(2026, 3, 8, 3, 30, 0));

        Assert.AreEqual(new DateTime(2026, 3, 8, 9, 30, 0, DateTimeKind.Utc), beforeTransition);
        Assert.AreEqual(new DateTime(2026, 3, 8, 10, 30, 0, DateTimeKind.Utc), afterTransition);
    }
}
