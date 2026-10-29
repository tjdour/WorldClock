using Microsoft.Extensions.AI;

namespace WorldClock.Maui.Services;

public sealed class ConversationHistory
{
    private readonly List<ChatMessage> messages = [];
    private readonly int maximumTurns;

    public ConversationHistory(int maximumTurns = 8)
    {
        if (maximumTurns < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(maximumTurns));
        }

        this.maximumTurns = maximumTurns;
    }

    public IReadOnlyList<ChatMessage> Messages => messages;

    public List<ChatMessage> CreateRequestMessages(string systemContext, string prompt)
    {
        List<ChatMessage> requestMessages = [new(ChatRole.System, systemContext)];
        requestMessages.AddRange(messages);
        requestMessages.Add(new ChatMessage(ChatRole.User, prompt));
        return requestMessages;
    }

    public void AddTurn(string userText, string assistantText)
    {
        messages.Add(new ChatMessage(ChatRole.User, userText));
        messages.Add(new ChatMessage(ChatRole.Assistant, assistantText));

        int maximumMessages = maximumTurns * 2;
        if (messages.Count > maximumMessages)
        {
            messages.RemoveRange(0, messages.Count - maximumMessages);
        }
    }
}
