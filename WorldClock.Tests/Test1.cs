using WorldClock.Core;
using WorldClock.Maui.Services;

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

[TestClass]
public sealed class ConversationHistoryTests
{
    [TestMethod]
    public void CreateRequestMessages_IncludesPriorUserAndAssistantTurns()
    {
        ConversationHistory history = new();
        history.AddTurn("What time is it in London?", "It is noon in London.");

        List<Microsoft.Extensions.AI.ChatMessage> messages = history.CreateRequestMessages(
            "Current date and UTC time: 2026-01-01 12:00:00 UTC.",
            "What about Nairobi?");

        Assert.HasCount(4, messages);
        Assert.AreEqual(Microsoft.Extensions.AI.ChatRole.System, messages[0].Role);
        Assert.AreEqual(Microsoft.Extensions.AI.ChatRole.User, messages[1].Role);
        Assert.AreEqual("What time is it in London?", messages[1].Text);
        Assert.AreEqual(Microsoft.Extensions.AI.ChatRole.Assistant, messages[2].Role);
        Assert.AreEqual("It is noon in London.", messages[2].Text);
        Assert.AreEqual(Microsoft.Extensions.AI.ChatRole.User, messages[3].Role);
        Assert.AreEqual("What about Nairobi?", messages[3].Text);
        StringAssert.Contains(messages[0].Text, "2026-01-01 12:00:00 UTC");
    }

    [TestMethod]
    public void AddTurn_TrimsOldestCompleteTurns()
    {
        ConversationHistory history = new(maximumTurns: 2);
        history.AddTurn("First", "First answer");
        history.AddTurn("Second", "Second answer");
        history.AddTurn("Third", "Third answer");

        Assert.HasCount(4, history.Messages);
        Assert.AreEqual("Second", history.Messages[0].Text);
        Assert.AreEqual("Second answer", history.Messages[1].Text);
        Assert.AreEqual("Third", history.Messages[2].Text);
        Assert.AreEqual("Third answer", history.Messages[3].Text);
    }
}
}
