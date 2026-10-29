using Microsoft.Extensions.AI;

namespace WorldClock.Maui.Services;

public sealed class AiAssistantService(
    FoundryChatClientFactory chatClientFactory,
    TimeZoneTools timeZoneTools)
{
    private const int MaximumPromptLength = 1000;
    private const int MaximumToolIterations = 4;

    private static readonly string SystemPrompt = """
        You are the World Clock assistant. Use only the supplied time-zone tools for current times, conversions, and time differences. Never calculate or guess a time, offset, or time-zone rule yourself. Use only cities in the packaged city catalog. If a tool reports an unknown or ambiguous city, ask the user to choose from the reported options. For a date without a year, use its next upcoming occurrence and state the year. For a conversion, use an ISO local date/time in the source city; if the tool reports an invalid or repeated daylight-saving time, ask the user to choose a valid time or occurrence. Do not claim a tool succeeded when it returned an error. Answer concisely and include the relevant city names and date.
        """;

    private readonly AIFunction[] tools =
    [
        AIFunctionFactory.Create(
            timeZoneTools.GetCurrentTime,
            name: "get_current_time",
            description: "Get the current local time and date for a city in the packaged catalog."),
        AIFunctionFactory.Create(
            timeZoneTools.ConvertTime,
            name: "convert_time",
            description: "Convert an ISO local date/time from a source city to a destination city."),
        AIFunctionFactory.Create(
            timeZoneTools.GetTimeDifference,
            name: "get_time_difference",
            description: "Compare two cities' UTC offsets at the current instant.")
    ];

    public async Task<string> AskAsync(
        string prompt,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(prompt))
        {
            throw new ArgumentException("Enter a question for the assistant.", nameof(prompt));
        }

        if (prompt.Length > MaximumPromptLength)
        {
            throw new ArgumentException(
                $"Keep the question under {MaximumPromptLength} characters.",
                nameof(prompt));
        }

        using IChatClient chatClient = chatClientFactory.Create()
            .AsBuilder()
            .UseFunctionInvocation(
                configure: options => options.MaximumIterationsPerRequest = MaximumToolIterations)
            .Build();

        List<ChatMessage> messages =
        [
            new(ChatRole.System, SystemPrompt),
            new(ChatRole.User, prompt)
        ];

        ChatResponse response = await chatClient.GetResponseAsync(
            messages,
            new ChatOptions { Tools = tools },
            cancellationToken);

        return string.IsNullOrWhiteSpace(response.Text)
            ? "I couldn't complete that request. Try rephrasing it or naming the cities explicitly."
            : response.Text.Trim();
    }
}
