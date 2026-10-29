using Microsoft.Extensions.AI;

namespace WorldClock.Maui.Services;

public sealed class AiAssistantService(
    FoundryChatClientFactory chatClientFactory,
    TimeZoneTools timeZoneTools)
{
    private const int MaximumPromptLength = 1000;
    private const int MaximumToolIterations = 4;
    private readonly ConversationHistory conversationHistory = new();

    private static readonly string SystemPrompt = """
        You are a friendly, conversational World Clock assistant. Understand ordinary language, remember the conversation, and resolve follow-up questions using earlier messages. The current date and UTC time are supplied with each request; use them to interpret words such as today, tomorrow, and next Tuesday.

        Automatically choose and call the supplied time-zone tools for current local times, conversions, and time differences. Interpret natural-language dates and times, then provide tool arguments in the required format yourself. Never ask the user to provide ISO 8601 timestamps or choose a tool/function. Ask a concise follow-up only when an essential detail is genuinely missing or ambiguous, such as which city or which occurrence of a repeated daylight-saving time.

        Use only cities in the packaged city catalog and only tool results for times, conversions, and offsets. Never calculate, invent, or guess a time, UTC offset, or time-zone rule yourself. If a tool reports an unknown or ambiguous city, ask the user to choose from its options. For a date without a year, use its next upcoming occurrence and state the year. Preserve daylight-saving validation: if a local time does not exist or occurs twice, explain the issue naturally and ask for a different time or which occurrence is meant. Do not claim a tool succeeded when it returned an error. Answer naturally and concisely, including relevant city names and dates.
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
            description: "Convert a requested local date and time in a source city to the destination city's local time. Interpret ordinary date/time wording and supply the required date-time argument format yourself."),
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

        DateTimeOffset now = DateTimeOffset.UtcNow;
        string systemContext = $"{SystemPrompt}\n\nCurrent date and UTC time: {now.ToString("yyyy-MM-dd HH:mm:ss 'UTC'", System.Globalization.CultureInfo.InvariantCulture)}.";
        List<ChatMessage> messages = conversationHistory.CreateRequestMessages(systemContext, prompt);

        ChatResponse response = await chatClient.GetResponseAsync(
            messages,
            new ChatOptions { Tools = tools },
            cancellationToken);

        string answer = string.IsNullOrWhiteSpace(response.Text)
            ? "I couldn't complete that request. Try rephrasing it or naming the cities explicitly."
            : response.Text.Trim();

        conversationHistory.AddTurn(prompt, answer);
        return answer;
    }
}
