namespace WorldClock.Maui
{
    public partial class AppShell : Shell
    {
        public AppShell(AssistantPage assistantPage)
        {
            InitializeComponent();

            ShellContent assistantContent = Items
                .OfType<TabBar>()
                .SelectMany(tabBar => tabBar.Items)
                .SelectMany(section => section.Items)
                .Single(content => content.Route == "AssistantPage");

            assistantContent.Content = assistantPage;
        }
    }
}
