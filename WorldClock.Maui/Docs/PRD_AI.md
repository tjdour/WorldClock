# World Clock — AI Enhancement PRD / Proposal

**MSSA PCAD21** | **Proposal:** October 19, 2026 | Presentation: est. October 29, 2026  
**GitHub:** https://github.com/tjdour/WorldClock/tree/master/WorldClock.Maui

## Existing Application (Submitted Baseline)
World Clock is a C#/.NET solution with a .NET MAUI desktop app (Clocks and Compare pages), reusable `WorldClock.Core` library, and original console app. It displays live clocks, adds/removes cities, and compares/converts times. A packaged `cities.json` catalog supports city selection; .NET `TimeZoneInfo` performs DST-aware conversions; EF Core/SQLite persists saved cities. MAUI pages currently use XAML and C# code-behind.

## Objective
Add a **text-based AI time-zone assistant** to the existing MAUI app. The LLM interprets natural-language requests and chooses approved tools; existing C# logic performs the actual time calculations.

## Required User Experience
- “What time is it in Tokyo right now?”
- “If it's 9 AM in Seattle on November 3, what time is it in Nairobi?”
- “How many hours ahead is Nairobi of Seattle today?”
- Add an **AI Assistant** page with text input, answers, loading indicators, and error messages. Preserve the existing Clocks and Compare functionality.

## AI Design and Integration
- **Model:** Microsoft Foundry `gpt-5-mini` (pending deployment access); selected for tool-calling support and modest expected cost.
- **SDK:** `Microsoft.Extensions.AI` and `IChatClient`.
- **Architecture:** `AssistantPage` (XAML/code-behind) → `AiAssistantService` (single bounded tool-calling orchestrator) ↔ LLM → `TimeZoneTools` → existing `WorldClockService`, `LocationManager`, and city catalog.
- **Tools:** `GetCurrentTime(city)`, `ConvertTime(source, destination, dateTime)`, and `GetTimeDifference(cityA, cityB)` for current UTC offsets. The difference tool matches the current `WorldClockService` API; all calculations remain in C#.
- **Prompting/guardrails:** Use tool results for factual times/offsets; do not guess. Validate city names against the catalog, clarify ambiguous city names and daylight-saving times, handle unsupported cities/provider errors, and bound tool calls. Use .NET User Secrets for local demonstration credentials; do not distribute a client build containing an API key. A shared/production app requires a secured backend.

## Acceptance Criteria and Scope
The three example requests return tool-grounded results matching existing app calculations; a DST-boundary test passes; ambiguous or invalid requests and model failures are handled clearly; existing clock display, comparison, and SQLite persistence continue working.

**Not included:** MVVM/repository refactors, voice, web search, cognitive services, multi-agent frameworks, or AI-initiated database changes.

**Effort:** Approximately **12–20 hours**: model connection (3–4h), assistant UI (3–4h), function tools/prompts (4–6h), testing/demo (2–6h).
