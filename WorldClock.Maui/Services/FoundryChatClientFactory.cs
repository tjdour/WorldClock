using Microsoft.Extensions.AI;
using OpenAI;
using OpenAI.Chat;
using System.ClientModel;

namespace WorldClock.Maui.Services;

public sealed class FoundryChatClientFactory(FoundryOptions options)
{
    public IChatClient Create()
    {
        if (!options.IsConfigured)
        {
            throw new InvalidOperationException(
                "Configure AZURE_OPENAI_ENDPOINT, AZURE_OPENAI_API_KEY, and AZURE_OPENAI_GPT_NAME with .NET User Secrets for a local AI demo.");
        }

        if (!Uri.TryCreate(options.Endpoint, UriKind.Absolute, out Uri? endpoint))
        {
            throw new InvalidOperationException(
                "AZURE_OPENAI_ENDPOINT must be an absolute URI.");
        }

        ChatClient client = new(
            options.DeploymentName!,
            new ApiKeyCredential(options.ApiKey!),
            new OpenAIClientOptions { Endpoint = endpoint });

        return client.AsIChatClient();
    }
}
