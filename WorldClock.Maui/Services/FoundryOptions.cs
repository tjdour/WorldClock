using Microsoft.Extensions.Configuration;

namespace WorldClock.Maui.Services;

public sealed class FoundryOptions
{
    public string? Endpoint { get; init; }
    public string? ApiKey { get; init; }
    public string? DeploymentName { get; init; }

    public bool IsConfigured =>
        !string.IsNullOrWhiteSpace(Endpoint) &&
        !string.IsNullOrWhiteSpace(ApiKey) &&
        !string.IsNullOrWhiteSpace(DeploymentName);

    public static FoundryOptions FromConfiguration(IConfiguration configuration) => new()
    {
        Endpoint = configuration["AZURE_OPENAI_ENDPOINT"],
        ApiKey = configuration["AZURE_OPENAI_API_KEY"],
        DeploymentName = configuration["AZURE_OPENAI_GPT_NAME"]
    };
}
