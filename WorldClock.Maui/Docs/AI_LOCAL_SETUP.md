# Local AI Assistant Setup

The MAUI app reads its Azure OpenAI settings from the `WorldClock.Maui` .NET User Secrets store. User Secrets keeps values outside the repository, but it is a local-development convenience—not encrypted storage and not suitable for a distributed client app.

The project User Secrets ID has already been initialized. After rotating the API key that was exposed in chat, set the replacement locally from the repository root:

```powershell
dotnet user-secrets set "AZURE_OPENAI_ENDPOINT" "<Azure OpenAI /openai/v1 endpoint>" --project WorldClock.Maui\WorldClock.Maui.csproj
dotnet user-secrets set "AZURE_OPENAI_GPT_NAME" "<deployment name>" --project WorldClock.Maui\WorldClock.Maui.csproj
dotnet user-secrets set "AZURE_OPENAI_API_KEY" "<newly rotated key>" --project WorldClock.Maui\WorldClock.Maui.csproj
```

Run the MAUI app locally from Visual Studio or with the Windows target. Do not include an API key in source, app settings, launch profiles, or a distributed MAUI package. For any shared or production build, put model calls behind a secured backend instead of calling Azure OpenAI directly from the client.
