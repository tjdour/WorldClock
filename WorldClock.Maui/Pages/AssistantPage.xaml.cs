using WorldClock.Maui.Services;

namespace WorldClock.Maui;

public partial class AssistantPage : ContentPage
{
    private readonly AiAssistantService assistantService;
    private bool requestInProgress;

    public AssistantPage(AiAssistantService assistantService)
    {
        InitializeComponent();
        this.assistantService = assistantService;
    }

    private async void OnAskClicked(object? sender, EventArgs e)
    {
        if (requestInProgress)
        {
            return;
        }

        string prompt = PromptEditor.Text?.Trim() ?? string.Empty;
        if (string.IsNullOrWhiteSpace(prompt))
        {
            StatusLabel.Text = "Enter a question first.";
            return;
        }

        requestInProgress = true;
        AskButton.IsEnabled = false;
        LoadingIndicator.IsVisible = true;
        LoadingIndicator.IsRunning = true;
        StatusLabel.Text = "Checking time-zone information...";
        AnswerLabel.IsVisible = false;

        try
        {
            using CancellationTokenSource timeout = new(TimeSpan.FromSeconds(60));
            AnswerLabel.Text = await assistantService.AskAsync(prompt, timeout.Token);
            AnswerLabel.IsVisible = true;
            StatusLabel.Text = string.Empty;
            PromptEditor.Text = string.Empty;
            await ResponseScrollView.ScrollToAsync(AnswerLabel, ScrollToPosition.End, true);
        }
        catch (ArgumentException exception)
        {
            StatusLabel.Text = exception.Message;
        }
        catch (InvalidOperationException exception)
        {
            StatusLabel.Text = exception.Message;
        }
        catch (OperationCanceledException)
        {
            StatusLabel.Text = "The request timed out. Check the Foundry deployment and try again.";
        }
        catch (HttpRequestException)
        {
            StatusLabel.Text = "Could not reach the AI service. Check your connection and deployment.";
        }
        catch (Exception)
        {
            StatusLabel.Text = "The AI request failed. Check the local User Secrets settings and Foundry deployment.";
        }
        finally
        {
            requestInProgress = false;
            AskButton.IsEnabled = true;
            LoadingIndicator.IsRunning = false;
            LoadingIndicator.IsVisible = false;
        }
    }
}
